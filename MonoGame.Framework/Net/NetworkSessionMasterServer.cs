﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Diagnostics;
using Lidgren.Network;

namespace Microsoft.Xna.Framework.Net
{
    internal enum MasterServerMessageType : byte
    {
        RequestGeneralInfo = 0,
        RegisterHost = 1,
        UnregisterHost = 2,
        RequestHosts = 3,
        RequestIntroduction = 4,
    };

    internal class HostData
    {
        public Guid Guid;
        public IPEndPoint InternalIp;
        public IPEndPoint ExternalIp;
        public NetworkSessionPublicInfo PublicInfo;
        public DateTime LastUpdated;

        public HostData(Guid guid, IPEndPoint internalIp, IPEndPoint externalIp, NetworkSessionPublicInfo publicInfo)
        {
            Guid = guid;
            InternalIp = internalIp;
            ExternalIp = externalIp;
            PublicInfo = publicInfo;
            LastUpdated = DateTime.Now;
        }

        public override string ToString()
        {
            return "[Guid: " + Guid + ", InternalIp: " + InternalIp + ", ExternalIp: " + ExternalIp + "]";
        }
    }

    public abstract class NetworkSessionMasterServer
    {
        private static readonly TimeSpan ReportStatusInterval = TimeSpan.FromSeconds(60.0);

        private NetPeer serverPeer;
        private IDictionary<Guid, HostData> hosts = new Dictionary<Guid, HostData>();
        private DateTime lastReportedStatus = DateTime.MinValue;

        public void Start()
        {
            var config = new NetPeerConfiguration(GameAppId)
            {
                Port = NetworkSessionSettings.MasterServerPort,
                AcceptIncomingConnections = false,
                AutoFlushSendQueue = true,
            };
            config.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            config.EnableMessageType(NetIncomingMessageType.UnconnectedData);

            serverPeer = new NetPeer(config);
            try
            {
                serverPeer.Start();
            }
            catch (Exception e)
            {
                throw new NetworkException("Could not start server peer", e);
            }

            Console.WriteLine("Master server with game app id " + GameAppId + " started on port " + config.Port + ".");
        }

        private List<Guid> hostsToRemove = new List<Guid>();

        protected void TrimHosts()
        {
            var currentTime = DateTime.Now;
            var threshold = NetworkSessionSettings.MasterServerRegistrationInterval + TimeSpan.FromSeconds(5.0);

            hostsToRemove.Clear();

            foreach (var host in hosts)
            {
                if ((currentTime - host.Value.LastUpdated) > threshold)
                {
                    hostsToRemove.Add(host.Key);
                }
            }

            foreach (var endPoint in hostsToRemove)
            {
                var host = hosts[endPoint];
                hosts.Remove(endPoint);

                Console.WriteLine("Host removed due to timeout. " + host);
            }
        }

        protected void ReportStatus()
        {
            var currentTime = DateTime.Now;
            if (currentTime - lastReportedStatus > ReportStatusInterval)
            {
                Console.WriteLine("Status: " + hosts.Count + " registered hosts.");

                lastReportedStatus = currentTime;
            }
        }

        protected void ReceiveMessages()
        {
            NetIncomingMessage msg;
            while ((msg = serverPeer.ReadMessage()) != null)
            {
                if (msg.MessageType == NetIncomingMessageType.UnconnectedData)
                {
                    if (!HandleMessage(msg))
                    {
                        Console.WriteLine("Encountered malformed message from " + msg.SenderEndPoint + ".");
                    }
                }
                else
                {
                    NetworkSession.HandleLidgrenMessage(msg);
                }
                serverPeer.Recycle(msg);
            }
        }

        internal static void RegisterHost(NetPeer peer, Guid guid, IPEndPoint internalIp, NetworkSessionPublicInfo publicInfo)
        {
            var request = peer.CreateMessage();
            request.Write(NetworkSessionSettings.GameAppId);
            request.Write(NetworkSessionSettings.MasterServerPayload);
            request.Write((byte)MasterServerMessageType.RegisterHost);
            request.Write(guid.ToString());
            request.Write(internalIp);
            publicInfo.Pack(request);

            var serverEndPoint = NetUtility.Resolve(NetworkSessionSettings.MasterServerAddress, NetworkSessionSettings.MasterServerPort);
            peer.SendUnconnectedMessage(request, serverEndPoint);

            Debug.WriteLine("Registering with master server (Guid: " + guid + ", InternalIp: " + internalIp + ", PublicInfo: ...)");
        }

