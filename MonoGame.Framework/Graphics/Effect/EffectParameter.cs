using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.Xna.Framework.Graphics
{
    [DebuggerDisplay("{DebugDisplayString}")]
    public class EffectParameter
    {
        /// <summary>
        /// The next state key used when an effect parameter
        /// is updated by any of the 'set' methods.
        /// </summary>
        internal static ulong NextStateKey { get; private set; }

        internal EffectParameter(   EffectParameterClass class_,
                                    EffectParameterType type,
                                    string name,
                                    int rowCount,
                                    int columnCount,
                                    string semantic,
                                    EffectAnnotationCollection annotations,
                                    EffectParameterCollection elements,
                                    EffectParameterCollection structMembers,
                                    object data,
                                    bool mojoDataLayout)
        {
            ParameterClass = class_;
            ParameterType = type;

            Name = name;
            Semantic = semantic;
            Annotations = annotations;

            RowCount = rowCount;
            ColumnCount = columnCount;

            Elements = elements;
            StructureMembers = structMembers;

            Data = data;
            StateKey = unchecked(NextStateKey++);

            MojoDataLayout = mojoDataLayout;
        }

        internal EffectParameter(EffectParameter cloneSource)
        {
            // Share all the immutable types.
            ParameterClass = cloneSource.ParameterClass;
            ParameterType = cloneSource.ParameterType;
            Name = cloneSource.Name;
            Semantic = cloneSource.Semantic;
            Annotations = cloneSource.Annotations;
            RowCount = cloneSource.RowCount;
            ColumnCount = cloneSource.ColumnCount;
            MojoDataLayout = cloneSource.MojoDataLayout;

            // Clone the mutable types.
            Elements = cloneSource.Elements.Clone();
            StructureMembers = cloneSource.StructureMembers.Clone();

            // The data is mutable, so we have to clone it.
            var array = cloneSource.Data as Array;
            if (array != null)
                Data = array.Clone();
            StateKey = unchecked(NextStateKey++);
        }

        public string Name { get; private set; }

        public string Semantic { get; private set; }

        public EffectParameterClass ParameterClass { get; private set; }

        public EffectParameterType ParameterType { get; private set; }

        public int RowCount { get; private set; }

        public int ColumnCount { get; private set; }

        public EffectParameterCollection Elements { get; private set; }

        public EffectParameterCollection StructureMembers { get; private set; }

        public EffectAnnotationCollection Annotations { get; private set; }


        // TODO: Using object adds alot of boxing/unboxing overhead
        // and garbage generation.  We should consider a templated
        // type implementation to fix this!

        internal object Data { get; private set; }

        /// <summary>
        /// The current state key which is used to detect
		/// if the parameter value has been changed.
        /// </summary>
        internal ulong StateKey { get; private set; }

        internal bool MojoDataLayout;

        /// <summary>
        /// Property referenced by the DebuggerDisplayAttribute.
        /// </summary>
        private string DebugDisplayString
        {
            get
            {
                var semanticStr = string.Empty;
                if (!string.IsNullOrEmpty(Semantic))
                    semanticStr = string.Concat(" <", Semantic, ">");

                return string.Concat("[", ParameterClass, " ", ParameterType, "]", semanticStr, " ", Name, " : ", GetDataValueString());
            }
        }

        private string GetDataValueString()
        {
            string valueStr;

            if (Data == null)
            {
                if (Elements == null)
                    valueStr = "(null)";
                else
                    valueStr = string.Join(", ", Elements.Select(e => e.GetDataValueString()));
            }
            else
            {
                switch (ParameterClass)
                {
                    // Object types are stored directly in the Data property.
                    // Display Data's string value.
                    case EffectParameterClass.Object:
                        valueStr = Data.ToString();
                        break;

                    // Matrix types are stored in a float[16] which we don't really have room for.
                    // Display "...".
                    case EffectParameterClass.Matrix:
                        valueStr = "...";
                        break;

                    // Scalar types are stored as a float[1].
                    // Display the first (and only) element's string value.                    
                    case EffectParameterClass.Scalar:
                        valueStr = (Data as Array).GetValue(0).ToString();
                        break;

                    // Vector types are stored as an Array<Type>.
                    // Display the string value of each array element.
                    case EffectParameterClass.Vector:
                        var array = Data as Array;
                        var arrayStr = new string[array.Length];
                        var idx = 0;
                        foreach (var e in array)
                        {
                            arrayStr[idx] = array.GetValue(idx).ToString();
                            idx++;
                        }

                        valueStr = string.Join(" ", arrayStr);
                        break;

                    // Handle additional cases here...
                    default:
                        valueStr = Data.ToString();
                        break;
                }
            }

            return string.Concat("{", valueStr, "}");
        }

        public bool GetValueBoolean ()
        {
            if (ParameterClass != EffectParameterClass.Scalar || ParameterType != EffectParameterType.Bool)
                throw new InvalidCastException();

#if OPENGL
            // MojoShader encodes even booleans into a float.
            if (MojoDataLayout)
                return ((float[])Data)[0] != 0.0f;
            else
                return ((int[])Data)[0] != 0;
#else
            return ((int[])Data)[0] != 0;
#endif
        }

        /*
		public bool[] GetValueBooleanArray ()
		{
			throw new NotImplementedException();
		}
        */

        public int GetValueInt32 ()
        {
            if (ParameterClass != EffectParameterClass.Scalar || ParameterType != EffectParameterType.Int32)
                throw new InvalidCastException();

#if OPENGL
            // MojoShader encodes integers into a float.
            if (MojoDataLayout)
                return (int)((float[])Data)[0];
            else
                return ((int[])Data)[0];
#else
            return ((int[])Data)[0];
#endif
        }

        public int[] GetValueInt32Array()
        {
            if (Elements != null && Elements.Count > 0)
            {
                var ret = new int[RowCount * ColumnCount * Elements.Count];
                for (int i = 0; i < Elements.Count; i++)
                {
                    var elmArray = Elements[i].GetValueInt32Array();
                    for (var j = 0; j < elmArray.Length; j++)
                        ret[RowCount * ColumnCount * i + j] = elmArray[j];
                }
                return ret;
            }

            switch (ParameterClass)
            {
                case EffectParameterClass.Scalar:
                    return new int[] { GetValueInt32() };
                default:
                    throw new NotImplementedException();
            }
        }

        public Matrix GetValueMatrix ()
        {
            if (ParameterClass != EffectParameterClass.Matrix || ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            if (RowCount != 4 || ColumnCount != 4)
                throw new InvalidCastException();

            var floatData = (float[])Data;
#if OpenGL
            // OpenGL matrices are transposed compared to DX 
            if(!MojoDataLayout) 
                return new Matrix(floatData[0], floatData[1], floatData[2], floatData[3],
                                  floatData[4], floatData[5], floatData[6], floatData[7],
                                  floatData[8], floatData[9], floatData[10], floatData[11],
                                  floatData[12], floatData[13], floatData[14], floatData[15]);
            else
                return new Matrix(floatData[0], floatData[4], floatData[8], floatData[12],
                                  floatData[1], floatData[5], floatData[9], floatData[13],
                                  floatData[2], floatData[6], floatData[10], floatData[14],
                                  floatData[3], floatData[7], floatData[11], floatData[15]);
#else
            return new Matrix(floatData[0], floatData[4], floatData[8], floatData[12],
                              floatData[1], floatData[5], floatData[9], floatData[13],
                              floatData[2], floatData[6], floatData[10], floatData[14],
                              floatData[3], floatData[7], floatData[11], floatData[15]);
#endif
        }

        public Matrix[] GetValueMatrixArray (int count)
        {
            if (ParameterClass != EffectParameterClass.Matrix || ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            var ret = new Matrix[count];
            for (var i = 0; i < count; i++)
            {
                ret[i] = Elements[i].GetValueMatrix();
            }

            return ret;
        }

        public Quaternion GetValueQuaternion ()
        {
            if (ParameterClass != EffectParameterClass.Vector || ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            var vecInfo = (float[])Data;
            return new Quaternion(vecInfo[0], vecInfo[1], vecInfo[2], vecInfo[3]);
        }

        /*
		public Quaternion[] GetValueQuaternionArray ()
		{
			throw new NotImplementedException();
		}
        */

        public Single GetValueSingle ()
        {
            // TODO: Should this fetch int and bool as a float?
            if (ParameterClass != EffectParameterClass.Scalar || ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            return ((float[])Data)[0];
        }

        public Single[] GetValueSingleArray ()
        {
            if (Elements != null && Elements.Count > 0)
            {
                var ret = new Single[RowCount * ColumnCount * Elements.Count];
                for (int i=0; i<Elements.Count; i++)
                {
                    var elmArray = Elements[i].GetValueSingleArray();
                    for (var j = 0; j < elmArray.Length; j++)
                        ret[RowCount*ColumnCount*i+j] = elmArray[j];
                }
                return ret;
            }

            switch(ParameterClass)
            {
                case EffectParameterClass.Scalar:
                    return new Single[] { GetValueSingle () };
                case EffectParameterClass.Vector:
                case EffectParameterClass.Matrix:
                    if (Data is Matrix)
                        return Matrix.ToFloatArray((Matrix)Data);
                    else
                        return (float[])Data;
                default:
                    throw new NotImplementedException();
            }
        }

        public string GetValueString ()
        {
            if (ParameterClass != EffectParameterClass.Object || ParameterType != EffectParameterType.String)
                throw new InvalidCastException();

            return ((string[])Data)[0];
        }

        public Texture2D GetValueTexture2D ()
        {
            if (ParameterClass != EffectParameterClass.Object || ParameterType != EffectParameterType.Texture2D)
                throw new InvalidCastException();

            return (Texture2D)Data;
        }

#if !GLES
        public Texture3D GetValueTexture3D ()
        {
            if (ParameterClass != EffectParameterClass.Object || ParameterType != EffectParameterType.Texture3D)
                throw new InvalidCastException();

            return (Texture3D)Data;
        }
#endif

        public TextureCube GetValueTextureCube ()
        {
            if (ParameterClass != EffectParameterClass.Object || ParameterType != EffectParameterType.TextureCube)
                throw new InvalidCastException();

            return (TextureCube)Data;
        }

        public Vector2 GetValueVector2 ()
        {
            if (ParameterClass != EffectParameterClass.Vector || ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            var vecInfo = (float[])Data;
            return new Vector2(vecInfo[0],vecInfo[1]);
        }

        public Vector2[] GetValueVector2Array()
        {
            if (ParameterClass != EffectParameterClass.Vector || ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();
            if (Elements != null && Elements.Count > 0)
            {
                Vector2[] result = new Vector2[Elements.Count];
                for (int i = 0; i < Elements.Count; i++)
                {
                    var v = Elements[i].GetValueSingleArray();
                    result[i] = new Vector2(v[0], v[1]);
                }
                return result;
            }

            return null;
        }

        public Vector3 GetValueVector3 ()
        {
            if (ParameterClass != EffectParameterClass.Vector || ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            var vecInfo = (float[])Data;
            return new Vector3(vecInfo[0],vecInfo[1],vecInfo[2]);
        }

        public Vector3[] GetValueVector3Array()
        {
            if (ParameterClass != EffectParameterClass.Vector || ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            if (Elements != null && Elements.Count > 0)
            {
                Vector3[] result = new Vector3[Elements.Count];
                for (int i = 0; i < Elements.Count; i++)
                {
                    var v = Elements[i].GetValueSingleArray();
                    result[i] = new Vector3(v[0], v[1], v[2]);
                }
                return result;
            }
            return null;
        }


        public Vector4 GetValueVector4 ()
        {
            if (ParameterClass != EffectParameterClass.Vector || ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            var vecInfo = (float[])Data;
            return new Vector4(vecInfo[0],vecInfo[1],vecInfo[2],vecInfo[3]);
        }

        public Vector4[] GetValueVector4Array()
        {
            if (ParameterClass != EffectParameterClass.Vector || ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            if (Elements != null && Elements.Count > 0)
            {
                Vector4[] result = new Vector4[Elements.Count];
                for (int i = 0; i < Elements.Count; i++)
                {
                    var v = Elements[i].GetValueSingleArray();
                    result[i] = new Vector4(v[0], v[1],v[2], v[3]);
                }
                return result;
            }
            return null;
        }

        public void SetValue (bool value)
        {
#if OPENGL
            if (ParameterClass != EffectParameterClass.Scalar || (ParameterType != EffectParameterType.Bool && ParameterType != EffectParameterType.Int32))
                throw new InvalidCastException();

            // MojoShader encodes even booleans into a float.
            if (MojoDataLayout)
                ((float[])Data)[0] = value ? 1 : 0;
            else
                ((int[])Data)[0] = value ? 1 : 0;
#else
            if (ParameterClass != EffectParameterClass.Scalar || ParameterType != EffectParameterType.Bool)
                throw new InvalidCastException();
            ((int[])Data)[0] = value ? 1 : 0;
#endif
            StateKey = unchecked(NextStateKey++);
        }

        /*
		public void SetValue (bool[] value)
		{
			throw new NotImplementedException();
		}
        */

        public void SetValue (int value)
        {
            if (ParameterClass != EffectParameterClass.Scalar || ParameterType != EffectParameterType.Int32)
                throw new InvalidCastException();

#if OPENGL
            // MojoShader encodes integers into a float.
            if (MojoDataLayout)
                ((float[])Data)[0] = value;
            else
                ((int[])Data)[0] = value;
#else
            ((int[])Data)[0] = value;
#endif
            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue(int[] value)
        {
            for (var i = 0; i < value.Length; i++)
                Elements[i].SetValue(value[i]);

            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue(Matrix value)
        {
            if (ParameterClass != EffectParameterClass.Matrix || ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();
#if OPENGL
            // OpenGL matrices are transposed compared to DX 
            if (!MojoDataLayout)
                SetMatrixTranspose(value);
            else
                SetMatrix(value);
#else
            SetMatrix(value);
#endif
            StateKey = unchecked(NextStateKey++);
        }

        public void SetValueTranspose(Matrix value)
        {
            if (ParameterClass != EffectParameterClass.Matrix || ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();
#if OPENGL
            // OpenGL matrices are transposed compared to DX 
            if (!MojoDataLayout)
                SetMatrix(value); 
            else
                SetMatrixTranspose(value);
#else
            SetMatrixTranspose(value);
#endif
            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue (Matrix[] value)
        {
            if (ParameterClass != EffectParameterClass.Matrix || ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

#if OPENGL
            // OpenGL matrices are transposed compared to DX 
            if (!MojoDataLayout)
                SetMatrixArrayTransposed(value);
            else
                SetMatrixArray(value);
#else
            SetMatrixArray(value);
#endif
            StateKey = unchecked(NextStateKey++);
        }

        public void SetValueTranspose(Matrix[] value)
        {
            if (ParameterClass != EffectParameterClass.Matrix || ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

#if OPENGL
            // OpenGL matrices are transposed compared to DX 
            if (!MojoDataLayout)
                SetMatrixArray(value);
            else
                SetMatrixArrayTransposed(value);
#else
            SetMatrixArrayTransposed(value);
#endif
            StateKey = unchecked(NextStateKey++);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetMatrix(Matrix value)
        {
            // HLSL expects matrices to be transposed by default.
            // These unrolled loops do the transpose during assignment.
            if (RowCount == 4 && ColumnCount == 4)
            {
                var fData = (float[])Data;

                fData[0] = value.M11;
                fData[1] = value.M21;
                fData[2] = value.M31;
                fData[3] = value.M41;

                fData[4] = value.M12;
                fData[5] = value.M22;
                fData[6] = value.M32;
                fData[7] = value.M42;

                fData[8] = value.M13;
                fData[9] = value.M23;
                fData[10] = value.M33;
                fData[11] = value.M43;

                fData[12] = value.M14;
                fData[13] = value.M24;
                fData[14] = value.M34;
                fData[15] = value.M44;
            }
            else if (RowCount == 4 && ColumnCount == 3)
            {
                var fData = (float[])Data;

                fData[0] = value.M11;
                fData[1] = value.M21;
                fData[2] = value.M31;
                fData[3] = value.M41;

                fData[4] = value.M12;
                fData[5] = value.M22;
                fData[6] = value.M32;
                fData[7] = value.M42;

                fData[8] = value.M13;
                fData[9] = value.M23;
                fData[10] = value.M33;
                fData[11] = value.M43;
            }
            else if (RowCount == 3 && ColumnCount == 4)
            {
                var fData = (float[])Data;

                fData[0] = value.M11;
                fData[1] = value.M21;
                fData[2] = value.M31;

                fData[3] = value.M12;
                fData[4] = value.M22;
                fData[5] = value.M32;

                fData[6] = value.M13;
                fData[7] = value.M23;
                fData[8] = value.M33;

                fData[9] = value.M14;
                fData[10] = value.M24;
                fData[11] = value.M34;
            }
            else if (RowCount == 3 && ColumnCount == 3)
            {
                var fData = (float[])Data;

                fData[0] = value.M11;
                fData[1] = value.M21;
                fData[2] = value.M31;

                fData[3] = value.M12;
                fData[4] = value.M22;
                fData[5] = value.M32;

                fData[6] = value.M13;
                fData[7] = value.M23;
                fData[8] = value.M33;
            }
            else if (RowCount == 3 && ColumnCount == 2)
            {
                var fData = (float[])Data;

                fData[0] = value.M11;
                fData[1] = value.M21;
                fData[2] = value.M31;

                fData[3] = value.M12;
                fData[4] = value.M22;
                fData[5] = value.M32;
            }
            else if (RowCount == 2 && ColumnCount == 3)
            {
                var fData = (float[])Data;

                fData[0] = value.M11;
                fData[1] = value.M21;

                fData[2] = value.M12;
                fData[3] = value.M22;

                fData[4] = value.M13;
                fData[5] = value.M23;
            }
            else if (RowCount == 2 && ColumnCount == 2)
            {
                var fData = (float[])Data;

                fData[0] = value.M11;
                fData[1] = value.M21;

                fData[2] = value.M12;
                fData[3] = value.M22;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetMatrixTranspose(Matrix value)
        {
            // HLSL expects matrices to be transposed by default, so copying them straight
            // from the in-memory version effectively transposes them back to row-major.
            if (RowCount == 4 && ColumnCount == 4)
            {
                var fData = (float[])Data;

                fData[0] = value.M11;
                fData[1] = value.M12;
                fData[2] = value.M13;
                fData[3] = value.M14;

                fData[4] = value.M21;
                fData[5] = value.M22;
                fData[6] = value.M23;
                fData[7] = value.M24;

                fData[8] = value.M31;
                fData[9] = value.M32;
                fData[10] = value.M33;
                fData[11] = value.M34;

                fData[12] = value.M41;
                fData[13] = value.M42;
                fData[14] = value.M43;
                fData[15] = value.M44;
            }
            else if (RowCount == 4 && ColumnCount == 3)
            {
                var fData = (float[])Data;

                fData[0] = value.M11;
                fData[1] = value.M12;
                fData[2] = value.M13;

                fData[3] = value.M21;
                fData[4] = value.M22;
                fData[5] = value.M23;

                fData[6] = value.M31;
                fData[7] = value.M32;
                fData[8] = value.M33;

                fData[9] = value.M41;
                fData[10] = value.M42;
                fData[11] = value.M43;
            }
            else if (RowCount == 3 && ColumnCount == 4)
            {
                var fData = (float[])Data;

                fData[0] = value.M11;
                fData[1] = value.M12;
                fData[2] = value.M13;
                fData[3] = value.M14;

                fData[4] = value.M21;
                fData[5] = value.M22;
                fData[6] = value.M23;
                fData[7] = value.M24;

                fData[8] = value.M31;
                fData[9] = value.M32;
                fData[10] = value.M33;
                fData[11] = value.M34;
            }
            else if (RowCount == 3 && ColumnCount == 3)
            {
                var fData = (float[])Data;

                fData[0] = value.M11;
                fData[1] = value.M12;
                fData[2] = value.M13;

                fData[3] = value.M21;
                fData[4] = value.M22;
                fData[5] = value.M23;

                fData[6] = value.M31;
                fData[7] = value.M32;
                fData[8] = value.M33;
            }
            else if (RowCount == 3 && ColumnCount == 2)
            {
                var fData = (float[])Data;

                fData[0] = value.M11;
                fData[1] = value.M12;

                fData[2] = value.M21;
                fData[3] = value.M22;

                fData[4] = value.M31;
                fData[5] = value.M32;
            }
            else if (RowCount == 2 && ColumnCount == 3)
            {
                var fData = (float[])Data;

                fData[0] = value.M11;
                fData[1] = value.M12;
                fData[2] = value.M13;

                fData[3] = value.M21;
                fData[4] = value.M22;
                fData[5] = value.M23;
            }
            else if (RowCount == 2 && ColumnCount == 2)
            {
                var fData = (float[])Data;

                fData[0] = value.M11;
                fData[1] = value.M12;

                fData[2] = value.M21;
                fData[3] = value.M22;
            }
        }

        private void SetMatrixArray(Matrix[] value)
        {
            if (RowCount == 4 && ColumnCount == 4)
            {
                for (var i = 0; i < value.Length; i++)
                {
                    var fData = (float[])Elements[i].Data;

                    fData[0] = value[i].M11;
                    fData[1] = value[i].M21;
                    fData[2] = value[i].M31;
                    fData[3] = value[i].M41;

                    fData[4] = value[i].M12;
                    fData[5] = value[i].M22;
                    fData[6] = value[i].M32;
                    fData[7] = value[i].M42;

                    fData[8] = value[i].M13;
                    fData[9] = value[i].M23;
                    fData[10] = value[i].M33;
                    fData[11] = value[i].M43;

                    fData[12] = value[i].M14;
                    fData[13] = value[i].M24;
                    fData[14] = value[i].M34;
                    fData[15] = value[i].M44;
                }
            }
            else if (RowCount == 4 && ColumnCount == 3)
            {
                for (var i = 0; i < value.Length; i++)
                {
                    var fData = (float[])Elements[i].Data;

                    fData[0] = value[i].M11;
                    fData[1] = value[i].M21;
                    fData[2] = value[i].M31;
                    fData[3] = value[i].M41;

                    fData[4] = value[i].M12;
                    fData[5] = value[i].M22;
                    fData[6] = value[i].M32;
                    fData[7] = value[i].M42;

                    fData[8] = value[i].M13;
                    fData[9] = value[i].M23;
                    fData[10] = value[i].M33;
                    fData[11] = value[i].M43;
                }
            }
            else if (RowCount == 3 && ColumnCount == 4)
            {
                for (var i = 0; i < value.Length; i++)
                {
                    var fData = (float[])Elements[i].Data;

                    fData[0] = value[i].M11;
                    fData[1] = value[i].M21;
                    fData[2] = value[i].M31;

                    fData[3] = value[i].M12;
                    fData[4] = value[i].M22;
                    fData[5] = value[i].M32;

                    fData[6] = value[i].M13;
                    fData[7] = value[i].M23;
                    fData[8] = value[i].M33;

                    fData[9] = value[i].M14;
                    fData[10] = value[i].M24;
                    fData[11] = value[i].M34;
                }
            }
            else if (RowCount == 3 && ColumnCount == 3)
            {
                for (var i = 0; i < value.Length; i++)
                {
                    var fData = (float[])Elements[i].Data;

                    fData[0] = value[i].M11;
                    fData[1] = value[i].M21;
                    fData[2] = value[i].M31;

                    fData[3] = value[i].M12;
                    fData[4] = value[i].M22;
                    fData[5] = value[i].M32;

                    fData[6] = value[i].M13;
                    fData[7] = value[i].M23;
                    fData[8] = value[i].M33;
                }
            }
            else if (RowCount == 3 && ColumnCount == 2)
            {
                for (var i = 0; i < value.Length; i++)
                {
                    var fData = (float[])Elements[i].Data;

                    fData[0] = value[i].M11;
                    fData[1] = value[i].M21;
                    fData[2] = value[i].M31;

                    fData[3] = value[i].M12;
                    fData[4] = value[i].M22;
                    fData[5] = value[i].M32;
                }
            }
            else if (RowCount == 2 && ColumnCount == 3)
            {
                for (var i = 0; i < value.Length; i++)
                {
                    var fData = (float[])Data;

                    fData[0] = value[i].M11;
                    fData[1] = value[i].M21;

                    fData[2] = value[i].M12;
                    fData[3] = value[i].M22;

                    fData[4] = value[i].M13;
                    fData[5] = value[i].M23;
                }
            }
            else if (RowCount == 2 && ColumnCount == 2)
            {
                for (var i = 0; i < value.Length; i++)
                {
                    var fData = (float[])Data;

                    fData[0] = value[i].M11;
                    fData[1] = value[i].M21;

                    fData[2] = value[i].M12;
                    fData[3] = value[i].M22;
                }
            }
        }

        private void SetMatrixArrayTransposed(Matrix[] value)
        {
            // HLSL expects matrices to be transposed by default, so copying them straight
            // from the in-memory version effectively transposes them back to row-major.
            if (RowCount == 4 && ColumnCount == 4)
            {
                for (var i = 0; i < value.Length; i++)
                {
                    var fData = (float[])Data;

                    fData[0] = value[i].M11;
                    fData[1] = value[i].M12;
                    fData[2] = value[i].M13;
                    fData[3] = value[i].M14;

                    fData[4] = value[i].M21;
                    fData[5] = value[i].M22;
                    fData[6] = value[i].M23;
                    fData[7] = value[i].M24;

                    fData[8] = value[i].M31;
                    fData[9] = value[i].M32;
                    fData[10] = value[i].M33;
                    fData[11] = value[i].M34;

                    fData[12] = value[i].M41;
                    fData[13] = value[i].M42;
                    fData[14] = value[i].M43;
                    fData[15] = value[i].M44;
                }
            }
            else if (RowCount == 4 && ColumnCount == 3)
            {
                for (var i = 0; i < value.Length; i++)
                {
                    var fData = (float[])Data;

                    fData[0] = value[i].M11;
                    fData[1] = value[i].M12;
                    fData[2] = value[i].M13;

                    fData[3] = value[i].M21;
                    fData[4] = value[i].M22;
                    fData[5] = value[i].M23;

                    fData[6] = value[i].M31;
                    fData[7] = value[i].M32;
                    fData[8] = value[i].M33;

                    fData[9] = value[i].M41;
                    fData[10] = value[i].M42;
                    fData[11] = value[i].M43;
                }
            }
            else if (RowCount == 3 && ColumnCount == 4)
            {
                for (var i = 0; i < value.Length; i++)
                {
                    var fData = (float[])Data;

                    fData[0] = value[i].M11;
                    fData[1] = value[i].M12;
                    fData[2] = value[i].M13;
                    fData[3] = value[i].M14;

                    fData[4] = value[i].M21;
                    fData[5] = value[i].M22;
                    fData[6] = value[i].M23;
                    fData[7] = value[i].M24;

                    fData[8] =  value[i].M31;
                    fData[9] =  value[i].M32;
                    fData[10] = value[i].M33;
                    fData[11] = value[i].M34;
                }
            }
            else if (RowCount == 3 && ColumnCount == 3)
            {
                for (var i = 0; i < value.Length; i++)
                {
                    var fData = (float[])Data;

                    fData[0] = value[i].M11;
                    fData[1] = value[i].M12;
                    fData[2] = value[i].M13;

                    fData[3] = value[i].M21;
                    fData[4] = value[i].M22;
                    fData[5] = value[i].M23;

                    fData[6] = value[i].M31;
                    fData[7] = value[i].M32;
                    fData[8] = value[i].M33;
                }
            }
            else if (RowCount == 3 && ColumnCount == 2)
            {
                for (var i = 0; i < value.Length; i++)
                {
                    var fData = (float[])Data;

                    fData[0] = value[i].M11;
                    fData[1] = value[i].M12;
                    fData[2] = value[i].M13;

                    fData[3] = value[i].M21;
                    fData[4] = value[i].M22;
                    fData[5] = value[i].M23;
                }
            }
            else if (RowCount == 2 && ColumnCount == 3)
            {
                for (var i = 0; i < value.Length; i++)
                {
                    var fData = (float[])Data;

                    fData[0] = value[i].M11;
                    fData[1] = value[i].M12;
                    fData[2] = value[i].M13;

                    fData[3] = value[i].M21;
                    fData[4] = value[i].M22;
                    fData[5] = value[i].M23;
                }
            }
            else if (RowCount == 2 && ColumnCount == 2)
            {
                for (var i = 0; i < value.Length; i++)
                {
                    var fData = (float[])Data;

                    fData[0] = value[i].M11;
                    fData[1] = value[i].M12;

                    fData[2] = value[i].M21;
                    fData[3] = value[i].M22;
                }
            }
        }

        public void SetValue (Quaternion value)
        {
            if (ParameterClass != EffectParameterClass.Vector || ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            var fData = (float[])Data;
            fData[0] = value.X;
            fData[1] = value.Y;
            fData[2] = value.Z;
            fData[3] = value.W;
            StateKey = unchecked(NextStateKey++);
        }

        /*
		public void SetValue (Quaternion[] value)
		{
			throw new NotImplementedException();
		}
        */

        public void SetValue (Single value)
        {
            if (ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();
            ((float[])Data)[0] = value;
            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue (Single[] value)
        {
            for (var i=0; i<value.Length; i++)
                Elements[i].SetValue (value[i]);

            StateKey = unchecked(NextStateKey++);
        }

        /*
		public void SetValue (string value)
		{
			throw new NotImplementedException();
		}
        */

        public void SetValue (Texture value)
        {
            if (this.ParameterType != EffectParameterType.Texture &&
                this.ParameterType != EffectParameterType.Texture1D &&
                this.ParameterType != EffectParameterType.Texture2D &&
                this.ParameterType != EffectParameterType.Texture3D &&
                this.ParameterType != EffectParameterType.TextureCube)
            {
                throw new InvalidCastException();
            }

            Data = value;
            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue (Vector2 value)
        {
            if (ParameterClass != EffectParameterClass.Vector || ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            var fData = (float[])Data;
            fData[0] = value.X;
            fData[1] = value.Y;
            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue (Vector2[] value)
        {
            for (var i = 0; i < value.Length; i++)
                Elements[i].SetValue (value[i]);
            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue (Vector3 value)
        {
            if (ParameterClass != EffectParameterClass.Vector || ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            var fData = (float[])Data;
            fData[0] = value.X;
            fData[1] = value.Y;
            fData[2] = value.Z;
            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue (Vector3[] value)
        {
            for (var i = 0; i < value.Length; i++)
                Elements[i].SetValue (value[i]);
            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue (Vector4 value)
        {
            if (ParameterClass != EffectParameterClass.Vector || ParameterType != EffectParameterType.Single)
                throw new InvalidCastException();

            var fData = (float[])Data;
            fData[0] = value.X;
            fData[1] = value.Y;
            fData[2] = value.Z;
            fData[3] = value.W;
            StateKey = unchecked(NextStateKey++);
        }

        public void SetValue (Vector4[] value)
        {
            for (var i = 0; i < value.Length; i++)
                Elements[i].SetValue (value[i]);
            StateKey = unchecked(NextStateKey++);
        }
    }
}
