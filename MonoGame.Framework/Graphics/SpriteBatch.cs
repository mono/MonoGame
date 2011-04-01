
using System;
using System.Text;
using System.Collections.Generic;
using OpenTK.Graphics.ES11;

using Microsoft.Xna.Framework;

namespace Microsoft.Xna.Framework.Graphics
{
	public class SpriteBatch : GraphicsResource
	{
		SpriteBatcher _batcher;
		
		SpriteSortMode _sortMode;
		BlendState _blendState;
		SamplerState _samplerState;
		DepthStencilState _depthStencilState; 
		RasterizerState _rasterizerState;		
		Effect _effect;		
		Matrix _matrix;

        public SpriteBatch ( GraphicsDevice graphicsDevice )
		{
			if (graphicsDevice == null )
			{
				throw new ArgumentException("graphicsDevice");
			}	
			
			this.graphicsDevice = graphicsDevice;
			
			_batcher = new SpriteBatcher();
		}
		
		public void Begin()
		{
			_sortMode = SpriteSortMode.Deferred;
			_blendState = BlendState.AlphaBlend;
			_depthStencilState = DepthStencilState.None;
			_samplerState = SamplerState.LinearClamp;
			_rasterizerState =  RasterizerState.CullCounterClockwise;
			_matrix = Matrix.Identity;
		}
		
		public void Begin(SpriteSortMode sortMode, BlendState blendState)
		{
			_sortMode = sortMode;
			_blendState = (blendState == null) ? BlendState.AlphaBlend : blendState;
			_depthStencilState = DepthStencilState.None;
			_samplerState = SamplerState.LinearClamp;
			_rasterizerState =  RasterizerState.CullCounterClockwise;
			_matrix = Matrix.Identity;
		}
		
		public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState )
		{
			_sortMode = sortMode;
			
			_blendState = (blendState == null) ? BlendState.AlphaBlend : blendState;
			_depthStencilState = (depthStencilState == null) ? DepthStencilState.None : depthStencilState;
			_samplerState = (samplerState == null) ? SamplerState.LinearClamp : samplerState;
			_rasterizerState =  (rasterizerState == null) ? RasterizerState.CullCounterClockwise : rasterizerState;
			
			_matrix = Matrix.Identity;
		}
		
