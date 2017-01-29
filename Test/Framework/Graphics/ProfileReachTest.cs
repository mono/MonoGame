﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;


namespace MonoGame.Tests.Graphics
{
    [TestFixture]
    class ProfileReachTest
    {
        const GraphicsProfile TestedProfile = GraphicsProfile.Reach;
        const int PowerOfTwoSize = 32;
        const int MaxTexture2DSize = 2048;
        const int MaxTextureCubeSize = 512;
        const int MaxPrimitives = 65535;
        const int MaxRenderTargets = 1;

        private TestGameBase _game;
        private GraphicsDeviceManager _gdm;
        private GraphicsDevice _gd;


        [SetUp]
        public virtual void SetUp()
        {
            _game = new TestGameBase();
            _gdm = new GraphicsDeviceManager(_game);
            _gdm.GraphicsProfile = GraphicsProfile.Reach;
            _game.InitializeOnly();
            _gd = _gdm.GraphicsDevice;
        }

        [TearDown]
        public virtual void TearDown()
        {
            _game.Dispose();
            _game = null;
            _gdm = null;
            _gd = null;
        }
        
        /// <summary>
        /// Ensure we have the correct profile
        /// </summary>
        private void CheckProfile()
        {
            Assert.AreEqual(_gd.GraphicsProfile, TestedProfile);
        }


        [TestCase(MaxTexture2DSize,   MaxTexture2DSize,   false, SurfaceFormat.Color)]
        [TestCase(MaxTexture2DSize,   MaxTexture2DSize-1, false, SurfaceFormat.Color)]
        [TestCase(MaxTexture2DSize-1, MaxTexture2DSize  , false, SurfaceFormat.Color)]
        [TestCase(MaxTexture2DSize-1, MaxTexture2DSize-1, false, SurfaceFormat.Color)]
        [TestCase(MaxTexture2DSize,   MaxTexture2DSize+1, false, SurfaceFormat.Color, ExpectedException = typeof(NotSupportedException))]
        [TestCase(MaxTexture2DSize+1, MaxTexture2DSize,   false, SurfaceFormat.Color, ExpectedException = typeof(NotSupportedException))]
        [TestCase(MaxTexture2DSize+1, MaxTexture2DSize+1, false, SurfaceFormat.Color, ExpectedException = typeof(NotSupportedException))]
        [TestCase(PowerOfTwoSize,   PowerOfTwoSize,   true,  SurfaceFormat.Color)]
        [TestCase(PowerOfTwoSize,   PowerOfTwoSize-1, true,  SurfaceFormat.Color,  ExpectedException = typeof(NotSupportedException))]
        [TestCase(PowerOfTwoSize-1, PowerOfTwoSize  , true,  SurfaceFormat.Color,  ExpectedException = typeof(NotSupportedException))]
        [TestCase(PowerOfTwoSize-1, PowerOfTwoSize-1, true,  SurfaceFormat.Color,  ExpectedException = typeof(NotSupportedException))]
        [TestCase(PowerOfTwoSize,   PowerOfTwoSize,   false, SurfaceFormat.Dxt1 )]
        [TestCase(PowerOfTwoSize,   PowerOfTwoSize-4, false, SurfaceFormat.Dxt1,  ExpectedException = typeof(NotSupportedException))]
        [TestCase(PowerOfTwoSize-4, PowerOfTwoSize  , false, SurfaceFormat.Dxt1,  ExpectedException = typeof(NotSupportedException))]
        [TestCase(PowerOfTwoSize-4, PowerOfTwoSize-4, false, SurfaceFormat.Dxt1,  ExpectedException = typeof(NotSupportedException))]
        [TestCase(PowerOfTwoSize,   PowerOfTwoSize,   false, SurfaceFormat.Dxt3 )]
        [TestCase(PowerOfTwoSize,   PowerOfTwoSize-4, false, SurfaceFormat.Dxt3,  ExpectedException = typeof(NotSupportedException))]
        [TestCase(PowerOfTwoSize-4, PowerOfTwoSize  , false, SurfaceFormat.Dxt3,  ExpectedException = typeof(NotSupportedException))]
        [TestCase(PowerOfTwoSize-4, PowerOfTwoSize-4, false, SurfaceFormat.Dxt3,  ExpectedException = typeof(NotSupportedException))]      
        [TestCase(PowerOfTwoSize,   PowerOfTwoSize,   false, SurfaceFormat.Dxt5 )]
        [TestCase(PowerOfTwoSize,   PowerOfTwoSize-4, false, SurfaceFormat.Dxt5,  ExpectedException = typeof(NotSupportedException))]
        [TestCase(PowerOfTwoSize-4, PowerOfTwoSize  , false, SurfaceFormat.Dxt5,  ExpectedException = typeof(NotSupportedException))]
        [TestCase(PowerOfTwoSize-4, PowerOfTwoSize-4, false, SurfaceFormat.Dxt5,  ExpectedException = typeof(NotSupportedException))]
        public void Texture2DSize(int width, int height, bool mipMap, SurfaceFormat surfaceFormat = SurfaceFormat.Color)
        {
            CheckProfile();

            Texture2D tx = new Texture2D(_gd, width, height, mipMap, surfaceFormat);
            tx.Dispose();
        }
        
