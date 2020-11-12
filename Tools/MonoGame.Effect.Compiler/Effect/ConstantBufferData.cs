using System.Collections.Generic;

namespace MonoGame.Effect
{
    internal partial class ConstantBufferData
    {
        public string Name { get; private set; }

        public string InstanceName { get; private set; }

        public int Size { get; private set; }

        public List<int> ParameterIndex { get; private set; }

        public List<int> ParameterOffset { get; private set; }

        public List<EffectObject.d3dx_parameter> Parameters { get; private set; }

        public ConstantBufferData(string name)
        {
            Name = name;
            InstanceName = "";

            ParameterIndex = new List<int>();
            ParameterOffset = new List<int>();
            Parameters = new List<EffectObject.d3dx_parameter>();
            Size = 0;
        }

        public bool SameAs(ConstantBufferData other)
        {
            // If the names of the constant buffers don't
            // match then consider them different right off 
            // the bat... even if their parameters are the same.
            if (Name != other.Name)
                return false;

            // Do we have the same count of parameters and size?
            if (    Size != other.Size ||
                    Parameters.Count != other.Parameters.Count)
                return false;
            
            // Compare the parameters themselves.
            for (var i = 0; i < Parameters.Count; i++)
            {
                var p1 = Parameters[i];
                var p2 = other.Parameters[i];

                // Check the importaint bits.
                if (    p1.name != p2.name ||
                        p1.rows != p2.rows ||
                        p1.columns != p2.columns ||
                        p1.class_ != p2.class_ ||
                        p1.type != p2.type ||
                        p1.bufferOffset != p2.bufferOffset)
                    return false;
            }

            // These are equal.
            return true;
        }

    }
}
