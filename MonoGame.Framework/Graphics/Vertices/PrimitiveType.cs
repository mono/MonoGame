// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Graphics
{
    /// <summary>
    /// Defines how vertex data is ordered.
    /// </summary>
	public enum PrimitiveType
	{
        /// <summary>
        /// Renders the specified vertices as a sequence of isolated triangles. Each group of three vertices defines a separate triangle. Back-face culling is affected by the current winding-order render state.
        /// </summary>
		TriangleList,

        /// <summary>
        /// Renders the vertices as a triangle strip. The back-face culling flag is flipped automatically on even-numbered triangles.
        /// </summary>
		TriangleStrip,

        /// <summary>
        /// Renders the vertices as a list of isolated straight line segments; the count may be any positive integer.
        /// </summary>
		LineList,

        /// <summary>
        /// Renders the vertices as a single polyline; the count may be any positive integer.
        /// </summary>
		LineStrip,

        /// <summary>
        /// Interprets the vertices as individual points. Must be used in combination with a geometry shader that expands the points into primitives.
        /// </summary>
        PointList,

        /// <summary>
        /// Like LineList but includes adjacent vertices for geometry shaders.
        /// </summary>
        LineListWithAdjacency,

        /// <summary>
        /// Like LineStrip but includes adjacent vertices for geometry shaders.
        /// </summary>
        LineStripWithAdjacency,

        /// <summary>
        /// Like TriangleList but includes adjacent vertices for geometry shaders.
        /// </summary>
        TriangleListWithAdjacency,

        /// <summary>
        /// Like TriangleStrip but includes adjacent vertices for geometry shaders.
        /// </summary>
        TriangleStripWithAdjacency,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 1 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith1ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 2 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith2ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 3 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith3ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 4 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith4ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 5 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith5ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 6 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith6ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 7 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith7ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 8 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith8ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 9 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith9ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 10 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith10ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 11 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith11ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 12 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith12ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 13 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith13ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 14 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith14ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 15 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith15ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 16 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith16ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 17 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith17ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 18 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith18ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 19 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith19ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 20 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith20ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 21 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith21ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 22 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith22ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 23 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith23ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 24 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith24ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 25 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith25ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 26 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith26ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 27 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith27ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 28 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith28ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 29 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith29ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 30 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith30ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 31 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith31ControlPoints,

        /// <summary>
        /// Interpretes the vertices as a list of patches. Each group of 32 vertices defines a patch. Must be used in combination with a hull shader.  
        /// </summary>
        PatchListWith32ControlPoints,
    }
}
