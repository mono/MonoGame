// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Microsoft.Xna.Framework.Content.Pipeline
{
    /// <summary>
    /// A helper for collecting instances of a particular type
    /// by scanning the types in loaded assemblies.
    /// </summary>
    public class LoadedTypeCollection<T> : IEnumerable<T>
    {
        private static List<T> _all;

        /// <summary>
        /// Creates a new LoadedTypeCollection.
        /// </summary>
        public LoadedTypeCollection()
        {
            // Scan the already loaded assemblies.
            if (_all == null)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var ass in assemblies)
                    ScanAssembly(ass);
            }

            // Hook into assembly loading events to gather any new
            // enumeration types that are found.
            AppDomain.CurrentDomain.AssemblyLoad += (sender, args) => ScanAssembly(args.LoadedAssembly);
        }

        private static void ScanAssembly(Assembly ass)
        {
            // Initialize the list on first use.
            if (_all == null)
                _all = new List<T>(24);

            var thisAss = typeof(T).Assembly;

            // If the assembly doesn't reference our assembly then it
            // cannot contain this type... so skip scanning it.
            var refAss = ass.GetReferencedAssemblies();
            if (thisAss.FullName != ass.FullName && refAss.All(r => r.FullName != thisAss.FullName))
                return;

            var definedTypes = ass.DefinedTypes;

            foreach (var type in definedTypes)
            {
                if (!type.IsSubclassOf(typeof(T)) || type.IsAbstract)
                    continue;

                // Create an instance of the type and add it to our list.
                var ttype = (T)Activator.CreateInstance(type);
                _all.Add(ttype);
            }
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()"/>
        public IEnumerator<T> GetEnumerator()
        {
            return _all.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
