#region License
/*
MIT License
Copyright � 2006 The Mono.Xna Team

All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion License

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;

#if MONOMAC
using MonoMac.OpenGL;
#elif WINDOWS
using OpenTK.Graphics.OpenGL;
#else
#if ES11
using OpenTK.Graphics.ES11;
#else
using OpenTK.Graphics.ES20;
#endif
#endif

using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content
{
    internal class Texture2DReader : ContentTypeReader<Texture2D>
    {
		internal Texture2DReader()
		{
			// Do nothing
		}

#if ANDROID
        static string[] supportedExtensions = new string[] { ".jpg", ".bmp", ".jpeg", ".png", ".gif" };
#else
        static string[] supportedExtensions = new string[] { ".jpg", ".bmp", ".jpeg", ".png", ".gif", ".pict", ".tga" };
#endif

        internal static string Normalize(string fileName)
        {
            return Normalize(fileName, supportedExtensions);
        }

        protected internal override Texture2D Read(ContentReader reader, Texture2D existingInstance)
		{
			Texture2D texture = null;
			
			SurfaceFormat surfaceFormat;
			if (reader.version < 5) {
				SurfaceFormat_Legacy legacyFormat = (SurfaceFormat_Legacy)reader.ReadInt32 ();
				switch(legacyFormat) {
				case SurfaceFormat_Legacy.Dxt1:
					surfaceFormat = SurfaceFormat.Dxt1;
					break;
				case SurfaceFormat_Legacy.Dxt3:
					surfaceFormat = SurfaceFormat.Dxt3;
					break;
				case SurfaceFormat_Legacy.Dxt5:
					surfaceFormat = SurfaceFormat.Dxt5;
					break;
				case SurfaceFormat_Legacy.Color:
					surfaceFormat = SurfaceFormat.Color;
					break;
				default:
					throw new NotImplementedException();
				}
			}
            else
            {
				surfaceFormat = (SurfaceFormat)reader.ReadInt32 ();
			}
			
			int width = (reader.ReadInt32 ());
			int height = (reader.ReadInt32 ());
			int levelCount = (reader.ReadInt32 ());

			SurfaceFormat convertedFormat = surfaceFormat;
			switch (surfaceFormat)
			{
#if IPHONE || ANDROID
				case SurfaceFormat.Dxt1:
				case SurfaceFormat.Dxt3:
				case SurfaceFormat.Dxt5:
					convertedFormat = SurfaceFormat.Color;
					break;
#else
				//dxt formats don't need mipmaps set
				case SurfaceFormat.Dxt1:
				case SurfaceFormat.Dxt3:
				case SurfaceFormat.Dxt5:
					levelCount = 1;
					break;
#endif
				case SurfaceFormat.NormalizedByte4:
					convertedFormat = SurfaceFormat.Color;
					break;
			}
			
			texture = new Texture2D(reader.GraphicsDevice, width, height, levelCount > 1, convertedFormat);
			
			for (int level=0; level<levelCount; level++)
			{
				int levelDataSizeInBytes = (reader.ReadInt32 ());
				byte[] levelData = reader.ReadBytes (levelDataSizeInBytes);
                int levelWidth = width >> level;
                int levelHeight = height >> level;
				//Convert the image data if required
				switch(surfaceFormat) {
#if IPHONE || ANDROID
				//no Dxt in OpenGL ES
				case SurfaceFormat.Dxt1:
					levelData = DxtUtil.DecompressDxt1(levelData, levelWidth, levelHeight);
					break;
				case SurfaceFormat.Dxt3:
                    levelData = DxtUtil.DecompressDxt3(levelData, levelWidth, levelHeight);
					break;
				case SurfaceFormat.Dxt5:
                    levelData = DxtUtil.DecompressDxt5(levelData, levelWidth, levelHeight);
					break;
#endif
				case SurfaceFormat.NormalizedByte4:
                    int pitch = levelWidth * 4;
                    for (int y = 0; y < levelHeight; y++)
                    {
                        for (int x = 0; x < levelWidth; x++)
                        {
							int color = BitConverter.ToInt32(levelData, y*pitch+x*4);
							levelData[y*pitch+x*4]   = (byte)(((color >> 16) & 0xff)); //R:=W
							levelData[y*pitch+x*4+1] = (byte)(((color >> 8 ) & 0xff)); //G:=V
							levelData[y*pitch+x*4+2] = (byte)(((color      ) & 0xff)); //B:=U
							levelData[y*pitch+x*4+3] = (byte)(((color >> 24) & 0xff)); //A:=Q
						}
					}
					break;
				}
				
				texture.SetData(level, null, levelData, 0, levelData.Length);
				
			}
			
			return texture;
		}
    }
}
