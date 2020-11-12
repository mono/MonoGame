using System.IO;

namespace MonoGame.Effect
{
    internal partial class ConstantBufferData
    {
        public void Write(BinaryWriter writer, Options options)
        {
            writer.Write(Name);
            writer.Write(InstanceName ?? "");

            writer.Write((ushort)Size);

            writer.Write((byte)ParameterIndex.Count);
            for (var i=0; i < ParameterIndex.Count; i++)
            {
                writer.Write((byte)ParameterIndex[i]);
                writer.Write((ushort)ParameterOffset[i]);
            }
        }
    }
}
