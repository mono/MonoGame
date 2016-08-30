﻿using System;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using Microsoft.Xna.Framework.Net.Message;
using Microsoft.Xna.Framework.GamerServices;
using Lidgren.Network;

namespace Microsoft.Xna.Framework.Net
{
    public sealed class NetworkSession : IDisposable
    {
        private const int Port = 14242;
        private const int DiscoveryTime = 1000;
        private const int JoinTime = 1000;
        public const int MaxPreviousGamers = 10;
        public const int MaxSupportedGamers = 64;

        internal static NetworkSession Session = null;

        internal static NetPeerConfiguration CreateNetPeerConfig(bool specifyPort)
        {
            NetPeerConfiguration config = new NetPeerConfiguration("MonoGameApp");

            config.Port = specifyPort ? Port : 0;
            config.AcceptIncomingConnections = true;

            config.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config.EnableMessageType(NetIncomingMessageType.UnconnectedData);

            return config;
        }

        public static NetworkSession Create(NetworkSessionType sessionType, IEnumerable<SignedInGamer> localGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties sessionProperties)
        {
            if (Session != null)
            {
                throw new InvalidOperationException("Only one NetworkSession allowed");
            }
            if (maxGamers < 2 || maxGamers > MaxSupportedGamers)
            {
                throw new ArgumentOutOfRangeException("maxGamers must be in the range [2, " + MaxSupportedGamers + "]");
            }
            if (privateGamerSlots < 0 || privateGamerSlots > maxGamers)
            {
                throw new ArgumentOutOfRangeException("privateGamerSlots must be in the range[0, maxGamers]");
            }

            NetPeer peer = new NetPeer(CreateNetPeerConfig(true));

            try
            {
                peer.Start();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Internal error", e);
            }

            Session = new NetworkSession(peer, null, maxGamers, privateGamerSlots, sessionType, sessionProperties, localGamers);
            return Session;
        }

        // ArgumentOutOfRangeException if maxLocalGamers is < 1 or > 4
        public static AvailableNetworkSessionCollection Find(NetworkSessionType sessionType, IEnumerable<SignedInGamer> localGamers, NetworkSessionProperties searchProperties)
        {
            if (sessionType == NetworkSessionType.Local)
            {
                throw new ArgumentException("Find cannot be used with NetworkSessionType.Local");
            }

            // Send discover requests on subnet
            NetPeer discoverPeer = new NetPeer(CreateNetPeerConfig(false));
            discoverPeer.Start();
            discoverPeer.DiscoverLocalPeers(Port);

            Thread.Sleep(DiscoveryTime);

            // Get list of answers
            List<AvailableNetworkSession> availableSessions = new List<AvailableNetworkSession>();

            NetIncomingMessage msg;
            while ((msg = discoverPeer.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryRequest:
                        // Ignore own message
                        break;
                    case NetIncomingMessageType.DiscoveryResponse:
                        NetworkSessionType remoteSessionType = (NetworkSessionType)msg.ReadByte();

                        int maxGamers = msg.ReadInt32();
                        int privateGamerSlots = msg.ReadInt32();
                        int currentGamerCount = msg.ReadInt32();
                        string hostGamertag = msg.ReadString();
                        int openPrivateGamerSlots = msg.ReadInt32();
                        int openPublicGamerSlots = msg.ReadInt32();
                        NetworkSessionProperties sessionProperties = null;

                        if (remoteSessionType == sessionType)
                        {
                            availableSessions.Add(new AvailableNetworkSession(msg.SenderEndPoint, localGamers, maxGamers, privateGamerSlots, sessionType, currentGamerCount, hostGamertag, openPrivateGamerSlots, openPublicGamerSlots, sessionProperties));
                        }
                        break;
                    // Error checking
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        Debug.WriteLine("Lidgren: " + msg.ReadString());
                        break;
                    default:
                        Debug.WriteLine("Unhandled type: " + msg.MessageType);
                        break;
                }

                discoverPeer.Recycle(msg);
            }

            discoverPeer.Shutdown("Discovery peer done");

            return new AvailableNetworkSessionCollection(availableSessions);
        }

