// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MonoGame.Framework.Utilities
{
    internal static partial class ReflectionHelpers
    {
        public static bool IsValueType(Type targetType)
        {
            if (targetType == null)
            {
                throw new NullReferenceException("Must supply the targetType parameter");
            }
#if NET45            
            return targetType.GetTypeInfo().IsValueType;
#else
            return targetType.IsValueType;
#endif
        }

        public static Type GetBaseType(Type targetType)
        {
            if (targetType == null)
            {
                throw new NullReferenceException("Must supply the targetType parameter");
            }
#if NET45            
            return targetType.GetTypeInfo().BaseType;
#else
            return targetType.BaseType;
#endif
        }

        /// <summary>
        /// Returns the Assembly of a Type
        /// </summary>
        public static Assembly GetAssembly(Type targetType)
        {
            if (targetType == null)
            {
                throw new NullReferenceException("Must supply the targetType parameter");
            }
#if NET45            
            return targetType.GetTypeInfo().Assembly;
#else
            return targetType.Assembly;
#endif
        }

        /// <summary>
        /// Returns true if the given type represents a non-object type that is not abstract.
        /// </summary>
        public static bool IsConcreteClass(Type t)
        {
            if (t == null)
            {
                throw new NullReferenceException("Must supply the t (type) parameter");
            }

            if (t == typeof(object))
                return false;
#if NET45            
            var ti = t.GetTypeInfo();
            if (ti.IsClass && !ti.IsAbstract)
                return true;
#else            
            if (t.IsClass && !t.IsAbstract)
                return true;
#endif
            return false;
        }

        public static MethodInfo GetMethodInfo(Type type, string methodName)
        {
#if NET45            
            return type.GetTypeInfo().GetDeclaredMethod(methodName);
#else
            return type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
#endif
        }

        public static MethodInfo GetPropertyGetMethod(PropertyInfo property)
        {
            if (property == null)
            {
                throw new NullReferenceException("Must supply the property parameter");
            }

#if NET45            
            return property.GetMethod;
#else
            return property.GetGetMethod();
#endif
        }

        public static MethodInfo GetPropertySetMethod(PropertyInfo property)
        {
            if (property == null)
            {
                throw new NullReferenceException("Must supply the property parameter");
            }

#if NET45            
            return property.SetMethod;
#else
            return property.GetSetMethod();
#endif
        }

        public static T GetCustomAttribute<T>(MemberInfo member) where T : Attribute
        {
            if (member == null)
                throw new NullReferenceException("Must supply the member parameter");

#if NET45            
            return member.GetCustomAttribute(typeof(T)) as T;
#else
            return Attribute.GetCustomAttribute(member, typeof(T)) as T;
#endif
        }

        /// <summary>
        /// Returns true if the get method of the given property exist and are public.
        /// Note that we allow a getter-only property to be serialized (and deserialized),
        /// *if* CanDeserializeIntoExistingObject is true for the property type.
        /// </summary>
        public static bool PropertyIsPublic(PropertyInfo property)
        {
            if (property == null)
            {
                throw new NullReferenceException("Must supply the property parameter");
            }

            var getMethod = GetPropertyGetMethod(property);

            if (getMethod == null || !getMethod.IsPublic)
                return false;

            return true;
        }

        /// <summary>
        /// Returns true if the given type can be assigned the given value
        /// </summary>
        public static bool IsAssignableFrom(Type type, object value)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (value == null)
                throw new ArgumentNullException("value");

            return IsAssignableFromType(type, value.GetType());
        }

        /// <summary>
        /// Returns true if the given type can be assigned a value with the given object type
        /// </summary>
        public static bool IsAssignableFromType(Type type, Type objectType)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (objectType == null)
                throw new ArgumentNullException("objectType");
#if NET45
            if (type.GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo()))
                return true;
#else
            if (type.IsAssignableFrom(objectType))
                return true;     
#endif
            return false;
        }

        // Returns the cached size of the unmanaged type in bytes.
        public static int FastSizeOf<T>()
        {
            return SizeOf<T>.Get();
        }

        /// <summary>
        /// Fallback handler for Marshal.SizeOf(type)
        /// </summary>
        internal static int ManagedSizeOf(Type type)
        {
            return Marshal.SizeOf<Type>(type);
        }

    }
}
