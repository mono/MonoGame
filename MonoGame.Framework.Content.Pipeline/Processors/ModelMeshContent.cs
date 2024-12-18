// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace Microsoft.Xna.Framework.Content.Pipeline.Processors
{
    /// <summary>
    /// Provides methods and properties for loading model mesh data.
    /// </summary>
    public sealed class ModelMeshContent
    {
        private BoundingSphere _boundingSphere;
        private ModelMeshPartContentCollection _meshParts;
        private string _name;
        private ModelBoneContent _parentBone;
        private MeshContent _sourceMesh;

        internal ModelMeshContent() { }

        internal ModelMeshContent(string name, MeshContent sourceMesh, ModelBoneContent parentBone,
                                  BoundingSphere boundingSphere, IList<ModelMeshPartContent> meshParts)
        {
            _name = name;
            _sourceMesh = sourceMesh;
            _parentBone = parentBone;
            _boundingSphere = boundingSphere;
            _meshParts = new ModelMeshPartContentCollection(meshParts);
        }

        /// <summary>
        /// Gets the bounding sphere of the mesh.
        /// </summary>
        public BoundingSphere BoundingSphere
        {
            get { return _boundingSphere; }
        }

        /// <summary>
        /// Gets the collection of mesh parts contained in this mesh.
        /// </summary>
        public ModelMeshPartContentCollection MeshParts
        {
            get { return _meshParts; }
        }

        /// <summary>
        /// Gets the name of the mesh.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the parent bone model.
        /// </summary>
        public ModelBoneContent ParentBone
        {
            get { return _parentBone; }
        }

        /// <summary>
        /// Gets the source mesh.
        /// </summary>
        public MeshContent SourceMesh
        {
            get { return _sourceMesh; }
        }

        /// <summary>
        /// Gets or sets the tag associated with the mesh.
        /// </summary>
        public object Tag { get; set; }
    }
}
