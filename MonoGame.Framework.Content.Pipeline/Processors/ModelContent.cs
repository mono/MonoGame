// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Content.Pipeline.Processors
{
    /// <summary>
    /// Provides properties and methods for managing model content.
    /// </summary>
    public sealed class ModelContent
    {
        private ModelBoneContentCollection _bones;
        private ModelMeshContentCollection _meshes;
        private ModelBoneContent _root;

        internal ModelContent() { }

        internal ModelContent(ModelBoneContent root, IList<ModelBoneContent> bones, IList<ModelMeshContent> meshes)
        {
            _root = root;
            _bones = new ModelBoneContentCollection(bones);
            _meshes = new ModelMeshContentCollection(meshes);
        }

        /// <summary>
        /// Returns the bone content collection.
        /// </summary>
        public ModelBoneContentCollection Bones
        {
            get { return _bones; }
        }

        /// <summary>
        /// Returns the bone mesh collection.
        /// </summary>
        public ModelMeshContentCollection Meshes
        {
            get { return _meshes; }
        }

        /// <summary>
        /// Returns the root bone.
        /// </summary>
        public ModelBoneContent Root
        {
            get { return _root; }
        }

        /// <summary>
        /// Gets or sets an object that can be used to tag this model content.
        /// </summary>
        public object Tag { get; set; }
    }
}
