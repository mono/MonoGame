﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

#if !NO_CONTENTPIPELINE

using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Tests.ContentPipeline
{
    class TestContentBuildLogger : ContentBuildLogger
    {
        public override void LogImportantMessage(string message, params object[] messageArgs)
        {
        }

        public override void LogMessage(string message, params object[] messageArgs)
        {
        }

        public override void LogWarning(string helpLink, ContentIdentity contentIdentity, string message, params object[] messageArgs)
        {
        }
    }
}

#endif
