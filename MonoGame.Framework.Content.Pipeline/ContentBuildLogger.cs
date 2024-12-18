// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Xna.Framework.Content.Pipeline
{
    /// <summary>
    /// Provides methods for reporting informational messages or warnings from content importers and processors.
    /// Do not use this class to report errors. Instead, report errors by throwing a PipelineException or InvalidContentException.
    /// </summary>
    public abstract class ContentBuildLogger
    {
        Stack<string> filenames = new Stack<string>();
        private int indentCount = 0;

        /// <summary/>
        protected string IndentString { get { return String.Empty.PadLeft(Math.Max(0, indentCount), '\t'); } }

        /// <summary>
        /// Gets or sets the base reference path used when reporting errors during the content build process.
        /// </summary>
        public string LoggerRootDirectory { get; set; }

        /// <summary>
        /// Initializes a new instance of ContentBuildLogger.
        /// </summary>
        protected ContentBuildLogger ()
        {
        }

        /// <summary>
        /// Returns the relative path to the filename from the root directory.
        /// </summary>
        /// <param name="filename">The target filename.</param>
        /// <param name="rootDirectory">The root directory. If not specified, the current directory is used.</param>
        /// <returns>The relative path.</returns>
        string GetRelativePath(string filename, string rootDirectory)
        {
            rootDirectory = Path.GetFullPath(string.IsNullOrEmpty(rootDirectory) ? "." : rootDirectory);
            filename = Path.GetFullPath(filename);
            if (filename.StartsWith(rootDirectory))
                return filename.Substring(rootDirectory.Length);
            return filename;
        }

        /// <summary>
        /// Gets the filename currently being processed, for use in warning and error messages.
        /// </summary>
        /// <param name="contentIdentity">Identity of a content item. If specified, GetCurrentFilename uses this value to refine the search. If no value is specified, the current PushFile state is used.</param>
        /// <returns>Name of the file being processed.</returns>
        protected string GetCurrentFilename(
            ContentIdentity contentIdentity
            )
        {
            if ((contentIdentity != null) && !string.IsNullOrEmpty(contentIdentity.SourceFilename))
                return GetRelativePath(contentIdentity.SourceFilename, LoggerRootDirectory);
            if (filenames.Count > 0)
                return GetRelativePath(filenames.Peek(), LoggerRootDirectory);
            return null;
        }

        /// <summary>
        /// Outputs a high-priority status message from a content importer or processor.
        /// </summary>
        /// <param name="message">Message being reported.</param>
        /// <param name="messageArgs">Arguments for the reported message.</param>
        public abstract void LogImportantMessage(
            string message,
            params Object[] messageArgs
            );

        /// <summary>
        /// Outputs a low priority status message from a content importer or processor.
        /// </summary>
        /// <param name="message">Message being reported.</param>
        /// <param name="messageArgs">Arguments for the reported message.</param>
        public abstract void LogMessage(
            string message,
            params Object[] messageArgs
            );

        /// <summary>
        /// Outputs a warning message from a content importer or processor.
        /// </summary>
        /// <param name="helpLink">Link to an existing online help topic containing related information.</param>
        /// <param name="contentIdentity">Identity of the content item that generated the message.</param>
        /// <param name="message">Message being reported.</param>
        /// <param name="messageArgs">Arguments for the reported message.</param>
        public abstract void LogWarning(
            string helpLink,
            ContentIdentity contentIdentity,
            string message,
            params Object[] messageArgs
            );

        /// <summary>
        /// Outputs a message indicating that a content asset has completed processing.
        /// </summary>
        public void PopFile()
        {
            filenames.Pop();
        }

        /// <summary>
        /// Outputs a message indicating that a content asset has begun processing.
        /// All logger warnings or error exceptions from this time forward to the next PopFile call refer to this file.
        /// </summary>
        /// <param name="filename">Name of the file containing future messages.</param>
        public void PushFile(string filename)
        {
            filenames.Push(filename);
        }

        /// <summary>
        /// Increments the indentation level.
        /// </summary>
        public void Indent()
        {
            indentCount++;
        }

        /// <summary>
        /// Decrements the indentation level.
        /// </summary>
        public void Unindent()
        {
            indentCount--;
        }
    }
}
