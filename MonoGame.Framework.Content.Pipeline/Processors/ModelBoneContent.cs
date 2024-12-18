// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content.Pipeline.Processors
{
    /// <summary>
    /// Provides properties and methods for managing model bone content.
    /// </summary>
    public sealed class ModelBoneContent
    {
        private ModelBoneContentCollection _children;
        private int _index;
        private string _name;
        private ModelBoneContent _parent;
        private Matrix _transform;

        internal ModelBoneContent() { }

        internal ModelBoneContent(string name, int index, Matrix transform, ModelBoneContent parent)
        {
            _name = name;
            _index = index;
            _transform = transform;
            _parent = parent;
        }

        /// <summary>
        /// Stores the children of this bone content.
        /// </summary>
        public ModelBoneContentCollection Children
        {
            get { return _children; }
            internal set { _children = value; }
        }

        /// <summary>
        /// Returns the index of this bone content.
        /// </summary>
        public int Index
        {
            get { return _index; }
        }

        /// <summary>
        /// Returns the name of this bone content.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Returns teh parent of this bone content.
        /// </summary>
        public ModelBoneContent Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// Returns or sets the transform matrix of this bone content.
        /// </summary>
        public Matrix Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }
    }
}
