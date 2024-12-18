// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Framework.Content.Pipeline.Builder
{
    /// <inheritdoc cref="ContentBuildLogger">ContentBuildLogger</inheritdoc>
    public class PipelineBuildLogger : ContentBuildLogger
    {
        /// <inheritdoc/>
        public override void LogMessage(string message, params object[] messageArgs)
        {
			System.Diagnostics.Trace.WriteLine(string.Format(message, messageArgs));
        }

        /// <inheritdoc/>
        public override void LogImportantMessage(string message, params object[] messageArgs)
        {
            // TODO: How do i make it high importance?
			System.Diagnostics.Trace.WriteLine(string.Format(message, messageArgs));
        }

        /// <inheritdoc/>
        public override void LogWarning(string helpLink, ContentIdentity contentIdentity, string message, params object[] messageArgs)
        {
            var msg = string.Format(message, messageArgs);
            var fileName = GetCurrentFilename(contentIdentity);
			System.Diagnostics.Trace.WriteLine(string.Format("{0}: {1}", fileName, msg));
        }

    }

}
