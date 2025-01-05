// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Microsoft.Xna.Framework
{ 
    /// <summary>
    /// Represents the right-handed 4x4 floating point matrix, which can store translation, scale and rotation information.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct Matrix : IEquatable<Matrix>
    {
        #region Public Constructors

        /// <summary>
        /// Constructs a matrix.
        /// </summary>
        /// <param name="m11">A first row and first column value.</param>
        /// <param name="m12">A first row and second column value.</param>
        /// <param name="m13">A first row and third column value.</param>
        /// <param name="m14">A first row and fourth column value.</param>
        /// <param name="m21">A second row and first column value.</param>
        /// <param name="m22">A second row and second column value.</param>
        /// <param name="m23">A second row and third column value.</param>
        /// <param name="m24">A second row and fourth column value.</param>
        /// <param name="m31">A third row and first column value.</param>
        /// <param name="m32">A third row and second column value.</param>
        /// <param name="m33">A third row and third column value.</param>
        /// <param name="m34">A third row and fourth column value.</param>
        /// <param name="m41">A fourth row and first column value.</param>
        /// <param name="m42">A fourth row and second column value.</param>
        /// <param name="m43">A fourth row and third column value.</param>
        /// <param name="m44">A fourth row and fourth column value.</param>
        public Matrix(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31,
                      float m32, float m33, float m34, float m41, float m42, float m43, float m44)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M14 = m14;
            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M24 = m24;
            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
            this.M34 = m34;
            this.M41 = m41;
            this.M42 = m42;
            this.M43 = m43;
            this.M44 = m44;
        }

        /// <summary>
        /// Constructs a matrix.
        /// </summary>
        /// <param name="row1">A first row of the created matrix.</param>
        /// <param name="row2">A second row of the created matrix.</param>
        /// <param name="row3">A third row of the created matrix.</param>
        /// <param name="row4">A fourth row of the created matrix.</param>
        public Matrix(Vector4 row1, Vector4 row2, Vector4 row3, Vector4 row4)
        {
            this.M11 = row1.X;
            this.M12 = row1.Y;
            this.M13 = row1.Z;
            this.M14 = row1.W;
            this.M21 = row2.X;
            this.M22 = row2.Y;
            this.M23 = row2.Z;
            this.M24 = row2.W;
            this.M31 = row3.X;
            this.M32 = row3.Y;
            this.M33 = row3.Z;
            this.M34 = row3.W;
            this.M41 = row4.X;
            this.M42 = row4.Y;
            this.M43 = row4.Z;
            this.M44 = row4.W;
        }

        #endregion

        #region Public Fields

        /// <summary>
        /// A first row and first column value.
        /// </summary>
        [DataMember]
        public float M11;

        /// <summary>
        /// A first row and second column value.
        /// </summary>
        [DataMember]
        public float M12;

        /// <summary>
        /// A first row and third column value.
        /// </summary>
        [DataMember]
        public float M13;

        /// <summary>
        /// A first row and fourth column value.
        /// </summary>
        [DataMember]
        public float M14;

        /// <summary>
        /// A second row and first column value.
        /// </summary>
        [DataMember]
        public float M21;

        /// <summary>
        /// A second row and second column value.
        /// </summary>
        [DataMember]
        public float M22;

        /// <summary>
        /// A second row and third column value.
        /// </summary>
        [DataMember]
        public float M23;

        /// <summary>
        /// A second row and fourth column value.
        /// </summary>
        [DataMember]
        public float M24;

        /// <summary>
        /// A third row and first column value.
        /// </summary>
        [DataMember]
        public float M31;

        /// <summary>
        /// A third row and second column value.
        /// </summary>
        [DataMember]
        public float M32;

        /// <summary>
        /// A third row and third column value.
        /// </summary>
        [DataMember]
        public float M33;

        /// <summary>
        /// A third row and fourth column value.
        /// </summary>
        [DataMember]
        public float M34;

        /// <summary>
        /// A fourth row and first column value.
        /// </summary>
        [DataMember]
        public float M41;

        /// <summary>
        /// A fourth row and second column value.
        /// </summary>
        [DataMember]
        public float M42;

        /// <summary>
        /// A fourth row and third column value.
        /// </summary>
        [DataMember]
        public float M43;

        /// <summary>
        /// A fourth row and fourth column value.
        /// </summary>
        [DataMember]
        public float M44;

        #endregion

        #region Indexers

        /// <summary>
        /// Get or set the matrix element at the given index, indexed in row major order.
        /// </summary>
        /// <param name="index">The linearized, zero-based index of the matrix element.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the index is less than <code>0</code> or larger than <code>15</code>.
        /// </exception>
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return M11;
                    case 1: return M12;
                    case 2: return M13;
                    case 3: return M14;
                    case 4: return M21;
                    case 5: return M22;
                    case 6: return M23;
                    case 7: return M24;
                    case 8: return M31;
                    case 9: return M32;
                    case 10: return M33;
                    case 11: return M34;
                    case 12: return M41;
                    case 13: return M42;
                    case 14: return M43;
                    case 15: return M44;
                }
                throw new ArgumentOutOfRangeException();
            }

            set
            {
                switch (index)
                {
                    case 0: M11 = value; break;
                    case 1: M12 = value; break;
                    case 2: M13 = value; break;
                    case 3: M14 = value; break;
                    case 4: M21 = value; break;
                    case 5: M22 = value; break;
                    case 6: M23 = value; break;
                    case 7: M24 = value; break;
                    case 8: M31 = value; break;
                    case 9: M32 = value; break;
                    case 10: M33 = value; break;
                    case 11: M34 = value; break;
                    case 12: M41 = value; break;
                    case 13: M42 = value; break;
                    case 14: M43 = value; break;
                    case 15: M44 = value; break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Get or set the value at the specified row and column (indices are zero-based).
        /// </summary>
        /// <param name="row">The row of the element.</param>
        /// <param name="column">The column of the element.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the row or column is less than <code>0</code> or larger than <code>3</code>.
        /// </exception>
        public float this[int row, int column]
        {
            get
            {
                return this[(row * 4) + column];
            }

            set
            {
                this[(row * 4) + column] = value;
            }
        }

        #endregion

        #region Private Members
        private static Matrix identity = new Matrix(1f, 0f, 0f, 0f, 
		                                            0f, 1f, 0f, 0f, 
		                                            0f, 0f, 1f, 0f, 
		                                            0f, 0f, 0f, 1f);
        #endregion

        #region Public Properties

        /// <summary>
        /// The backward vector formed from the third row M31, M32, M33 elements.
        /// </summary>
        public Vector3 Backward
        {
            get
            {
                return new Vector3(this.M31, this.M32, this.M33);
            }
            set
            {
                this.M31 = value.X;
                this.M32 = value.Y;
                this.M33 = value.Z;
            }
        }

        /// <summary>
        /// The down vector formed from the second row -M21, -M22, -M23 elements.
        /// </summary>
        public Vector3 Down
        {
            get
            {
                return new Vector3(-this.M21, -this.M22, -this.M23);
            }
            set
            {
                this.M21 = -value.X;
                this.M22 = -value.Y;
                this.M23 = -value.Z;
            }
        }

        /// <summary>
        /// The forward vector formed from the third row -M31, -M32, -M33 elements.
        /// </summary>
        public Vector3 Forward
        {
            get
            {
                return new Vector3(-this.M31, -this.M32, -this.M33);
            }
            set
            {
                this.M31 = -value.X;
                this.M32 = -value.Y;
                this.M33 = -value.Z;
            }
        }

        /// <summary>
        /// Returns the identity matrix.
        /// </summary>
        public static Matrix Identity
        {
            get { return identity; }
        }

        /// <summary>
        /// The left vector formed from the first row -M11, -M12, -M13 elements.
        /// </summary>
        public Vector3 Left
        {
            get
            {
                return new Vector3(-this.M11, -this.M12, -this.M13);
            }
            set
            {
                this.M11 = -value.X;
                this.M12 = -value.Y;
                this.M13 = -value.Z;
            }
        }

        /// <summary>
        /// The right vector formed from the first row M11, M12, M13 elements.
        /// </summary>
        public Vector3 Right
        {
            get
            {
                return new Vector3(this.M11, this.M12, this.M13);
            }
            set
            {
                this.M11 = value.X;
                this.M12 = value.Y;
                this.M13 = value.Z;
            }
        }

        /// <summary>
        /// Position stored in this matrix.
        /// </summary>
        public Vector3 Translation
        {
            get
            {
                return new Vector3(this.M41, this.M42, this.M43);
            }
            set
            {
                this.M41 = value.X;
                this.M42 = value.Y;
                this.M43 = value.Z;
            }
        }

        /// <summary>
        /// The upper vector formed from the second row M21, M22, M23 elements.
        /// </summary>
        public Vector3 Up
        {
            get
            {
                return new Vector3(this.M21, this.M22, this.M23);
            }
            set
            {
                this.M21 = value.X;
                this.M22 = value.Y;
                this.M23 = value.Z;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new <see cref="Matrix"/> which contains sum of two matrixes.
        /// </summary>
        /// <param name="matrix1">The first matrix to add.</param>
        /// <param name="matrix2">The second matrix to add.</param>
        /// <returns>The result of the matrix addition.</returns>
        public static Matrix Add(Matrix matrix1, Matrix matrix2)
        {
            matrix1.M11 += matrix2.M11;
            matrix1.M12 += matrix2.M12;
            matrix1.M13 += matrix2.M13;
            matrix1.M14 += matrix2.M14;
            matrix1.M21 += matrix2.M21;
            matrix1.M22 += matrix2.M22;
            matrix1.M23 += matrix2.M23;
            matrix1.M24 += matrix2.M24;
            matrix1.M31 += matrix2.M31;
            matrix1.M32 += matrix2.M32;
            matrix1.M33 += matrix2.M33;
            matrix1.M34 += matrix2.M34;
            matrix1.M41 += matrix2.M41;
            matrix1.M42 += matrix2.M42;
            matrix1.M43 += matrix2.M43;
            matrix1.M44 += matrix2.M44;
            return matrix1;
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> which contains sum of two matrixes.
        /// </summary>
        /// <param name="matrix1">The first matrix to add.</param>
        /// <param name="matrix2">The second matrix to add.</param>
        /// <param name="result">The result of the matrix addition as an output parameter.</param>
        public static void Add(ref Matrix matrix1, ref Matrix matrix2, out Matrix result)
        {
            result.M11 = matrix1.M11 + matrix2.M11;
            result.M12 = matrix1.M12 + matrix2.M12;
            result.M13 = matrix1.M13 + matrix2.M13;
            result.M14 = matrix1.M14 + matrix2.M14;
            result.M21 = matrix1.M21 + matrix2.M21;
            result.M22 = matrix1.M22 + matrix2.M22;
            result.M23 = matrix1.M23 + matrix2.M23;
            result.M24 = matrix1.M24 + matrix2.M24;
            result.M31 = matrix1.M31 + matrix2.M31;
            result.M32 = matrix1.M32 + matrix2.M32;
            result.M33 = matrix1.M33 + matrix2.M33;
            result.M34 = matrix1.M34 + matrix2.M34;
            result.M41 = matrix1.M41 + matrix2.M41;
            result.M42 = matrix1.M42 + matrix2.M42;
            result.M43 = matrix1.M43 + matrix2.M43;
            result.M44 = matrix1.M44 + matrix2.M44;

        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> for spherical billboarding that rotates around specified object position.
        /// </summary>
        /// <param name="objectPosition">Position of billboard object. It will rotate around that vector.</param>
        /// <param name="cameraPosition">The camera position.</param>
        /// <param name="cameraUpVector">The camera up vector.</param>
        /// <param name="cameraForwardVector">Optional camera forward vector.</param>
        /// <returns>The <see cref="Matrix"/> for spherical billboarding.</returns>
        public static Matrix CreateBillboard(Vector3 objectPosition, Vector3 cameraPosition,
            Vector3 cameraUpVector, Nullable<Vector3> cameraForwardVector)
        {
            Matrix result;

            // Delegate to the other overload of the function to do the work
            CreateBillboard(ref objectPosition, ref cameraPosition, ref cameraUpVector, cameraForwardVector, out result);

            return result;
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> for spherical billboarding that rotates around specified object position.
        /// </summary>
        /// <param name="objectPosition">Position of billboard object. It will rotate around that vector.</param>
        /// <param name="cameraPosition">The camera position.</param>
        /// <param name="cameraUpVector">The camera up vector.</param>
        /// <param name="cameraForwardVector">Optional camera forward vector.</param>
        /// <param name="result">The <see cref="Matrix"/> for spherical billboarding as an output parameter.</param>
        public static void CreateBillboard(ref Vector3 objectPosition, ref Vector3 cameraPosition,
            ref Vector3 cameraUpVector, Vector3? cameraForwardVector, out Matrix result)
        {
            Vector3 vector;
            Vector3 vector2;
            Vector3 vector3;
            vector.X = objectPosition.X - cameraPosition.X;
            vector.Y = objectPosition.Y - cameraPosition.Y;
            vector.Z = objectPosition.Z - cameraPosition.Z;
            float num = vector.LengthSquared();
            if (num < 0.0001f)
            {
                vector = cameraForwardVector.HasValue ? -cameraForwardVector.Value : Vector3.Forward;
            }
            else
            {
                Vector3.Multiply(ref vector, 1f / MathF.Sqrt(num), out vector);
            }
            Vector3.Cross(ref cameraUpVector, ref vector, out vector3);
            vector3.Normalize();
            Vector3.Cross(ref vector, ref vector3, out vector2);
            result.M11 = vector3.X;
            result.M12 = vector3.Y;
            result.M13 = vector3.Z;
            result.M14 = 0;
            result.M21 = vector2.X;
            result.M22 = vector2.Y;
            result.M23 = vector2.Z;
            result.M24 = 0;
            result.M31 = vector.X;
            result.M32 = vector.Y;
            result.M33 = vector.Z;
            result.M34 = 0;
            result.M41 = objectPosition.X;
            result.M42 = objectPosition.Y;
            result.M43 = objectPosition.Z;
            result.M44 = 1;
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> for cylindrical billboarding that rotates around specified axis.
        /// </summary>
        /// <param name="objectPosition">Object position the billboard will rotate around.</param>
        /// <param name="cameraPosition">Camera position.</param>
        /// <param name="rotateAxis">Axis of billboard for rotation.</param>
        /// <param name="cameraForwardVector">Optional camera forward vector.</param>
        /// <param name="objectForwardVector">Optional object forward vector.</param>
        /// <returns>The <see cref="Matrix"/> for cylindrical billboarding.</returns>
        public static Matrix CreateConstrainedBillboard(Vector3 objectPosition, Vector3 cameraPosition,
            Vector3 rotateAxis, Nullable<Vector3> cameraForwardVector, Nullable<Vector3> objectForwardVector)
        {
            Matrix result;
            CreateConstrainedBillboard(ref objectPosition, ref cameraPosition, ref rotateAxis,
                cameraForwardVector, objectForwardVector, out result);
            return result;
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> for cylindrical billboarding that rotates around specified axis.
        /// </summary>
        /// <param name="objectPosition">Object position the billboard will rotate around.</param>
        /// <param name="cameraPosition">Camera position.</param>
        /// <param name="rotateAxis">Axis of billboard for rotation.</param>
        /// <param name="cameraForwardVector">Optional camera forward vector.</param>
        /// <param name="objectForwardVector">Optional object forward vector.</param>
        /// <param name="result">The <see cref="Matrix"/> for cylindrical billboarding as an output parameter.</param>
        public static void CreateConstrainedBillboard(ref Vector3 objectPosition, ref Vector3 cameraPosition,
            ref Vector3 rotateAxis, Vector3? cameraForwardVector, Vector3? objectForwardVector, out Matrix result)
        {
            float num;
		    Vector3 vector;
		    Vector3 vector2;
		    Vector3 vector3;
		    vector2.X = objectPosition.X - cameraPosition.X;
		    vector2.Y = objectPosition.Y - cameraPosition.Y;
		    vector2.Z = objectPosition.Z - cameraPosition.Z;
		    float num2 = vector2.LengthSquared();
		    if (num2 < 0.0001f)
		    {
		        vector2 = cameraForwardVector.HasValue ? -cameraForwardVector.Value : Vector3.Forward;
		    }
		    else
		    {
		        Vector3.Multiply(ref vector2, 1f / MathF.Sqrt(num2), out vector2);
		    }
		    Vector3 vector4 = rotateAxis;
		    Vector3.Dot(ref rotateAxis, ref vector2, out num);
		    if (Math.Abs(num) > 0.9982547f)
		    {
		        if (objectForwardVector.HasValue)
		        {
		            vector = objectForwardVector.Value;
		            Vector3.Dot(ref rotateAxis, ref vector, out num);
		            if (Math.Abs(num) > 0.9982547f)
		            {
		                num = ((rotateAxis.X * Vector3.Forward.X) + (rotateAxis.Y * Vector3.Forward.Y)) + (rotateAxis.Z * Vector3.Forward.Z);
		                vector = (Math.Abs(num) > 0.9982547f) ? Vector3.Right : Vector3.Forward;
		            }
		        }
		        else
		        {
		            num = ((rotateAxis.X * Vector3.Forward.X) + (rotateAxis.Y * Vector3.Forward.Y)) + (rotateAxis.Z * Vector3.Forward.Z);
		            vector = (Math.Abs(num) > 0.9982547f) ? Vector3.Right : Vector3.Forward;
		        }
		        Vector3.Cross(ref rotateAxis, ref vector, out vector3);
		        vector3.Normalize();
		        Vector3.Cross(ref vector3, ref rotateAxis, out vector);
		        vector.Normalize();
		    }
		    else
		    {
		        Vector3.Cross(ref rotateAxis, ref vector2, out vector3);
		        vector3.Normalize();
		        Vector3.Cross(ref vector3, ref vector4, out vector);
		        vector.Normalize();
		    }
		    result.M11 = vector3.X;
		    result.M12 = vector3.Y;
		    result.M13 = vector3.Z;
		    result.M14 = 0;
		    result.M21 = vector4.X;
		    result.M22 = vector4.Y;
		    result.M23 = vector4.Z;
		    result.M24 = 0;
		    result.M31 = vector.X;
		    result.M32 = vector.Y;
		    result.M33 = vector.Z;
		    result.M34 = 0;
		    result.M41 = objectPosition.X;
		    result.M42 = objectPosition.Y;
		    result.M43 = objectPosition.Z;
		    result.M44 = 1;

        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> which contains the rotation moment around specified axis.
        /// </summary>
        /// <param name="axis">The axis of rotation.</param>
        /// <param name="angle">The angle of rotation in radians.</param>
        /// <returns>The rotation <see cref="Matrix"/>.</returns>
        public static Matrix CreateFromAxisAngle(Vector3 axis, float angle)
        {
            Matrix result;
            CreateFromAxisAngle(ref axis, angle, out result);
            return result;
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> which contains the rotation moment around specified axis.
        /// </summary>
        /// <param name="axis">The axis of rotation.</param>
        /// <param name="angle">The angle of rotation in radians.</param>
        /// <param name="result">The rotation <see cref="Matrix"/> as an output parameter.</param>
        public static void CreateFromAxisAngle(ref Vector3 axis, float angle, out Matrix result)
        {
            float x = axis.X;
		    float y = axis.Y;
		    float z = axis.Z;
		    float num2 = MathF.Sin(angle);
		    float num = MathF.Cos(angle);
		    float num11 = x * x;
		    float num10 = y * y;
		    float num9 = z * z;
		    float num8 = x * y;
		    float num7 = x * z;
		    float num6 = y * z;
		    result.M11 = num11 + (num * (1f - num11));
		    result.M12 = (num8 - (num * num8)) + (num2 * z);
		    result.M13 = (num7 - (num * num7)) - (num2 * y);
		    result.M14 = 0;
		    result.M21 = (num8 - (num * num8)) - (num2 * z);
		    result.M22 = num10 + (num * (1f - num10));
		    result.M23 = (num6 - (num * num6)) + (num2 * x);
		    result.M24 = 0;
		    result.M31 = (num7 - (num * num7)) + (num2 * y);
		    result.M32 = (num6 - (num * num6)) - (num2 * x);
		    result.M33 = num9 + (num * (1f - num9));
		    result.M34 = 0;
		    result.M41 = 0;
		    result.M42 = 0;
		    result.M43 = 0;
		    result.M44 = 1;
        }

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> from a <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="quaternion"><see cref="Quaternion"/> of rotation moment.</param>
        /// <returns>The rotation <see cref="Matrix"/>.</returns>
        public static Matrix CreateFromQuaternion(Quaternion quaternion)
        {
            Matrix result;
            CreateFromQuaternion(ref quaternion, out result);
            return result;
        }

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> from a <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="quaternion"><see cref="Quaternion"/> of rotation moment.</param>
        /// <param name="result">The rotation <see cref="Matrix"/> as an output parameter.</param>
        public static void CreateFromQuaternion(ref Quaternion quaternion, out Matrix result)
        {
            float num9 = quaternion.X * quaternion.X;
		    float num8 = quaternion.Y * quaternion.Y;
		    float num7 = quaternion.Z * quaternion.Z;
		    float num6 = quaternion.X * quaternion.Y;
		    float num5 = quaternion.Z * quaternion.W;
		    float num4 = quaternion.Z * quaternion.X;
		    float num3 = quaternion.Y * quaternion.W;
		    float num2 = quaternion.Y * quaternion.Z;
		    float num = quaternion.X * quaternion.W;
		    result.M11 = 1f - (2f * (num8 + num7));
		    result.M12 = 2f * (num6 + num5);
		    result.M13 = 2f * (num4 - num3);
		    result.M14 = 0f;
		    result.M21 = 2f * (num6 - num5);
		    result.M22 = 1f - (2f * (num7 + num9));
		    result.M23 = 2f * (num2 + num);
		    result.M24 = 0f;
		    result.M31 = 2f * (num4 + num3);
		    result.M32 = 2f * (num2 - num);
		    result.M33 = 1f - (2f * (num8 + num9));
		    result.M34 = 0f;
		    result.M41 = 0f;
		    result.M42 = 0f;
		    result.M43 = 0f;
		    result.M44 = 1f;
        }

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> from the specified yaw, pitch and roll values.
        /// </summary>
        /// <param name="yaw">The yaw rotation value in radians.</param>
        /// <param name="pitch">The pitch rotation value in radians.</param>
        /// <param name="roll">The roll rotation value in radians.</param>
        /// <returns>The rotation <see cref="Matrix"/>.</returns>
        /// <remarks>For more information about yaw, pitch and roll visit http://en.wikipedia.org/wiki/Euler_angles.
        /// </remarks>
		public static Matrix CreateFromYawPitchRoll(float yaw, float pitch, float roll)
		{
			Matrix matrix;
            CreateFromYawPitchRoll(yaw, pitch, roll, out matrix);
		    return matrix;
		}

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> from the specified yaw, pitch and roll values.
        /// </summary>
        /// <param name="yaw">The yaw rotation value in radians.</param>
        /// <param name="pitch">The pitch rotation value in radians.</param>
        /// <param name="roll">The roll rotation value in radians.</param>
        /// <param name="result">The rotation <see cref="Matrix"/> as an output parameter.</param>
        /// <remarks>For more information about yaw, pitch and roll visit http://en.wikipedia.org/wiki/Euler_angles.
        /// </remarks>
		public static void CreateFromYawPitchRoll(float yaw, float pitch, float roll, out Matrix result)
		{
			Quaternion quaternion;
		    Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll, out quaternion);
		    CreateFromQuaternion(ref quaternion, out result);
		}

        /// <summary>
        /// Creates a new viewing <see cref="Matrix"/>.
        /// </summary>
        /// <param name="cameraPosition">Position of the camera.</param>
        /// <param name="cameraTarget">Lookup vector of the camera.</param>
        /// <param name="cameraUpVector">The direction of the upper edge of the camera.</param>
        /// <returns>The viewing <see cref="Matrix"/>.</returns>
        public static Matrix CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
        {
            Matrix matrix;
            CreateLookAt(ref cameraPosition, ref cameraTarget, ref cameraUpVector, out matrix);
            return matrix;
        }

        /// <summary>
        /// Creates a new viewing <see cref="Matrix"/>.
        /// </summary>
        /// <param name="cameraPosition">Position of the camera.</param>
        /// <param name="cameraTarget">Lookup vector of the camera.</param>
        /// <param name="cameraUpVector">The direction of the upper edge of the camera.</param>
        /// <param name="result">The viewing <see cref="Matrix"/> as an output parameter.</param>
        public static void CreateLookAt(ref Vector3 cameraPosition, ref Vector3 cameraTarget, ref Vector3 cameraUpVector, out Matrix result)
        {
            var vector = Vector3.Normalize(cameraPosition - cameraTarget);
            var vector2 = Vector3.Normalize(Vector3.Cross(cameraUpVector, vector));
            var vector3 = Vector3.Cross(vector, vector2);
		    result.M11 = vector2.X;
		    result.M12 = vector3.X;
		    result.M13 = vector.X;
		    result.M14 = 0f;
		    result.M21 = vector2.Y;
		    result.M22 = vector3.Y;
		    result.M23 = vector.Y;
		    result.M24 = 0f;
		    result.M31 = vector2.Z;
		    result.M32 = vector3.Z;
		    result.M33 = vector.Z;
		    result.M34 = 0f;
		    result.M41 = -Vector3.Dot(vector2, cameraPosition);
		    result.M42 = -Vector3.Dot(vector3, cameraPosition);
		    result.M43 = -Vector3.Dot(vector, cameraPosition);
		    result.M44 = 1f;
        }

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for orthographic view.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="zNearPlane">Depth of the near plane.</param>
        /// <param name="zFarPlane">Depth of the far plane.</param>
        /// <returns>The new projection <see cref="Matrix"/> for orthographic view.</returns>
        public static Matrix CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane)
        {
            Matrix matrix;
            CreateOrthographic(width, height, zNearPlane, zFarPlane, out matrix);
		    return matrix;
        }

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for orthographic view.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="zNearPlane">Depth of the near plane.</param>
        /// <param name="zFarPlane">Depth of the far plane.</param>
        /// <param name="result">The new projection <see cref="Matrix"/> for orthographic view as an output parameter.</param>
        public static void CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane, out Matrix result)
        {
            result.M11 = 2f / width;
		    result.M12 = result.M13 = result.M14 = 0f;
		    result.M22 = 2f / height;
		    result.M21 = result.M23 = result.M24 = 0f;
		    result.M33 = 1f / (zNearPlane - zFarPlane);
		    result.M31 = result.M32 = result.M34 = 0f;
		    result.M41 = result.M42 = 0f;
		    result.M43 = zNearPlane / (zNearPlane - zFarPlane);
		    result.M44 = 1f;
        }

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for customized orthographic view.
        /// </summary>
        /// <param name="left">Lower x-value at the near plane.</param>
        /// <param name="right">Upper x-value at the near plane.</param>
        /// <param name="bottom">Lower y-coordinate at the near plane.</param>
        /// <param name="top">Upper y-value at the near plane.</param>
        /// <param name="zNearPlane">Depth of the near plane.</param>
        /// <param name="zFarPlane">Depth of the far plane.</param>
        /// <returns>The new projection <see cref="Matrix"/> for customized orthographic view.</returns>
        public static Matrix CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
        {
			Matrix matrix;
            CreateOrthographicOffCenter(left, right, bottom, top, zNearPlane, zFarPlane, out matrix);
			return matrix;
        }

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for customized orthographic view.
        /// </summary>
        /// <param name="viewingVolume">The viewing volume.</param>
        /// <param name="zNearPlane">Depth of the near plane.</param>
        /// <param name="zFarPlane">Depth of the far plane.</param>
        /// <returns>The new projection <see cref="Matrix"/> for customized orthographic view.</returns>
        public static Matrix CreateOrthographicOffCenter(Rectangle viewingVolume, float zNearPlane, float zFarPlane)
        {
            Matrix matrix;
            CreateOrthographicOffCenter(viewingVolume.Left, viewingVolume.Right, viewingVolume.Bottom, viewingVolume.Top, zNearPlane, zFarPlane, out matrix);
            return matrix;
        }

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for customized orthographic view.
        /// </summary>
        /// <param name="left">Lower x-value at the near plane.</param>
        /// <param name="right">Upper x-value at the near plane.</param>
        /// <param name="bottom">Lower y-coordinate at the near plane.</param>
        /// <param name="top">Upper y-value at the near plane.</param>
        /// <param name="zNearPlane">Depth of the near plane.</param>
        /// <param name="zFarPlane">Depth of the far plane.</param>
        /// <param name="result">The new projection <see cref="Matrix"/> for customized orthographic view as an output parameter.</param>
        public static void CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane, out Matrix result)
        {
			result.M11 = (float)(2.0 / ((double)right - (double)left));
			result.M12 = 0.0f;
			result.M13 = 0.0f;
			result.M14 = 0.0f;
			result.M21 = 0.0f;
			result.M22 = (float)(2.0 / ((double)top - (double)bottom));
			result.M23 = 0.0f;
			result.M24 = 0.0f;
			result.M31 = 0.0f;
			result.M32 = 0.0f;
			result.M33 = (float)(1.0 / ((double)zNearPlane - (double)zFarPlane));
			result.M34 = 0.0f;
			result.M41 = (float)(((double)left + (double)right) / ((double)left - (double)right));
			result.M42 = (float)(((double)top + (double)bottom) / ((double)bottom - (double)top));
			result.M43 = (float)((double)zNearPlane / ((double)zNearPlane - (double)zFarPlane));
			result.M44 = 1.0f;
		}

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for perspective view.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="nearPlaneDistance">Distance to the near plane.</param>
        /// <param name="farPlaneDistance">Distance to the far plane.</param>
        /// <returns>The new projection <see cref="Matrix"/> for perspective view.</returns>
        public static Matrix CreatePerspective(float width, float height, float nearPlaneDistance, float farPlaneDistance)
        {
            Matrix matrix;
            CreatePerspective(width, height, nearPlaneDistance, farPlaneDistance, out matrix);
		    return matrix;
        }

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for perspective view.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="nearPlaneDistance">Distance to the near plane.</param>
        /// <param name="farPlaneDistance">Distance to the far plane, or <see cref="float.PositiveInfinity"/>.</param>
        /// <param name="result">The new projection <see cref="Matrix"/> for perspective view as an output parameter.</param>
        public static void CreatePerspective(float width, float height, float nearPlaneDistance, float farPlaneDistance, out Matrix result)
        {
            if (nearPlaneDistance <= 0f)
		    {
		        throw new ArgumentException("nearPlaneDistance <= 0");
		    }
		    if (farPlaneDistance <= 0f)
		    {
		        throw new ArgumentException("farPlaneDistance <= 0");
		    }
		    if (nearPlaneDistance >= farPlaneDistance)
		    {
		        throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
		    }

            var negFarRange = float.IsPositiveInfinity(farPlaneDistance) ? -1.0f : farPlaneDistance / (nearPlaneDistance - farPlaneDistance);

            result.M11 = (2.0f * nearPlaneDistance) / width;
            result.M12 = result.M13 = result.M14 = 0.0f;
            result.M22 = (2.0f * nearPlaneDistance) / height;
            result.M21 = result.M23 = result.M24 = 0.0f;            
            result.M33 = negFarRange;
            result.M31 = result.M32 = 0.0f;
            result.M34 = -1.0f;
            result.M41 = result.M42 = result.M44 = 0.0f;
            result.M43 = nearPlaneDistance * negFarRange;
        }

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for perspective view with field of view.
        /// </summary>
        /// <param name="fieldOfView">Field of view in the y direction in radians.</param>
        /// <param name="aspectRatio">Width divided by height of the viewing volume.</param>
        /// <param name="nearPlaneDistance">Distance to the near plane.</param>
        /// <param name="farPlaneDistance">Distance to the far plane, or <see cref="float.PositiveInfinity"/>.</param>
        /// <returns>The new projection <see cref="Matrix"/> for perspective view with FOV.</returns>
        public static Matrix CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            Matrix result;
            CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance, out result);
            return result;
        }

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for perspective view with field of view.
        /// </summary>
        /// <param name="fieldOfView">Field of view in the y direction in radians.</param>
        /// <param name="aspectRatio">Width divided by height of the viewing volume.</param>
        /// <param name="nearPlaneDistance">Distance of the near plane.</param>
        /// <param name="farPlaneDistance">Distance of the far plane, or <see cref="float.PositiveInfinity"/>.</param>
        /// <param name="result">The new projection <see cref="Matrix"/> for perspective view with FOV as an output parameter.</param>
        public static void CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance, out Matrix result)
        {
            if ((fieldOfView <= 0f) || (fieldOfView >= 3.141593f))
		    {
		        throw new ArgumentException("fieldOfView <= 0 or >= PI");
		    }
		    if (nearPlaneDistance <= 0f)
		    {
		        throw new ArgumentException("nearPlaneDistance <= 0");
		    }
		    if (farPlaneDistance <= 0f)
		    {
		        throw new ArgumentException("farPlaneDistance <= 0");
		    }
		    if (nearPlaneDistance >= farPlaneDistance)
		    {
		        throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
		    }

            var yScale = 1.0f / (float)Math.Tan((double)fieldOfView * 0.5f);
            var xScale = yScale / aspectRatio;
            var negFarRange = float.IsPositiveInfinity(farPlaneDistance) ? -1.0f : farPlaneDistance / (nearPlaneDistance - farPlaneDistance);

            result.M11 = xScale;
            result.M12 = result.M13 = result.M14 = 0.0f;
            result.M22 = yScale;
            result.M21 = result.M23 = result.M24 = 0.0f;
            result.M31 = result.M32 = 0.0f;            
            result.M33 = negFarRange;
            result.M34 = -1.0f;
            result.M41 = result.M42 = result.M44 = 0.0f;
            result.M43 = nearPlaneDistance * negFarRange;
        }

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for customized perspective view.
        /// </summary>
        /// <param name="left">Lower x-value at the near plane.</param>
        /// <param name="right">Upper x-value at the near plane.</param>
        /// <param name="bottom">Lower y-coordinate at the near plane.</param>
        /// <param name="top">Upper y-value at the near plane.</param>
        /// <param name="nearPlaneDistance">Distance to the near plane.</param>
        /// <param name="farPlaneDistance">Distance to the far plane.</param>
        /// <returns>The new <see cref="Matrix"/> for customized perspective view.</returns>
        public static Matrix CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance)
        {
            Matrix result;
            CreatePerspectiveOffCenter(left, right, bottom, top, nearPlaneDistance, farPlaneDistance, out result);
            return result;
        }

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for customized perspective view.
        /// </summary>
        /// <param name="viewingVolume">The viewing volume.</param>
        /// <param name="nearPlaneDistance">Distance to the near plane.</param>
        /// <param name="farPlaneDistance">Distance to the far plane.</param>
        /// <returns>The new <see cref="Matrix"/> for customized perspective view.</returns>
        public static Matrix CreatePerspectiveOffCenter(Rectangle viewingVolume, float nearPlaneDistance, float farPlaneDistance)
        {
            Matrix result;
            CreatePerspectiveOffCenter(viewingVolume.Left, viewingVolume.Right, viewingVolume.Bottom, viewingVolume.Top, nearPlaneDistance, farPlaneDistance, out result);
            return result;
        }

        /// <summary>
        /// Creates a new projection <see cref="Matrix"/> for customized perspective view.
        /// </summary>
        /// <param name="left">Lower x-value at the near plane.</param>
        /// <param name="right">Upper x-value at the near plane.</param>
        /// <param name="bottom">Lower y-coordinate at the near plane.</param>
        /// <param name="top">Upper y-value at the near plane.</param>
        /// <param name="nearPlaneDistance">Distance to the near plane.</param>
        /// <param name="farPlaneDistance">Distance to the far plane.</param>
        /// <param name="result">The new <see cref="Matrix"/> for customized perspective view as an output parameter.</param>
        public static void CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance, out Matrix result)
        {
            if (nearPlaneDistance <= 0f)
		    {
		        throw new ArgumentException("nearPlaneDistance <= 0");
		    }
		    if (farPlaneDistance <= 0f)
		    {
		        throw new ArgumentException("farPlaneDistance <= 0");
		    }
		    if (nearPlaneDistance >= farPlaneDistance)
		    {
		        throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
		    }
		    result.M11 = (2f * nearPlaneDistance) / (right - left);
		    result.M12 = result.M13 = result.M14 = 0;
		    result.M22 = (2f * nearPlaneDistance) / (top - bottom);
		    result.M21 = result.M23 = result.M24 = 0;
		    result.M31 = (left + right) / (right - left);
		    result.M32 = (top + bottom) / (top - bottom);
		    result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
		    result.M34 = -1;
		    result.M43 = (nearPlaneDistance * farPlaneDistance) / (nearPlaneDistance - farPlaneDistance);
		    result.M41 = result.M42 = result.M44 = 0;
        }

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> around X axis.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <returns>The rotation <see cref="Matrix"/> around X axis.</returns>
        public static Matrix CreateRotationX(float radians)
        {
            Matrix result;
            CreateRotationX(radians, out result);
            return result;
        }

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> around X axis.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <param name="result">The rotation <see cref="Matrix"/> around X axis as an output parameter.</param>
        public static void CreateRotationX(float radians, out Matrix result)
        {
            result = Matrix.Identity;

			var val1 = MathF.Cos(radians);
			var val2 = MathF.Sin(radians);
			
            result.M22 = val1;
            result.M23 = val2;
            result.M32 = -val2;
            result.M33 = val1;
        }

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> around Y axis.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <returns>The rotation <see cref="Matrix"/> around Y axis.</returns>
        public static Matrix CreateRotationY(float radians)
        {
            Matrix result;
            CreateRotationY(radians, out result);
            return result;
        }

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> around Y axis.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <param name="result">The rotation <see cref="Matrix"/> around Y axis as an output parameter.</param>
        public static void CreateRotationY(float radians, out Matrix result)
        {
            result = Matrix.Identity;

            var val1 = MathF.Cos(radians);
			var val2 = MathF.Sin(radians);
			
            result.M11 = val1;
            result.M13 = -val2;
            result.M31 = val2;
            result.M33 = val1;
        }

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> around Z axis.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <returns>The rotation <see cref="Matrix"/> around Z axis.</returns>
        public static Matrix CreateRotationZ(float radians)
        {
            Matrix result;
            CreateRotationZ(radians, out result);
            return result;
        }

        /// <summary>
        /// Creates a new rotation <see cref="Matrix"/> around Z axis.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <param name="result">The rotation <see cref="Matrix"/> around Z axis as an output parameter.</param>
        public static void CreateRotationZ(float radians, out Matrix result)
        {
            result = Matrix.Identity;

			var val1 = MathF.Cos(radians);
			var val2 = MathF.Sin(radians);
			
            result.M11 = val1;
            result.M12 = val2;
            result.M21 = -val2;
            result.M22 = val1;
        }

        /// <summary>
        /// Creates a new scaling <see cref="Matrix"/>.
        /// </summary>
        /// <param name="scale">Scale value for all three axises.</param>
        /// <returns>The scaling <see cref="Matrix"/>.</returns>
        public static Matrix CreateScale(float scale)
        {
            Matrix result;
            CreateScale(scale, scale, scale, out result);
            return result;
        }

        /// <summary>
        /// Creates a new scaling <see cref="Matrix"/>.
        /// </summary>
        /// <param name="scale">Scale value for all three axises.</param>
        /// <param name="result">The scaling <see cref="Matrix"/> as an output parameter.</param>
        public static void CreateScale(float scale, out Matrix result)
        {
            CreateScale(scale, scale, scale, out result);
        }

        /// <summary>
        /// Creates a new scaling <see cref="Matrix"/>.
        /// </summary>
        /// <param name="xScale">Scale value for X axis.</param>
        /// <param name="yScale">Scale value for Y axis.</param>
        /// <param name="zScale">Scale value for Z axis.</param>
        /// <returns>The scaling <see cref="Matrix"/>.</returns>
        public static Matrix CreateScale(float xScale, float yScale, float zScale)
        {
            Matrix result;
            CreateScale(xScale, yScale, zScale, out result);
            return result;
        }

        /// <summary>
        /// Creates a new scaling <see cref="Matrix"/>.
        /// </summary>
        /// <param name="xScale">Scale value for X axis.</param>
        /// <param name="yScale">Scale value for Y axis.</param>
        /// <param name="zScale">Scale value for Z axis.</param>
        /// <param name="result">The scaling <see cref="Matrix"/> as an output parameter.</param>
        public static void CreateScale(float xScale, float yScale, float zScale, out Matrix result)
        {
			result.M11 = xScale;
			result.M12 = 0;
			result.M13 = 0;
			result.M14 = 0;
			result.M21 = 0;
			result.M22 = yScale;
			result.M23 = 0;
			result.M24 = 0;
			result.M31 = 0;
			result.M32 = 0;
			result.M33 = zScale;
			result.M34 = 0;
			result.M41 = 0;
			result.M42 = 0;
			result.M43 = 0;
			result.M44 = 1;
        }

        /// <summary>
        /// Creates a new scaling <see cref="Matrix"/>.
        /// </summary>
        /// <param name="scales"><see cref="Vector3"/> representing x,y and z scale values.</param>
        /// <returns>The scaling <see cref="Matrix"/>.</returns>
        public static Matrix CreateScale(Vector3 scales)
        {
            Matrix result;
            CreateScale(ref scales, out result);
            return result;
        }

        /// <summary>
        /// Creates a new scaling <see cref="Matrix"/>.
        /// </summary>
        /// <param name="scales"><see cref="Vector3"/> representing x,y and z scale values.</param>
        /// <param name="result">The scaling <see cref="Matrix"/> as an output parameter.</param>
        public static void CreateScale(ref Vector3 scales, out Matrix result)
        {
            result.M11 = scales.X;
            result.M12 = 0;
            result.M13 = 0;
            result.M14 = 0;
            result.M21 = 0;
            result.M22 = scales.Y;
            result.M23 = 0;
            result.M24 = 0;
            result.M31 = 0;
            result.M32 = 0;
            result.M33 = scales.Z;
            result.M34 = 0;
            result.M41 = 0;
            result.M42 = 0;
            result.M43 = 0;
            result.M44 = 1;
        }


        /// <summary>
        /// Creates a new <see cref="Matrix"/> that flattens geometry into a specified <see cref="Plane"/> as if casting a shadow from a specified light source. 
        /// </summary>
        /// <param name="lightDirection">A vector specifying the direction from which the light that will cast the shadow is coming.</param>
        /// <param name="plane">The plane onto which the new matrix should flatten geometry so as to cast a shadow.</param>
        /// <returns>A <see cref="Matrix"/> that can be used to flatten geometry onto the specified plane from the specified direction. </returns>
        public static Matrix CreateShadow(Vector3 lightDirection, Plane plane)
        {
            Matrix result;
            CreateShadow(ref lightDirection, ref plane, out result);
            return result;
        }


        /// <summary>
        /// Creates a new <see cref="Matrix"/> that flattens geometry into a specified <see cref="Plane"/> as if casting a shadow from a specified light source. 
        /// </summary>
        /// <param name="lightDirection">A vector specifying the direction from which the light that will cast the shadow is coming.</param>
        /// <param name="plane">The plane onto which the new matrix should flatten geometry so as to cast a shadow.</param>
        /// <param name="result">A <see cref="Matrix"/> that can be used to flatten geometry onto the specified plane from the specified direction as an output parameter.</param>
        public static void CreateShadow(ref Vector3 lightDirection, ref Plane plane, out Matrix result)
        {
            float dot = (plane.Normal.X * lightDirection.X) + (plane.Normal.Y * lightDirection.Y) + (plane.Normal.Z * lightDirection.Z);
            float x = -plane.Normal.X;
            float y = -plane.Normal.Y;
            float z = -plane.Normal.Z;
            float d = -plane.D;

            result.M11 = (x * lightDirection.X) + dot;
            result.M12 = x * lightDirection.Y;
            result.M13 = x * lightDirection.Z;
            result.M14 = 0;
            result.M21 = y * lightDirection.X;
            result.M22 = (y * lightDirection.Y) + dot;
            result.M23 = y * lightDirection.Z;
            result.M24 = 0;            
            result.M31 = z * lightDirection.X;
            result.M32 = z * lightDirection.Y;
            result.M33 = (z * lightDirection.Z) + dot;
            result.M34 = 0;            
            result.M41 = d * lightDirection.X;
            result.M42 = d * lightDirection.Y;
            result.M43 = d * lightDirection.Z;
            result.M44 = dot;
        }
        
        /// <summary>
        /// Creates a new translation <see cref="Matrix"/>.
        /// </summary>
        /// <param name="xPosition">X coordinate of translation.</param>
        /// <param name="yPosition">Y coordinate of translation.</param>
        /// <param name="zPosition">Z coordinate of translation.</param>
        /// <returns>The translation <see cref="Matrix"/>.</returns>
        public static Matrix CreateTranslation(float xPosition, float yPosition, float zPosition)
        {
            Matrix result;
            CreateTranslation(xPosition, yPosition, zPosition, out result);
            return result;
        }

        /// <summary>
        /// Creates a new translation <see cref="Matrix"/>.
        /// </summary>
        /// <param name="position">X,Y and Z coordinates of translation.</param>
        /// <param name="result">The translation <see cref="Matrix"/> as an output parameter.</param>
        public static void CreateTranslation(ref Vector3 position, out Matrix result)
        {
            result.M11 = 1;
            result.M12 = 0;
            result.M13 = 0;
            result.M14 = 0;
            result.M21 = 0;
            result.M22 = 1;
            result.M23 = 0;
            result.M24 = 0;
            result.M31 = 0;
            result.M32 = 0;
            result.M33 = 1;
            result.M34 = 0;
            result.M41 = position.X;
            result.M42 = position.Y;
            result.M43 = position.Z;
            result.M44 = 1;
        }

        /// <summary>
        /// Creates a new translation <see cref="Matrix"/>.
        /// </summary>
        /// <param name="position">X,Y and Z coordinates of translation.</param>
        /// <returns>The translation <see cref="Matrix"/>.</returns>
        public static Matrix CreateTranslation(Vector3 position)
        {
			Matrix result;
            CreateTranslation(ref position, out result);
			return result;
        }

        /// <summary>
        /// Creates a new translation <see cref="Matrix"/>.
        /// </summary>
        /// <param name="xPosition">X coordinate of translation.</param>
        /// <param name="yPosition">Y coordinate of translation.</param>
        /// <param name="zPosition">Z coordinate of translation.</param>
        /// <param name="result">The translation <see cref="Matrix"/> as an output parameter.</param>
        public static void CreateTranslation(float xPosition, float yPosition, float zPosition, out Matrix result)
        {
            result.M11 = 1;
			result.M12 = 0;
			result.M13 = 0;
			result.M14 = 0;
			result.M21 = 0;
			result.M22 = 1;
			result.M23 = 0;
			result.M24 = 0;
			result.M31 = 0;
			result.M32 = 0;
			result.M33 = 1;
			result.M34 = 0;
			result.M41 = xPosition;
			result.M42 = yPosition;
			result.M43 = zPosition;
			result.M44 = 1;
        }
        
        /// <summary>
        /// Creates a new reflection <see cref="Matrix"/>.
        /// </summary>
        /// <param name="value">The plane that used for reflection calculation.</param>
        /// <returns>The reflection <see cref="Matrix"/>.</returns>
        public static Matrix CreateReflection(Plane value)
        {
            Matrix result;
            CreateReflection(ref value, out result);
            return result;
        }

        /// <summary>
        /// Creates a new reflection <see cref="Matrix"/>.
        /// </summary>
        /// <param name="value">The plane that used for reflection calculation.</param>
        /// <param name="result">The reflection <see cref="Matrix"/> as an output parameter.</param>
        public static void CreateReflection(ref Plane value, out Matrix result)
        {
            Plane plane;
            Plane.Normalize(ref value, out plane);
            float x = plane.Normal.X;
            float y = plane.Normal.Y;
            float z = plane.Normal.Z;
            float num3 = -2f * x;
            float num2 = -2f * y;
            float num = -2f * z;
            result.M11 = (num3 * x) + 1f;
            result.M12 = num2 * x;
            result.M13 = num * x;
            result.M14 = 0;
            result.M21 = num3 * y;
            result.M22 = (num2 * y) + 1;
            result.M23 = num * y;
            result.M24 = 0;
            result.M31 = num3 * z;
            result.M32 = num2 * z;
            result.M33 = (num * z) + 1;
            result.M34 = 0;
            result.M41 = num3 * plane.D;
            result.M42 = num2 * plane.D;
            result.M43 = num * plane.D;
            result.M44 = 1;
        }

        /// <summary>
        /// Creates a new world <see cref="Matrix"/>.
        /// </summary>
        /// <param name="position">The position vector.</param>
        /// <param name="forward">The forward direction vector.</param>
        /// <param name="up">The upward direction vector. Usually <see cref="Vector3.Up"/>.</param>
        /// <returns>The world <see cref="Matrix"/>.</returns>
        public static Matrix CreateWorld(Vector3 position, Vector3 forward, Vector3 up)
        {
            Matrix ret;
                        CreateWorld(ref position, ref forward, ref up, out ret);
                        return ret;
        }

        /// <summary>
        /// Creates a new world <see cref="Matrix"/>.
        /// </summary>
        /// <param name="position">The position vector.</param>
        /// <param name="forward">The forward direction vector.</param>
        /// <param name="up">The upward direction vector. Usually <see cref="Vector3.Up"/>.</param>
        /// <param name="result">The world <see cref="Matrix"/> as an output parameter.</param>
        public static void CreateWorld(ref Vector3 position, ref Vector3 forward, ref Vector3 up, out Matrix result)
        {
                        Vector3 x, y, z;
                        Vector3.Normalize(ref forward, out z);
                        Vector3.Cross(ref forward, ref up, out x);
                        Vector3.Cross(ref x, ref forward, out y);
                        x.Normalize();
                        y.Normalize();            
                        
                        result = new Matrix();
                        result.Right = x;
                        result.Up = y;
                        result.Forward = z;
                        result.Translation = position;
                        result.M44 = 1f;
        }

        /// <summary>
        /// Decomposes this matrix to translation, rotation and scale elements. Returns <c>true</c> if matrix can be decomposed; <c>false</c> otherwise.
        /// </summary>
        /// <param name="scale">Scale vector as an output parameter.</param>
        /// <param name="rotation">Rotation quaternion as an output parameter.</param>
        /// <param name="translation">Translation vector as an output parameter.</param>
        /// <returns><c>true</c> if matrix can be decomposed; <c>false</c> otherwise.</returns>
        public bool Decompose(out Vector3 scale, out Quaternion rotation, out Vector3 translation)
        {
            translation.X = this.M41;
            translation.Y = this.M42;
            translation.Z = this.M43;

            float xs = (Math.Sign(M11 * M12 * M13 * M14) < 0) ? -1 : 1;
            float ys = (Math.Sign(M21 * M22 * M23 * M24) < 0) ? -1 : 1;
            float zs = (Math.Sign(M31 * M32 * M33 * M34) < 0) ? -1 : 1;

            scale.X = xs * MathF.Sqrt(this.M11 * this.M11 + this.M12 * this.M12 + this.M13 * this.M13);
            scale.Y = ys * MathF.Sqrt(this.M21 * this.M21 + this.M22 * this.M22 + this.M23 * this.M23);
            scale.Z = zs * MathF.Sqrt(this.M31 * this.M31 + this.M32 * this.M32 + this.M33 * this.M33);

            if (scale.X == 0.0 || scale.Y == 0.0 || scale.Z == 0.0)
            {
                rotation = Quaternion.Identity;
                return false;
            }

            Matrix m1 = new Matrix(this.M11 / scale.X, M12 / scale.X, M13 / scale.X, 0,
                                   this.M21 / scale.Y, M22 / scale.Y, M23 / scale.Y, 0,
                                   this.M31 / scale.Z, M32 / scale.Z, M33 / scale.Z, 0,
                                   0, 0, 0, 1);

            rotation = Quaternion.CreateFromRotationMatrix(m1);
            return true;
        }	

		/// <summary>
        /// Returns a determinant of this <see cref="Matrix"/>.
        /// </summary>
        /// <returns>Determinant of this <see cref="Matrix"/></returns>
        /// <remarks>See more about determinant here - http://en.wikipedia.org/wiki/Determinant.
        /// </remarks>
        public float Determinant()
        {
            float num22 = this.M11;
		    float num21 = this.M12;
		    float num20 = this.M13;
		    float num19 = this.M14;
		    float num12 = this.M21;
		    float num11 = this.M22;
		    float num10 = this.M23;
		    float num9 = this.M24;
		    float num8 = this.M31;
		    float num7 = this.M32;
		    float num6 = this.M33;
		    float num5 = this.M34;
		    float num4 = this.M41;
		    float num3 = this.M42;
		    float num2 = this.M43;
		    float num = this.M44;
		    float num18 = (num6 * num) - (num5 * num2);
		    float num17 = (num7 * num) - (num5 * num3);
		    float num16 = (num7 * num2) - (num6 * num3);
		    float num15 = (num8 * num) - (num5 * num4);
		    float num14 = (num8 * num2) - (num6 * num4);
		    float num13 = (num8 * num3) - (num7 * num4);
		    return ((((num22 * (((num11 * num18) - (num10 * num17)) + (num9 * num16))) - (num21 * (((num12 * num18) - (num10 * num15)) + (num9 * num14)))) + (num20 * (((num12 * num17) - (num11 * num15)) + (num9 * num13)))) - (num19 * (((num12 * num16) - (num11 * num14)) + (num10 * num13))));
        }

        /// <summary>
        /// Divides the elements of a <see cref="Matrix"/> by the elements of another matrix.
        /// </summary>
        /// <param name="matrix1">Source <see cref="Matrix"/>.</param>
        /// <param name="matrix2">Divisor <see cref="Matrix"/>.</param>
        /// <returns>The result of dividing the matrix.</returns>
        public static Matrix Divide(Matrix matrix1, Matrix matrix2)
        {
		    matrix1.M11 = matrix1.M11 / matrix2.M11;
		    matrix1.M12 = matrix1.M12 / matrix2.M12;
		    matrix1.M13 = matrix1.M13 / matrix2.M13;
		    matrix1.M14 = matrix1.M14 / matrix2.M14;
		    matrix1.M21 = matrix1.M21 / matrix2.M21;
		    matrix1.M22 = matrix1.M22 / matrix2.M22;
		    matrix1.M23 = matrix1.M23 / matrix2.M23;
		    matrix1.M24 = matrix1.M24 / matrix2.M24;
		    matrix1.M31 = matrix1.M31 / matrix2.M31;
		    matrix1.M32 = matrix1.M32 / matrix2.M32;
		    matrix1.M33 = matrix1.M33 / matrix2.M33;
		    matrix1.M34 = matrix1.M34 / matrix2.M34;
		    matrix1.M41 = matrix1.M41 / matrix2.M41;
		    matrix1.M42 = matrix1.M42 / matrix2.M42;
		    matrix1.M43 = matrix1.M43 / matrix2.M43;
		    matrix1.M44 = matrix1.M44 / matrix2.M44;
		    return matrix1;
        }

        /// <summary>
        /// Divides the elements of a <see cref="Matrix"/> by the elements of another matrix.
        /// </summary>
        /// <param name="matrix1">Source <see cref="Matrix"/>.</param>
        /// <param name="matrix2">Divisor <see cref="Matrix"/>.</param>
        /// <param name="result">The result of dividing the matrix as an output parameter.</param>
        public static void Divide(ref Matrix matrix1, ref Matrix matrix2, out Matrix result)
        {
            result.M11 = matrix1.M11 / matrix2.M11;
		    result.M12 = matrix1.M12 / matrix2.M12;
		    result.M13 = matrix1.M13 / matrix2.M13;
		    result.M14 = matrix1.M14 / matrix2.M14;
		    result.M21 = matrix1.M21 / matrix2.M21;
		    result.M22 = matrix1.M22 / matrix2.M22;
		    result.M23 = matrix1.M23 / matrix2.M23;
		    result.M24 = matrix1.M24 / matrix2.M24;
		    result.M31 = matrix1.M31 / matrix2.M31;
		    result.M32 = matrix1.M32 / matrix2.M32;
		    result.M33 = matrix1.M33 / matrix2.M33;
		    result.M34 = matrix1.M34 / matrix2.M34;
		    result.M41 = matrix1.M41 / matrix2.M41;
		    result.M42 = matrix1.M42 / matrix2.M42;
		    result.M43 = matrix1.M43 / matrix2.M43;
		    result.M44 = matrix1.M44 / matrix2.M44;
        }

        /// <summary>
        /// Divides the elements of a <see cref="Matrix"/> by a scalar.
        /// </summary>
        /// <param name="matrix1">Source <see cref="Matrix"/>.</param>
        /// <param name="divider">Divisor scalar.</param>
        /// <returns>The result of dividing a matrix by a scalar.</returns>
        public static Matrix Divide(Matrix matrix1, float divider)
        {
		    float num = 1f / divider;
		    matrix1.M11 = matrix1.M11 * num;
		    matrix1.M12 = matrix1.M12 * num;
		    matrix1.M13 = matrix1.M13 * num;
		    matrix1.M14 = matrix1.M14 * num;
		    matrix1.M21 = matrix1.M21 * num;
		    matrix1.M22 = matrix1.M22 * num;
		    matrix1.M23 = matrix1.M23 * num;
		    matrix1.M24 = matrix1.M24 * num;
		    matrix1.M31 = matrix1.M31 * num;
		    matrix1.M32 = matrix1.M32 * num;
		    matrix1.M33 = matrix1.M33 * num;
		    matrix1.M34 = matrix1.M34 * num;
		    matrix1.M41 = matrix1.M41 * num;
		    matrix1.M42 = matrix1.M42 * num;
		    matrix1.M43 = matrix1.M43 * num;
		    matrix1.M44 = matrix1.M44 * num;
		    return matrix1;
        }

        /// <summary>
        /// Divides the elements of a <see cref="Matrix"/> by a scalar.
        /// </summary>
        /// <param name="matrix1">Source <see cref="Matrix"/>.</param>
        /// <param name="divider">Divisor scalar.</param>
        /// <param name="result">The result of dividing a matrix by a scalar as an output parameter.</param>
        public static void Divide(ref Matrix matrix1, float divider, out Matrix result)
        {
            float num = 1f / divider;
		    result.M11 = matrix1.M11 * num;
		    result.M12 = matrix1.M12 * num;
		    result.M13 = matrix1.M13 * num;
		    result.M14 = matrix1.M14 * num;
		    result.M21 = matrix1.M21 * num;
		    result.M22 = matrix1.M22 * num;
		    result.M23 = matrix1.M23 * num;
		    result.M24 = matrix1.M24 * num;
		    result.M31 = matrix1.M31 * num;
		    result.M32 = matrix1.M32 * num;
		    result.M33 = matrix1.M33 * num;
		    result.M34 = matrix1.M34 * num;
		    result.M41 = matrix1.M41 * num;
		    result.M42 = matrix1.M42 * num;
		    result.M43 = matrix1.M43 * num;
		    result.M44 = matrix1.M44 * num;
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Matrix"/> without any tolerance.
        /// </summary>
        /// <param name="other">The <see cref="Matrix"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public bool Equals(Matrix other)
        {
            return ((((((this.M11 == other.M11) && (this.M22 == other.M22)) && ((this.M33 == other.M33) && (this.M44 == other.M44))) && (((this.M12 == other.M12) && (this.M13 == other.M13)) && ((this.M14 == other.M14) && (this.M21 == other.M21)))) && ((((this.M23 == other.M23) && (this.M24 == other.M24)) && ((this.M31 == other.M31) && (this.M32 == other.M32))) && (((this.M34 == other.M34) && (this.M41 == other.M41)) && (this.M42 == other.M42)))) && (this.M43 == other.M43));
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Object"/> without any tolerance.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public override bool Equals(object obj)
        {
            bool flag = false;
		    if (obj is Matrix)
		    {
		        flag = this.Equals((Matrix) obj);
		    }
		    return flag;
        }

        /// <summary>
        /// Gets the hash code of this <see cref="Matrix"/>.
        /// </summary>
        /// <returns>Hash code of this <see cref="Matrix"/>.</returns>
        public override int GetHashCode()
        {
            return (((((((((((((((this.M11.GetHashCode() + this.M12.GetHashCode()) + this.M13.GetHashCode()) + this.M14.GetHashCode()) + this.M21.GetHashCode()) + this.M22.GetHashCode()) + this.M23.GetHashCode()) + this.M24.GetHashCode()) + this.M31.GetHashCode()) + this.M32.GetHashCode()) + this.M33.GetHashCode()) + this.M34.GetHashCode()) + this.M41.GetHashCode()) + this.M42.GetHashCode()) + this.M43.GetHashCode()) + this.M44.GetHashCode());
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> which contains inversion of the specified matrix. 
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/>.</param>
        /// <returns>The inverted matrix.</returns>
        public static Matrix Invert(Matrix matrix)
        {
            Matrix result;
            Invert(ref matrix, out result);
            return result;
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> which contains inversion of the specified matrix. 
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/>.</param>
        /// <param name="result">The inverted matrix as output parameter.</param>
        public static void Invert(ref Matrix matrix, out Matrix result)
        {
			float num1 = matrix.M11;
			float num2 = matrix.M12;
			float num3 = matrix.M13;
			float num4 = matrix.M14;
			float num5 = matrix.M21;
			float num6 = matrix.M22;
			float num7 = matrix.M23;
			float num8 = matrix.M24;
			float num9 = matrix.M31;
			float num10 = matrix.M32;
			float num11 = matrix.M33;
			float num12 = matrix.M34;
			float num13 = matrix.M41;
			float num14 = matrix.M42;
			float num15 = matrix.M43;
			float num16 = matrix.M44;
			float num17 = (float) ((double) num11 * (double) num16 - (double) num12 * (double) num15);
			float num18 = (float) ((double) num10 * (double) num16 - (double) num12 * (double) num14);
			float num19 = (float) ((double) num10 * (double) num15 - (double) num11 * (double) num14);
			float num20 = (float) ((double) num9 * (double) num16 - (double) num12 * (double) num13);
			float num21 = (float) ((double) num9 * (double) num15 - (double) num11 * (double) num13);
			float num22 = (float) ((double) num9 * (double) num14 - (double) num10 * (double) num13);
			float num23 = (float) ((double) num6 * (double) num17 - (double) num7 * (double) num18 + (double) num8 * (double) num19);
			float num24 = (float) -((double) num5 * (double) num17 - (double) num7 * (double) num20 + (double) num8 * (double) num21);
			float num25 = (float) ((double) num5 * (double) num18 - (double) num6 * (double) num20 + (double) num8 * (double) num22);
			float num26 = (float) -((double) num5 * (double) num19 - (double) num6 * (double) num21 + (double) num7 * (double) num22);
			float num27 = (float) (1.0 / ((double) num1 * (double) num23 + (double) num2 * (double) num24 + (double) num3 * (double) num25 + (double) num4 * (double) num26));
			
			result.M11 = num23 * num27;
			result.M21 = num24 * num27;
			result.M31 = num25 * num27;
			result.M41 = num26 * num27;
			result.M12 = (float) -((double) num2 * (double) num17 - (double) num3 * (double) num18 + (double) num4 * (double) num19) * num27;
			result.M22 = (float) ((double) num1 * (double) num17 - (double) num3 * (double) num20 + (double) num4 * (double) num21) * num27;
			result.M32 = (float) -((double) num1 * (double) num18 - (double) num2 * (double) num20 + (double) num4 * (double) num22) * num27;
			result.M42 = (float) ((double) num1 * (double) num19 - (double) num2 * (double) num21 + (double) num3 * (double) num22) * num27;
			float num28 = (float) ((double) num7 * (double) num16 - (double) num8 * (double) num15);
			float num29 = (float) ((double) num6 * (double) num16 - (double) num8 * (double) num14);
			float num30 = (float) ((double) num6 * (double) num15 - (double) num7 * (double) num14);
			float num31 = (float) ((double) num5 * (double) num16 - (double) num8 * (double) num13);
			float num32 = (float) ((double) num5 * (double) num15 - (double) num7 * (double) num13);
			float num33 = (float) ((double) num5 * (double) num14 - (double) num6 * (double) num13);
			result.M13 = (float) ((double) num2 * (double) num28 - (double) num3 * (double) num29 + (double) num4 * (double) num30) * num27;
			result.M23 = (float) -((double) num1 * (double) num28 - (double) num3 * (double) num31 + (double) num4 * (double) num32) * num27;
			result.M33 = (float) ((double) num1 * (double) num29 - (double) num2 * (double) num31 + (double) num4 * (double) num33) * num27;
			result.M43 = (float) -((double) num1 * (double) num30 - (double) num2 * (double) num32 + (double) num3 * (double) num33) * num27;
			float num34 = (float) ((double) num7 * (double) num12 - (double) num8 * (double) num11);
			float num35 = (float) ((double) num6 * (double) num12 - (double) num8 * (double) num10);
			float num36 = (float) ((double) num6 * (double) num11 - (double) num7 * (double) num10);
			float num37 = (float) ((double) num5 * (double) num12 - (double) num8 * (double) num9);
			float num38 = (float) ((double) num5 * (double) num11 - (double) num7 * (double) num9);
			float num39 = (float) ((double) num5 * (double) num10 - (double) num6 * (double) num9);
			result.M14 = (float) -((double) num2 * (double) num34 - (double) num3 * (double) num35 + (double) num4 * (double) num36) * num27;
			result.M24 = (float) ((double) num1 * (double) num34 - (double) num3 * (double) num37 + (double) num4 * (double) num38) * num27;
			result.M34 = (float) -((double) num1 * (double) num35 - (double) num2 * (double) num37 + (double) num4 * (double) num39) * num27;
			result.M44 = (float) ((double) num1 * (double) num36 - (double) num2 * (double) num38 + (double) num3 * (double) num39) * num27;
			
			
			/*
			
			
            ///
            // Use Laplace expansion theorem to calculate the inverse of a 4x4 matrix
            // 
            // 1. Calculate the 2x2 determinants needed the 4x4 determinant based on the 2x2 determinants 
            // 3. Create the adjugate matrix, which satisfies: A * adj(A) = det(A) * I
            // 4. Divide adjugate matrix with the determinant to find the inverse
            
            float det1, det2, det3, det4, det5, det6, det7, det8, det9, det10, det11, det12;
            float detMatrix;
            FindDeterminants(ref matrix, out detMatrix, out det1, out det2, out det3, out det4, out det5, out det6, 
                             out det7, out det8, out det9, out det10, out det11, out det12);
            
            float invDetMatrix = 1f / detMatrix;
            
            Matrix ret; // Allow for matrix and result to point to the same structure
            
            ret.M11 = (matrix.M22*det12 - matrix.M23*det11 + matrix.M24*det10) * invDetMatrix;
            ret.M12 = (-matrix.M12*det12 + matrix.M13*det11 - matrix.M14*det10) * invDetMatrix;
            ret.M13 = (matrix.M42*det6 - matrix.M43*det5 + matrix.M44*det4) * invDetMatrix;
            ret.M14 = (-matrix.M32*det6 + matrix.M33*det5 - matrix.M34*det4) * invDetMatrix;
            ret.M21 = (-matrix.M21*det12 + matrix.M23*det9 - matrix.M24*det8) * invDetMatrix;
            ret.M22 = (matrix.M11*det12 - matrix.M13*det9 + matrix.M14*det8) * invDetMatrix;
            ret.M23 = (-matrix.M41*det6 + matrix.M43*det3 - matrix.M44*det2) * invDetMatrix;
            ret.M24 = (matrix.M31*det6 - matrix.M33*det3 + matrix.M34*det2) * invDetMatrix;
            ret.M31 = (matrix.M21*det11 - matrix.M22*det9 + matrix.M24*det7) * invDetMatrix;
            ret.M32 = (-matrix.M11*det11 + matrix.M12*det9 - matrix.M14*det7) * invDetMatrix;
            ret.M33 = (matrix.M41*det5 - matrix.M42*det3 + matrix.M44*det1) * invDetMatrix;
            ret.M34 = (-matrix.M31*det5 + matrix.M32*det3 - matrix.M34*det1) * invDetMatrix;
            ret.M41 = (-matrix.M21*det10 + matrix.M22*det8 - matrix.M23*det7) * invDetMatrix;
            ret.M42 = (matrix.M11*det10 - matrix.M12*det8 + matrix.M13*det7) * invDetMatrix;
            ret.M43 = (-matrix.M41*det4 + matrix.M42*det2 - matrix.M43*det1) * invDetMatrix;
            ret.M44 = (matrix.M31*det4 - matrix.M32*det2 + matrix.M33*det1) * invDetMatrix;
            
            result = ret;
            */
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> that contains linear interpolation of the values in specified matrixes.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix"/>.</param>
        /// <param name="matrix2">The second <see cref="Vector2"/>.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <returns>>The result of linear interpolation of the specified matrixes.</returns>
        public static Matrix Lerp(Matrix matrix1, Matrix matrix2, float amount)
        {
		    matrix1.M11 = matrix1.M11 + ((matrix2.M11 - matrix1.M11) * amount);
		    matrix1.M12 = matrix1.M12 + ((matrix2.M12 - matrix1.M12) * amount);
		    matrix1.M13 = matrix1.M13 + ((matrix2.M13 - matrix1.M13) * amount);
		    matrix1.M14 = matrix1.M14 + ((matrix2.M14 - matrix1.M14) * amount);
		    matrix1.M21 = matrix1.M21 + ((matrix2.M21 - matrix1.M21) * amount);
		    matrix1.M22 = matrix1.M22 + ((matrix2.M22 - matrix1.M22) * amount);
		    matrix1.M23 = matrix1.M23 + ((matrix2.M23 - matrix1.M23) * amount);
		    matrix1.M24 = matrix1.M24 + ((matrix2.M24 - matrix1.M24) * amount);
		    matrix1.M31 = matrix1.M31 + ((matrix2.M31 - matrix1.M31) * amount);
		    matrix1.M32 = matrix1.M32 + ((matrix2.M32 - matrix1.M32) * amount);
		    matrix1.M33 = matrix1.M33 + ((matrix2.M33 - matrix1.M33) * amount);
		    matrix1.M34 = matrix1.M34 + ((matrix2.M34 - matrix1.M34) * amount);
		    matrix1.M41 = matrix1.M41 + ((matrix2.M41 - matrix1.M41) * amount);
		    matrix1.M42 = matrix1.M42 + ((matrix2.M42 - matrix1.M42) * amount);
		    matrix1.M43 = matrix1.M43 + ((matrix2.M43 - matrix1.M43) * amount);
		    matrix1.M44 = matrix1.M44 + ((matrix2.M44 - matrix1.M44) * amount);
		    return matrix1;
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> that contains linear interpolation of the values in specified matrixes.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix"/>.</param>
        /// <param name="matrix2">The second <see cref="Vector2"/>.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <param name="result">The result of linear interpolation of the specified matrixes as an output parameter.</param>
        public static void Lerp(ref Matrix matrix1, ref Matrix matrix2, float amount, out Matrix result)
        {
            result.M11 = matrix1.M11 + ((matrix2.M11 - matrix1.M11) * amount);
		    result.M12 = matrix1.M12 + ((matrix2.M12 - matrix1.M12) * amount);
		    result.M13 = matrix1.M13 + ((matrix2.M13 - matrix1.M13) * amount);
		    result.M14 = matrix1.M14 + ((matrix2.M14 - matrix1.M14) * amount);
		    result.M21 = matrix1.M21 + ((matrix2.M21 - matrix1.M21) * amount);
		    result.M22 = matrix1.M22 + ((matrix2.M22 - matrix1.M22) * amount);
		    result.M23 = matrix1.M23 + ((matrix2.M23 - matrix1.M23) * amount);
		    result.M24 = matrix1.M24 + ((matrix2.M24 - matrix1.M24) * amount);
		    result.M31 = matrix1.M31 + ((matrix2.M31 - matrix1.M31) * amount);
		    result.M32 = matrix1.M32 + ((matrix2.M32 - matrix1.M32) * amount);
		    result.M33 = matrix1.M33 + ((matrix2.M33 - matrix1.M33) * amount);
		    result.M34 = matrix1.M34 + ((matrix2.M34 - matrix1.M34) * amount);
		    result.M41 = matrix1.M41 + ((matrix2.M41 - matrix1.M41) * amount);
		    result.M42 = matrix1.M42 + ((matrix2.M42 - matrix1.M42) * amount);
		    result.M43 = matrix1.M43 + ((matrix2.M43 - matrix1.M43) * amount);
		    result.M44 = matrix1.M44 + ((matrix2.M44 - matrix1.M44) * amount);
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> that contains a multiplication of two matrix.
        /// </summary>
        /// <param name="matrix1">Source <see cref="Matrix"/>.</param>
        /// <param name="matrix2">Source <see cref="Matrix"/>.</param>
        /// <returns>Result of the matrix multiplication.</returns>
        public static Matrix Multiply(Matrix matrix1, Matrix matrix2)
        {
            var m11 = (((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21)) + (matrix1.M13 * matrix2.M31)) + (matrix1.M14 * matrix2.M41);
            var m12 = (((matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22)) + (matrix1.M13 * matrix2.M32)) + (matrix1.M14 * matrix2.M42);
            var m13 = (((matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23)) + (matrix1.M13 * matrix2.M33)) + (matrix1.M14 * matrix2.M43);
            var m14 = (((matrix1.M11 * matrix2.M14) + (matrix1.M12 * matrix2.M24)) + (matrix1.M13 * matrix2.M34)) + (matrix1.M14 * matrix2.M44);
            var m21 = (((matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21)) + (matrix1.M23 * matrix2.M31)) + (matrix1.M24 * matrix2.M41);
            var m22 = (((matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22)) + (matrix1.M23 * matrix2.M32)) + (matrix1.M24 * matrix2.M42);
            var m23 = (((matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23)) + (matrix1.M23 * matrix2.M33)) + (matrix1.M24 * matrix2.M43);
            var m24 = (((matrix1.M21 * matrix2.M14) + (matrix1.M22 * matrix2.M24)) + (matrix1.M23 * matrix2.M34)) + (matrix1.M24 * matrix2.M44);
            var m31 = (((matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21)) + (matrix1.M33 * matrix2.M31)) + (matrix1.M34 * matrix2.M41);
            var m32 = (((matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22)) + (matrix1.M33 * matrix2.M32)) + (matrix1.M34 * matrix2.M42);
            var m33 = (((matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23)) + (matrix1.M33 * matrix2.M33)) + (matrix1.M34 * matrix2.M43);
            var m34 = (((matrix1.M31 * matrix2.M14) + (matrix1.M32 * matrix2.M24)) + (matrix1.M33 * matrix2.M34)) + (matrix1.M34 * matrix2.M44);
            var m41 = (((matrix1.M41 * matrix2.M11) + (matrix1.M42 * matrix2.M21)) + (matrix1.M43 * matrix2.M31)) + (matrix1.M44 * matrix2.M41);
            var m42 = (((matrix1.M41 * matrix2.M12) + (matrix1.M42 * matrix2.M22)) + (matrix1.M43 * matrix2.M32)) + (matrix1.M44 * matrix2.M42);
            var m43 = (((matrix1.M41 * matrix2.M13) + (matrix1.M42 * matrix2.M23)) + (matrix1.M43 * matrix2.M33)) + (matrix1.M44 * matrix2.M43);
           	var m44 = (((matrix1.M41 * matrix2.M14) + (matrix1.M42 * matrix2.M24)) + (matrix1.M43 * matrix2.M34)) + (matrix1.M44 * matrix2.M44);
            matrix1.M11 = m11;
			matrix1.M12 = m12;
			matrix1.M13 = m13;
			matrix1.M14 = m14;
			matrix1.M21 = m21;
			matrix1.M22 = m22;
			matrix1.M23 = m23;
			matrix1.M24 = m24;
			matrix1.M31 = m31;
			matrix1.M32 = m32;
			matrix1.M33 = m33;
			matrix1.M34 = m34;
			matrix1.M41 = m41;
			matrix1.M42 = m42;
			matrix1.M43 = m43;
			matrix1.M44 = m44;
			return matrix1;
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> that contains a multiplication of two matrix.
        /// </summary>
        /// <param name="matrix1">Source <see cref="Matrix"/>.</param>
        /// <param name="matrix2">Source <see cref="Matrix"/>.</param>
        /// <param name="result">Result of the matrix multiplication as an output parameter.</param>
        public static void Multiply(ref Matrix matrix1, ref Matrix matrix2, out Matrix result)
        {
            var m11 = (((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21)) + (matrix1.M13 * matrix2.M31)) + (matrix1.M14 * matrix2.M41);
            var m12 = (((matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22)) + (matrix1.M13 * matrix2.M32)) + (matrix1.M14 * matrix2.M42);
            var m13 = (((matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23)) + (matrix1.M13 * matrix2.M33)) + (matrix1.M14 * matrix2.M43);
            var m14 = (((matrix1.M11 * matrix2.M14) + (matrix1.M12 * matrix2.M24)) + (matrix1.M13 * matrix2.M34)) + (matrix1.M14 * matrix2.M44);
            var m21 = (((matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21)) + (matrix1.M23 * matrix2.M31)) + (matrix1.M24 * matrix2.M41);
            var m22 = (((matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22)) + (matrix1.M23 * matrix2.M32)) + (matrix1.M24 * matrix2.M42);
            var m23 = (((matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23)) + (matrix1.M23 * matrix2.M33)) + (matrix1.M24 * matrix2.M43);
            var m24 = (((matrix1.M21 * matrix2.M14) + (matrix1.M22 * matrix2.M24)) + (matrix1.M23 * matrix2.M34)) + (matrix1.M24 * matrix2.M44);
            var m31 = (((matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21)) + (matrix1.M33 * matrix2.M31)) + (matrix1.M34 * matrix2.M41);
            var m32 = (((matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22)) + (matrix1.M33 * matrix2.M32)) + (matrix1.M34 * matrix2.M42);
            var m33 = (((matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23)) + (matrix1.M33 * matrix2.M33)) + (matrix1.M34 * matrix2.M43);
            var m34 = (((matrix1.M31 * matrix2.M14) + (matrix1.M32 * matrix2.M24)) + (matrix1.M33 * matrix2.M34)) + (matrix1.M34 * matrix2.M44);
            var m41 = (((matrix1.M41 * matrix2.M11) + (matrix1.M42 * matrix2.M21)) + (matrix1.M43 * matrix2.M31)) + (matrix1.M44 * matrix2.M41);
            var m42 = (((matrix1.M41 * matrix2.M12) + (matrix1.M42 * matrix2.M22)) + (matrix1.M43 * matrix2.M32)) + (matrix1.M44 * matrix2.M42);
            var m43 = (((matrix1.M41 * matrix2.M13) + (matrix1.M42 * matrix2.M23)) + (matrix1.M43 * matrix2.M33)) + (matrix1.M44 * matrix2.M43);
           	var m44 = (((matrix1.M41 * matrix2.M14) + (matrix1.M42 * matrix2.M24)) + (matrix1.M43 * matrix2.M34)) + (matrix1.M44 * matrix2.M44);
            result.M11 = m11;
			result.M12 = m12;
			result.M13 = m13;
			result.M14 = m14;
			result.M21 = m21;
			result.M22 = m22;
			result.M23 = m23;
			result.M24 = m24;
			result.M31 = m31;
			result.M32 = m32;
			result.M33 = m33;
			result.M34 = m34;
			result.M41 = m41;
			result.M42 = m42;
			result.M43 = m43;
			result.M44 = m44;
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> that contains a multiplication of <see cref="Matrix"/> and a scalar.
        /// </summary>
        /// <param name="matrix1">Source <see cref="Matrix"/>.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        /// <returns>Result of the matrix multiplication with a scalar.</returns>
        public static Matrix Multiply(Matrix matrix1, float scaleFactor)
        {
            matrix1.M11 *= scaleFactor;
            matrix1.M12 *= scaleFactor;
            matrix1.M13 *= scaleFactor;
            matrix1.M14 *= scaleFactor;
            matrix1.M21 *= scaleFactor;
            matrix1.M22 *= scaleFactor;
            matrix1.M23 *= scaleFactor;
            matrix1.M24 *= scaleFactor;
            matrix1.M31 *= scaleFactor;
            matrix1.M32 *= scaleFactor;
            matrix1.M33 *= scaleFactor;
            matrix1.M34 *= scaleFactor;
            matrix1.M41 *= scaleFactor;
            matrix1.M42 *= scaleFactor;
            matrix1.M43 *= scaleFactor;
            matrix1.M44 *= scaleFactor;
            return matrix1;
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> that contains a multiplication of <see cref="Matrix"/> and a scalar.
        /// </summary>
        /// <param name="matrix1">Source <see cref="Matrix"/>.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        /// <param name="result">Result of the matrix multiplication with a scalar as an output parameter.</param>
        public static void Multiply(ref Matrix matrix1, float scaleFactor, out Matrix result)
        {
            result.M11 = matrix1.M11 * scaleFactor;
            result.M12 = matrix1.M12 * scaleFactor;
            result.M13 = matrix1.M13 * scaleFactor;
            result.M14 = matrix1.M14 * scaleFactor;
            result.M21 = matrix1.M21 * scaleFactor;
            result.M22 = matrix1.M22 * scaleFactor;
            result.M23 = matrix1.M23 * scaleFactor;
            result.M24 = matrix1.M24 * scaleFactor;
            result.M31 = matrix1.M31 * scaleFactor;
            result.M32 = matrix1.M32 * scaleFactor;
            result.M33 = matrix1.M33 * scaleFactor;
            result.M34 = matrix1.M34 * scaleFactor;
            result.M41 = matrix1.M41 * scaleFactor;
            result.M42 = matrix1.M42 * scaleFactor;
            result.M43 = matrix1.M43 * scaleFactor;
            result.M44 = matrix1.M44 * scaleFactor;

        }

        /// <summary>
        /// Copy the values of specified <see cref="Matrix"/> to the float array.
        /// </summary>
        /// <param name="matrix">The source <see cref="Matrix"/>.</param>
        /// <returns>The array which matrix values will be stored.</returns>
        /// <remarks>
        /// Required for OpenGL 2.0 projection matrix stuff.
        /// </remarks>
        public static float[] ToFloatArray(Matrix matrix)
        {
            float[] matarray = {
									matrix.M11, matrix.M12, matrix.M13, matrix.M14,
									matrix.M21, matrix.M22, matrix.M23, matrix.M24,
									matrix.M31, matrix.M32, matrix.M33, matrix.M34,
									matrix.M41, matrix.M42, matrix.M43, matrix.M44
								};
            return matarray;
        }

        /// <summary>
        /// Returns a matrix with the all values negated.
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/>.</param>
        /// <returns>Result of the matrix negation.</returns>
        public static Matrix Negate(Matrix matrix)
        {
		    matrix.M11 = -matrix.M11;
		    matrix.M12 = -matrix.M12;
		    matrix.M13 = -matrix.M13;
		    matrix.M14 = -matrix.M14;
		    matrix.M21 = -matrix.M21;
		    matrix.M22 = -matrix.M22;
		    matrix.M23 = -matrix.M23;
		    matrix.M24 = -matrix.M24;
		    matrix.M31 = -matrix.M31;
		    matrix.M32 = -matrix.M32;
		    matrix.M33 = -matrix.M33;
		    matrix.M34 = -matrix.M34;
		    matrix.M41 = -matrix.M41;
		    matrix.M42 = -matrix.M42;
		    matrix.M43 = -matrix.M43;
		    matrix.M44 = -matrix.M44;
		    return matrix;
        }

        /// <summary>
        /// Returns a matrix with the all values negated.
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/>.</param>
        /// <param name="result">Result of the matrix negation as an output parameter.</param>
        public static void Negate(ref Matrix matrix, out Matrix result)
        {
            result.M11 = -matrix.M11;
		    result.M12 = -matrix.M12;
		    result.M13 = -matrix.M13;
		    result.M14 = -matrix.M14;
		    result.M21 = -matrix.M21;
		    result.M22 = -matrix.M22;
		    result.M23 = -matrix.M23;
		    result.M24 = -matrix.M24;
		    result.M31 = -matrix.M31;
		    result.M32 = -matrix.M32;
		    result.M33 = -matrix.M33;
		    result.M34 = -matrix.M34;
		    result.M41 = -matrix.M41;
		    result.M42 = -matrix.M42;
		    result.M43 = -matrix.M43;
		    result.M44 = -matrix.M44;
        }

        /// <summary>
        /// Converts a <see cref="System.Numerics.Matrix4x4"/> to a <see cref="Matrix"/>.
        /// </summary>
        /// <param name="value">The converted value.</param>
        public static implicit operator Matrix(System.Numerics.Matrix4x4 value)
        {
            return new Matrix(
                value.M11, value.M12, value.M13, value.M14,
                value.M21, value.M22, value.M23, value.M24,
                value.M31, value.M32, value.M33, value.M34,
                value.M41, value.M42, value.M43, value.M44);
        }

        /// <summary>
        /// Adds two matrixes.
        /// </summary>
        /// <param name="matrix1">Source <see cref="Matrix"/> on the left of the add sign.</param>
        /// <param name="matrix2">Source <see cref="Matrix"/> on the right of the add sign.</param>
        /// <returns>Sum of the matrixes.</returns>
        public static Matrix operator +(Matrix matrix1, Matrix matrix2)
        {
            matrix1.M11 = matrix1.M11 + matrix2.M11;
            matrix1.M12 = matrix1.M12 + matrix2.M12;
            matrix1.M13 = matrix1.M13 + matrix2.M13;
            matrix1.M14 = matrix1.M14 + matrix2.M14;
            matrix1.M21 = matrix1.M21 + matrix2.M21;
            matrix1.M22 = matrix1.M22 + matrix2.M22;
            matrix1.M23 = matrix1.M23 + matrix2.M23;
            matrix1.M24 = matrix1.M24 + matrix2.M24;
            matrix1.M31 = matrix1.M31 + matrix2.M31;
            matrix1.M32 = matrix1.M32 + matrix2.M32;
            matrix1.M33 = matrix1.M33 + matrix2.M33;
            matrix1.M34 = matrix1.M34 + matrix2.M34;
            matrix1.M41 = matrix1.M41 + matrix2.M41;
            matrix1.M42 = matrix1.M42 + matrix2.M42;
            matrix1.M43 = matrix1.M43 + matrix2.M43;
            matrix1.M44 = matrix1.M44 + matrix2.M44;
            return matrix1;
        }

        /// <summary>
        /// Divides the elements of a <see cref="Matrix"/> by the elements of another <see cref="Matrix"/>.
        /// </summary>
        /// <param name="matrix1">Source <see cref="Matrix"/> on the left of the div sign.</param>
        /// <param name="matrix2">Divisor <see cref="Matrix"/> on the right of the div sign.</param>
        /// <returns>The result of dividing the matrixes.</returns>
        public static Matrix operator /(Matrix matrix1, Matrix matrix2)
        {
		    matrix1.M11 = matrix1.M11 / matrix2.M11;
		    matrix1.M12 = matrix1.M12 / matrix2.M12;
		    matrix1.M13 = matrix1.M13 / matrix2.M13;
		    matrix1.M14 = matrix1.M14 / matrix2.M14;
		    matrix1.M21 = matrix1.M21 / matrix2.M21;
		    matrix1.M22 = matrix1.M22 / matrix2.M22;
		    matrix1.M23 = matrix1.M23 / matrix2.M23;
		    matrix1.M24 = matrix1.M24 / matrix2.M24;
		    matrix1.M31 = matrix1.M31 / matrix2.M31;
		    matrix1.M32 = matrix1.M32 / matrix2.M32;
		    matrix1.M33 = matrix1.M33 / matrix2.M33;
		    matrix1.M34 = matrix1.M34 / matrix2.M34;
		    matrix1.M41 = matrix1.M41 / matrix2.M41;
		    matrix1.M42 = matrix1.M42 / matrix2.M42;
		    matrix1.M43 = matrix1.M43 / matrix2.M43;
		    matrix1.M44 = matrix1.M44 / matrix2.M44;
		    return matrix1;
        }

        /// <summary>
        /// Divides the elements of a <see cref="Matrix"/> by a scalar.
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/> on the left of the div sign.</param>
        /// <param name="divider">Divisor scalar on the right of the div sign.</param>
        /// <returns>The result of dividing a matrix by a scalar.</returns>
        public static Matrix operator /(Matrix matrix, float divider)
        {
		    float num = 1f / divider;
		    matrix.M11 = matrix.M11 * num;
		    matrix.M12 = matrix.M12 * num;
		    matrix.M13 = matrix.M13 * num;
		    matrix.M14 = matrix.M14 * num;
		    matrix.M21 = matrix.M21 * num;
		    matrix.M22 = matrix.M22 * num;
		    matrix.M23 = matrix.M23 * num;
		    matrix.M24 = matrix.M24 * num;
		    matrix.M31 = matrix.M31 * num;
		    matrix.M32 = matrix.M32 * num;
		    matrix.M33 = matrix.M33 * num;
		    matrix.M34 = matrix.M34 * num;
		    matrix.M41 = matrix.M41 * num;
		    matrix.M42 = matrix.M42 * num;
		    matrix.M43 = matrix.M43 * num;
		    matrix.M44 = matrix.M44 * num;
		    return matrix;
        }

        /// <summary>
        /// Compares whether two <see cref="Matrix"/> instances are equal without any tolerance.
        /// </summary>
        /// <param name="matrix1">Source <see cref="Matrix"/> on the left of the equal sign.</param>
        /// <param name="matrix2">Source <see cref="Matrix"/> on the right of the equal sign.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(Matrix matrix1, Matrix matrix2)
        {
            return (
                matrix1.M11 == matrix2.M11 &&
                matrix1.M12 == matrix2.M12 &&
                matrix1.M13 == matrix2.M13 &&
                matrix1.M14 == matrix2.M14 &&
                matrix1.M21 == matrix2.M21 &&
                matrix1.M22 == matrix2.M22 &&
                matrix1.M23 == matrix2.M23 &&
                matrix1.M24 == matrix2.M24 &&
                matrix1.M31 == matrix2.M31 &&
                matrix1.M32 == matrix2.M32 &&
                matrix1.M33 == matrix2.M33 &&
                matrix1.M34 == matrix2.M34 &&
                matrix1.M41 == matrix2.M41 &&
                matrix1.M42 == matrix2.M42 &&
                matrix1.M43 == matrix2.M43 &&
                matrix1.M44 == matrix2.M44                  
                );
        }

        /// <summary>
        /// Compares whether two <see cref="Matrix"/> instances are not equal without any tolerance.
        /// </summary>
        /// <param name="matrix1">Source <see cref="Matrix"/> on the left of the not equal sign.</param>
        /// <param name="matrix2">Source <see cref="Matrix"/> on the right of the not equal sign.</param>
        /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(Matrix matrix1, Matrix matrix2)
        {
            return (
                matrix1.M11 != matrix2.M11 ||
                matrix1.M12 != matrix2.M12 ||
                matrix1.M13 != matrix2.M13 ||
                matrix1.M14 != matrix2.M14 ||
                matrix1.M21 != matrix2.M21 ||
                matrix1.M22 != matrix2.M22 ||
                matrix1.M23 != matrix2.M23 ||
                matrix1.M24 != matrix2.M24 ||
                matrix1.M31 != matrix2.M31 ||
                matrix1.M32 != matrix2.M32 ||
                matrix1.M33 != matrix2.M33 ||
                matrix1.M34 != matrix2.M34 || 
                matrix1.M41 != matrix2.M41 ||
                matrix1.M42 != matrix2.M42 ||
                matrix1.M43 != matrix2.M43 ||
                matrix1.M44 != matrix2.M44                  
                );
        }

        /// <summary>
        /// Multiplies two matrixes.
        /// </summary>
        /// <param name="matrix1">Source <see cref="Matrix"/> on the left of the mul sign.</param>
        /// <param name="matrix2">Source <see cref="Matrix"/> on the right of the mul sign.</param>
        /// <returns>Result of the matrix multiplication.</returns>
        /// <remarks>
        /// Using matrix multiplication algorithm - see http://en.wikipedia.org/wiki/Matrix_multiplication.
        /// </remarks>
        public static Matrix operator *(Matrix matrix1, Matrix matrix2)
        {
            var m11 = (((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21)) + (matrix1.M13 * matrix2.M31)) + (matrix1.M14 * matrix2.M41);
            var m12 = (((matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22)) + (matrix1.M13 * matrix2.M32)) + (matrix1.M14 * matrix2.M42);
            var m13 = (((matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23)) + (matrix1.M13 * matrix2.M33)) + (matrix1.M14 * matrix2.M43);
            var m14 = (((matrix1.M11 * matrix2.M14) + (matrix1.M12 * matrix2.M24)) + (matrix1.M13 * matrix2.M34)) + (matrix1.M14 * matrix2.M44);
            var m21 = (((matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21)) + (matrix1.M23 * matrix2.M31)) + (matrix1.M24 * matrix2.M41);
            var m22 = (((matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22)) + (matrix1.M23 * matrix2.M32)) + (matrix1.M24 * matrix2.M42);
            var m23 = (((matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23)) + (matrix1.M23 * matrix2.M33)) + (matrix1.M24 * matrix2.M43);
            var m24 = (((matrix1.M21 * matrix2.M14) + (matrix1.M22 * matrix2.M24)) + (matrix1.M23 * matrix2.M34)) + (matrix1.M24 * matrix2.M44);
            var m31 = (((matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21)) + (matrix1.M33 * matrix2.M31)) + (matrix1.M34 * matrix2.M41);
            var m32 = (((matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22)) + (matrix1.M33 * matrix2.M32)) + (matrix1.M34 * matrix2.M42);
            var m33 = (((matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23)) + (matrix1.M33 * matrix2.M33)) + (matrix1.M34 * matrix2.M43);
            var m34 = (((matrix1.M31 * matrix2.M14) + (matrix1.M32 * matrix2.M24)) + (matrix1.M33 * matrix2.M34)) + (matrix1.M34 * matrix2.M44);
            var m41 = (((matrix1.M41 * matrix2.M11) + (matrix1.M42 * matrix2.M21)) + (matrix1.M43 * matrix2.M31)) + (matrix1.M44 * matrix2.M41);
            var m42 = (((matrix1.M41 * matrix2.M12) + (matrix1.M42 * matrix2.M22)) + (matrix1.M43 * matrix2.M32)) + (matrix1.M44 * matrix2.M42);
            var m43 = (((matrix1.M41 * matrix2.M13) + (matrix1.M42 * matrix2.M23)) + (matrix1.M43 * matrix2.M33)) + (matrix1.M44 * matrix2.M43);
           	var m44 = (((matrix1.M41 * matrix2.M14) + (matrix1.M42 * matrix2.M24)) + (matrix1.M43 * matrix2.M34)) + (matrix1.M44 * matrix2.M44);
            matrix1.M11 = m11;
			matrix1.M12 = m12;
			matrix1.M13 = m13;
			matrix1.M14 = m14;
			matrix1.M21 = m21;
			matrix1.M22 = m22;
			matrix1.M23 = m23;
			matrix1.M24 = m24;
			matrix1.M31 = m31;
			matrix1.M32 = m32;
			matrix1.M33 = m33;
			matrix1.M34 = m34;
			matrix1.M41 = m41;
			matrix1.M42 = m42;
			matrix1.M43 = m43;
			matrix1.M44 = m44;
			return matrix1;
        }

        /// <summary>
        /// Multiplies the elements of matrix by a scalar.
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/> on the left of the mul sign.</param>
        /// <param name="scaleFactor">Scalar value on the right of the mul sign.</param>
        /// <returns>Result of the matrix multiplication with a scalar.</returns>
        public static Matrix operator *(Matrix matrix, float scaleFactor)
        {
		    matrix.M11 = matrix.M11 * scaleFactor;
		    matrix.M12 = matrix.M12 * scaleFactor;
		    matrix.M13 = matrix.M13 * scaleFactor;
		    matrix.M14 = matrix.M14 * scaleFactor;
		    matrix.M21 = matrix.M21 * scaleFactor;
		    matrix.M22 = matrix.M22 * scaleFactor;
		    matrix.M23 = matrix.M23 * scaleFactor;
		    matrix.M24 = matrix.M24 * scaleFactor;
		    matrix.M31 = matrix.M31 * scaleFactor;
		    matrix.M32 = matrix.M32 * scaleFactor;
		    matrix.M33 = matrix.M33 * scaleFactor;
		    matrix.M34 = matrix.M34 * scaleFactor;
		    matrix.M41 = matrix.M41 * scaleFactor;
		    matrix.M42 = matrix.M42 * scaleFactor;
		    matrix.M43 = matrix.M43 * scaleFactor;
		    matrix.M44 = matrix.M44 * scaleFactor;
		    return matrix;
        }

        /// <summary>
        /// Subtracts the values of one <see cref="Matrix"/> from another <see cref="Matrix"/>.
        /// </summary>
        /// <param name="matrix1">Source <see cref="Matrix"/> on the left of the sub sign.</param>
        /// <param name="matrix2">Source <see cref="Matrix"/> on the right of the sub sign.</param>
        /// <returns>Result of the matrix subtraction.</returns>
        public static Matrix operator -(Matrix matrix1, Matrix matrix2)
        {
		    matrix1.M11 = matrix1.M11 - matrix2.M11;
		    matrix1.M12 = matrix1.M12 - matrix2.M12;
		    matrix1.M13 = matrix1.M13 - matrix2.M13;
		    matrix1.M14 = matrix1.M14 - matrix2.M14;
		    matrix1.M21 = matrix1.M21 - matrix2.M21;
		    matrix1.M22 = matrix1.M22 - matrix2.M22;
		    matrix1.M23 = matrix1.M23 - matrix2.M23;
		    matrix1.M24 = matrix1.M24 - matrix2.M24;
		    matrix1.M31 = matrix1.M31 - matrix2.M31;
		    matrix1.M32 = matrix1.M32 - matrix2.M32;
		    matrix1.M33 = matrix1.M33 - matrix2.M33;
		    matrix1.M34 = matrix1.M34 - matrix2.M34;
		    matrix1.M41 = matrix1.M41 - matrix2.M41;
		    matrix1.M42 = matrix1.M42 - matrix2.M42;
		    matrix1.M43 = matrix1.M43 - matrix2.M43;
		    matrix1.M44 = matrix1.M44 - matrix2.M44;
		    return matrix1;
        }

        /// <summary>
        /// Inverts values in the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="matrix">Source <see cref="Matrix"/> on the right of the sub sign.</param>
        /// <returns>Result of the inversion.</returns>
        public static Matrix operator -(Matrix matrix)
        {
		    matrix.M11 = -matrix.M11;
		    matrix.M12 = -matrix.M12;
		    matrix.M13 = -matrix.M13;
		    matrix.M14 = -matrix.M14;
		    matrix.M21 = -matrix.M21;
		    matrix.M22 = -matrix.M22;
		    matrix.M23 = -matrix.M23;
		    matrix.M24 = -matrix.M24;
		    matrix.M31 = -matrix.M31;
		    matrix.M32 = -matrix.M32;
		    matrix.M33 = -matrix.M33;
		    matrix.M34 = -matrix.M34;
		    matrix.M41 = -matrix.M41;
		    matrix.M42 = -matrix.M42;
		    matrix.M43 = -matrix.M43;
		    matrix.M44 = -matrix.M44;
			return matrix;
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> that contains subtraction of one matrix from another.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix"/>.</param>
        /// <param name="matrix2">The second <see cref="Matrix"/>.</param>
        /// <returns>The result of the matrix subtraction.</returns>
        public static Matrix Subtract(Matrix matrix1, Matrix matrix2)
        {
		    matrix1.M11 = matrix1.M11 - matrix2.M11;
		    matrix1.M12 = matrix1.M12 - matrix2.M12;
		    matrix1.M13 = matrix1.M13 - matrix2.M13;
		    matrix1.M14 = matrix1.M14 - matrix2.M14;
		    matrix1.M21 = matrix1.M21 - matrix2.M21;
		    matrix1.M22 = matrix1.M22 - matrix2.M22;
		    matrix1.M23 = matrix1.M23 - matrix2.M23;
		    matrix1.M24 = matrix1.M24 - matrix2.M24;
		    matrix1.M31 = matrix1.M31 - matrix2.M31;
		    matrix1.M32 = matrix1.M32 - matrix2.M32;
		    matrix1.M33 = matrix1.M33 - matrix2.M33;
		    matrix1.M34 = matrix1.M34 - matrix2.M34;
		    matrix1.M41 = matrix1.M41 - matrix2.M41;
		    matrix1.M42 = matrix1.M42 - matrix2.M42;
		    matrix1.M43 = matrix1.M43 - matrix2.M43;
		    matrix1.M44 = matrix1.M44 - matrix2.M44;
		    return matrix1;
        }

        /// <summary>
        /// Creates a new <see cref="Matrix"/> that contains subtraction of one matrix from another.
        /// </summary>
        /// <param name="matrix1">The first <see cref="Matrix"/>.</param>
        /// <param name="matrix2">The second <see cref="Matrix"/>.</param>
        /// <param name="result">The result of the matrix subtraction as an output parameter.</param>
        public static void Subtract(ref Matrix matrix1, ref Matrix matrix2, out Matrix result)
        {
            result.M11 = matrix1.M11 - matrix2.M11;
		    result.M12 = matrix1.M12 - matrix2.M12;
		    result.M13 = matrix1.M13 - matrix2.M13;
		    result.M14 = matrix1.M14 - matrix2.M14;
		    result.M21 = matrix1.M21 - matrix2.M21;
		    result.M22 = matrix1.M22 - matrix2.M22;
		    result.M23 = matrix1.M23 - matrix2.M23;
		    result.M24 = matrix1.M24 - matrix2.M24;
		    result.M31 = matrix1.M31 - matrix2.M31;
		    result.M32 = matrix1.M32 - matrix2.M32;
		    result.M33 = matrix1.M33 - matrix2.M33;
		    result.M34 = matrix1.M34 - matrix2.M34;
		    result.M41 = matrix1.M41 - matrix2.M41;
		    result.M42 = matrix1.M42 - matrix2.M42;
		    result.M43 = matrix1.M43 - matrix2.M43;
		    result.M44 = matrix1.M44 - matrix2.M44;
        }

        internal string DebugDisplayString
        {
            get
            {
                if (this == Identity)
                {
                    return "Identity";
                }

                return string.Concat(
                     "( ", this.M11.ToString(), "  ", this.M12.ToString(), "  ", this.M13.ToString(), "  ", this.M14.ToString(), " )  \r\n",
                     "( ", this.M21.ToString(), "  ", this.M22.ToString(), "  ", this.M23.ToString(), "  ", this.M24.ToString(), " )  \r\n",
                     "( ", this.M31.ToString(), "  ", this.M32.ToString(), "  ", this.M33.ToString(), "  ", this.M34.ToString(), " )  \r\n",
                     "( ", this.M41.ToString(), "  ", this.M42.ToString(), "  ", this.M43.ToString(), "  ", this.M44.ToString(), " )");
            }
        }

        /// <summary>
        /// Returns a <see cref="String"/> representation of this <see cref="Matrix"/> in the format:
        /// {M11:[<see cref="M11"/>] M12:[<see cref="M12"/>] M13:[<see cref="M13"/>] M14:[<see cref="M14"/>]}
        /// {M21:[<see cref="M21"/>] M12:[<see cref="M22"/>] M13:[<see cref="M23"/>] M14:[<see cref="M24"/>]}
        /// {M31:[<see cref="M31"/>] M32:[<see cref="M32"/>] M33:[<see cref="M33"/>] M34:[<see cref="M34"/>]}
        /// {M41:[<see cref="M41"/>] M42:[<see cref="M42"/>] M43:[<see cref="M43"/>] M44:[<see cref="M44"/>]}
        /// </summary>
        /// <returns>A <see cref="String"/> representation of this <see cref="Matrix"/>.</returns>
        public override string ToString()
        {
            return "{M11:" + M11 + " M12:" + M12 + " M13:" + M13 + " M14:" + M14 + "}"
                + " {M21:" + M21 + " M22:" + M22 + " M23:" + M23 + " M24:" + M24 + "}"
                + " {M31:" + M31 + " M32:" + M32 + " M33:" + M33 + " M34:" + M34 + "}"
                + " {M41:" + M41 + " M42:" + M42 + " M43:" + M43 + " M44:" + M44 + "}";
        }

        /// <summary>
        /// Swap the matrix rows and columns.
        /// </summary>
        /// <param name="matrix">The matrix for transposing operation.</param>
        /// <returns>The new <see cref="Matrix"/> which contains the transposing result.</returns>
        public static Matrix Transpose(Matrix matrix)
        {
            Matrix ret;
            Transpose(ref matrix, out ret);
            return ret;
        }

        /// <summary>
        /// Swap the matrix rows and columns.
        /// </summary>
        /// <param name="matrix">The matrix for transposing operation.</param>
        /// <param name="result">The new <see cref="Matrix"/> which contains the transposing result as an output parameter.</param>
        public static void Transpose(ref Matrix matrix, out Matrix result)
        {
            Matrix ret;
            
            ret.M11 = matrix.M11;
            ret.M12 = matrix.M21;
            ret.M13 = matrix.M31;
            ret.M14 = matrix.M41;

            ret.M21 = matrix.M12;
            ret.M22 = matrix.M22;
            ret.M23 = matrix.M32;
            ret.M24 = matrix.M42;

            ret.M31 = matrix.M13;
            ret.M32 = matrix.M23;
            ret.M33 = matrix.M33;
            ret.M34 = matrix.M43;

            ret.M41 = matrix.M14;
            ret.M42 = matrix.M24;
            ret.M43 = matrix.M34;
            ret.M44 = matrix.M44;
            
            result = ret;
        }

        /// <summary>
        /// Returns a <see cref="System.Numerics.Matrix4x4"/>.
        /// </summary>
        public System.Numerics.Matrix4x4 ToNumerics()
        {
            return Unsafe.As<Matrix, System.Numerics.Matrix4x4>(ref this);
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Helper method for using the Laplace expansion theorem using two rows expansions to calculate major and 
        /// minor determinants of a 4x4 matrix. This method is used for inverting a matrix.
        /// </summary>
        private static void FindDeterminants(ref Matrix matrix, out float major, 
                                             out float minor1, out float minor2, out float minor3, out float minor4, out float minor5, out float minor6,
                                             out float minor7, out float minor8, out float minor9, out float minor10, out float minor11, out float minor12)
        {
                double det1 = (double)matrix.M11 * (double)matrix.M22 - (double)matrix.M12 * (double)matrix.M21;
                double det2 = (double)matrix.M11 * (double)matrix.M23 - (double)matrix.M13 * (double)matrix.M21;
                double det3 = (double)matrix.M11 * (double)matrix.M24 - (double)matrix.M14 * (double)matrix.M21;
                double det4 = (double)matrix.M12 * (double)matrix.M23 - (double)matrix.M13 * (double)matrix.M22;
                double det5 = (double)matrix.M12 * (double)matrix.M24 - (double)matrix.M14 * (double)matrix.M22;
                double det6 = (double)matrix.M13 * (double)matrix.M24 - (double)matrix.M14 * (double)matrix.M23;
                double det7 = (double)matrix.M31 * (double)matrix.M42 - (double)matrix.M32 * (double)matrix.M41;
                double det8 = (double)matrix.M31 * (double)matrix.M43 - (double)matrix.M33 * (double)matrix.M41;
                double det9 = (double)matrix.M31 * (double)matrix.M44 - (double)matrix.M34 * (double)matrix.M41;
                double det10 = (double)matrix.M32 * (double)matrix.M43 - (double)matrix.M33 * (double)matrix.M42;
                double det11 = (double)matrix.M32 * (double)matrix.M44 - (double)matrix.M34 * (double)matrix.M42;
                double det12 = (double)matrix.M33 * (double)matrix.M44 - (double)matrix.M34 * (double)matrix.M43;
                
                major = (float)(det1*det12 - det2*det11 + det3*det10 + det4*det9 - det5*det8 + det6*det7);
                minor1 = (float)det1;
                minor2 = (float)det2;
                minor3 = (float)det3;
                minor4 = (float)det4;
                minor5 = (float)det5;
                minor6 = (float)det6;
                minor7 = (float)det7;
                minor8 = (float)det8;
                minor9 = (float)det9;
                minor10 = (float)det10;
                minor11 = (float)det11;
                minor12 = (float)det12;
        }
		
        #endregion
    }
}
