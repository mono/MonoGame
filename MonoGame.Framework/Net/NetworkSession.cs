﻿using System;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using Microsoft.Xna.Framework.GamerServices;
using Lidgren.Network;

namespace Microsoft.Xna.Framework.Net
{
    internal enum NetActionIndex
    {
        ConnectToAllRequest,
        ConnectToAllSuccessful,
        GamerJoinRequest,
        GamerJoinResponse,
        GamerJoined,
        GamerLeft,
        UserMessage
    }

    internal interface INetAction
    {
        NetActionIndex Index { get; }
        NetDeliveryMethod DeliveryMethod { get; }
        int SequenceChannel { get; }
        void EncodeData(NetworkSession session, NetOutgoingMessage msg); // May not run if local message
        void DecodeData(NetworkSession session, NetIncomingMessage msg); // May not run if local message
        void Trigger(NetworkSession session);
    }

    internal struct ConnectToAllRequestAction : INetAction // Host to any other peer
    {
        internal ICollection<NetConnection> requestedConnections;

        internal ICollection<IPEndPoint> endPoints;

        public ConnectToAllRequestAction(ICollection<NetConnection> requestedConnections)
        {
            this.requestedConnections = requestedConnections;
            this.endPoints = null;
        }

        public NetActionIndex Index { get { return NetActionIndex.ConnectToAllRequest; } }
        public NetDeliveryMethod DeliveryMethod { get { return NetDeliveryMethod.ReliableOrdered; } }
        public int SequenceChannel { get { return 1; } }

        public void EncodeData(NetworkSession session, NetOutgoingMessage msg)
        {
            if (!session.IsHost)
            {
                throw new NetworkException("Only host should send ConnectToAllRequest");
            }

            msg.Write((int)requestedConnections.Count);
            foreach (NetConnection c in requestedConnections)
            {
                msg.Write(c.RemoteEndPoint);
            }
            Debug.WriteLine("ConnectToAllRequest sent (with " + requestedConnections.Count + " connections)");
        }

        public void DecodeData(NetworkSession session, NetIncomingMessage msg)
        {
            int requestedConnectionCount = msg.ReadInt32();

            endPoints = new List<IPEndPoint>(requestedConnectionCount);

            for (int i = 0; i < requestedConnectionCount; i++)
            {
                endPoints.Add(msg.ReadIPEndPoint());
            }
            Debug.WriteLine("ConnectToAllRequest received");
        }

        public void Trigger(NetworkSession session)
        {
            session.pendingEndPoints = new List<IPEndPoint>();

            foreach (IPEndPoint endPoint in endPoints)
            {
                session.pendingEndPoints.Add(endPoint);

                if (!session.IsConnectedToEndPoint(endPoint))
                {
                    session.peer.Connect(endPoint);
                }
            }
        }
    }

    internal struct ConnectToAllSuccessfulAction : INetAction // Any peer to all peers
    {
        internal NetworkMachine machine;
        internal NetConnection sender;

        internal ConnectToAllSuccessfulAction(NetworkMachine machine)
        {
            this.machine = machine;
            this.sender = null;
        }

        public NetActionIndex Index { get { return NetActionIndex.ConnectToAllSuccessful; } }
        public NetDeliveryMethod DeliveryMethod { get { return NetDeliveryMethod.ReliableOrdered; } }
        public int SequenceChannel { get { return 1; } }

        public void EncodeData(NetworkSession session, NetOutgoingMessage msg)
        { }

        public void DecodeData(NetworkSession session, NetIncomingMessage msg)
        {
            machine = msg.SenderConnection.Tag as NetworkMachine;
            sender = msg.SenderConnection;
        }

        public void Trigger(NetworkSession session)
        {
            // The local or remote machine is now considered fully connected
            machine.IsPending = false;

            // Remote peer?
            if (!machine.IsLocal)
            {
                if (session.IsHost)
                {
                    session.pendingPeerConnections.Remove(sender);
                }

                // Send gamer joined messages to the newly fully connected remote peer
                foreach (LocalNetworkGamer localGamer in session.machine.localGamers)
                {
                    session.SendMessageToPeer(new GamerJoinedAction(localGamer), sender);
                }
            }
        }

