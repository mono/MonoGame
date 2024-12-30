using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Tests.Framework
{

// TODO: Mac implements its own GameWindow class that cannot 
// be overloaded...  if you hate this hack, go fix it.
#if !MONOMAC

    internal class MockWindow : GameWindow
    {
        public override bool AllowUserResizing { get; set; }

        public override Rectangle ClientBounds
        {
            get { throw new NotImplementedException(); }
        }

        public override Point Position { get; set; }

        public override DisplayOrientation CurrentOrientation
        {
            get { throw new NotImplementedException(); }
        }

        public override IntPtr Handle
        {
            get { throw new NotImplementedException(); }
        }

        public override string ScreenDeviceName
        {
            get { throw new NotImplementedException(); }
        }

        public override void BeginScreenDeviceChange(bool willBeFullScreen)
        {
            throw new NotImplementedException();
        }

        public override void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight)
        {
            throw new NotImplementedException();
        }

        protected internal override void SetSupportedOrientations(DisplayOrientation orientations)
        {
            throw new NotImplementedException();
        }

        protected override void SetTitle(string title)
        {
            throw new NotImplementedException();
        }

        public override void MaximizeWindow()
        {
            throw new NotImplementedException();
        }
    }

#endif // !MONOMAC

}
