#region License
/* FNA - XNA4 Reimplementation for Desktop Platforms
 * Copyright 2009-2019 Ethan Lee and the MonoGame Team
 *
 * Released under the Microsoft Public License.
 * See LICENSE for details.
 */
#endregion

#region Using Statements
using System;

using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Microsoft.Xna.Framework.Media
{
	public sealed partial class Video
	{
		#region Internal Variables: Theorafile

		internal IntPtr theora;

        /// Gets the width of this video, in pixels.
        /// </summary>
        public int UvWidth { get; set; }

        /// <summary>
        /// Gets the height of this video, in pixels.
        /// </summary>
        public int UvHeight { get; set; }

        //hack as my test videos (compiled with VLC) with sound always return Theorafile.EndOfStream(Video.theora) == 1,
        //and the audiostream is left with a PendingBufferCount of 1 for up to ~20 seconds after the actual sound has stopped
        public bool UseElapsedTimeForStop = true;

        #endregion

        #region Internal Constructors

        private void PlatformInitialize()
		{
			// MG Note: we double check that the metadata in the XNB matches
			//          what Theorafile reads from the actual video file

			Theorafile.OpenFile(FileName, out theora);
			int width, height;
			double fps;
            Theorafile.th_pixel_fmt th_Pixel_Fmt;
			Theorafile.VideoInfo(theora, out width, out height, out fps, out th_Pixel_Fmt);

            /* If you got here, you've still got the XNB file! Well done!
			 * Except if you're running FNA, you're not using the WMV anymore.
			 * But surely it's the same video, right...?
			 * Well, consider this a check more than anything. If this bothers
			 * you, just remove the XNB file and we'll read the OGV straight up.
			 * -flibit
			 */

            switch (th_Pixel_Fmt)
            {
                case Theorafile.th_pixel_fmt.TH_PF_420:
                    UvWidth = Width / 2;
                    UvHeight = Height / 2;
                    break;
                case Theorafile.th_pixel_fmt.TH_PF_422:
                    UvWidth = Width / 2;
                    UvHeight = Height;
                    break;
                case Theorafile.th_pixel_fmt.TH_PF_444:
                    UvWidth = Width;
                    UvHeight = Height;
                    break;
            }

            if (width != Width || height != Height)
			{
				throw new InvalidOperationException(
					"XNB/OGV width/height mismatch!" +
					" Width: " + Width.ToString() +
					" Height: " + Height.ToString()
				);
			}
			if (Math.Abs(fps - FramesPerSecond) >= 1.0f)
			{
				throw new InvalidOperationException(
					"XNB/OGV framesPerSecond mismatch!" +
					" FPS: " + FramesPerSecond.ToString()
				);
			}
		}

        #endregion

        #region Destructor

        private void PlatformDispose(bool disposing)
		{
			if (theora != IntPtr.Zero)
			{
				Theorafile.CloseFile(ref theora);
			}
		}

		#endregion
	}
}