        public void CleanUp()
        { }
    }

    internal struct GamerJoinRequestAction : INetAction // Any peer to host
    {
        internal NetConnection sender;

        public NetActionIndex Index { get { return NetActionIndex.GamerJoinRequest; } }
        public NetDeliveryMethod DeliveryMethod { get { return NetDeliveryMethod.ReliableOrdered; } }
        public int SequenceChannel { get { return 1; } }

        public void EncodeData(NetworkSession session, NetOutgoingMessage msg)
        { }

        public void DecodeData(NetworkSession session, NetIncomingMessage msg)
        {
            sender = msg.SenderConnection;
        }

        public void Trigger(NetworkSession session)
        {
            if (session.IsHost)
            {
                session.SendMessageToPeer(new GamerJoinResponseAction(session), sender);
            }
        }

        public void CleanUp()
        { }
    }

    internal struct GamerJoinResponseAction : INetAction // Host to any peer
    {
        internal bool wasApprovedByHost;
        internal byte id;

        public GamerJoinResponseAction(NetworkSession session)
        {
            if (!session.IsHost)
            {
                throw new InvalidOperationException("Only host should create GamerJoinResponseAction");
            }

            wasApprovedByHost = session.GetNewUniqueId(out id);
        }

        public NetActionIndex Index { get { return NetActionIndex.GamerJoinResponse; } }
        public NetDeliveryMethod DeliveryMethod { get { return NetDeliveryMethod.ReliableOrdered; } }
        public int SequenceChannel { get { return 1; } }

        public void EncodeData(NetworkSession session, NetOutgoingMessage msg)
        {
            if (!session.IsHost)
            {
                throw new NetworkException("Only host should send GamerJoinResponseAction");
            }

            msg.Write(wasApprovedByHost);
            msg.Write(id);
        }

        public void DecodeData(NetworkSession session, NetIncomingMessage msg)
        {
            if (!session.IsHost && msg.SenderConnection != session.hostConnection)
            {
                throw new InvalidOperationException();
            }

            wasApprovedByHost = msg.ReadBoolean();
            id = msg.ReadByte();
        }

        public void Trigger(NetworkSession session)
        {
            if (!wasApprovedByHost)
            {
                Debug.WriteLine("Our gamer join request was not accepted by the host!");
                return;
            }

            if (session.pendingSignedInGamers.Count == 0)
            {
                Debug.WriteLine("No pending signed in gamers but received gamer join response from host!");
                return;
            }

            // Host approved request, now possible to create network gamer
            bool isFirstSignedInGamer = session.pendingSignedInGamers.Count == session.initiallyPendingSignedInGamersCount;
            SignedInGamer signedInGamer = session.pendingSignedInGamers[0];
            session.pendingSignedInGamers.RemoveAt(0);

            bool isGuest = !isFirstSignedInGamer;
            bool isHost = session.IsHost && isFirstSignedInGamer;
            LocalNetworkGamer localGamer = new LocalNetworkGamer(id, isGuest, isHost, false, session, signedInGamer);

            session.SendMessageToEveryone(new GamerJoinedAction(localGamer));
        }
    }

    internal struct GamerJoinedAction : INetAction
    {
        internal LocalNetworkGamer localGamer;
        
        internal NetworkMachine machine;
        internal string displayName;
        internal string gamertag;
        internal byte id;
        internal bool isGuest;
        internal bool isHost;
        internal bool isPrivateSlot;

        public GamerJoinedAction(LocalNetworkGamer localGamer)
        {
            if (localGamer == null)
            {
                throw new InvalidOperationException();
            }

            this.localGamer = localGamer;

            this.machine = localGamer.Machine;
            this.displayName = localGamer.DisplayName;
            this.gamertag = localGamer.Gamertag;
            this.id = localGamer.Id;
            this.isGuest = localGamer.IsGuest;
            this.isHost = localGamer.IsHost;
            this.isPrivateSlot = false;
        }

