using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame.Effect
{
    internal partial class ConstantBufferData
    {
        public ConstantBufferData (ShaderConductor.UniformBuffer ub)
        {
            Name = ub.blockName;
            InstanceName = ub.instanceName;
            Size = ub.byteSize;

            ParameterIndex = new List<int>();
            var parameters = new List<EffectObject.d3dx_parameter>();

            // Gather all the parameters.
            foreach (ShaderConductor.Parameter p in ub.parameters)
            {
                var dxParam = CreateDXParamForConductorParam(p);
                parameters.Add(dxParam);
            }

            // Sort them by the offset for some consistent results.
            Parameters = parameters.OrderBy(e => e.bufferOffset).ToList();

            // Store the parameter offsets.
            ParameterOffset = new List<int>();
            foreach (var param in Parameters)
                ParameterOffset.Add(param.bufferOffset);
        }

        private static EffectObject.d3dx_parameter CreateDXParamForConductorParam(ShaderConductor.Parameter p)
        {
            var param = new EffectObject.d3dx_parameter();
            var size = p.rows * p.columns * 4;
            var data = new byte[size];

            param.rows = (uint)p.columns; // OpenGL to DirectX requires transpose
            param.columns = (uint)p.rows; // OpenGL to DirectX requires transpose
            param.name = p.name;
            param.semantic = string.Empty;
            param.bufferOffset = p.offset;
            param.semantic = string.Empty;
            param.data = data;
            param.member_count = 0;

            switch (p.type)
            {
                case 1: param.type = EffectObject.D3DXPARAMETER_TYPE.BOOL; break;
                case 2: param.type = EffectObject.D3DXPARAMETER_TYPE.INT; break;
                case 3: param.type = EffectObject.D3DXPARAMETER_TYPE.FLOAT; break;
                default:
                    throw new Exception("Unsupported parameter class!");
            }

            if (p.columns == 1)
            {
                param.class_ = p.rows == 1 ?
                    EffectObject.D3DXPARAMETER_CLASS.SCALAR :
                    EffectObject.D3DXPARAMETER_CLASS.VECTOR;
            }
            else
                param.class_ = EffectObject.D3DXPARAMETER_CLASS.MATRIX_COLUMNS;

            // Flatten the array, MonoGame uses 1D arrays  
            int elementCount = 1;
            for (int dim = 0; dim < p.arrayDimensions; dim++)
                elementCount *= p.arraySize[dim];

            param.element_count = elementCount > 1 ? (uint)elementCount : 0;

            if (param.member_count > 0)
            {
                param.member_handles = new EffectObject.d3dx_parameter[0];
                /*
                param.member_handles = new EffectObject.d3dx_parameter[param.member_count];
                for (var i = 0; i < param.member_count; i++)
                {
                    var mparam = GetParameterFromType(type.GetMemberType(i));
                    mparam.name = type.GetMemberTypeName(i) ?? string.Empty;
                    param.member_handles[i] = mparam;
                }*/
            }
            else
            {
                param.member_handles = CreateArrayFromParameter(param);
            }

            return param;
        }
    }
}
