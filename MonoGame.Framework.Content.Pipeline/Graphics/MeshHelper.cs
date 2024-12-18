using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Xna.Framework.Content.Pipeline.Graphics
{
    /// <summary>
    /// Class to provide helper methods for processing mesh data.
    /// </summary>
    public static class MeshHelper
    {
        static bool IsFinite(float v)
        {
            return !float.IsInfinity(v) && !float.IsNaN(v);
        }

        static bool IsFinite(this Vector3 v)
        {
            return IsFinite(v.X) && IsFinite(v.Y) && IsFinite(v.Z);
        }

        /// <summary>
        /// Generates vertex normals by accumulation of triangle face normals.
        /// </summary>
        /// <param name="mesh">The mesh which will receive the normals.</param>
        /// <param name="overwriteExistingNormals">Overwrite or skip over geometry with existing normals.</param>
        /// <remarks>
        /// This calls <see cref="CalculateNormals(GeometryContent, bool)"/> to do the work.
        /// </remarks>
        public static void CalculateNormals(MeshContent mesh, bool overwriteExistingNormals)
        {
            foreach (var geom in mesh.Geometry)
                CalculateNormals(geom, overwriteExistingNormals);
        }

        /// <summary>
        /// Generates vertex normals by accumulation of triangle face normals.
        /// </summary>
        /// <param name="geom">The geometry which will receive the normals.</param>
        /// <param name="overwriteExistingNormals">Overwrite or skip over geometry with existing normals.</param>
        /// <remarks>
        /// We use a "Mean Weighted Equally" method generate vertex normals from triangle
        /// face normals.  If normal cannot be calculated from the geometry we set it to zero.
        /// </remarks>
        public static void CalculateNormals(GeometryContent geom, bool overwriteExistingNormals)
        {
            VertexChannel<Vector3> channel;
            // Look for an existing normals channel.
            if (!geom.Vertices.Channels.Contains(VertexChannelNames.Normal()))
            {
                // We don't have existing normals, so add a new channel.
                channel = geom.Vertices.Channels.Add<Vector3>(VertexChannelNames.Normal(), null);
            }
            else
            {
                // If we're not supposed to overwrite the existing
                // normals then we're done here.
                if (!overwriteExistingNormals)
                    return;

                channel = geom.Vertices.Channels.Get<Vector3>(VertexChannelNames.Normal());
            }

            var positionIndices = geom.Vertices.PositionIndices;
            Debug.Assert(positionIndices.Count == channel.Count, "The position and channel sizes were different!");

            // Accumulate all the triangle face normals for each vertex.
            var normals = new Vector3[positionIndices.Count];
            for (var i = 0; i < geom.Indices.Count; i += 3)
            {
                var ia = geom.Indices[i + 0];
                var ib = geom.Indices[i + 1];
                var ic = geom.Indices[i + 2];

                var aa = geom.Vertices.Positions[ia];
                var bb = geom.Vertices.Positions[ib];
                var cc = geom.Vertices.Positions[ic];

                var faceNormal = Vector3.Cross(cc - bb, bb - aa);
                var len = faceNormal.Length();
                if (len > 0.0f)
                {
                    faceNormal = faceNormal / len;

                    // We are using the "Mean Weighted Equally" method where each
                    // face has an equal weight in the final normal calculation.
                    //
                    // We could maybe switch to "Mean Weighted by Angle" which is said
                    // to look best in most cases, but is more expensive to calculate.
                    //
                    // There is also an idea of weighting by triangle area, but IMO the
                    // triangle area doesn't always have a direct relationship to the
                    // shape of a mesh.
                    //
                    // For more ideas see:
                    //
                    // "A Comparison of Algorithms for Vertex Normal Computation"
                    // by Shuangshuang Jin, Robert R. Lewis, David West.
                    //

                    normals[ia] += faceNormal;
                    normals[ib] += faceNormal;
                    normals[ic] += faceNormal;
                }
            }

            // Normalize the gathered vertex normals.
            for (var i = 0; i < normals.Length; i++)
            {
                var normal = normals[i];
                var len = normal.Length();
                if (len > 0.0f)
                    normals[i] = normal / len;
                else
                {
                    // TODO: It would be nice to be able to log this to
                    // the pipeline so that it can be fixed in the model.

                    // TODO: We could maybe void this by a better algorithm
                    // above for generating the normals.

                    // We have a zero length normal.  You can argue that putting
                    // anything here is better than nothing, but by leaving it to
                    // zero it allows the caller to detect this and react to it.
                    normals[i] = Vector3.Zero;
                }
            }

            // Set the new normals on the vertex channel.
            for (var i = 0; i < channel.Count; i++)
                channel[i] = normals[i];
        }

        /// <summary>
        /// Generate the tangents and binormals (tangent frames) for each vertex in the mesh.
        /// </summary>
        /// <param name="mesh">The mesh which will have add tangent and binormal channels added.</param>
        /// <param name="textureCoordinateChannelName">The Vector2 texture coordinate channel used to generate tangent frames.</param>
        /// <param name="tangentChannelName"></param>
        /// <param name="binormalChannelName"></param>
        public static void CalculateTangentFrames(MeshContent mesh, string textureCoordinateChannelName, string tangentChannelName, string binormalChannelName)
        {
            foreach (var geom in mesh.Geometry)
                CalculateTangentFrames(geom, textureCoordinateChannelName, tangentChannelName, binormalChannelName);
        }

        /// <summary>
        /// Generate the tangents and binormals (tangent frames) for each vertex in the mesh geometry.
        /// </summary>
        /// <param name="geom">The mesh geometry which will have add tangent and binormal channels added.</param>
        /// <param name="textureCoordinateChannelName">The Vector2 texture coordinate channel used to generate tangent frames.</param>
        /// <param name="tangentChannelName"></param>
        /// <param name="binormalChannelName"></param>
        public static void CalculateTangentFrames(GeometryContent geom, string textureCoordinateChannelName, string tangentChannelName, string binormalChannelName)
        {
            if (!geom.Vertices.Channels.Contains(VertexChannelNames.Normal(0)))
            {
                return;
                // TODO: We could generate the normals here, but it's not working.
                //MeshHelper.CalculateNormals(geom, true);
            }

            var verts = geom.Vertices;
            var indices = geom.Indices;
            var channels = geom.Vertices.Channels;

            var normals = channels.Get<Vector3>(VertexChannelNames.Normal(0));
            var uvs = channels.Get<Vector2>(textureCoordinateChannelName);

            Vector3[] tangents, bitangents;
            CalculateTangentFrames(verts.Positions, indices, normals, uvs, out tangents, out bitangents);

            // All the indices are 1:1 with the others, so we
            // can just add the new channels in place.

            if (!string.IsNullOrEmpty(tangentChannelName))
                channels.Add(tangentChannelName, tangents);

            if (!string.IsNullOrEmpty(binormalChannelName))
                channels.Add(binormalChannelName, bitangents);
        }

        /// <summary>
        /// Calculates the tangent and bitangent vectors for a mesh.
        /// </summary>
        /// <param name="positions">The list of vertex positions.</param>
        /// <param name="indices">The list of indices that define the triangles in the mesh.</param>
        /// <param name="normals">The list of vertex normals.</param>
        /// <param name="textureCoords">The list of texture coordinates.</param>
        /// <param name="tangents">The output list of tangent vectors.</param>
        /// <param name="bitangents">The output list of bitangent vectors.</param>
        public static void CalculateTangentFrames(IList<Vector3> positions,
                                                  IList<int> indices,
                                                  IList<Vector3> normals,
                                                  IList<Vector2> textureCoords,
                                                  out Vector3[] tangents,
                                                  out Vector3[] bitangents)
        {
            // Lengyel, Eric. “Computing Tangent Space Basis Vectors for an Arbitrary Mesh”.
            // Terathon Software 3D Graphics Library, 2001.
            // http://www.terathon.com/code/tangent.html

            // Hegde, Siddharth. "Messing with Tangent Space". Gamasutra, 2007.
            // http://www.gamasutra.com/view/feature/129939/messing_with_tangent_space.php

            var numVerts = positions.Count;
            var numIndices = indices.Count;

            var tan1 = new Vector3[numVerts];
            var tan2 = new Vector3[numVerts];

            for (var index = 0; index < numIndices; index += 3)
            {
                var i1 = indices[index + 0];
                var i2 = indices[index + 1];
                var i3 = indices[index + 2];

                var w1 = textureCoords[i1];
                var w2 = textureCoords[i2];
                var w3 = textureCoords[i3];

                var s1 = w2.X - w1.X;
                var s2 = w3.X - w1.X;
                var t1 = w2.Y - w1.Y;
                var t2 = w3.Y - w1.Y;

                var denom = s1 * t2 - s2 * t1;
                if (Math.Abs(denom) < float.Epsilon)
                {
                    // The triangle UVs are zero sized one dimension.
                    //
                    // So we cannot calculate the vertex tangents for this
                    // one trangle, but maybe it can with other trangles.
                    continue;
                }

                var r = 1.0f / denom;
                Debug.Assert(IsFinite(r), "Bad r!");

                var v1 = positions[i1];
                var v2 = positions[i2];
                var v3 = positions[i3];

                var x1 = v2.X - v1.X;
                var x2 = v3.X - v1.X;
                var y1 = v2.Y - v1.Y;
                var y2 = v3.Y - v1.Y;
                var z1 = v2.Z - v1.Z;
                var z2 = v3.Z - v1.Z;

                var sdir = new Vector3()
                {
                    X = (t2 * x1 - t1 * x2) * r,
                    Y = (t2 * y1 - t1 * y2) * r,
                    Z = (t2 * z1 - t1 * z2) * r,
                };

                var tdir = new Vector3()
                {
                    X = (s1 * x2 - s2 * x1) * r,
                    Y = (s1 * y2 - s2 * y1) * r,
                    Z = (s1 * z2 - s2 * z1) * r,
                };

                tan1[i1] += sdir;
                Debug.Assert(tan1[i1].IsFinite(), "Bad tan1[i1]!");
                tan1[i2] += sdir;
                Debug.Assert(tan1[i2].IsFinite(), "Bad tan1[i2]!");
                tan1[i3] += sdir;
                Debug.Assert(tan1[i3].IsFinite(), "Bad tan1[i3]!");

                tan2[i1] += tdir;
                Debug.Assert(tan2[i1].IsFinite(), "Bad tan2[i1]!");
                tan2[i2] += tdir;
                Debug.Assert(tan2[i2].IsFinite(), "Bad tan2[i2]!");
                tan2[i3] += tdir;
                Debug.Assert(tan2[i3].IsFinite(), "Bad tan2[i3]!");
            }

            tangents = new Vector3[numVerts];
            bitangents = new Vector3[numVerts];

            // At this point we have all the vectors accumulated, but we need to average
            // them all out. So we loop through all the final verts and do a Gram-Schmidt
            // orthonormalize, then make sure they're all unit length.
            for (var i = 0; i < numVerts; i++)
            {
                var n = normals[i];
                Debug.Assert(n.IsFinite(), "Bad normal!");
                Debug.Assert(n.Length() >= 0.9999f, "Bad normal!");

                var t = tan1[i];
                if (t.LengthSquared() < float.Epsilon)
                {
                    // TODO: Ideally we could spit out a warning to the
                    // content logging here!

                    // We couldn't find a good tanget for this vertex.
                    //
                    // Rather than set them to zero which could produce
                    // errors in other parts of the pipeline, we just take
                    // a guess at something that may look ok.

                    t = Vector3.Cross(n, Vector3.UnitX);
                    if (t.LengthSquared() < float.Epsilon)
                        t = Vector3.Cross(n, Vector3.UnitY);

                    tangents[i] = Vector3.Normalize(t);
                    bitangents[i] = Vector3.Cross(n, tangents[i]);
                    continue;
                }

                // Gram-Schmidt orthogonalize
                // TODO: This can be zero can cause NaNs on
                // normalize... how do we fix this?
                var tangent = t - n * Vector3.Dot(n, t);
                tangent = Vector3.Normalize(tangent);
                Debug.Assert(tangent.IsFinite(), "Bad tangent!");
                tangents[i] = tangent;

                // Calculate handedness
                var w = (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0F) ? -1.0F : 1.0F;
                Debug.Assert(IsFinite(w), "Bad handedness!");

                // Calculate the bitangent
                var bitangent = Vector3.Cross(n, tangent) * w;
                Debug.Assert(bitangent.IsFinite(), "Bad bitangent!");
                bitangents[i] = bitangent;
            }
        }

        /// <summary>
        /// Search for the root bone of the skeletion.
        /// </summary>
        /// <param name="node">The node from which to begin the search for the skeleton.</param>
        /// <returns>The root bone of the skeletion or null if none is found.</returns>
        public static BoneContent FindSkeleton(NodeContent node)
        {
            // We should always get a node to search!
            if (node == null)
                throw new ArgumentNullException("node");

            // Search up thru the hierarchy.
            for (; node != null; node = node.Parent)
            {
                // First if this node is a bone then search up for the root.
                var root = node as BoneContent;
                if (root != null)
                {
                    while (root.Parent is BoneContent)
                        root = (BoneContent)root.Parent;
                    return root;
                }

                // Next try searching the children for a root bone.
                foreach (var nodeContent in node.Children)
                {
                    var bone = nodeContent as BoneContent;
                    if (bone == null)
                        continue;

                    // If we found a bone
                    if (root != null)
                        throw new InvalidContentException("DuplicateSkeleton", node.Identity);

                    // This is our new root.
                    root = bone;
                }

                // If we found a root bone then return it, else
                // we continue the search to the node parent.
                if (root != null)
                    return root;
            }

            // We didn't find any bones!
            return null;
        }

        /// <summary>
        /// Traverses a skeleton depth-first and builds a list of its bones.
        /// </summary>
        public static IList<BoneContent> FlattenSkeleton(BoneContent skeleton)
        {
            if (skeleton == null)
                throw new ArgumentNullException("skeleton");

            var results = new List<BoneContent>();
            var work = new Stack<NodeContent>(new[] { skeleton });
            while (work.Count > 0)
            {
                var top = work.Pop();
                var bone = top as BoneContent;
                if (bone != null)
                    results.Add(bone);

                for (var i = top.Children.Count - 1; i >= 0; i--)
                    work.Push(top.Children[i]);
            }

            return results;
        }

        /// <summary>
        /// Merge any positions in the <see cref="PositionCollection"/> of the
        /// specified mesh that are at a distance less than the specified tolerance
        /// from each other.
        /// </summary>
        /// <param name="mesh">Mesh to be processed.</param>
        /// <param name="tolerance">Tolerance value that determines how close
        /// positions must be to each other to be merged.</param>
        /// <remarks>
        /// This method will also update the <see cref="VertexContent.PositionIndices"/>
        /// in the <see cref="GeometryContent"/> of the specified mesh.
        /// </remarks>
        public static void MergeDuplicatePositions(MeshContent mesh, float tolerance)
        {
            if (mesh == null)
                throw new ArgumentNullException("mesh");

            // TODO Improve performance with spatial partitioning scheme
            var indexLists = new List<IndexUpdateList>();
            foreach (var geom in mesh.Geometry)
            {
                var list = new IndexUpdateList(geom.Vertices.PositionIndices);
                indexLists.Add(list);
            }

            for (var i = mesh.Positions.Count - 1; i >= 1; i--)
            {
                var pi = mesh.Positions[i];
                for (var j = i - 1; j >= 0; j--)
                {
                    var pj = mesh.Positions[j];
                    if (Vector3.Distance(pi, pj) <= tolerance)
                    {
                        foreach (var list in indexLists)
                            list.Update(i, j);
                        mesh.Positions.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// Merge vertices with the same <see cref="VertexContent.PositionIndices"/> and
        /// <see cref="VertexChannel"/> data within the specified
        /// <see cref="GeometryContent"/>.
        /// </summary>
        /// <param name="geometry">Geometry to be processed.</param>
        public static void MergeDuplicateVertices(GeometryContent geometry)
        {
            if (geometry == null)
                throw new ArgumentNullException("geometry");

            var verts = geometry.Vertices;
            var hashMap = new Dictionary<int, List<VertexData>>();

            var indices = new IndexUpdateList(geometry.Indices);
            var vIndex = 0;

            for (var i = 0; i < geometry.Indices.Count; i++)
            {
                var iIndex = geometry.Indices[i];
                var iData = new VertexData
                {
                    Index = iIndex,
                    PositionIndex = verts.PositionIndices[vIndex],
                    ChannelData = new object[verts.Channels.Count]
                };

                for (var channel = 0; channel < verts.Channels.Count; channel++)
                    iData.ChannelData[channel] = verts.Channels[channel][vIndex];

                var hash = iData.ComputeHash();

                var merged = false;
                List<VertexData> candidates;
                if (hashMap.TryGetValue(hash, out candidates))
                {
                    for (var candidateIndex = 0; candidateIndex < candidates.Count; candidateIndex++)
                    {
                        var c = candidates[candidateIndex];
                        if (!iData.ContentEquals(c))
                            continue;

                        // Match! Update the corresponding indices and remove the vertex
                        indices.Update(iIndex, c.Index);
                        verts.RemoveAt(vIndex);
                        merged = true;
                    }
                    if (!merged)
                        candidates.Add(iData);
                }
                else
                {
                    // no vertices with the same hash yet, create a new list for the data
                    hashMap.Add(hash, new List<VertexData> { iData });
                }

                if (!merged)
                    vIndex++;
            }

            // update the indices because of the vertices we removed
            indices.Pack();
        }

        /// <summary>
        /// Merge vertices with the same <see cref="VertexContent.PositionIndices"/> and
        /// <see cref="VertexChannel"/> data within the <see cref="MeshContent.Geometry"/>
        /// of this mesh. If you want to merge positions too, call
        /// <see cref="MergeDuplicatePositions"/> on your mesh before this function.
        /// </summary>
        /// <param name="mesh">Mesh to be processed</param>
        public static void MergeDuplicateVertices(MeshContent mesh)
        {
            if (mesh == null)
                throw new ArgumentNullException("mesh");
            foreach (var geom in mesh.Geometry)
                MergeDuplicateVertices(geom);
        }

        /// <summary>
        /// Optimizes the mesh for caching.
        /// </summary>
        /// <remarks>
        /// This method is not currently implemented.
        /// </remarks>
        /// <param name="mesh">Mesh data to optimize.</param>
        public static void OptimizeForCache(MeshContent mesh)
        {
            // We don't throw here as non-optimized still works.
        }

        /// <summary>
        /// Reverses the triangle winding order of the mesh.
        /// </summary>
        /// <param name="mesh">The mesh which will be modified.</param>
        /// <remarks>
        /// This method is useful when changing the direction of backface culling
        /// like when switching between left/right handed coordinate systems.
        /// </remarks>
        public static void SwapWindingOrder(MeshContent mesh)
        {
            // Gotta have a mesh to run!
            if (mesh == null)
                throw new ArgumentNullException("mesh");

            foreach (var geom in mesh.Geometry)
            {
                for (var i = 0; i < geom.Indices.Count; i += 3)
                {
                    var first = geom.Indices[i];
                    var last = geom.Indices[i+2];
                    geom.Indices[i] = last;
                    geom.Indices[i+2] = first;
                }
            }
        }

        /// <summary>
        /// Transforms the contents of a node and its descendants.
        /// </summary>
        /// <remarks>The node transforms themselves are unaffected.</remarks>
        /// <param name="scene">The root node of the scene to transform.</param>
        /// <param name="transform">The transform matrix to apply to the scene.</param>
        public static void TransformScene(NodeContent scene, Matrix transform)
        {
            if (scene == null)
                throw new ArgumentException("scene");

            // If the transformation is an identity matrix, this is a no-op and
            // we can save ourselves a bunch of work in the first place.
            if (transform == Matrix.Identity)
                return;

            var inverseTransform = Matrix.Invert(transform);

            var work = new Stack<NodeContent>();
            work.Push(scene);

            while (work.Count > 0)
            {
                var node = work.Pop();
                foreach (var child in node.Children)
                    work.Push(child);

                // Transform the mesh content.
                var mesh = node as MeshContent;
                if (mesh != null)
                    mesh.TransformContents(ref transform);

                // Transform local coordinate system using "similarity transform".
                node.Transform = inverseTransform * node.Transform * transform;

                // Transform animations.
                foreach (var animationContent in node.Animations.Values)
                    foreach (var animationChannel in animationContent.Channels.Values)
                        for (int i = 0; i < animationChannel.Count; i++)
                            animationChannel[i].Transform = inverseTransform * animationChannel[i].Transform * transform;
            }
        }

        /// <summary>
        /// Determines whether the specified transform is left-handed.
        /// </summary>
        /// <param name="xform">The transform.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="xform"/> is left-handed; otherwise,
        /// <see langword="false"/> if <paramref name="xform"/> is right-handed.
        /// </returns>
        internal static bool IsLeftHanded(ref Matrix xform)
        {
            // Check sign of determinant of upper-left 3x3 matrix:
            //   positive determinant ... right-handed
            //   negative determinant ... left-handed

            // Since XNA does not have a 3x3 matrix, use the "scalar triple product"
            // (see http://en.wikipedia.org/wiki/Triple_product) to calculate the
            // determinant.
            float d = Vector3.Dot(xform.Right, Vector3.Cross(xform.Forward, xform.Up));
            return d < 0.0f;
        }

        #region Private helpers

        private static void UpdatePositionIndices(MeshContent mesh, int from, int to)
        {
            foreach (var geom in mesh.Geometry)
            {
                for (var i = 0; i < geom.Vertices.PositionIndices.Count; i++)
                {
                    var index = geom.Vertices.PositionIndices[i];
                    if (index == from)
                        geom.Vertices.PositionIndices[i] = to;
                }
            }
        }

        private class VertexData
        {
            public int Index;
            public int PositionIndex;
            public object[] ChannelData;

            // Compute a hash based on PositionIndex and ChannelData
            public int ComputeHash()
            {
                var hash = PositionIndex;
                foreach (var channel in ChannelData)
                    hash ^= channel.GetHashCode();

                return hash;
            }

            // Check equality on PositionIndex and ChannelData
            public bool ContentEquals(VertexData other)
            {
                if (PositionIndex != other.PositionIndex)
                    return false;

                if (ChannelData.Length != other.ChannelData.Length)
                    return false;

                for (var i = 0; i < ChannelData.Length; i++)
                {
                    if (!Equals(ChannelData[i], other.ChannelData[i]))
                        return false;
                }

                return true;
            }
        }

        // takes an IndexCollection and can efficiently update index values
        private class IndexUpdateList
        {
            private readonly IList<int> _collectionToUpdate;
            private readonly Dictionary<int, List<int>> _indexPositions;

            // create the list, presort the values and compute the start positions of each value
            public IndexUpdateList(IList<int> collectionToUpdate)
            {
                _collectionToUpdate = collectionToUpdate;
                _indexPositions = new Dictionary<int, List<int>>();
                Initialize();
            }

            private void Initialize()
            {
                for (var pos = 0; pos < _collectionToUpdate.Count; pos++)
                {
                    var v = _collectionToUpdate[pos];
                    if (_indexPositions.ContainsKey(v))
                        _indexPositions[v].Add(pos);
                    else
                        _indexPositions.Add(v, new List<int> {pos});
                }
            }

            public void Update(int from, int to)
            {
                if (from == to || !_indexPositions.ContainsKey(from))
                    return;

                foreach (var pos in _indexPositions[from])
                    _collectionToUpdate[pos] = to;

                if (_indexPositions.ContainsKey(to))
                    _indexPositions[to].AddRange(_indexPositions[from]);
                else
                    _indexPositions.Add(to, _indexPositions[from]);

                _indexPositions.Remove(from);
            }

            // Pack all indices together starting from zero
            // E.g. [5, 5, 3, 5, 21, 3] -> [1, 1, 0, 1, 2, 0]
            // note that the order must be kept
            public void Pack()
            {
                if (_collectionToUpdate.Count == 0)
                    return;

                var sorted = new SortedSet<int>(_collectionToUpdate);

                var newIndex = 0;
                foreach (var value in sorted)
                {
                    foreach (var pos in _indexPositions[value])
                        _collectionToUpdate[pos] = newIndex;

                    newIndex++;
                }

            }

        }

        #endregion
    }
}