        [TestCase(MaxTextureCubeSize  )]
        [TestCase(MaxTextureCubeSize/2)]
        [TestCase(MaxTextureCubeSize*2, ExpectedException = typeof(NotSupportedException))]        
        [TestCase(MaxTextureCubeSize+1, ExpectedException = typeof(NotSupportedException))] // nonPowerOfTwo or maxSize
        [TestCase(MaxTextureCubeSize+4, ExpectedException = typeof(NotSupportedException))] // nonPowerOfTwo or maxSize
        [TestCase(PowerOfTwoSize-4,     ExpectedException = typeof(NotSupportedException))]
        public void TextureCubeSize(int size)
        {
            CheckProfile();
            
            TextureCube tx = new TextureCube(_gd, size, false, SurfaceFormat.Color);
            tx.Dispose();
        }
        
        [TestCase(16, 16, 16, ExpectedException = typeof(NotSupportedException))]
        public void Texture3DSize(int width, int height, int depth)
        {
            CheckProfile();
            
            Texture3D tx = new Texture3D(_gd, width, height, depth, false, SurfaceFormat.Color);            
            tx.Dispose();
        }
        
        [TestCase("DrawPrimitives", 0, MaxPrimitives)]
        [TestCase("DrawPrimitives", 3, MaxPrimitives)]
        [TestCase("DrawPrimitives", 0, MaxPrimitives+1, ExpectedException = typeof(NotSupportedException))]
        [TestCase("DrawIndexedPrimitives", 0, MaxPrimitives )]
        [TestCase("DrawIndexedPrimitives", 0, MaxPrimitives+1, ExpectedException = typeof(NotSupportedException))]
        [TestCase("DrawUserPrimitives", 0, MaxPrimitives )]
        [TestCase("DrawUserPrimitives", 3, MaxPrimitives)]
        [TestCase("DrawUserPrimitives", 0, MaxPrimitives+1, ExpectedException = typeof(NotSupportedException))]
        [TestCase("DrawUserIndexedPrimitives", 0, MaxPrimitives )]
        [TestCase("DrawUserIndexedPrimitives", 0, MaxPrimitives+1, ExpectedException = typeof(NotSupportedException))]
        public void MaximumPrimitivesPerDrawCall(string method, int vertexStart, int primitiveCount)
        {
            CheckProfile();

            int verticesCount = vertexStart + 3*primitiveCount;
            var effect = new BasicEffect(_gd);
            effect.CurrentTechnique.Passes[0].Apply();
            
            switch(method)
            {
                case "DrawPrimitives":
                    var vb = new VertexBuffer(_gd, VertexPositionColor.VertexDeclaration, verticesCount, BufferUsage.None);
                    _gd.SetVertexBuffer(vb);
                    _gd.DrawPrimitives(PrimitiveType.TriangleList, vertexStart, primitiveCount);
                    vb.Dispose();
                    break;
                case "DrawIndexedPrimitives":
                    var vb2 = new VertexBuffer(_gd, VertexPositionColor.VertexDeclaration, verticesCount, BufferUsage.None);
                    var ib2 = new IndexBuffer(_gd, IndexElementSize.SixteenBits, verticesCount, BufferUsage.None);
                    _gd.SetVertexBuffer(vb2);
                    _gd.Indices = ib2;
                    _gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, vertexStart, verticesCount, 0, primitiveCount);
                    vb2.Dispose();
                    ib2.Dispose();
                    break;
                case "DrawUserPrimitives":
                    var vertices = new VertexPositionColor[verticesCount];
                    _gd.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, vertexStart, primitiveCount);
                    break;
                case "DrawUserIndexedPrimitives":
                    var vertices2 = new VertexPositionColor[verticesCount];
                    var indices16bit = new short[verticesCount];
                    _gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices2, 0, vertices2.Length, indices16bit, 0, primitiveCount);
                    break;
                case "DrawUserIndexedPrimitives_32bit":
                    var vertices3 = new VertexPositionColor[verticesCount];
                    var indices32bit = new int[verticesCount];
                    _gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices3, 0, vertices3.Length, indices32bit, 0, primitiveCount);
                    break;
                default:
                    throw new ArgumentException("method");
            }
            
            effect.Dispose();
        }

        [TestCase(IndexElementSize.SixteenBits  )]
        [TestCase(IndexElementSize.ThirtyTwoBits, ExpectedException = typeof(NotSupportedException))]
        public void IndexBufferElementSize(IndexElementSize elementSize)
        {
            CheckProfile();

            IndexBuffer ib = new IndexBuffer(_gd, elementSize, 16, BufferUsage.None);
            ib.Dispose();
        }

        [TestCase(IndexElementSize.SixteenBits)]
        [TestCase(IndexElementSize.ThirtyTwoBits, ExpectedException = typeof(NotSupportedException))]
        public void IndicesElementSize(IndexElementSize elementSize)
        {
            CheckProfile();

            int verticesCount = 3 * 16;
            var effect = new BasicEffect(_gd);
            effect.CurrentTechnique.Passes[0].Apply();
            var vertices = new VertexPositionColor[verticesCount];

            switch (elementSize)
            {
                case IndexElementSize.SixteenBits:
                    var indices16bit = new short[3 * 16];
                    _gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices16bit, 0, 16);
                    break;
                case IndexElementSize.ThirtyTwoBits:
                    var indices32bit = new int[3 * 16];
                    _gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices32bit, 0, 16);
                    break;
                default:
                    throw new ArgumentException("method");
            }
            
            effect.Dispose();
        }
        
        [TestCase(ExpectedException = typeof(NotSupportedException))]
        public void OcclusionQuery()
        {
            CheckProfile();

            var oc = new OcclusionQuery(_gd);
            oc.Dispose();
        }

        [TestCase(MaxRenderTargets)]
        [TestCase(MaxRenderTargets + 1, ExpectedException = typeof(NotSupportedException))]
        public void MultipleRenderTargets(int count)
        {
            CheckProfile();
            
            var rtBinding = new RenderTargetBinding[count];
            for(int i=0;i<count;i++)
                rtBinding[i] = new RenderTargetBinding(new RenderTarget2D(_gd, PowerOfTwoSize, PowerOfTwoSize));

            _gd.SetRenderTargets(rtBinding);

            _gd.SetRenderTarget(null);
            for(int i=0;i<count;i++)
                rtBinding[i].RenderTarget.Dispose();
        }
    }
}
