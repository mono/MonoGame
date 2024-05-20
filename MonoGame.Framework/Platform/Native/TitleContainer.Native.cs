// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.IO;

namespace Microsoft.Xna.Framework;

partial class TitleContainer
{
    static partial void PlatformInit()
    {

    }

    private static Stream PlatformOpenStream(string safeName)
    {
        // TODO: How do we handle this?
        //
        // It seems like we still need platform specific C# code here?
        //
        // Or do we make the native code transform the string?
        //
        var absolutePath = Path.Combine(Location, safeName);
        return File.OpenRead(absolutePath);
    }
}
