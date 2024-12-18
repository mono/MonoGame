using System;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Provides methods for building a mesh for use in the Content Pipeline.
    /// </summary>
    public sealed class MeshBuilder
    {
        private readonly MeshContent _meshContent;

        private MaterialContent _currentMaterial;
        private OpaqueDataDictionary _currentOpaqueData;
        private bool _geometryDirty;
        private GeometryContent _currentGeometryContent;

        private readonly List<VertexChannel> _vertexChannels;
        private readonly List<object> _vertexChannelData;

        private bool _finishedCreation;
        private bool _finishedMesh;

        /// <summary>
        /// Gets or sets the current value for position merging of the mesh.
        /// </summary>
        public bool MergeDuplicatePositions { get; set; }

        /// <summary>
        /// Gets or sets the tolerance for <see cref="MergeDuplicatePositions"/>.
        /// </summary>
        public float MergePositionTolerance { get; set; }

        /// <summary>
        /// Gets or sets the name of the current  <see cref="MeshContent"/> object being processed.
        /// </summary>
        public string Name
        {
            get
            {
                return _meshContent.Name;
            }
            set
            {
                _meshContent.Name = value;
            }
        }

        /// <summary>
        /// Reverses the triangle winding order of the specified mesh.
        /// </summary>
        public bool SwapWindingOrder { get; set; }


        private MeshBuilder(string name)
        {
            _meshContent = new MeshContent();
            _vertexChannels = new List<VertexChannel>();
            _vertexChannelData = new List<object>();
            _currentGeometryContent = new GeometryContent();
            _currentOpaqueData = new OpaqueDataDictionary();
            _geometryDirty = true;
            Name = name;
        }

        /// <summary>
        /// Adds a vertex into the index collection.
        /// </summary>
        /// <param name="indexIntoVertexCollection">Index of the inserted vertex, in the collection.
        /// This corresponds to the value returned by <see cref="CreatePosition(float,float,float)"/>.</param>
        public void AddTriangleVertex(int indexIntoVertexCollection)
        {
            if (_finishedMesh)
                throw new InvalidOperationException( "This MeshBuilder can no longer be used because FinishMesh has been called.");

            _finishedCreation = true;

            if (_geometryDirty)
            {
                _currentGeometryContent = new GeometryContent();
                _currentGeometryContent.Material = _currentMaterial;
                foreach (var kvp in _currentOpaqueData)
                    _currentGeometryContent.OpaqueData.Add(kvp.Key, kvp.Value);

                // we have to copy our vertex channels to the new geometry
                foreach (var channel in _vertexChannels)
                {
                    _currentGeometryContent.Vertices.Channels.Add(channel.Name, channel.ElementType, null);
                }
                _meshContent.Geometry.Add(_currentGeometryContent);
                _geometryDirty = false;

            }
            // Add the vertex to the mesh and then add the vertex position to the indices list
            var pos = _currentGeometryContent.Vertices.Add(indexIntoVertexCollection);

            // Then add the data for the other channels
            for (var i = 0; i < _vertexChannels.Count; i++)
            {
                var channel = _currentGeometryContent.Vertices.Channels[i];
                var data = _vertexChannelData[i];
                if (data == null)
                    throw new InvalidOperationException(string.Format("Missing vertex channel data for channel {0}", channel.Name));

                channel.Items.Add(data);
            }

            _currentGeometryContent.Indices.Add(pos);
        }

        /// <summary>
        /// Creates a new vertex channel with the specified usage and adds it to the mesh builder.
        /// </summary>
        /// <typeparam name="T">The type of data to be stored in the vertex channel.</typeparam>
        /// <param name="usage">The usage of the vertex channel.</param>
        /// <returns>The new count of vertex channels.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the mesh builder has already been finished.</exception>
        public int CreateVertexChannel<T>(string usage)
        {
            if (_finishedMesh)
                throw new InvalidOperationException("This MeshBuilder can no longer be used because FinishMesh has been called.");
            if (_finishedCreation)
                throw new InvalidOperationException("Functions starting with 'Create' must be called before calling AddTriangleVertex");

            var channel = new VertexChannel<T>(usage);
            _vertexChannels.Add(channel);
            _vertexChannelData.Add(default(T));

            _currentGeometryContent.Vertices.Channels.Add<T>(usage, null);

            return _vertexChannels.Count - 1;
        }

        /// <summary>
        /// Inserts the specified vertex position into the vertex channel.
        /// </summary>
        /// <param name="x">Value of the x component of the vector.</param>
        /// <param name="y">Value of the y component of the vector.</param>
        /// <param name="z">Value of the z component of the vector.</param>
        /// <returns>Index of the inserted vertex.</returns>
        public int CreatePosition(float x, float y, float z)
        {
            return CreatePosition(new Vector3(x, y, z));
        }

        /// <summary>
        /// Inserts the specified vertex position into the vertex channel at the specified index.
        /// </summary>
        /// <param name="pos">Value of the vertex being inserted.</param>
        /// <returns>Index of the vertex being inserted.</returns>
        public int CreatePosition(Vector3 pos)
        {
            if (_finishedMesh)
                throw new InvalidOperationException( "This MeshBuilder can no longer be used because FinishMesh has been called.");
            if (_finishedCreation)
                throw new InvalidOperationException("Functions starting with 'Create' must be called before calling AddTriangleVertex");

            _meshContent.Positions.Add(pos);
            return _meshContent.Positions.Count - 1;
        }

        /// <summary>
        /// Ends the creation of a mesh.
        /// </summary>
        /// <returns>Resultant mesh.</returns>
        public MeshContent FinishMesh()
        {
            if (_finishedMesh)
                return _meshContent;

            if (MergeDuplicatePositions)
                MeshHelper.MergeDuplicatePositions(_meshContent, MergePositionTolerance);

            MeshHelper.MergeDuplicateVertices(_meshContent);

            MeshHelper.CalculateNormals(_meshContent, false);
            if (SwapWindingOrder)
                MeshHelper.SwapWindingOrder(_meshContent);

            _finishedMesh = true;
            return _meshContent;
        }

        /// <summary>
        /// Sets the material for the next triangles.
        /// </summary>
        /// <param name="material">Material for the next triangles.</param>
        /// <remarks>
        /// Sets the material for the triangles being defined next. This material
        /// and the opaque data dictionary, set with <see cref="SetOpaqueData"/>
        /// define the <see cref="GeometryContent"/>  object containing the next
        /// triangles. When you set a new material or opaque data dictionary the
        /// triangles you add afterwards will belong to a new
        /// <see cref="GeometryContent"/> object.
        /// </remarks>
        public void SetMaterial(MaterialContent material)
        {
            if (_currentMaterial == material)
                return;

            _currentMaterial = material;
            _geometryDirty = true;
        }

        /// <summary>
        /// Sets the opaque data for the next triangles.
        /// </summary>
        /// <param name="opaqueData">Opaque data dictionary for the next triangles.</param>
        /// <remarks>
        /// Sets the opaque data dictionary for the triangles being defined next. This dictionary
        /// and the material, set with <see cref="SetMaterial"/>, define the <see cref="GeometryContent"/>
        /// object containing the next triangles. When you set a new material or opaque data dictionary
        /// the triangles you add afterwards will belong to a new <see cref="GeometryContent"/> object.
        /// </remarks>
        public void SetOpaqueData(OpaqueDataDictionary opaqueData)
        {
            if (_currentOpaqueData == opaqueData)
                return;

            _currentOpaqueData = opaqueData;
            _geometryDirty = true;
        }

        /// <summary>
        /// Sets the specified vertex data with new data.
        /// </summary>
        /// <param name="vertexDataIndex">Index of the vertex data channel being set. This should match the index returned by CreateVertexChannel.</param>
        /// <param name="channelData">New data values for the vertex data. The data type being set must match the data type for the vertex channel specified by vertexDataIndex.</param>
        public void SetVertexChannelData(int vertexDataIndex, object channelData)
        {
            if (_currentGeometryContent.Vertices.Channels[vertexDataIndex].ElementType != channelData.GetType())
                throw new InvalidOperationException(string.Format("Channel {0} data has a different type from input. Expected: {1}. Actual: {2}",
                    vertexDataIndex, _currentGeometryContent.Vertices.Channels[vertexDataIndex].ElementType, channelData.GetType()));

            _vertexChannelData[vertexDataIndex] = channelData;
        }

        /// <summary>
        /// Initializes the creation of a mesh.
        /// </summary>
        /// <param name="name">Name of the mesh.</param>
        /// <returns>Object used when building the mesh.</returns>
        public static MeshBuilder StartMesh(string name)
        {
            return new MeshBuilder(name);
        }
    }
}