        public NetActionIndex Index { get { return NetActionIndex.GamerJoined; } }
        public NetDeliveryMethod DeliveryMethod { get { return NetDeliveryMethod.ReliableOrdered; } }
        public int SequenceChannel { get { return 1; } }

        public void EncodeData(NetworkSession session, NetOutgoingMessage msg)
        {
            msg.Write(displayName);
            msg.Write(gamertag);
            msg.Write(id);
            msg.Write(isGuest);
            msg.Write(isHost);
            msg.Write(isPrivateSlot);
        }

        public void DecodeData(NetworkSession session, NetIncomingMessage msg)
        {
            machine = msg.SenderConnection.Tag as NetworkMachine;
            displayName = msg.ReadString();
            gamertag = msg.ReadString();
            id = msg.ReadByte();
            isGuest = msg.ReadBoolean();
            isHost = msg.ReadBoolean();
            isPrivateSlot = msg.ReadBoolean();

            if (session.IsHost && isHost)
            {
                // New gamer is claiming to be host but we know we are, kick their machine
                machine.RemoveFromSession();
            }
        }

        public void Trigger(NetworkSession session)
        {
            if (localGamer != null)
            {
                machine.AddLocalGamer(localGamer);
                session.AddGamer(localGamer);
                
                session.InvokeGamerJoinedEvent(new GamerJoinedEventArgs(localGamer));
            }
            else
            {
                NetworkGamer remoteGamer = new NetworkGamer(displayName, gamertag, id, isGuest, isHost, false, isPrivateSlot, machine, session);
                machine.AddRemoteGamer(remoteGamer);
                session.AddGamer(remoteGamer);

                session.InvokeGamerJoinedEvent(new GamerJoinedEventArgs(remoteGamer));
            }
        }
    }

    internal struct GamerLeftAction : INetAction
    {
        internal LocalNetworkGamer localGamer;

        internal NetworkMachine machine;
        internal byte id;

        public GamerLeftAction(LocalNetworkGamer localGamer)
        {
            this.localGamer = localGamer;

            this.machine = localGamer.Machine;
            this.id = localGamer.Id;
        }

        public NetActionIndex Index { get { return NetActionIndex.GamerLeft; } }
        public NetDeliveryMethod DeliveryMethod { get { return NetDeliveryMethod.ReliableOrdered; } }
        public int SequenceChannel { get { return 1; } }

        public void EncodeData(NetworkSession session, NetOutgoingMessage msg)
        {
            msg.Write(id);
        }

        public void DecodeData(NetworkSession session, NetIncomingMessage msg)
        {
            machine = msg.SenderConnection.Tag as NetworkMachine;
            id = msg.ReadByte();
        }

        public void Trigger(NetworkSession session)
        {
            if (localGamer != null)
            {
                session.InvokeGamerLeftEvent(new GamerLeftEventArgs(localGamer));

                machine.RemoveLocalGamer(localGamer);
                session.RemoveGamer(localGamer);
            }
            else
            {
                NetworkGamer remoteGamer = session.FindGamerById(id);

                if (remoteGamer == null)
                {
                    Debug.Write("GamerLeftAction provided incorrect remote gamer id");
                    return;
                }

                session.InvokeGamerLeftEvent(new GamerLeftEventArgs(remoteGamer));

                machine.RemoveRemoteGamer(remoteGamer);
                session.RemoveGamer(remoteGamer);
            }
        }
    }

    internal struct UserMessageAction : INetAction
    {
        private byte senderId;
        private bool sendToAll;
        private byte recipientId;
        private Packet packet;

