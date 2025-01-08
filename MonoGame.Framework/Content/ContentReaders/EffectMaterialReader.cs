// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Utilities;

namespace Microsoft.Xna.Framework.Content
{
    [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
    internal class EffectMaterialReader : ContentTypeReader<EffectMaterial>
	{
		protected internal override EffectMaterial Read (ContentReader input, EffectMaterial existingInstance)
		{
			var effect = input.ReadExternalReference<Effect> ();
			var effectMaterial = new EffectMaterial (effect);

			var dict = input.ReadObject<Dictionary<string, object>> ();

			foreach (KeyValuePair<string, object> item in dict) {
				var parameter = effectMaterial.Parameters [item.Key];
				if (parameter != null) {

					Type itemType = item.Value.GetType();

					if (ReflectionHelpers.IsAssignableFromType(typeof(Texture), itemType)) {
						parameter.SetValue ((Texture)item.Value);
					}
					else if (ReflectionHelpers.IsAssignableFromType(typeof(int), itemType)) {
						parameter.SetValue((int) item.Value);
					}
					else if (ReflectionHelpers.IsAssignableFromType(typeof(int[]), itemType)) {
						parameter.SetValue((int[]) item.Value);
					}
					else if (ReflectionHelpers.IsAssignableFromType(typeof(bool), itemType)) {
						parameter.SetValue((bool) item.Value);
					}
					else if (ReflectionHelpers.IsAssignableFromType(typeof(float), itemType)) {
						parameter.SetValue((float) item.Value);
					}
					else if (ReflectionHelpers.IsAssignableFromType(typeof(float []), itemType)) {
						parameter.SetValue((float[]) item.Value);
					}
					else if (ReflectionHelpers.IsAssignableFromType(typeof(Vector2), itemType)) {
						parameter.SetValue((Vector2) item.Value);
					}
					else if (ReflectionHelpers.IsAssignableFromType(typeof(Vector2 []), itemType)) {
						parameter.SetValue((Vector2 []) item.Value);
					}
					else if (ReflectionHelpers.IsAssignableFromType(typeof(Vector3), itemType)) {
						parameter.SetValue((Vector3) item.Value);
					}
					else if (ReflectionHelpers.IsAssignableFromType(typeof(Vector3 []), itemType)) {
						parameter.SetValue((Vector3 []) item.Value);
					}
					else if (ReflectionHelpers.IsAssignableFromType(typeof(Vector4), itemType)) {
						parameter.SetValue((Vector4) item.Value);
					}
					else if (ReflectionHelpers.IsAssignableFromType(typeof(Vector4 []), itemType)) {
						parameter.SetValue((Vector4 []) item.Value);
					}
					else if (ReflectionHelpers.IsAssignableFromType(typeof(Matrix), itemType)) {
						parameter.SetValue((Matrix) item.Value);
					}
					else if (ReflectionHelpers.IsAssignableFromType(typeof(Matrix []), itemType)) {
						parameter.SetValue((Matrix[]) item.Value);
					}
					else if (ReflectionHelpers.IsAssignableFromType(typeof(Quaternion), itemType)) {
						parameter.SetValue((Quaternion) item.Value);
					}
					else {
						throw new NotSupportedException ("Parameter type is not supported");
					}
				} else {
					Debug.WriteLine ("No parameter " + item.Key);
				}
			}

			return effectMaterial;
		}
	}
}
