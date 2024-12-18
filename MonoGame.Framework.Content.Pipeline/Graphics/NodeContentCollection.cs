// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Provides a collection of child <see cref="NodeContent"/> objects for a content item.
    ///
    /// Links from a child object to its parent are maintained as the collection contents are modified.
    /// </summary>
    public class NodeContentCollection : ChildCollection<NodeContent, NodeContent>
    {
        /// <summary>
        /// Creates an instance of NodeContentCollection.
        /// </summary>
        /// <param name="parent">Parent object of the child objects returned in the collection.</param>
        internal NodeContentCollection(NodeContent parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Gets the parent of a child object.
        /// </summary>
        /// <param name="child">The child of the parent being retrieved.</param>
        /// <returns>The parent of the child object.</returns>
        protected override NodeContent GetParent(NodeContent child)
        {
            return child.Parent;
        }

        /// <summary>
        /// Modifies the value of the parent object of the specified child object.
        /// </summary>
        /// <param name="child">The child of the parent being modified.</param>
        /// <param name="parent">The new value for the parent object.</param>
        protected override void SetParent(NodeContent child, NodeContent parent)
        {
            child.Parent = parent;
        }
    }
}
