using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MonoGame.Effect
{
    internal partial class ConstantBufferData
    {
        public ConstantBufferData(SharpDX.D3DCompiler.ConstantBuffer cb)
        {
            Name = cb.Description.Name;
            Size = cb.Description.Size;

            ParameterIndex = new List<int>();

            var parameters = new List<EffectObject.d3dx_parameter>();

            // Gather all the parameters.
            for (var i = 0; i < cb.Description.VariableCount; i++)
            {
                var vdesc = cb.GetVariable(i);

                var param = GetParameterFromType(vdesc.GetVariableType());

                param.name = vdesc.Description.Name;
                param.semantic = string.Empty;
                param.bufferOffset = vdesc.Description.StartOffset;

                var size = param.columns * param.rows * 4;
                var data = new byte[size];

                if (vdesc.Description.DefaultValue != IntPtr.Zero)
                    Marshal.Copy(vdesc.Description.DefaultValue, data, 0, (int)size);

                param.data = data;

                parameters.Add(param);
            }

            // Sort them by the offset for some consistent results.
            Parameters = parameters.OrderBy(e => e.bufferOffset).ToList();

            // Store the parameter offsets.
            ParameterOffset = new List<int>();
            foreach (var param in Parameters)
                ParameterOffset.Add(param.bufferOffset);
        }

        private static EffectObject.d3dx_parameter GetParameterFromType(SharpDX.D3DCompiler.ShaderReflectionType type)
        {
            var param = new EffectObject.d3dx_parameter();
            param.rows = (uint)type.Description.RowCount;
            param.columns = (uint)type.Description.ColumnCount;
            param.name = type.Description.Name ?? string.Empty;
            param.semantic = string.Empty;
            param.bufferOffset = type.Description.Offset;

            switch (type.Description.Class)
            {
                case SharpDX.D3DCompiler.ShaderVariableClass.Scalar:
                    param.class_ = EffectObject.D3DXPARAMETER_CLASS.SCALAR;
                    break;

                case SharpDX.D3DCompiler.ShaderVariableClass.Vector:
                    param.class_ = EffectObject.D3DXPARAMETER_CLASS.VECTOR;
                    break;

                case SharpDX.D3DCompiler.ShaderVariableClass.MatrixColumns:
                    param.class_ = EffectObject.D3DXPARAMETER_CLASS.MATRIX_COLUMNS;
                    break;

                default:
                    throw new Exception("Unsupported parameter class!");
            }

            switch (type.Description.Type)
            {
                case SharpDX.D3DCompiler.ShaderVariableType.Bool:
                    param.type = EffectObject.D3DXPARAMETER_TYPE.BOOL;
                    break;

                case SharpDX.D3DCompiler.ShaderVariableType.Float:
                    param.type = EffectObject.D3DXPARAMETER_TYPE.FLOAT;
                    break;

                case SharpDX.D3DCompiler.ShaderVariableType.Int:
                    param.type = EffectObject.D3DXPARAMETER_TYPE.INT;
                    break;

                default:
                    throw new Exception("Unsupported parameter type!");
            }

            param.member_count = (uint)type.Description.MemberCount;
            param.element_count = (uint)type.Description.ElementCount;

            if (param.member_count > 0)
            {
                param.member_handles = new EffectObject.d3dx_parameter[param.member_count];
                for (var i = 0; i < param.member_count; i++)
                {
                    var mparam = GetParameterFromType(type.GetMemberType(i));
                    mparam.name = type.GetMemberTypeName(i) ?? string.Empty;
                    param.member_handles[i] = mparam;
                }
            }
            else
            {
                param.member_handles = CreateArrayFromParameter(param);
            }

            return param;
        }

        private static EffectObject.d3dx_parameter[] CreateArrayFromParameter(EffectObject.d3dx_parameter param)
        {
            var array = new EffectObject.d3dx_parameter[param.element_count];
            for (var i = 0; i < param.element_count; i++)
            {
                var mparam = new EffectObject.d3dx_parameter();

                mparam.name = string.Empty;
                mparam.semantic = string.Empty;
                mparam.type = param.type;
                mparam.class_ = param.class_;
                mparam.rows = param.rows;
                mparam.columns = param.columns;
                mparam.data = new byte[param.columns * param.rows * 4];

                array[i] = mparam;
            }

            return array;
        }
    }
}
