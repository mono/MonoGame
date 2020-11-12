// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Graphics
{
    internal partial class EffectResource
    {
#if IOS || ANDROID
        const string AlphaTestEffectName = "Microsoft.Xna.Framework.Platform.Graphics.Effect.Resources.AlphaTestEffect.ogles.mgfxo";
        const string BasicEffectName = "Microsoft.Xna.Framework.Platform.Graphics.Effect.Resources.BasicEffect.ogles.mgfxo";
        const string DualTextureEffectName = "Microsoft.Xna.Framework.Platform.Graphics.Effect.Resources.DualTextureEffect.ogles.mgfxo";
        const string EnvironmentMapEffectName = "Microsoft.Xna.Framework.Platform.Graphics.Effect.Resources.EnvironmentMapEffect.ogles.mgfxo";
        const string SkinnedEffectName = "Microsoft.Xna.Framework.Platform.Graphics.Effect.Resources.SkinnedEffect.ogles.mgfxo";
        const string SpriteEffectName = "Microsoft.Xna.Framework.Platform.Graphics.Effect.Resources.SpriteEffect.ogles.mgfxo";
#else
        const string AlphaTestEffectName = "Microsoft.Xna.Framework.Platform.Graphics.Effect.Resources.AlphaTestEffect.ogl.mgfxo";
        const string BasicEffectName = "Microsoft.Xna.Framework.Platform.Graphics.Effect.Resources.BasicEffect.ogl.mgfxo";
        const string DualTextureEffectName = "Microsoft.Xna.Framework.Platform.Graphics.Effect.Resources.DualTextureEffect.ogl.mgfxo";
        const string EnvironmentMapEffectName = "Microsoft.Xna.Framework.Platform.Graphics.Effect.Resources.EnvironmentMapEffect.ogl.mgfxo";
        const string SkinnedEffectName = "Microsoft.Xna.Framework.Platform.Graphics.Effect.Resources.SkinnedEffect.ogl.mgfxo";
        const string SpriteEffectName = "Microsoft.Xna.Framework.Platform.Graphics.Effect.Resources.SpriteEffect.ogl.mgfxo";
#endif
    }
}
