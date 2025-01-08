// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class CurveReader : ContentTypeReader<Curve>
	{
		protected internal override Curve Read(ContentReader input, Curve existingInstance)
		{
			Curve curve = existingInstance;
			if (curve == null)
			{
				curve = new Curve();
			}         
			
			curve.PreLoop = (CurveLoopType)input.ReadInt32();
			curve.PostLoop = (CurveLoopType)input.ReadInt32();
			int num6 = input.ReadInt32();
			
			for (int i = 0; i < num6; i++)
			{
				float position = input.ReadSingle();
				float num4 = input.ReadSingle();
				float tangentIn = input.ReadSingle();
				float tangentOut = input.ReadSingle();
				CurveContinuity continuity = (CurveContinuity)input.ReadInt32();
				curve.Keys.Add(new CurveKey(position, num4, tangentIn, tangentOut, continuity));
			}		
			return curve;         
		}
	}
}