        public UserMessageAction(LocalNetworkGamer sender, NetworkGamer recipient, Packet packet, SendDataOptions options)
        {
            switch (options)
            {
                case SendDataOptions.InOrder:
                    this.DeliveryMethod = NetDeliveryMethod.UnreliableSequenced;
                    break;
                case SendDataOptions.Reliable:
                    this.DeliveryMethod = NetDeliveryMethod.ReliableUnordered;
                    break;
                case SendDataOptions.ReliableInOrder:
                    this.DeliveryMethod = NetDeliveryMethod.ReliableOrdered;
                    break;
                case SendDataOptions.Chat:
                    this.DeliveryMethod = NetDeliveryMethod.ReliableUnordered;
                    break;
                case SendDataOptions.Chat & SendDataOptions.InOrder:
                    this.DeliveryMethod = NetDeliveryMethod.ReliableOrdered;
                    break;
                default:
                    this.DeliveryMethod = NetDeliveryMethod.Unreliable;
                    break;
            }

            this.senderId = sender.Id;
            this.sendToAll = recipient == null;
            this.recipientId = recipient != null ? recipient.Id : (byte)255;
            this.packet = packet;
        }

        public NetActionIndex Index { get { return NetActionIndex.UserMessage; } }
        public NetDeliveryMethod DeliveryMethod { get; }
        public int SequenceChannel { get { return 0; } }

        public void EncodeData(NetworkSession session, NetOutgoingMessage msg)
        {
            msg.Write(senderId);
            msg.Write(sendToAll);
            msg.Write(recipientId);
            msg.Write(packet.length);
            msg.Write(packet.data);
        }

        public void DecodeData(NetworkSession session, NetIncomingMessage msg)
        {
            senderId = msg.ReadByte();
            sendToAll = msg.ReadBoolean();
            recipientId = msg.ReadByte();
            int length = msg.ReadInt32();
            packet = session.packetPool.GetPacket(length);
            msg.ReadBytes(packet.data, 0, length);
        }

        public void Trigger(NetworkSession session)
        {
            NetworkGamer sender = session.FindGamerById(senderId);

            if (sendToAll)
            {
                foreach (LocalNetworkGamer localGamer in session.LocalGamers)
                {
                    localGamer.inboundPackets.Add(new InboundPacket(packet, sender));
                }
            }
            else
            {
                LocalNetworkGamer localGamer = session.FindGamerById(recipientId) as LocalNetworkGamer;

                if (localGamer == null)
                {
                    Debug.WriteLine("User message sent to the wrong peer!");
                    return;
                }

                localGamer.inboundPackets.Add(new InboundPacket(packet, sender));
            }
        }
    }

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

            Session = new NetworkSession(peer, true, null, maxGamers, privateGamerSlots, sessionType, sessionProperties, localGamers);
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

            bool isHost = false;
            NetConnection hostConnection = peer.GetConnection(availableSession.remoteEndPoint);
            int maxGamers = availableSession.maxGamers;
            int privateGamerSlots = availableSession.privateGamerSlots;
            NetworkSessionType sessionType = availableSession.sessionType;
            NetworkSessionProperties sessionProperties = availableSession.SessionProperties;
            IEnumerable<SignedInGamer> localGamers = availableSession.localGamers;

            Session = new NetworkSession(peer, isHost, hostConnection, maxGamers, privateGamerSlots, sessionType, sessionProperties, localGamers);
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

        internal NetworkSession(NetPeer peer, bool isHost, NetConnection hostConnection, int maxGamers, int privateGamerSlots, NetworkSessionType type, NetworkSessionProperties properties, IEnumerable<SignedInGamer> signedInGamers)
        {
            this.packetPool = new PacketPool();

            this.peer = peer;
            this.machine = new NetworkMachine(null, isHost);
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
            this.IsHost = isHost;
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

            if (isHost)
            {
                // Initialize empty pending end point list so that the host is approved automatically
                pendingEndPoints = new List<IPEndPoint>();
            }
        }

        public GamerCollection<NetworkGamer> AllGamers { get; }
        public bool AllowHostMigration { get; set; } // any peer can get, only host can set
        public bool AllowJoinInProgress { get; set; } // any peer can get, only host can set
        public int BytesPerSecondReceived { get; } // todo
        public int BytesPerSecondSent { get; } // todo