        internal static void UnregisterHost(NetPeer peer, Guid guid)
        {
            var request = peer.CreateMessage();
            request.Write(NetworkSessionSettings.GameAppId);
            request.Write(NetworkSessionSettings.MasterServerPayload);
            request.Write((byte)MasterServerMessageType.UnregisterHost);
            request.Write(guid.ToString());

            var serverEndPoint = NetUtility.Resolve(NetworkSessionSettings.MasterServerAddress, NetworkSessionSettings.MasterServerPort);
            peer.SendUnconnectedMessage(request, serverEndPoint);

            Debug.WriteLine("Unregistering with master server (Guid: " + guid + ")");
        }

        internal static void RequestHosts(NetPeer peer)
        {
            var request = peer.CreateMessage();
            request.Write(NetworkSessionSettings.GameAppId);
            request.Write(NetworkSessionSettings.MasterServerPayload);
            request.Write((byte)MasterServerMessageType.RequestHosts);

            var serverEndPoint = NetUtility.Resolve(NetworkSessionSettings.MasterServerAddress, NetworkSessionSettings.MasterServerPort);
            peer.SendUnconnectedMessage(request, serverEndPoint);
        }

        internal static void SerializeRequestHostsResponse(NetOutgoingMessage response, Guid guid, NetworkSessionPublicInfo publicInfo)
        {
            response.Write(guid.ToString());
            publicInfo.Pack(response);
        }

