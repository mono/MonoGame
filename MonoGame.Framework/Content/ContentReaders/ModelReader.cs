// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class ModelReader : ContentTypeReader<Model>
	{
//      List<VertexBuffer> vertexBuffers = new List<VertexBuffer>();
//      List<IndexBuffer> indexBuffers = new List<IndexBuffer>();
//      List<Effect> effects = new List<Effect>();
//      List<GraphicsResource> sharedResources = new List<GraphicsResource>();

		public ModelReader ()
		{
		}
		
		static int ReadBoneReference(ContentReader reader, uint boneCount)
        {
            uint boneId;

            // Read the bone ID, which may be encoded as either an 8 or 32 bit value.
            if (boneCount < 255)
            {
                boneId = reader.ReadByte();
            }
            else
            {
                boneId = reader.ReadUInt32();
            }

            // Print out the bone ID.
            if (boneId != 0)
            {
                //Debug.WriteLine("bone #{0}", boneId - 1);
                return (int)(boneId - 1);
            }
            else
            {
                //Debug.WriteLine("null");
            }

            return -1;
        }
		
		protected internal override Model Read(ContentReader reader, Model existingInstance)
		{
            // Read the bone names and transforms.
            uint boneCount = reader.ReadUInt32();
            //Debug.WriteLine("Bone count: {0}", boneCount);

            List<ModelBone> bones = new List<ModelBone>((int)boneCount);

            for (uint i = 0; i < boneCount; i++)
            {
                string name = reader.ReadObject<string>();
				var matrix = reader.ReadMatrix();
                var bone = new ModelBone { Transform = matrix, Index = (int)i, Name = name };
                bones.Add(bone);
            }
			
            // Read the bone hierarchy.
            for (int i = 0; i < boneCount; i++)
            {
                var bone = bones[i];

                //Debug.WriteLine("Bone {0} hierarchy:", i);

                // Read the parent bone reference.
                //Debug.WriteLine("Parent: ");
                var parentIndex = ReadBoneReference(reader, boneCount);

                if (parentIndex != -1)
                {
                    bone.Parent = bones[parentIndex];
                }

                // Read the child bone references.
                uint childCount = reader.ReadUInt32();

                if (childCount != 0)
                {
                    //Debug.WriteLine("Children:");

                    for (uint j = 0; j < childCount; j++)
                    {
                        var childIndex = ReadBoneReference(reader, boneCount);
                        if (childIndex != -1)
                        {
                            bone.AddChild(bones[childIndex]);
                        }
                    }
                }
            }

            List<ModelMesh> meshes = new List<ModelMesh>();

            //// Read the mesh data.
            int meshCount = reader.ReadInt32();
            //Debug.WriteLine("Mesh count: {0}", meshCount);

            for (int i = 0; i < meshCount; i++)
            {

                //Debug.WriteLine("Mesh {0}", i);
                string name = reader.ReadObject<string>();
                var parentBoneIndex = ReadBoneReference(reader, boneCount);
				var boundingSphere = reader.ReadBoundingSphere();

                // Tag
                var meshTag = reader.ReadObject<object>();

                // Read the mesh part data.
                int partCount = reader.ReadInt32();
                //Debug.WriteLine("Mesh part count: {0}", partCount);

                List<ModelMeshPart> parts = new List<ModelMeshPart>(partCount);

                for (uint j = 0; j < partCount; j++)
                {
                    ModelMeshPart part;
                    if (existingInstance != null)
                        part = existingInstance.Meshes[i].MeshParts[(int)j];
                    else
                        part = new ModelMeshPart();

					part.VertexOffset = reader.ReadInt32();
                    part.NumVertices = reader.ReadInt32();
                    part.StartIndex = reader.ReadInt32();
                    part.PrimitiveCount = reader.ReadInt32();

                    // tag
                    part.Tag = reader.ReadObject<object>();
					
					parts.Add(part);
					
					int jj = (int)j;
					reader.ReadSharedResource<VertexBuffer>(delegate (VertexBuffer v)
					{
						parts[jj].VertexBuffer = v;
					});
					reader.ReadSharedResource<IndexBuffer>(delegate (IndexBuffer v)
					{
						parts[jj].IndexBuffer = v;
					});
					reader.ReadSharedResource<Effect>(delegate (Effect v)
					{
						parts[jj].Effect = v;
					});

					
                }

                if (existingInstance != null)
                    continue;

				ModelMesh mesh = new ModelMesh(reader.GetGraphicsDevice(), parts);

                // Tag reassignment
                mesh.Tag = meshTag;

				mesh.Name = name;
				mesh.ParentBone = bones[parentBoneIndex];
				mesh.ParentBone.AddMesh(mesh);
				mesh.BoundingSphere = boundingSphere;
				meshes.Add(mesh);
            }

            if (existingInstance != null)
            {
                // Read past remaining data and return existing instance
                ReadBoneReference(reader, boneCount);
                reader.ReadObject<object>();
                return existingInstance;
            }

            // Read the final pieces of model data.
            var rootBoneIndex = ReadBoneReference(reader, boneCount);

            Model model = new Model(reader.GetGraphicsDevice(), bones, meshes);

            model.Root = bones[rootBoneIndex];
		
			model.BuildHierarchy();
			
			// Tag?
            model.Tag = reader.ReadObject<object>();
			
			return model;
		}
	}
}

