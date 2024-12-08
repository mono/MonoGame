// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Graphics
{
    internal partial class ConstantBuffer : GraphicsResource
    {
        private readonly byte[] _buffer;

        private readonly int[] _parameters;

        private readonly int[] _offsets;

        private readonly string _name;

        private ulong _stateKey;

        private bool _dirty;
        private bool Dirty
        {
            get { return _dirty; }
        }

        public ConstantBuffer(ConstantBuffer cloneSource)
        {
            GraphicsDevice = cloneSource.GraphicsDevice;

            // Share the immutable types.
            _name = cloneSource._name;
            _parameters = cloneSource._parameters;
            _offsets = cloneSource._offsets;

            // Clone the mutable types.
            _buffer = (byte[])cloneSource._buffer.Clone();
            PlatformInitialize();
        }

        public ConstantBuffer(GraphicsDevice device,
                              int sizeInBytes,
                              int[] parameterIndexes,
                              int[] parameterOffsets,
                              string name)
        {
            GraphicsDevice = device;

            _buffer = new byte[sizeInBytes];

            _parameters = parameterIndexes;
            _offsets = parameterOffsets;

            _name = name;

            PlatformInitialize();
        }

        internal void Clear()
        {
            PlatformClear();
        }

        private void SetData(int offset, int rows, int columns, object data, ulong stateKey)
        {
            if (stateKey < _stateKey)
                return;
            _dirty = true;

            // Shader registers are always 4 bytes and all the
            // incoming data objects should be 4 bytes per element.
            const int elementSize = 4;
            const int rowSize = elementSize * 4;

            // Take care of a single element.
            if (rows == 1 && columns == 1)
            {
                // EffectParameter stores all values in arrays by default.             
                if (data is Array)
                    Buffer.BlockCopy(data as Array, 0, _buffer, offset, elementSize);
                else
                {
                    // TODO: When we eventually expose the internal Shader 
                    // API then we will need to deal with non-array elements.
                    throw new NotImplementedException();
                }
            }


            // Take care of the single copy case!
            else if (rows == 1 || (rows == 4 && columns == 4)) {
                Buffer.BlockCopy(data as Array, 0, _buffer, offset, rows*columns*elementSize);
            } else
            {
                var source = data as Array;

                var stride = (columns*elementSize);
                for (var y = 0; y < rows; y++)
                    Buffer.BlockCopy(source, stride*y, _buffer, offset + (rowSize*y), columns*elementSize);
            }
        }

        private int SetParameter(int offset, EffectParameter param)
        {
            const int elementSize = 4;
            const int rowSize = elementSize * 4;

            var rowsUsed = 0;

            var elements = param.Elements;
            if (elements.Count > 0)
            {
                for (var i=0; i < elements.Count; i++)
                {
                    var rowsUsedSubParam = SetParameter(offset, elements[i]);
                    offset += rowsUsedSubParam * rowSize;
                    rowsUsed += rowsUsedSubParam;
                }
            }
            else if (param.Data != null)
            {
                switch (param.ParameterType)
                {
                    case EffectParameterType.Single:
                    case EffectParameterType.Int32:
                    case EffectParameterType.Bool:
                        // HLSL assumes matrices are column-major, whereas in-memory we use row-major.
                        // TODO: HLSL can be told to use row-major. We should handle that too.
                        if (param.ParameterClass == EffectParameterClass.Matrix)
                        {
                            rowsUsed = param.ColumnCount;
                            SetData(offset, param.ColumnCount, param.RowCount, param.Data, param.StateKey);
                        }
                        else
                        {
                            rowsUsed = param.RowCount;
                            SetData(offset, param.RowCount, param.ColumnCount, param.Data, param.StateKey);
                        }
                        break;
                    default:
                        throw new NotSupportedException("Not supported!");
                }
            }

            return rowsUsed;
        }

        public void Update(EffectParameterCollection parameters)
        {
            // TODO:  We should be doing some sort of dirty state 
            // testing here.
            //
            // It should let us skip all parameter updates if
            // nothing has changed.  It should not be per-parameter
            // as that is why you should use multiple constant
            // buffers.

            // If our state key becomes larger than the 
            // next state key then the keys have rolled 
            // over and we need to reset.
            if (_stateKey > EffectParameter.NextStateKey)
                _stateKey = 0;
            
            for (var p = 0; p < _parameters.Length; p++)
            {
                var index = _parameters[p];
                var param = parameters[index];
                var offset = _offsets[p];
                SetParameter(offset, param);
            }

            _stateKey = EffectParameter.NextStateKey;
        }
    }
}