        internal static bool ParseRequestHostsResponse(NetIncomingMessage response, out Guid hostGuid, out NetworkSessionPublicInfo hostPublicInfo)
        {
            hostGuid = Guid.Empty;
            hostPublicInfo = null;

            Guid guid;
            NetworkSessionPublicInfo publicInfo = new NetworkSessionPublicInfo();
            try
            {
                guid = new Guid(response.ReadString());
                if (!publicInfo.Unpack(response))
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            hostGuid = guid;
            hostPublicInfo = publicInfo;
            return true;
        }

        internal static void RequestIntroduction(NetPeer peer, Guid guid, IPEndPoint internalIp)
        {
            var request = peer.CreateMessage();
            request.Write(NetworkSessionSettings.GameAppId);
            request.Write(NetworkSessionSettings.MasterServerPayload);
            request.Write((byte)MasterServerMessageType.RequestIntroduction);
            request.Write(guid.ToString());
            request.Write(internalIp);

            var serverEndPoint = NetUtility.Resolve(NetworkSessionSettings.MasterServerAddress, NetworkSessionSettings.MasterServerPort);
            peer.SendUnconnectedMessage(request, serverEndPoint);
        }

        internal static void RequestGeneralInfo(NetPeer peer)
        {
            var request = peer.CreateMessage();
            request.Write(NetworkSessionSettings.GameAppId);
            request.Write(NetworkSessionSettings.MasterServerPayload); // Note that payload does not need to match to get general info
            request.Write((byte)MasterServerMessageType.RequestGeneralInfo);

            var serverEndPoint = NetUtility.Resolve(NetworkSessionSettings.MasterServerAddress, NetworkSessionSettings.MasterServerPort);
            peer.SendUnconnectedMessage(request, serverEndPoint);
        }

        internal static bool ParseRequestGeneralInfoResponse(NetIncomingMessage response, out string info)
        {
            info = null;

            string _info;
            try
            {
                _info = response.ReadString();
            }
            catch
            {
                return false;
            }

            info = _info;
            return true;
        }

        protected bool HandleMessage(NetIncomingMessage msg)
        {
            string senderGameAppId;
            string senderPayload;
            try
            {
                senderGameAppId = msg.ReadString();
                senderPayload = msg.ReadString();
            }
            catch
            {
                return false;
            }
            if (!senderGameAppId.Equals(GameAppId, StringComparison.InvariantCulture))
            {
                Console.WriteLine("Received message with incorrect game app id from " + msg.SenderEndPoint + ".");
                return true;
            }
            var payloadValid = ValidatePayload(senderPayload);
            var messageType = (MasterServerMessageType)msg.ReadByte();

            if (messageType == MasterServerMessageType.RequestGeneralInfo)
            {
                // Note: Payload does not need to be valid to request general info (useful to handle new version alerts)
                var response = serverPeer.CreateMessage();
                response.Write(GeneralInfo);
                serverPeer.SendUnconnectedMessage(response, msg.SenderEndPoint);
                return true;
            }

            if (!payloadValid)
            {
                Console.WriteLine("Received message that failed payload validation from " + msg.SenderEndPoint + ".");
                return true;
            }

            if (messageType == MasterServerMessageType.RegisterHost)
            {
                Guid guid;
                IPEndPoint internalIp, externalIp;
                NetworkSessionPublicInfo publicInfo = new NetworkSessionPublicInfo();
                try
                {
                    guid = new Guid(msg.ReadString());
                    internalIp = msg.ReadIPEndPoint();
                    externalIp = msg.SenderEndPoint;
                    if (!publicInfo.Unpack(msg))
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }

                hosts[guid] = new HostData(guid, internalIp, externalIp, publicInfo);

                Console.WriteLine("Host registered/updated. " + hosts[guid]);
            }
            else if (messageType == MasterServerMessageType.UnregisterHost)
            {
                Guid guid;
                try
                {
                    guid = new Guid(msg.ReadString());
                }
                catch
                {
                    return false;
                }

                if (hosts.ContainsKey(guid))
                {
                    var host = hosts[guid];
                    if (msg.SenderEndPoint.Equals(host.ExternalIp))
                    {
                        hosts.Remove(guid);

                        Console.WriteLine("Host unregistered. " + host);
                    }
                    else
                    {
                        Console.WriteLine("Unregister requested for host not registered by " + msg.SenderEndPoint + ".");
                    }
                }
                else
                {
                    Console.WriteLine("Unregister requested for unknown host from " + msg.SenderEndPoint + ".");
                }
            }
            else if (messageType == MasterServerMessageType.RequestHosts)
            {
                foreach (var host in hosts.Values)
                {
                    var response = serverPeer.CreateMessage();
                    SerializeRequestHostsResponse(response, host.Guid, host.PublicInfo);
                    serverPeer.SendUnconnectedMessage(response, msg.SenderEndPoint);
                }

                Console.WriteLine("List of " + hosts.Count + " hosts sent to " + msg.SenderEndPoint + ".");
            }
            else if (messageType == MasterServerMessageType.RequestIntroduction)
            {
                Guid guid;
                IPEndPoint clientInternalIp, clientExternalIp;
                try
                {
                    guid = new Guid(msg.ReadString());
                    clientInternalIp = msg.ReadIPEndPoint();
                    clientExternalIp = msg.SenderEndPoint;
                }
                catch
                {
                    return false;
                }

                if (hosts.ContainsKey(guid))
                {
                    var host = hosts[guid];
                    serverPeer.Introduce(host.InternalIp, host.ExternalIp, clientInternalIp, clientExternalIp, string.Empty);
                    Console.WriteLine("Introduced host " + host + " and client [InternalIp: " + clientInternalIp + ", ExternalIp: " + clientExternalIp + "].");
                }
                else
                {
                    Console.WriteLine("Introduction requested for unknwon host from " + msg.SenderEndPoint + ".");
                }
            }

            return true;
        }

        public void Update()
        {
            if (serverPeer == null || serverPeer.Status == NetPeerStatus.NotRunning)
            {
                return;
            }

            ReceiveMessages();

            TrimHosts();

            ReportStatus();
        }

        public void Shutdown()
        {
            if (serverPeer == null || serverPeer.Status == NetPeerStatus.NotRunning || serverPeer.Status == NetPeerStatus.ShutdownRequested)
            {
                return;
            }

            serverPeer.Shutdown("Done");

            Console.WriteLine("Master server shut down.");
        }

        public abstract string GameAppId { get; }
        public abstract string GeneralInfo { get; }
        public abstract bool ValidatePayload(string payload);
    }
}
