// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using SharpDX.Direct3D11;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class GraphicsDebug
    {
        private readonly GraphicsDevice _device;
        private readonly InfoQueue _infoQueue;
        private readonly Queue<GraphicsDebugMessage> _cachedMessages;
        private bool _hasPushedFilters = false;

        /// <summary>
        /// Creates a new instance of the <see cref="GraphicsDebug"/> class.
        /// </summary>
        /// <param name="device">The <see cref="GraphicsDevice"/> that the <see cref="GraphicsDebug"/> class is attached to.</param>
        public GraphicsDebug(GraphicsDevice device)
        {
            _device = device;
            _infoQueue = _device._d3dDevice.QueryInterfaceOrNull<InfoQueue>();
            _cachedMessages = new Queue<GraphicsDebugMessage>();

            if (_infoQueue != null)
            {
                _infoQueue.PushEmptyRetrievalFilter();
                _infoQueue.PushEmptyStorageFilter();
                _hasPushedFilters = true;
            }
        }

        private bool PlatformTryDequeueMessage(out GraphicsDebugMessage message)
        {
            if (_infoQueue == null)
            {
                message = null;
                return false;
            }

            if (!_hasPushedFilters)
            {
                _infoQueue.PushEmptyRetrievalFilter();
                _infoQueue.PushEmptyStorageFilter();
                _hasPushedFilters = true;
            }

            if (_cachedMessages.Count > 0)
            {
                message = _cachedMessages.Dequeue();
                return true;
            }

            if (_infoQueue.NumStoredMessagesAllowedByRetrievalFilter > 0)
            {
                // Grab all current messages and put them in the cached messages queue.
                for (var i = 0; i < _infoQueue.NumStoredMessagesAllowedByRetrievalFilter; i++)
                {
                    var dxMessage = _infoQueue.GetMessage(i);
                    _cachedMessages.Enqueue(new GraphicsDebugMessage
                    {
                        Message = dxMessage.Description,
                        Id = (int)dxMessage.Id,
                        IdName = dxMessage.Id.ToString(),
                        Severity = dxMessage.Severity.ToString(),
                        Category = dxMessage.Category.ToString()
                    });
                }

                _infoQueue.ClearStoredMessages();
            }

            if (_cachedMessages.Count > 0)
            {
                message = _cachedMessages.Dequeue();
                return true;
            }

            // No messages to grab from DirectX.
            message = null;
            return false;
        }
    }
}