		public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect)
		{
			_sortMode = sortMode;
			
			_blendState = (blendState == null) ? BlendState.AlphaBlend : blendState;
			_depthStencilState = (depthStencilState == null) ? DepthStencilState.None : depthStencilState;
			_samplerState = (samplerState == null) ? SamplerState.LinearClamp : samplerState;
			_rasterizerState =  (rasterizerState == null) ? RasterizerState.CullCounterClockwise : rasterizerState;
			
			_effect = effect;
		}
		
		public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix transformMatrix)
		{
			_sortMode = sortMode;
			
			_blendState = (blendState == null) ? BlendState.AlphaBlend : blendState;
			_depthStencilState = (depthStencilState == null) ? DepthStencilState.None : depthStencilState;
			_samplerState = (samplerState == null) ? SamplerState.LinearClamp : samplerState;
			_rasterizerState =  (rasterizerState == null) ? RasterizerState.CullCounterClockwise : rasterizerState;
			
			_effect = effect;
			_matrix = transformMatrix;
		}
		
		public void End()
		{			
			// set the blend mode
			if ( _blendState == BlendState.NonPremultiplied )
			{
				GL.Enable(All.Blend);
				GL.BlendFunc(All.One, All.OneMinusSrcAlpha);
			}
			
			if ( _blendState == BlendState.AlphaBlend )
			{
				GL.Enable(All.Blend);
				GL.BlendFunc(All.SrcAlpha, All.OneMinusSrcAlpha);
			}
			
			if ( _blendState == BlendState.Additive )
			{
				GL.Enable(All.Blend);
				GL.BlendFunc(All.SrcAlpha,All.One);
			}
			
			if ( _blendState == BlendState.Opaque )
			{
				GL.Disable(All.Blend);
			}			
			
			// set camera
			GL.MatrixMode(All.Projection);
			GL.LoadIdentity();							
			
			// Switch on the flags.
	        switch (this.graphicsDevice.PresentationParameters.DisplayOrientation)
	        {
				case DisplayOrientation.LandscapeLeft:
                {
					GL.Rotate(-90, 0, 0, 1); 
					GL.Ortho(0, this.graphicsDevice.Viewport.Height, this.graphicsDevice.Viewport.Width,  0, -1, 1);
					break;
				}
				
				case DisplayOrientation.LandscapeRight:
                {
					GL.Rotate(90, 0, 0, 1); 
					GL.Ortho(0, this.graphicsDevice.Viewport.Height, this.graphicsDevice.Viewport.Width,  0, -1, 1);
					break;
				}
				
			case DisplayOrientation.PortraitUpsideDown:
                {
					GL.Rotate(180, 0, 0, 1); 
					GL.Ortho(0, this.graphicsDevice.Viewport.Width, this.graphicsDevice.Viewport.Height,  0, -1, 1);
					break;
				}
				
				default:
				{
					GL.Ortho(0, this.graphicsDevice.Viewport.Width, this.graphicsDevice.Viewport.Height, 0, -1, 1);
					break;
				}
			}			
			
			// Enable Scissor Tests if necessary
			if ( this.graphicsDevice.RenderState.ScissorTestEnable )
			{
				GL.Enable(All.ScissorTest);				
			}
			
			GL.MatrixMode(All.Modelview);
			GL.LoadMatrix( ref _matrix.M11 );	
						
			GL.Viewport(0, 0, this.graphicsDevice.Viewport.Width, this.graphicsDevice.Viewport.Height);
			
			// Enable Scissor Tests if necessary
			if ( this.graphicsDevice.RenderState.ScissorTestEnable )
			{
				GL.Scissor(this.graphicsDevice.ScissorRectangle.X, this.graphicsDevice.ScissorRectangle.Y, this.graphicsDevice.ScissorRectangle.Width, this.graphicsDevice.ScissorRectangle.Height );
			}
			
			// Initialize OpenGL states (ideally move this to initialize somewhere else)	
			GL.Disable(All.DepthTest);
			GL.TexEnv(All.TextureEnv, All.TextureEnvMode,(int) All.BlendSrc);
			GL.Enable(All.Texture2D);
			GL.EnableClientState(All.VertexArray);
			GL.EnableClientState(All.ColorArray);
			GL.EnableClientState(All.TextureCoordArray);
			
			// Enable Culling for better performance
			GL.Enable(All.CullFace);
			GL.FrontFace(All.Cw);
			GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);						
			
			_batcher.DrawBatch ( _sortMode );
		}
		
		public void Draw 
			( 
			 Texture2D texture,
			 Vector2 position,
			 Nullable<Rectangle> sourceRectangle,
			 Color color,
			 float rotation,
			 Vector2 origin,
			 Vector2 scale,
			 SpriteEffects effect,
			 float depth 
			 )
		{
			if (texture == null )
			{
				throw new ArgumentException("texture");
			}
			
			SpriteBatchItem item = _batcher.CreateBatchItem();
			
			item.Depth = depth;
			item.TextureID = (int) texture.ID;
			
			Rectangle rect;
			if ( sourceRectangle.HasValue)
				rect = sourceRectangle.Value;
			else
				rect = new Rectangle( 0, 0, texture.Image.ImageWidth, texture.Image.ImageHeight );
						
			Vector2 texCoordTL = texture.Image.GetTextureCoord ( rect.X, rect.Y );
			Vector2 texCoordBR = texture.Image.GetTextureCoord ( rect.X+rect.Width, rect.Y+rect.Height );
			
			if ( effect == SpriteEffects.FlipVertically )
			{
				float temp = texCoordBR.Y;
				texCoordBR.Y = texCoordTL.Y;
				texCoordTL.Y = temp;
			}
			else if ( effect == SpriteEffects.FlipHorizontally )
			{
				float temp = texCoordBR.X;
				texCoordBR.X = texCoordTL.X;
				texCoordTL.X = temp;
			}
			
			item.Set
				(
				 position.X,
				 position.Y,
				 -origin.X*scale.X,
				 -origin.Y*scale.Y,
				 rect.Width*scale.X,
				 rect.Height*scale.Y,
				 (float)Math.Sin(rotation),
				 (float)Math.Cos(rotation),
				 color,
				 texCoordTL,
				 texCoordBR
				 );
		}
		
		public void Draw 
			( 
			 Texture2D texture,
			 Vector2 position,
			 Nullable<Rectangle> sourceRectangle,
			 Color color,
			 float rotation,
			 Vector2 origin,
			 float scale,
			 SpriteEffects effect,
			 float depth 
			 )
		{
			if (texture == null )
			{
				throw new ArgumentException("texture");
			}
			
			SpriteBatchItem item = _batcher.CreateBatchItem();
			
			item.Depth = depth;
			item.TextureID = (int) texture.ID;
			
			Rectangle rect;
			if ( sourceRectangle.HasValue)
				rect = sourceRectangle.Value;
			else
				rect = new Rectangle( 0, 0, texture.Image.ImageWidth, texture.Image.ImageHeight );
			
			Vector2 texCoordTL = texture.Image.GetTextureCoord ( rect.X, rect.Y );
			Vector2 texCoordBR = texture.Image.GetTextureCoord ( rect.X+rect.Width, rect.Y+rect.Height );
			
			if ( effect == SpriteEffects.FlipVertically )
			{
				float temp = texCoordBR.Y;
				texCoordBR.Y = texCoordTL.Y;
				texCoordTL.Y = temp;
			}
			else if ( effect == SpriteEffects.FlipHorizontally )
			{
				float temp = texCoordBR.X;
				texCoordBR.X = texCoordTL.X;
				texCoordTL.X = temp;
			}
			item.Set
				(
				 position.X,
				 position.Y,
				 -origin.X*scale,
				 -origin.Y*scale,
				 rect.Width*scale,
				 rect.Height*scale,
				 (float)Math.Sin(rotation),
				 (float)Math.Cos(rotation),
				 color,
				 texCoordTL,
				 texCoordBR
				 );
		}
		
		public void Draw (
         	Texture2D texture,
         	Rectangle destinationRectangle,
         	Nullable<Rectangle> sourceRectangle,
         	Color color,
         	float rotation,
         	Vector2 origin,
         	SpriteEffects effect,
         	float depth
			)
		{
			if (texture == null )
			{
				throw new ArgumentException("texture");
			}
			
			SpriteBatchItem item = _batcher.CreateBatchItem();
			
			item.Depth = depth;
			item.TextureID = (int) texture.ID;
			
			Rectangle rect;
			if ( sourceRectangle.HasValue)
				rect = sourceRectangle.Value;
			else
				rect = new Rectangle( 0, 0, texture.Image.ImageWidth, texture.Image.ImageHeight );

			Vector2 texCoordTL = texture.Image.GetTextureCoord ( rect.X, rect.Y );
			Vector2 texCoordBR = texture.Image.GetTextureCoord ( rect.X+rect.Width, rect.Y+rect.Height );
			if ( effect == SpriteEffects.FlipVertically )
			{
				float temp = texCoordBR.Y;
				texCoordBR.Y = texCoordTL.Y;
				texCoordTL.Y = temp;
			}
			else if ( effect == SpriteEffects.FlipHorizontally )
			{
				float temp = texCoordBR.X;
				texCoordBR.X = texCoordTL.X;
				texCoordTL.X = temp;
			}
			
			item.Set 
				( 
				 destinationRectangle.X, 
				 destinationRectangle.Y, 
				 -origin.X, 
				 -origin.Y, 
				 destinationRectangle.Width,
				 destinationRectangle.Height,
				 (float)Math.Sin(rotation),
				 (float)Math.Cos(rotation),
				 color,
				 texCoordTL,
				 texCoordBR );			
		}
		
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
		{
			if (texture == null )
			{
				throw new ArgumentException("texture");
			}
			
			SpriteBatchItem item = _batcher.CreateBatchItem();
			
			item.Depth = 0.0f;
			item.TextureID = (int) texture.ID;
			
			Rectangle rect;
			if ( sourceRectangle.HasValue)
				rect = sourceRectangle.Value;
			else
				rect = new Rectangle( 0, 0, texture.Image.ImageWidth, texture.Image.ImageHeight );
			
			Vector2 texCoordTL = texture.Image.GetTextureCoord ( rect.X, rect.Y );
			Vector2 texCoordBR = texture.Image.GetTextureCoord ( rect.X+rect.Width, rect.Y+rect.Height );
			
			item.Set ( position.X, position.Y, rect.Width, rect.Height, color, texCoordTL, texCoordBR );
		}
		
		public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
		{
			if (texture == null )
			{
				throw new ArgumentException("texture");
			}
			
			SpriteBatchItem item = _batcher.CreateBatchItem();
			
			item.Depth = 0.0f;
			item.TextureID = (int) texture.ID;
			
			Rectangle rect;
			if ( sourceRectangle.HasValue)
				rect = sourceRectangle.Value;
			else
				rect = new Rectangle( 0, 0, texture.Image.ImageWidth, texture.Image.ImageHeight );
			
			Vector2 texCoordTL = texture.Image.GetTextureCoord ( rect.X, rect.Y );
			Vector2 texCoordBR = texture.Image.GetTextureCoord ( rect.X+rect.Width, rect.Y+rect.Height );
			
			item.Set 
				( 
				 destinationRectangle.X, 
				 destinationRectangle.Y, 
				 destinationRectangle.Width, 
				 destinationRectangle.Height, 
				 color, 
				 texCoordTL, 
				 texCoordBR );
		}
		
		public void Draw 
			( 
			 Texture2D texture,
			 Vector2 position,
			 Color color
			 )
		{
			if (texture == null )
			{
				throw new ArgumentException("texture");
			}
			
			SpriteBatchItem item = _batcher.CreateBatchItem();
			
			item.Depth = 0;
			item.TextureID = (int) texture.ID;
			
			Rectangle rect = new Rectangle( 0, 0, texture.Image.ImageWidth, texture.Image.ImageHeight );
			
			Vector2 texCoordTL = texture.Image.GetTextureCoord ( rect.X, rect.Y );
			Vector2 texCoordBR = texture.Image.GetTextureCoord ( rect.X+rect.Width, rect.Y+rect.Height );
			
			item.Set 
				(
				 position.X,
			     position.Y,
				 rect.Width,
				 rect.Height,
				 color,
				 texCoordTL,
				 texCoordBR
				 );
		}
		
		public void Draw (Texture2D texture, Rectangle rectangle, Color color)
		{
			if (texture == null )
			{
				throw new ArgumentException("texture");
			}
			
			SpriteBatchItem item = _batcher.CreateBatchItem();
			
			item.Depth = 0;
			item.TextureID = (int) texture.ID;
			
			Vector2 texCoordTL = texture.Image.GetTextureCoord ( 0, 0 );
			Vector2 texCoordBR = texture.Image.GetTextureCoord ( texture.Image.ImageWidth, texture.Image.ImageHeight );
			
			item.Set
				(
				 rectangle.X,
				 rectangle.Y,
				 rectangle.Width,
				 rectangle.Height,
				 color,
				 texCoordTL,
				 texCoordBR
			    );
		}
		
		
		public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color)
		{
			if (spriteFont == null )
			{
				throw new ArgumentException("spriteFont");
			}
			
			Vector2 p = position;
			
            foreach (char c in text)
            {
                if (c == '\n')
                {
                    p.Y += spriteFont.LineSpacing;
                    p.X = position.X;
                    continue;
                }
                if (spriteFont.characterData.ContainsKey(c) == false) 
					continue;
                GlyphData g = spriteFont.characterData[c];
				
				SpriteBatchItem item = _batcher.CreateBatchItem();
				
				item.Depth = 0.0f;
				item.TextureID = (int) spriteFont._texture.ID;

				Vector2 texCoordTL = spriteFont._texture.Image.GetTextureCoord ( g.Glyph.X, g.Glyph.Y );
				Vector2 texCoordBR = spriteFont._texture.Image.GetTextureCoord ( g.Glyph.X+g.Glyph.Width, g.Glyph.Y+g.Glyph.Height );

				item.Set
					(
					 p.X,
					 p.Y+g.Cropping.Y,
					 g.Glyph.Width,
					 g.Glyph.Height,
					 color,
					 texCoordTL,
					 texCoordBR
					 );
		                
				p.X += (g.Kerning.Y + g.Kerning.Z + spriteFont.Spacing);
            }			
		}
		
		public void DrawString
			(
			SpriteFont spriteFont, 
			string text, 
			Vector2 position,
			Color color,
			float rotation,
			Vector2 origin,
			float scale,
			SpriteEffects effects,
			float depth
			)
		{
			if (spriteFont == null )
			{
				throw new ArgumentException("spriteFont");
			}
			
			Vector2 p = new Vector2(-origin.X,-origin.Y);
			
			float sin = (float)Math.Sin(rotation);
			float cos = (float)Math.Cos(rotation);
			
            foreach (char c in text)
            {
                if (c == '\n')
                {
                    p.Y += spriteFont.LineSpacing;
                    p.X = -origin.X;
                    continue;
                }
                if (spriteFont.characterData.ContainsKey(c) == false) 
					continue;
                GlyphData g = spriteFont.characterData[c];
				
				SpriteBatchItem item = _batcher.CreateBatchItem();
				
				item.Depth = depth;
				item.TextureID = (int) spriteFont._texture.ID;

				Vector2 texCoordTL = spriteFont._texture.Image.GetTextureCoord ( g.Glyph.X, g.Glyph.Y );
				Vector2 texCoordBR = spriteFont._texture.Image.GetTextureCoord ( g.Glyph.X+g.Glyph.Width, g.Glyph.Y+g.Glyph.Height );
				
				if ( effects == SpriteEffects.FlipVertically )
				{
					float temp = texCoordBR.Y;
					texCoordBR.Y = texCoordTL.Y;
					texCoordTL.Y = temp;
				}
				else if ( effects == SpriteEffects.FlipHorizontally )
				{
					float temp = texCoordBR.X;
					texCoordBR.X = texCoordTL.X;
					texCoordTL.X = temp;
				}
				
				item.Set
					(
					 position.X,
					 position.Y,
					 p.X*scale,
					 (p.Y+g.Cropping.Y)*scale,
					 g.Glyph.Width*scale,
					 g.Glyph.Height*scale,
					 sin,
					 cos,
					 color,
					 texCoordTL,
					 texCoordBR
					 );

				p.X += (g.Kerning.Y + g.Kerning.Z + spriteFont.Spacing);
            }			
		}
		
		public void DrawString
			(
			SpriteFont spriteFont, 
			string text, 
			Vector2 position,
			Color color,
			float rotation,
			Vector2 origin,
			Vector2 scale,
			SpriteEffects effects,
			float depth
			)
		{			
			if (spriteFont == null )
			{
				throw new ArgumentException("spriteFont");
			}
			
			Vector2 p = new Vector2(-origin.X,-origin.Y);
			
			float sin = (float)Math.Sin(rotation);
			float cos = (float)Math.Cos(rotation);
			
            foreach (char c in text)
            {
                if (c == '\n')
                {
                    p.Y += spriteFont.LineSpacing;
                    p.X = -origin.X;
                    continue;
                }
                if (spriteFont.characterData.ContainsKey(c) == false) 
					continue;
                GlyphData g = spriteFont.characterData[c];
				
				SpriteBatchItem item = _batcher.CreateBatchItem();
				
				item.Depth = depth;
				item.TextureID = (int) spriteFont._texture.ID;

				Vector2 texCoordTL = spriteFont._texture.Image.GetTextureCoord ( g.Glyph.X, g.Glyph.Y );
				Vector2 texCoordBR = spriteFont._texture.Image.GetTextureCoord ( g.Glyph.X+g.Glyph.Width, g.Glyph.Y+g.Glyph.Height );
				
				if ( effects == SpriteEffects.FlipVertically )
				{
					float temp = texCoordBR.Y;
					texCoordBR.Y = texCoordTL.Y;
					texCoordTL.Y = temp;
				}
				else if ( effects == SpriteEffects.FlipHorizontally )
				{
					float temp = texCoordBR.X;
					texCoordBR.X = texCoordTL.X;
					texCoordTL.X = temp;
				}
				
				item.Set
					(
					 position.X,
					 position.Y,
					 p.X*scale.X,
					 (p.Y+g.Cropping.Y)*scale.Y,
					 g.Glyph.Width*scale.X,
					 g.Glyph.Height*scale.Y,
					 sin,
					 cos,
					 color,
					 texCoordTL,
					 texCoordBR
					 );

				p.X += (g.Kerning.Y + g.Kerning.Z + spriteFont.Spacing);
            }			
		}
		
		public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color)
		{
			DrawString ( spriteFont, text.ToString(), position, color );
		}
		
		public void DrawString
			(
			SpriteFont spriteFont, 
			StringBuilder text, 
			Vector2 position,
			Color color,
			float rotation,
			Vector2 origin,
			float scale,
			SpriteEffects effects,
			float depth
			)
		{
			DrawString ( spriteFont, text.ToString(), position, color, rotation, origin, scale, effects, depth );
		}
		
		public void DrawString
			(
			SpriteFont spriteFont, 
			StringBuilder text, 
			Vector2 position,
			Color color,
			float rotation,
			Vector2 origin,
			Vector2 scale,
			SpriteEffects effects,
			float depth
			)
		{
			DrawString ( spriteFont, text.ToString(), position, color, rotation, origin, scale, effects, depth );
		}
	}
}

