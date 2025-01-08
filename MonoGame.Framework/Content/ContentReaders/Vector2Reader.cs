// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content
{
    #if !NET45
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    #endif
    internal class Vector2Reader : ContentTypeReader<Vector2>
	{
		public Vector2Reader ()
		{
		}

		protected internal override Vector2 Read (ContentReader input, Vector2 existingInstance)
		{
			return input.ReadVector2 ();
		}
	}
}
