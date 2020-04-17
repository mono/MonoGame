// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Input;

namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// Arguments for the <see cref="IImeService.TextComposition" /> event.
    /// </summary>
    public struct TextCompositionEventArgs
    {
        public TextCompositionEventArgs(string compositionString, int cursorPosition,
            string[] candidateList = null, uint candidatePageStart = 0, uint candidatePageSize = 0, uint candidateSelection = 0)
        {
            CompositionString = compositionString;
            CursorPosition = cursorPosition;

            CandidateList = candidateList;
            CandidatePageStart = candidatePageStart;
            CandidatePageSize = candidatePageSize;
            CandidateSelection = candidateSelection;
        }

        /// <summary>
        /// The full string as it's composited by the IMM.
        /// </summary>    
        public readonly string CompositionString;

        /// <summary>
        /// The position of the cursor inside the composited string.
        /// </summary>    
        public readonly int CursorPosition;

        /// <summary>
        /// The candidate text list for the composition.
        /// Note candidate properties are only valid in  WindowsDX(or UAP) platform.
        /// It could be empty if compsition string does not generates candidates.
        /// </summary>    
        public readonly string[] CandidateList;

        /// <summary>
        /// First candidate index of current page.
        /// </summary>
        public readonly uint CandidatePageStart;

        /// <summary>
        /// How many candidates should display per page.
        /// </summary>
        public readonly uint CandidatePageSize;

        /// <summary>
        /// The selected candidate index.
        /// </summary>
        public readonly uint CandidateSelection;
    }
}