        public NetworkGamer Host
        {
            get
            {
                NetworkMachine hostMachine = IsHost ? machine : hostConnection.Tag as NetworkMachine;

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
            allGamers.Add(gamer);

            if (!gamer.IsLocal)
            {
                allRemoteGamers.Add(gamer);
            }
        }

        internal void RemoveGamer(NetworkGamer gamer)
        {
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

        // To everyone
        internal void SendMessageToEveryone(INetAction action)
        {
            Debug.WriteLine("Sending " + action.Index + " to everyone...");

            // Handle remote peers (only encode once)
            if (peer.Connections.Count > 0)
            {
                NetOutgoingMessage msg = peer.CreateMessage();
                msg.Write((byte)action.Index);
                action.EncodeData(this, msg);
                peer.SendMessage(msg, peer.Connections, action.DeliveryMethod, action.SequenceChannel);
            }

            // Handle self
            action.Trigger(this);
        }

        // To self only
        internal void SendMessageToSelf(INetAction action)
        {
            Debug.WriteLine("Sending " + action.Index + " to self...");

            action.Trigger(this);
        }

        // To specific peer
        internal void SendMessageToPeer(INetAction action, NetConnection recipient)
        {
            if (IsHost && recipient == null)
            {
                SendMessageToSelf(action);
                return;
            }

            if (recipient == hostConnection)
            {
                Debug.WriteLine("Sending " + action.Index + " to host...");
            }
            else
            {
                Debug.WriteLine("Sending " + action.Index + " to peer...");
            }


            NetOutgoingMessage msg = peer.CreateMessage();
            msg.Write((byte)action.Index);
            action.EncodeData(this, msg);
            peer.SendMessage(msg, recipient, action.DeliveryMethod, action.SequenceChannel);
        }

        private static Type[] actionIndexToTypeMap =
        {
            typeof(ConnectToAllRequestAction),
            typeof(ConnectToAllSuccessfulAction),
            typeof(GamerJoinRequestAction),
            typeof(GamerJoinResponseAction),
            typeof(GamerJoinedAction),
            typeof(GamerLeftAction),
            typeof(UserMessageAction)
        };

        private void ReceiveMessage(NetIncomingMessage msg)
        {
            byte index = msg.ReadByte();
            if (msg.SenderConnection == hostConnection)
            {
                Debug.WriteLine("Received " + (NetActionIndex)index + " from host...");
            }
            else
            {
                Debug.WriteLine("Received " + (NetActionIndex)index + " from peer...");
            }

            INetAction action = (INetAction)Activator.CreateInstance(actionIndexToTypeMap[index]);
            action.DecodeData(this, msg);
            action.Trigger(this);
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
                    INetAction userAction = new UserMessageAction(outboundPacket.sender, outboundPacket.recipient, outboundPacket.packet, outboundPacket.options);

                    if (outboundPacket.recipient != null)
                    {
                        if (outboundPacket.recipient.IsLocal)
                        {
                            SendMessageToSelf(userAction);
                        }
                        else
                        {
                            SendMessageToPeer(userAction, outboundPacket.recipient.Machine.connection);
                        }
                    }
                    else
                    {
                        SendMessageToEveryone(userAction);
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
                            msg.SenderConnection.Tag = new NetworkMachine(msg.SenderConnection, msg.SenderConnection == hostConnection);

                            if (IsHost)
                            {
                                // Save snapshot of current connections and send them to new peer
                                ICollection<NetConnection> requestedConnections = new HashSet<NetConnection>(peer.Connections);
                                requestedConnections.Remove(msg.SenderConnection);
                                pendingPeerConnections.Add(msg.SenderConnection, requestedConnections);

                                SendMessageToPeer(new ConnectToAllRequestAction(requestedConnections), msg.SenderConnection);
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

                                        SendMessageToPeer(new ConnectToAllRequestAction(pendingPeer.Value), pendingPeer.Key);
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

                        ReceiveMessage(msg);
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
                    SendMessageToEveryone(new ConnectToAllSuccessfulAction(machine));

                    // Handle pending signed in gamers
                    if (pendingSignedInGamers.Count > 0)
                    {
                        SendMessageToPeer(new GamerJoinRequestAction(), hostConnection);
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

                machine.RemoveLocalGamer(localGamer);
                RemoveGamer(localGamer);
            }

            peer.Shutdown("Peer done");

            Session = null;

            IsDisposed = true;
        }
    }
}