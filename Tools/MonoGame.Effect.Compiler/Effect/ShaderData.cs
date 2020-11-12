using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Effect
{
	internal partial class ShaderData
	{
		public ShaderData(ShaderStage shaderStage, int sharedIndex, byte[] bytecode)
		{
            ShaderStage = shaderStage;
            SharedIndex = sharedIndex;
			Bytecode = (byte[])bytecode.Clone();	    
		}

        public ShaderStage ShaderStage { get; private set; }

        public struct Sampler
		{
			public MojoShader.MOJOSHADER_samplerType type;
			public int textureSlot;
            public int samplerSlot;
			public string samplerName;
			public string parameterName;
			public int parameter;
			public SamplerState state;
		}

		public struct Attribute
		{
            public string name;
            public VertexElementUsage usage;
			public int index;
#pragma warning disable 649
            public int location;
#pragma warning restore 649
        }

		/// <summary>
		/// The index to the constant buffers which are 
		/// required by this shader at runtime.
		/// </summary>
		public int[] _cbuffers;

		public Sampler[] _samplers;

		public Attribute[] _attributes;

		public byte[] ShaderCode { get; set; }


#region Non-Serialized Stuff

		public byte[] Bytecode { get; private set; }

		// The index of the shader in the shared list.
		public int SharedIndex { get; private set; }

#endregion // Non-Serialized Stuff

	}
}