        public static NetworkSession Join(AvailableNetworkSession availableSession)
        {
            if (Session != null)
            {
                throw new InvalidOperationException("Only one NetworkSession allowed");
            }
            if (availableSession == null)
            {
                throw new ArgumentNullException("availableSession");
            }
            // TODO: NetworkSessionJoinException if availableSession full/not joinable/cannot be found

            NetPeer peer = new NetPeer(CreateNetPeerConfig(false));
            peer.Start();
            peer.Connect(availableSession.remoteEndPoint);

            Thread.Sleep(JoinTime);

            if (peer.ConnectionsCount == 0)
            {
                throw new NetworkSessionJoinException("Connection failed", NetworkSessionJoinError.SessionNotFound);
            }
            
            NetConnection hostConnection = peer.GetConnection(availableSession.remoteEndPoint);
            int maxGamers = availableSession.maxGamers;
            int privateGamerSlots = availableSession.privateGamerSlots;
            NetworkSessionType sessionType = availableSession.sessionType;
            NetworkSessionProperties sessionProperties = availableSession.SessionProperties;
            IEnumerable<SignedInGamer> localGamers = availableSession.localGamers;

            Session = new NetworkSession(peer, hostConnection, maxGamers, privateGamerSlots, sessionType, sessionProperties, localGamers);
            return Session;
        }

        internal PacketPool packetPool;

        internal NetPeer peer;
        internal NetworkMachine machine;
        internal NetConnection hostConnection;

        internal IList<SignedInGamer> pendingSignedInGamers;
        internal int initiallyPendingSignedInGamersCount;
        
        internal ICollection<IPEndPoint> pendingEndPoints;

        // Host stores which connections were open when a particular peer connected
        internal Dictionary<NetConnection, ICollection<NetConnection>> pendingPeerConnections = new Dictionary<NetConnection, ICollection<NetConnection>>();

        private byte uniqueIdCount;
        
        private IList<NetworkGamer> allGamers;
        private IList<NetworkGamer> allRemoteGamers;

        private NetBuffer internalBuffer;

        internal NetworkSession(NetPeer peer, NetConnection hostConnection, int maxGamers, int privateGamerSlots, NetworkSessionType type, NetworkSessionProperties properties, IEnumerable<SignedInGamer> signedInGamers)
        {
            this.packetPool = new PacketPool();

            this.peer = peer;
            this.machine = new NetworkMachine(null, hostConnection == null);
            this.hostConnection = hostConnection;

            this.pendingSignedInGamers = new List<SignedInGamer>(signedInGamers);
            this.initiallyPendingSignedInGamersCount = this.pendingSignedInGamers.Count;

            this.allGamers = new List<NetworkGamer>();
            this.allRemoteGamers = new List<NetworkGamer>();

            this.AllGamers = new GamerCollection<NetworkGamer>(this.allGamers);
            this.AllowHostMigration = false;
            this.AllowJoinInProgress = false;
            this.BytesPerSecondReceived = 0;
            this.BytesPerSecondSent = 0;
            this.IsDisposed = false;
            this.IsHost = hostConnection == null;
            this.LocalGamers = new GamerCollection<LocalNetworkGamer>(this.machine.localGamers);
            this.MaxGamers = maxGamers;
            this.PrivateGamerSlots = privateGamerSlots;
            this.RemoteGamers = new GamerCollection<NetworkGamer>(this.allRemoteGamers);
            this.SessionProperties = properties;
            this.SessionState = NetworkSessionState.Lobby;
            this.SessionType = type;
            this.SimulatedLatency = TimeSpan.Zero;
            this.SimulatedPacketLoss = 0.0f;

            // Store machine in peer tag
            this.peer.Tag = this.machine;

            if (hostConnection == null)
            {
                // Initialize empty pending end point list so that the host is approved automatically
                this.pendingEndPoints = new List<IPEndPoint>();
            }

            this.internalBuffer = new NetBuffer();
        }

        public GamerCollection<NetworkGamer> AllGamers { get; }
        public bool AllowHostMigration { get; set; } // any peer can get, only host can set
        public bool AllowJoinInProgress { get; set; } // any peer can get, only host can set
        public int BytesPerSecondReceived { get; } // todo
        public int BytesPerSecondSent { get; } // todo

        internal NetworkMachine HostMachine
        {
            get { return IsHost ? machine : hostConnection.Tag as NetworkMachine; }
        }

        public NetworkGamer Host
        {
            get
            {
                NetworkMachine hostMachine = HostMachine;

                if (hostMachine == null || hostMachine.Gamers.Count == 0)
                {
                    return null;
                }

                return hostMachine.Gamers[0];
            }
        }

        public bool IsDisposed { get; private set; }

