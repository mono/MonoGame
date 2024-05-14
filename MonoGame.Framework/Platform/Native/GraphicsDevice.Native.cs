// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Graphics;

public partial class GraphicsDevice
{
    private void PlatformSetup()
    {

    }

    private void PlatformInitialize()
    {
    }

    private void OnPresentationChanged()
    {
    }

    private void PlatformClear(ClearOptions options, Vector4 color, float depth, int stencil)
    {

    }

    private void PlatformDispose()
    {
    }

    private void PlatformPresent()
    {
    }

    private void PlatformSetViewport(ref Viewport value)
    {
    }

    private void PlatformApplyDefaultRenderTarget()
    {
    }

    private void PlatformResolveRenderTargets()
    {
        // Resolving MSAA render targets should be done here.
    }

    private IRenderTarget PlatformApplyRenderTargets()
    {
        return null;
    }

    private void PlatformBeginApplyState()
    {
    }

    private void PlatformApplyBlend()
    {
    }

    private void PlatformApplyState(bool applyShaders)
    {
    }

    private void PlatformDrawIndexedPrimitives(PrimitiveType primitiveType, int baseVertex, int startIndex, int primitiveCount)
    {
    }

    private void PlatformDrawUserPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, VertexDeclaration vertexDeclaration, int vertexCount) where T : struct
    {
    }

    private void PlatformDrawPrimitives(PrimitiveType primitiveType, int vertexStart, int vertexCount)
    {
    }

    private void PlatformDrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, short[] indexData, int indexOffset, int primitiveCount, VertexDeclaration vertexDeclaration) where T : struct
    {
    }

    private void PlatformDrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, int[] indexData, int indexOffset, int primitiveCount, VertexDeclaration vertexDeclaration) where T : struct
    {
    }

    private void PlatformDrawInstancedPrimitives(PrimitiveType primitiveType, int baseVertex, int startIndex, int primitiveCount, int baseInstance, int instanceCount)
    {
    }

    private void PlatformGetBackBufferData<T>(Rectangle? rect, T[] data, int startIndex, int count) where T : struct
    {
        throw new NotImplementedException();
    }

    private void PlatformDrawInstancedPrimitivesIndirect(PrimitiveType primitiveType, IndirectDrawBuffer indirectDrawBuffer, int alignedByteOffsetForArgs)
    {
    }

    private void PlatformDrawIndexedInstancedPrimitivesIndirect(PrimitiveType primitiveType, IndirectDrawBuffer indirectDrawBuffer, int alignedByteOffsetForArgs)
    {
    }

    private void PlatformDispatchCompute(int threadGroupCountX, int threadGroupCountY, int threadGroupCountZ)
    {
    }

    private void PlatformDispatchComputeIndirect(IndirectDrawBuffer indirectDrawBuffer, int alignedByteOffsetForArgs)
    {
    }

    private void CopyTextureDataInternal(Texture srcTexture, Texture dstTexture, int srcArrayIndex, int dstArrayIndex, int srcMipLevel, int dstMipLevel, int srcMipLevelCount, int dstMipLevelCount, int copyWidth, int copyHeight, int copyDepth, int srcOffsetX, int srcOffsetY, int srcOffsetZ, int dstOffsetX, int dstOffsetY, int dstOffsetZ)
    {
    }

    private void CopyBufferDataInternal(BufferResource scBuffer, BufferResource dstBuffer, int numBytesToCopy, int srcOffsetInBytes, int dstOffsetInBytes)
    {
    }

    private void CopyStructuredBufferCounterValueInternal(StructuredBuffer srcBuffer, BufferResource dstBuffer, int dstByteOffset)
    {
    }

    private static Rectangle PlatformGetTitleSafeArea(int x, int y, int width, int height)
    {
        return new Rectangle(x, y, width, height);
    }
}
