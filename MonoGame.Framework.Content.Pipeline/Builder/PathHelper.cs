// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Framework.Content.Pipeline.Builder
{
    /// <summary>
    /// Methods provided to normalize and manipulate paths.
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// The/universal/standard/directory/seperator.
        /// </summary>
        public const char DirectorySeparator = '/';

        /// <summary>
        /// Returns a path string normalized to the/universal/standard.
        /// </summary>
        public static string Normalize(string path)
        {
            return path.Replace('\\', '/');
        }

        /// <summary>
        /// Returns a directory path string normalized to the/universal/standard
        /// with a trailing seperator.
        /// </summary>
        public static string NormalizeDirectory(string path)
        {
            return path.Replace('\\', '/').TrimEnd('/') + '/';
        }

        /// <summary>
        /// Returns a path string normalized to the\Windows\standard.
        /// </summary>
        public static string NormalizeWindows(string path)
        {
            return path.Replace('/', '\\');
        }

        /// <summary>
        /// Returns a path relative to the base path.
        /// </summary>
        /// <param name="basePath">The path to make relative to.  Must end with directory seperator.</param>
        /// <param name="path">The path to be made relative to the basePath.</param>
        /// <returns>The relative path or the original string if it is not absolute or cannot be made relative.</returns>
        public static string GetRelativePath(string basePath, string path)
        {
            Uri uri;
            if (!Uri.TryCreate(path, UriKind.Absolute, out uri))
                return path;

            uri = new Uri(basePath).MakeRelativeUri(uri);
            var str = Uri.UnescapeDataString(uri.ToString());

            return Normalize(str);
        }
    }
}