        public bool IsEveryoneReady
        {
            get
            {
                foreach (NetworkGamer gamer in allGamers)
                {
                    if (!gamer.IsReady)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public bool IsHost { get; }
        public GamerCollection<LocalNetworkGamer> LocalGamers { get; }
        public int MaxGamers { get; set; } // only host can set
        public GamerCollection<NetworkGamer> PreviousGamers { get; }
        public int PrivateGamerSlots { get; set; } // only host can set
        public GamerCollection<NetworkGamer> RemoteGamers { get; }
        public NetworkSessionProperties SessionProperties { get; } // should be synchronized
        public NetworkSessionState SessionState { get; }
        public NetworkSessionType SessionType { get; }

        public TimeSpan SimulatedLatency // TODO: Should be applied even to local messages
        {
            get { return new TimeSpan(0, 0, 0, 0, (int)(peer.Configuration.SimulatedRandomLatency * 1000.0f)); }
            set { peer.Configuration.SimulatedRandomLatency = (float)(value.TotalMilliseconds * 0.001); }
        }

        public float SimulatedPacketLoss // TODO: Should be applied even to local messages
        {
            get { return peer.Configuration.SimulatedLoss; }
            set { peer.Configuration.SimulatedLoss = value; }
        }

        internal int CurrentGamerCount { get { return allGamers.Count; } }
        internal string HostGamertag { get { return machine.localGamers.Count > 0 ? machine.localGamers[0].Gamertag : "Game starting up..."; } }
        internal int OpenPrivateGamerSlots { get { return PrivateGamerSlots; } }
        internal int OpenPublicGamerSlots { get { return MaxGamers - PrivateGamerSlots - CurrentGamerCount; } }

        public event EventHandler<GamerJoinedEventArgs> GamerJoined;
        public event EventHandler<GamerLeftEventArgs> GamerLeft;
        public event EventHandler<GameStartedEventArgs> GameStarted;
        public event EventHandler<GameEndedEventArgs> GameEnded;
        public event EventHandler<HostChangedEventArgs> HostChanged;
        public static event EventHandler<InviteAcceptedEventArgs> InviteAccepted;
        public event EventHandler<NetworkSessionEndedEventArgs> SessionEnded;
        public event EventHandler<WriteLeaderboardsEventArgs> WriteArbitratedLeaderboard; // No documentation exists
        public event EventHandler<WriteLeaderboardsEventArgs> WriteTrueSkill; // No documentation exists
        public event EventHandler<WriteLeaderboardsEventArgs> WriteUnarbitratedLeaderboard; // No documentation exists

        internal void InvokeGamerJoinedEvent(GamerJoinedEventArgs args)
        {
            GamerJoined?.Invoke(this, args);
        }

        internal void InvokeGamerLeftEvent(GamerLeftEventArgs args)
        {
            GamerLeft?.Invoke(this, args);
        }

        public void AddLocalGamer(SignedInGamer gamer)
        {
            throw new NotImplementedException();
        }

        public void StartGame() // only host
        {
            throw new NotImplementedException();
        }

        public void EndGame() // only host
        {
            throw new NotImplementedException();
        }

        public void ResetReady() // only host
        {
            throw new NotImplementedException();
        }

        public NetworkGamer FindGamerById(byte gamerId)
        {
            foreach (NetworkGamer gamer in AllGamers)
            {
                if (gamer.Id == gamerId)
                {
                    return gamer;
                }
            }

            return null;
        }

        internal void AddGamer(NetworkGamer gamer)
        {
            gamer.Machine.AddGamer(gamer);

            allGamers.Add(gamer);
            if (!gamer.IsLocal)
            {
                allRemoteGamers.Add(gamer);
            }
        }

        internal void RemoveGamer(NetworkGamer gamer)
        {
            gamer.Machine.RemoveGamer(gamer);

            allGamers.Remove(gamer);
            if (!gamer.IsLocal)
            {
                allRemoteGamers.Remove(gamer);
            }
        }

        internal bool GetNewUniqueId(out byte id)
        {
            // TODO: Make foolproof
            if (uniqueIdCount >= 255)
            {
                id = 255;
                return false;
            }

            id = uniqueIdCount;
            uniqueIdCount++;
            return true;
        }

        internal bool IsConnectedToEndPoint(IPEndPoint endPoint)
        {
            return peer.GetConnection(endPoint) != null;
        }

        private string MachineOwnerName(NetworkMachine machine)
        {
            if (machine.IsLocal)
            {
                if (machine.IsHost)
                {
                    return "self (host)";
                }
                else
                {
                    return "self";
                }
            }
            else if (machine.IsHost)
            {
                return "host";
            }
            else
            {
                return "peer";
            }
        }

        internal NetDeliveryMethod ToDeliveryMethod(SendDataOptions options)
        {
            switch (options)
            {
                case SendDataOptions.InOrder:
                    return NetDeliveryMethod.UnreliableSequenced;
                case SendDataOptions.Reliable:
                    return NetDeliveryMethod.ReliableUnordered;
                case SendDataOptions.ReliableInOrder:
                    return NetDeliveryMethod.ReliableOrdered;
                case SendDataOptions.Chat:
                    return NetDeliveryMethod.ReliableUnordered;
                case SendDataOptions.Chat & SendDataOptions.InOrder:
                    return NetDeliveryMethod.ReliableOrdered;
                default:
                    throw new InvalidOperationException("Could not convert SendDataOptions!");
            }
        }

        internal void EncodeMessage(IInternalMessageSender message, NetBuffer output)
        {
            output.Write((byte)message.MessageType);

            message.Send(output, machine);
        }

        internal void Send(IInternalMessageSender message)
        {
            Debug.WriteLine("Sending " + message.MessageType + " to all peers...");

            // Send to all peers
            if (peer.Connections.Count > 0)
            {
                NetOutgoingMessage msg = peer.CreateMessage();
                EncodeMessage(message, msg);
                peer.SendMessage(msg, peer.Connections, ToDeliveryMethod(message.Options), message.SequenceChannel);
            }

            // Send to self (Should be done last since then all Send() calls happen before any Receive() call)
            Send(message, machine);
        }

        internal void Send(IInternalMessageSender message, NetworkMachine recipient)
        {
            if (recipient == null)
            {
                throw new ArgumentNullException("recipient");
            }

            Debug.WriteLine("Sending " + message.MessageType + " to " + MachineOwnerName(recipient) + "...");

            if (recipient.IsLocal)
            {
                internalBuffer.LengthBits = 0;
                EncodeMessage(message, internalBuffer);

                internalBuffer.Position = 0;
                Receive(internalBuffer, machine);
            }
            else
            {
                NetOutgoingMessage msg = peer.CreateMessage();
                EncodeMessage(message, msg);
                peer.SendMessage(msg, recipient.connection, ToDeliveryMethod(message.Options), message.SequenceChannel);
            }
        }

        private static Type[] messageToReceiverTypeMap =
        {
            typeof(ConnectToAllRequestMessageReceiver),
            typeof(NoLongerPendingMessageReceiver),
            typeof(GamerJoinRequestMessageReceiver),
            typeof(GamerJoinResponseMessageReceiver),
            typeof(GamerJoinedMessageReceiver),
            typeof(GamerLeftMessageReceiver),
            typeof(UserMessageReceiver)
        };

        internal void Receive(NetBuffer input, NetworkMachine sender)
        {
            InternalMessageType messageType = (InternalMessageType)input.ReadByte();

            Debug.WriteLine("Receiving " + messageType + " from " + MachineOwnerName(sender) + "...");

            Type receiverToInstantiate = messageToReceiverTypeMap[(byte)messageType];
            IInternalMessageReceiver receiver = (IInternalMessageReceiver)Activator.CreateInstance(receiverToInstantiate);
            receiver.Receive(input, machine, sender);
        }

        public void Update()
        {
            // Recycle inbound packets from last frame
            foreach (LocalNetworkGamer localGamer in machine.localGamers)
            {
                localGamer.RecycleInboundPackets();
            }

            // Send accumulated outbound packets -> will create new inbound packets
            foreach (LocalNetworkGamer localGamer in machine.localGamers)
            {
                foreach (OutboundPacket outboundPacket in localGamer.outboundPackets)
                {
                    IInternalMessageSender userMessage = new UserMessageSender(outboundPacket.sender, outboundPacket.recipient, outboundPacket.options, outboundPacket.packet);

                    if (outboundPacket.recipient == null)
                    {
                        Send(userMessage);
                    }
                    else
                    {
                        Send(userMessage, outboundPacket.recipient.Machine);
                    }
                }

                localGamer.RecycleOutboundPackets();
            }

            // Handle incoming messages -> will create new inbound packets
            NetIncomingMessage msg;
            while ((msg = peer.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    // Discovery
                    case NetIncomingMessageType.DiscoveryRequest:
                        Debug.WriteLine("Discovery request received");
                        NetOutgoingMessage response = peer.CreateMessage();
                        response.Write((byte)SessionType);
                        response.Write(MaxGamers);
                        response.Write(PrivateGamerSlots);
                        response.Write(CurrentGamerCount);
                        response.Write(HostGamertag);
                        response.Write(OpenPrivateGamerSlots);
                        response.Write(OpenPublicGamerSlots);
                        peer.SendDiscoveryResponse(response, msg.SenderEndPoint);
                        break;
                    // Peer state changes
                    case NetIncomingMessageType.StatusChanged:
                        if (msg.SenderConnection == null)
                        {
                            throw new NetworkException("Sender connection is null");
                        }

                        NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                        Debug.WriteLine("Status now: " + status + "; Reason: " + msg.ReadString());

                        if (status == NetConnectionStatus.Connected)
                        {
                            // Create a pending network machine
                            NetworkMachine senderMachine = new NetworkMachine(msg.SenderConnection, msg.SenderConnection == hostConnection);
                            msg.SenderConnection.Tag = senderMachine;

                            // TODO: Examine this solution...
                            if (!machine.IsPending)
                            {
                                Send(new NoLongerPendingMessageSender(), senderMachine);
                            }

                            if (IsHost)
                            {
                                // Save snapshot of current connections and send them to new peer
                                ICollection<NetConnection> requestedConnections = new HashSet<NetConnection>(peer.Connections);
                                requestedConnections.Remove(msg.SenderConnection);
                                pendingPeerConnections.Add(msg.SenderConnection, requestedConnections);

                                Send(new ConnectToAllRequestMessageSender(requestedConnections), senderMachine);
                            }
                        }

                        if (status == NetConnectionStatus.Disconnected)
                        {
                            // Remove gamers
                            NetworkMachine disconnectedMachine = msg.SenderConnection.Tag as NetworkMachine;

                            foreach (NetworkGamer gamer in disconnectedMachine.gamers)
                            {
                                InvokeGamerLeftEvent(new GamerLeftEventArgs(gamer));
                            }

                            if (IsHost)
                            {
                                // If disconnected peer was pending, remove it
                                pendingPeerConnections.Remove(msg.SenderConnection);

                                // Update pending peers
                                foreach (var pendingPeer in pendingPeerConnections)
                                {
                                    NetworkMachine pendingMachine = pendingPeer.Key.Tag as NetworkMachine;

                                    if (!pendingMachine.IsPending)
                                    {
                                        continue;
                                    }

                                    if (pendingPeer.Value.Contains(msg.SenderConnection))
                                    {
                                        pendingPeer.Value.Remove(msg.SenderConnection);

                                        Send(new ConnectToAllRequestMessageSender(pendingPeer.Value), pendingMachine);
                                    }
                                }
                            }
                            else
                            {
                                if (msg.SenderConnection == hostConnection)
                                {
                                    // TODO: Host migration
                                    Dispose();
                                }
                            }
                        }
                        break;
                    // Unconnected data
                    case NetIncomingMessageType.UnconnectedData:
                        Debug.WriteLine("Unconnected data received!");
                        break;
                    // Custom data
                    case NetIncomingMessageType.Data:
                        if (msg.SenderConnection == null)
                        {
                            throw new NetworkException("Sender connection is null");
                        }

                        Receive(msg, msg.SenderConnection.Tag as NetworkMachine);
                        break;
                    // Error checking
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        Debug.WriteLine("Lidgren: " + msg.ReadString());
                        break;
                    default:
                        Debug.WriteLine("Unhandled type: " + msg.MessageType);
                        break;
                }

                peer.Recycle(msg);
            }

            // Handle pending machine
            if (machine.IsPending && pendingEndPoints != null)
            {
                bool done = true;

                foreach (IPEndPoint endPoint in pendingEndPoints)
                {
                    if (!IsConnectedToEndPoint(endPoint))
                    {
                        done = false;
                    }
                }

                if (done)
                {
                    Send(new NoLongerPendingMessageSender());

                    // Handle pending signed in gamers
                    if (pendingSignedInGamers.Count > 0)
                    {
                        Send(new GamerJoinRequestMessageSender(), HostMachine);
                    }
                }
            }
        }

        public void Dispose()
        {
            while (machine.localGamers.Count > 0)
            {
                LocalNetworkGamer localGamer = machine.localGamers[machine.localGamers.Count - 1];

                InvokeGamerLeftEvent(new GamerLeftEventArgs(localGamer));
                
                RemoveGamer(localGamer);
            }

            peer.Shutdown("Peer done");

            Session = null;

            IsDisposed = true;
        }
    }
}