﻿using System.Diagnostics;
using Microsoft.Xna.Framework.Net.Backend;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Net.Messages
{
    internal class AllowEndPointRequest : InternalMessage
    {
        public void Create(BasePeerEndPoint endPoint, NetworkMachine recipient)
        {
            if (!CurrentMachine.IsHost)
            {
                throw new NetworkException("Only host can send AllowEndPointRequest");
            }

            var msg = Backend.GetMessage(recipient?.peer, SendDataOptions.ReliableInOrder, 1);
            msg.Write((byte)InternalMessageIndex.AllowEndPointRequest);
            msg.Write(endPoint);
            Queue.Place(msg);
        }

        public override void Receive(BaseIncomingMessage msg, NetworkMachine senderMachine)
        {
            if (!senderMachine.IsHost)
            {
                // TODO: SuspiciousHostClaim
                Debug.Assert(false);
                return;
            }

            var endPoint = msg.ReadPeerEndPoint();

            CurrentMachine.Session.allowlist.Add(endPoint);

            CurrentMachine.Session.InternalMessages.AllowEndPointResponse.Create(endPoint, senderMachine);
        }
    }
}