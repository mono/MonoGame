#if DIRECTX
    using SharpDX.Direct3D11;
#endif

namespace Microsoft.Xna.Framework.Graphics
{
    public class ComputeSurface3D : ComputeSurface
    {
        internal Texture3D Texture { get; private set; }

        public ComputeSurface3D(GraphicsDevice graphics,int width,int height,int depth,bool mipmap,SurfaceFormat surfaceFormat)
        {
            Texture = new Texture3D(graphics, width, height, depth, mipmap, surfaceFormat,false,true); 
#if DIRECTX
            _uav = new UnorderedAccessView(Texture.GraphicsDevice._d3dDevice,Texture._texture);
#else
            Texture.Dispose();
            throw new NotImplementedException();
#endif
        }

        public override void Dispose()
        {
            Texture.Dispose();

            base.Dispose();
        }
    }
}
