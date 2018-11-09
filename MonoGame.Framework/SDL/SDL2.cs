// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using MonoGame.Utilities;

internal static class Sdl
{
    public static IntPtr NativeLibrary = GetNativeLibrary();

    private static IntPtr GetNativeLibrary()
    {
        var ret = IntPtr.Zero;

        // Load bundled library
        var assemblyLocation = Path.GetDirectoryName((new Uri(typeof(Sdl).Assembly.CodeBase)).LocalPath);
        if (CurrentPlatform.OS == OS.Windows && Environment.Is64BitProcess)
            ret = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "x64/SDL2.dll"));
        else if (CurrentPlatform.OS == OS.Windows && !Environment.Is64BitProcess)
            ret = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "x86/SDL2.dll"));
        else if (CurrentPlatform.OS == OS.Linux && Environment.Is64BitProcess)
            ret = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "x64/libSDL2-2.0.so.0"));
        else if (CurrentPlatform.OS == OS.Linux && !Environment.Is64BitProcess)
            ret = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "x86/libSDL2-2.0.so.0"));
        else if (CurrentPlatform.OS == OS.MacOSX)
            ret = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "libSDL2-2.0.0.dylib"));

        // Load system library
        if (ret == IntPtr.Zero)
        {
            if (CurrentPlatform.OS == OS.Windows)
                ret = FuncLoader.LoadLibrary("SDL2.dll");
            else if (CurrentPlatform.OS == OS.Linux)
                ret = FuncLoader.LoadLibrary("libSDL2-2.0.so.0");
            else
                ret = FuncLoader.LoadLibrary("libSDL2-2.0.0.dylib");
        }

        // Welp, all failed, PANIC!!!
        if (ret == IntPtr.Zero)
            throw new Exception("Failed to load SDL library.");

        return ret;
    }

    public static int Major;
    public static int Minor;
    public static int Patch;

    public static unsafe string GetString(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
            return "";

        var ptr = (byte*)handle;
        while (*ptr != 0)
            ptr++;

        var bytes = new byte[ptr - (byte*)handle];
        Marshal.Copy(handle, bytes, 0, bytes.Length);

        return Encoding.UTF8.GetString(bytes);
    }

    [Flags]
    public enum InitFlags
    {
        Video = 0x00000020,
        Joystick = 0x00000200,
        Haptic = 0x00001000,
        GameController = 0x00002000,
    }

    public enum EventType : uint
    {
        First = 0,

        Quit = 0x100,

        WindowEvent = 0x200,
        SysWM = 0x201,

        KeyDown = 0x300,
        KeyUp = 0x301,
        TextEditing = 0x302,
        TextInput = 0x303,

        MouseMotion = 0x400,
        MouseButtonDown = 0x401,
        MouseButtonup = 0x402,
        MouseWheel = 0x403,

        JoyAxisMotion = 0x600,
        JoyBallMotion = 0x601,
        JoyHatMotion = 0x602,
        JoyButtonDown = 0x603,
        JoyButtonUp = 0x604,
        JoyDeviceAdded = 0x605,
        JoyDeviceRemoved = 0x606,

        ControllerAxisMotion = 0x650,
        ControllerButtonDown = 0x651,
        ControllerButtonUp = 0x652,
        ControllerDeviceAdded = 0x653,
        ControllerDeviceRemoved = 0x654,
        ControllerDeviceRemapped = 0x654,

        FingerDown = 0x700,
        FingerUp = 0x701,
        FingerMotion = 0x702,

        DollarGesture = 0x800,
        DollarRecord = 0x801,
        MultiGesture = 0x802,

        ClipboardUpdate = 0x900,

        DropFile = 0x1000,

        AudioDeviceAdded = 0x1100,
        AudioDeviceRemoved = 0x1101,

        RenderTargetsReset = 0x2000,
        RenderDeviceReset = 0x2001,

        UserEvent = 0x8000,

        Last = 0xFFFF
    }

    public enum EventAction
    {
        AddEvent = 0x0,
        PeekEvent = 0x1,
        GetEvent = 0x2,
    }

    [StructLayout(LayoutKind.Explicit, Size = 56)]
    public struct Event
    {
        [FieldOffset(0)]
        public EventType Type;
        [FieldOffset(0)]
        public Window.Event Window;
        [FieldOffset(0)]
        public Keyboard.Event Key;
        [FieldOffset(0)]
        public Mouse.MotionEvent Motion;
        [FieldOffset(0)]
        public Keyboard.TextEditingEvent Edit;
        [FieldOffset(0)]
        public Keyboard.TextInputEvent Text;
        [FieldOffset(0)]
        public Mouse.WheelEvent Wheel;
        [FieldOffset(0)]
        public Joystick.DeviceEvent JoystickDevice;
        [FieldOffset(0)]
        public GameController.DeviceEvent ControllerDevice;
        [FieldOffset(0)]
        public Drop.DropEvent Drop;
    }

    public struct Rectangle
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }

    public struct Version
    {
        public byte Major;
        public byte Minor;
        public byte Patch;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int d_sdl_init(int flags);
    public static d_sdl_init SDL_Init = FuncLoader.LoadFunction<d_sdl_init>(NativeLibrary, "SDL_Init");

    public static void Init(int flags)
    {
        GetError(SDL_Init(flags));
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void d_sdl_disablescreensaver();
    public static d_sdl_disablescreensaver DisableScreenSaver = FuncLoader.LoadFunction<d_sdl_disablescreensaver>(NativeLibrary, "SDL_DisableScreenSaver");

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void d_sdl_getversion(out Version version);
    public static d_sdl_getversion GetVersion = FuncLoader.LoadFunction<d_sdl_getversion>(NativeLibrary, "SDL_GetVersion");

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int d_sdl_pollevent([Out] out Event _event);
    public static d_sdl_pollevent PollEvent = FuncLoader.LoadFunction<d_sdl_pollevent>(NativeLibrary, "SDL_PollEvent");

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int d_sdl_pumpevents();
    public static d_sdl_pumpevents PumpEvents = FuncLoader.LoadFunction<d_sdl_pumpevents>(NativeLibrary, "SDL_PumpEvents");

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr d_sdl_creatergbsurfacefrom(IntPtr pixels, int width, int height, int depth, int pitch, uint rMask, uint gMask, uint bMask, uint aMask);
    private static d_sdl_creatergbsurfacefrom SDL_CreateRGBSurfaceFrom = FuncLoader.LoadFunction<d_sdl_creatergbsurfacefrom>(NativeLibrary, "SDL_CreateRGBSurfaceFrom");

    public static IntPtr CreateRGBSurfaceFrom(byte[] pixels, int width, int height, int depth, int pitch, uint rMask, uint gMask, uint bMask, uint aMask)
    {
        var handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
        try
        {
            return SDL_CreateRGBSurfaceFrom(handle.AddrOfPinnedObject(), width, height, depth, pitch, rMask, gMask, bMask, aMask);
        }
        finally
        {
            handle.Free();
        }
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void d_sdl_freesurface(IntPtr surface);
    public static d_sdl_freesurface FreeSurface = FuncLoader.LoadFunction<d_sdl_freesurface>(NativeLibrary, "SDL_FreeSurface");

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr d_sdl_geterror();
    private static d_sdl_geterror SDL_GetError = FuncLoader.LoadFunction<d_sdl_geterror>(NativeLibrary, "SDL_GetError");

    public static string GetError()
    {
        return GetString(SDL_GetError());
    }

    public static int GetError(int value)
    {
        if (value < 0)
            Debug.WriteLine(GetError());

        return value;
    }

    public static IntPtr GetError(IntPtr pointer)
    {
        if (pointer == IntPtr.Zero)
            Debug.WriteLine(GetError());

        return pointer;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void d_sdl_clearerror();
    public static d_sdl_clearerror ClearError = FuncLoader.LoadFunction<d_sdl_clearerror>(NativeLibrary, "SDL_ClearError");

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr d_sdl_gethint(string name);
    public static d_sdl_gethint SDL_GetHint = FuncLoader.LoadFunction<d_sdl_gethint>(NativeLibrary, "SDL_GetHint");

    public static string GetHint(string name)
    {
        return GetString(SDL_GetHint(name));
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr d_sdl_loadbmp_rw(IntPtr src, int freesrc);
    private static d_sdl_loadbmp_rw SDL_LoadBMP_RW = FuncLoader.LoadFunction<d_sdl_loadbmp_rw>(NativeLibrary, "SDL_LoadBMP_RW");

    public static IntPtr LoadBMP_RW(IntPtr src, int freesrc)
    {
        return GetError(SDL_LoadBMP_RW(src, freesrc));
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void d_sdl_quit();
    public static d_sdl_quit Quit = FuncLoader.LoadFunction<d_sdl_quit>(NativeLibrary, "SDL_Quit");

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr d_sdl_rwfrommem(byte[] mem, int size);
    private static d_sdl_rwfrommem SDL_RWFromMem = FuncLoader.LoadFunction<d_sdl_rwfrommem>(NativeLibrary, "SDL_RWFromMem");

    public static IntPtr RwFromMem(byte[] mem, int size)
    {
        return GetError(SDL_RWFromMem(mem, size));
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int d_sdl_sethint(string name, string value);
    public static d_sdl_sethint SetHint = FuncLoader.LoadFunction<d_sdl_sethint>(NativeLibrary, "SDL_SetHint");

    public static class Window
    {
        public const int PosUndefined = 0x1FFF0000;
        public const int PosCentered = 0x2FFF0000;

        public enum EventId : byte
        {
            None,
            Shown,
            Hidden,
            Exposed,
            Moved,
            Resized,
            SizeChanged,
            Minimized,
            Maximized,
            Restored,
            Enter,
            Leave,
            FocusGained,
            FocusLost,
            Close,
        }

        public static class State
        {
            public const int Fullscreen = 0x00000001;
            public const int OpenGL = 0x00000002;
            public const int Shown = 0x00000004;
            public const int Hidden = 0x00000008;
            public const int Borderless = 0x00000010;
            public const int Resizable = 0x00000020;
            public const int Minimized = 0x00000040;
            public const int Maximized = 0x00000080;
            public const int Grabbed = 0x00000100;
            public const int InputFocus = 0x00000200;
            public const int MouseFocus = 0x00000400;
            public const int FullscreenDesktop = 0x00001001;
            public const int Foreign = 0x00000800;
            public const int AllowHighDPI = 0x00002000;
            public const int MouseCapture = 0x00004000;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Event
        {
            public EventType Type;
            public uint TimeStamp;
            public uint WindowID;
            public EventId EventID;
            private byte padding1;
            private byte padding2;
            private byte padding3;
            public int Data1;
            public int Data2;
        }

        public enum SysWMType
        {
            Unknow,
            Windows,
            X11,
            Directfb,
            Cocoa,
            UiKit,
            Wayland,
            Mir,
            WinRt,
            Android
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SDL_SysWMinfo
        {
            public Version version;
            public SysWMType subsystem;
            public IntPtr window;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_sdl_createwindow(string title, int x, int y, int w, int h, int flags);
        private static d_sdl_createwindow SDL_CreateWindow = FuncLoader.LoadFunction<d_sdl_createwindow>(NativeLibrary, "SDL_CreateWindow");

        public static IntPtr Create(string title, int x, int y, int w, int h, int flags)
        {
            return GetError(SDL_CreateWindow(title, x, y, w, h, flags));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_destroywindow(IntPtr window);
        public static d_sdl_destroywindow Destroy = FuncLoader.LoadFunction<d_sdl_destroywindow>(NativeLibrary, "SDL_DestroyWindow");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_getwindowdisplayindex(IntPtr window);
        private static d_sdl_getwindowdisplayindex SDL_GetWindowDisplayIndex = FuncLoader.LoadFunction<d_sdl_getwindowdisplayindex>(NativeLibrary, "SDL_GetWindowDisplayIndex");

        public static int GetDisplayIndex(IntPtr window)
        {
            return GetError(SDL_GetWindowDisplayIndex(window));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int d_sdl_getwindowflags(IntPtr window);
        public static d_sdl_getwindowflags GetWindowFlags = FuncLoader.LoadFunction<d_sdl_getwindowflags>(NativeLibrary, "SDL_GetWindowFlags");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_setwindowicon(IntPtr window, IntPtr icon);
        public static d_sdl_setwindowicon SetIcon = FuncLoader.LoadFunction<d_sdl_setwindowicon>(NativeLibrary, "SDL_SetWindowIcon");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_getwindowposition(IntPtr window, out int x, out int y);
        public static d_sdl_getwindowposition GetPosition = FuncLoader.LoadFunction<d_sdl_getwindowposition>(NativeLibrary, "SDL_GetWindowPosition");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_getwindowsize(IntPtr window, out int w, out int h);
        public static d_sdl_getwindowsize GetSize = FuncLoader.LoadFunction<d_sdl_getwindowsize>(NativeLibrary, "SDL_GetWindowSize");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_setwindowbordered(IntPtr window, int bordered);
        public static d_sdl_setwindowbordered SetBordered = FuncLoader.LoadFunction<d_sdl_setwindowbordered>(NativeLibrary, "SDL_SetWindowBordered");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_setwindowfullscreen(IntPtr window, int flags);
        private static d_sdl_setwindowfullscreen SDL_SetWindowFullscreen = FuncLoader.LoadFunction<d_sdl_setwindowfullscreen>(NativeLibrary, "SDL_SetWindowFullscreen");

        public static void SetFullscreen(IntPtr window, int flags)
        {
            GetError(SDL_SetWindowFullscreen(window, flags));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_setwindowposition(IntPtr window, int x, int y);
        public static d_sdl_setwindowposition SetPosition = FuncLoader.LoadFunction<d_sdl_setwindowposition>(NativeLibrary, "SDL_SetWindowPosition");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_setwindowresizable(IntPtr window, bool resizable);
        public static d_sdl_setwindowresizable SetResizable = FuncLoader.LoadFunction<d_sdl_setwindowresizable>(NativeLibrary, "SDL_SetWindowResizable");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_setwindowsize(IntPtr window, int w, int h);
        public static d_sdl_setwindowsize SetSize = FuncLoader.LoadFunction<d_sdl_setwindowsize>(NativeLibrary, "SDL_SetWindowSize");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void d_sdl_setwindowtitle(IntPtr window, ref byte value);
        private static d_sdl_setwindowtitle SDL_SetWindowTitle = FuncLoader.LoadFunction<d_sdl_setwindowtitle>(NativeLibrary, "SDL_SetWindowTitle");

        public static void SetTitle(IntPtr handle, string title)
        {
            var bytes = Encoding.UTF8.GetBytes(title);
            SDL_SetWindowTitle(handle, ref bytes[0]);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_showwindow(IntPtr window);
        public static d_sdl_showwindow Show = FuncLoader.LoadFunction<d_sdl_showwindow>(NativeLibrary, "SDL_ShowWindow");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool d_sdl_getwindowwminfo(IntPtr window, ref SDL_SysWMinfo sysWMinfo);
        public static d_sdl_getwindowwminfo GetWindowWMInfo = FuncLoader.LoadFunction<d_sdl_getwindowwminfo>(NativeLibrary, "SDL_GetWindowWMInfo");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int d_sdl_getwindowborderssize(IntPtr window, out int top, out int left, out int right, out int bottom);
        public static d_sdl_getwindowborderssize GetBorderSize = FuncLoader.LoadFunction<d_sdl_getwindowborderssize>(NativeLibrary, "SDL_GetWindowBordersSize");
    }

    public static class Display
    {
        public struct Mode
        {
            public uint Format;
            public int Width;
            public int Height;
            public int RefreshRate;
            public IntPtr DriverData;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_getdisplaybounds(int displayIndex, out Rectangle rect);
        private static d_sdl_getdisplaybounds SDL_GetDisplayBounds = FuncLoader.LoadFunction<d_sdl_getdisplaybounds>(NativeLibrary, "SDL_GetDisplayBounds");

        public static void GetBounds(int displayIndex, out Rectangle rect)
        {
            GetError(SDL_GetDisplayBounds(displayIndex, out rect));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_getcurrentdisplaymode(int displayIndex, out Mode mode);
        private static d_sdl_getcurrentdisplaymode SDL_GetCurrentDisplayMode = FuncLoader.LoadFunction<d_sdl_getcurrentdisplaymode>(NativeLibrary, "SDL_GetCurrentDisplayMode");

        public static void GetCurrentDisplayMode(int displayIndex, out Mode mode)
        {
            GetError(SDL_GetCurrentDisplayMode(displayIndex, out mode));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_getdisplaymode(int displayIndex, int modeIndex, out Mode mode);
        private static d_sdl_getdisplaymode SDL_GetDisplayMode = FuncLoader.LoadFunction<d_sdl_getdisplaymode>(NativeLibrary, "SDL_GetDisplayMode");

        public static void GetDisplayMode(int displayIndex, int modeIndex, out Mode mode)
        {
            GetError(SDL_GetDisplayMode(displayIndex, modeIndex, out mode));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_getclosestdisplaymode(int displayIndex, Mode mode, out Mode closest);
        private static d_sdl_getclosestdisplaymode SDL_GetClosestDisplayMode = FuncLoader.LoadFunction<d_sdl_getclosestdisplaymode>(NativeLibrary, "SDL_GetClosestDisplayMode");

        public static void GetClosestDisplayMode(int displayIndex, Mode mode, out Mode closest)
        {
            GetError(SDL_GetClosestDisplayMode(displayIndex, mode, out closest));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_sdl_getdisplayname(int index);
        private static d_sdl_getdisplayname SDL_GetDisplayName = FuncLoader.LoadFunction<d_sdl_getdisplayname>(NativeLibrary, "SDL_GetDisplayName");

        public static string GetDisplayName(int index)
        {
            return GetString(GetError(SDL_GetDisplayName(index)));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_getnumdisplaymodes(int displayIndex);
        private static d_sdl_getnumdisplaymodes SDL_GetNumDisplayModes = FuncLoader.LoadFunction<d_sdl_getnumdisplaymodes>(NativeLibrary, "SDL_GetNumDisplayModes");

        public static int GetNumDisplayModes(int displayIndex)
        {
            return GetError(SDL_GetNumDisplayModes(displayIndex));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_getnumvideodisplays();
        private static d_sdl_getnumvideodisplays SDL_GetNumVideoDisplays = FuncLoader.LoadFunction<d_sdl_getnumvideodisplays>(NativeLibrary, "SDL_GetNumVideoDisplays");

        public static int GetNumVideoDisplays()
        {
            return GetError(SDL_GetNumVideoDisplays());
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_getwindowdisplayindex(IntPtr window);
        private static d_sdl_getwindowdisplayindex SDL_GetWindowDisplayIndex = FuncLoader.LoadFunction<d_sdl_getwindowdisplayindex>(NativeLibrary, "SDL_GetWindowDisplayIndex");

        public static int GetWindowDisplayIndex(IntPtr window)
        {
            return GetError(SDL_GetWindowDisplayIndex(window));
        }
    }

    public static class GL
    {
        public enum Attribute
        {
            RedSize,
            GreenSize,
            BlueSize,
            AlphaSize,
            BufferSize,
            DoubleBuffer,
            DepthSize,
            StencilSize,
            AccumRedSize,
            AccumGreenSize,
            AccumBlueSize,
            AccumAlphaSize,
            Stereo,
            MultiSampleBuffers,
            MultiSampleSamples,
            AcceleratedVisual,
            RetainedBacking,
            ContextMajorVersion,
            ContextMinorVersion,
            ContextEgl,
            ContextFlags,
            ContextProfileMAsl,
            ShareWithCurrentContext,
            FramebufferSRGBCapable,
            ContextReleaseBehaviour,
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_sdl_gl_createcontext(IntPtr window);
        private static d_sdl_gl_createcontext SDL_GL_CreateContext = FuncLoader.LoadFunction<d_sdl_gl_createcontext>(NativeLibrary, "SDL_GL_CreateContext");

        public static IntPtr CreateContext(IntPtr window)
        {
            return GetError(SDL_GL_CreateContext(window));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_gl_deletecontext(IntPtr context);
        public static d_sdl_gl_deletecontext DeleteContext = FuncLoader.LoadFunction<d_sdl_gl_deletecontext>(NativeLibrary, "SDL_GL_DeleteContext");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_sdl_gl_getcurrentcontext();
        private static d_sdl_gl_getcurrentcontext SDL_GL_GetCurrentContext = FuncLoader.LoadFunction<d_sdl_gl_getcurrentcontext>(NativeLibrary, "SDL_GL_GetCurrentContext");

        public static IntPtr GetCurrentContext()
        {
            return GetError(SDL_GL_GetCurrentContext());
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_sdl_gl_getprocaddress(string proc);
        public static d_sdl_gl_getprocaddress GetProcAddress = FuncLoader.LoadFunction<d_sdl_gl_getprocaddress>(NativeLibrary, "SDL_GL_GetProcAddress");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int d_sdl_gl_getswapinterval();
        public static d_sdl_gl_getswapinterval GetSwapInterval = FuncLoader.LoadFunction<d_sdl_gl_getswapinterval>(NativeLibrary, "SDL_GL_GetSwapInterval");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int d_sdl_gl_makecurrent(IntPtr window, IntPtr context);
        public static d_sdl_gl_makecurrent MakeCurrent = FuncLoader.LoadFunction<d_sdl_gl_makecurrent>(NativeLibrary, "SDL_GL_MakeCurrent");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_gl_setattribute(Attribute attr, int value);
        private static d_sdl_gl_setattribute SDL_GL_SetAttribute = FuncLoader.LoadFunction<d_sdl_gl_setattribute>(NativeLibrary, "SDL_GL_SetAttribute");

        public static int SetAttribute(Attribute attr, int value)
        {
            return GetError(SDL_GL_SetAttribute(attr, value));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int d_sdl_gl_setswapinterval(int interval);
        public static d_sdl_gl_setswapinterval SetSwapInterval = FuncLoader.LoadFunction<d_sdl_gl_setswapinterval>(NativeLibrary, "SDL_GL_SetSwapInterval");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_gl_swapwindow(IntPtr window);
        public static d_sdl_gl_swapwindow SwapWindow = FuncLoader.LoadFunction<d_sdl_gl_swapwindow>(NativeLibrary, "SDL_GL_SwapWindow");
    }

    public static class Mouse
    {
        [Flags]
        public enum Button
        {
            Left = 1 << 0,
            Middle = 1 << 1,
            Right = 1 << 2,
            X1Mask = 1 << 3,
            X2Mask = 1 << 4
        }

        public enum SystemCursor
        {
            Arrow,
            IBeam,
            Wait,
            Crosshair,
            WaitArrow,
            SizeNWSE,
            SizeNESW,
            SizeWE,
            SizeNS,
            SizeAll,
            No,
            Hand
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MotionEvent
        {
            public EventType Type;
            public uint Timestamp;
            public uint WindowID;
            public uint Which;
            public byte State;
            private byte _padding1;
            private byte _padding2;
            private byte _padding3;
            public int X;
            public int Y;
            public int Xrel;
            public int Yrel;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WheelEvent
        {
            public EventType Type;
            public uint TimeStamp;
            public uint WindowId;
            public uint Which;
            public int X;
            public int Y;
            public uint Direction;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_sdl_createcolorcursor(IntPtr surface, int x, int y);
        private static d_sdl_createcolorcursor SDL_CreateColorCursor = FuncLoader.LoadFunction<d_sdl_createcolorcursor>(NativeLibrary, "SDL_CreateColorCursor");

        public static IntPtr CreateColorCursor(IntPtr surface, int x, int y)
        {
            return GetError(SDL_CreateColorCursor(surface, x, y));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_sdl_createsystemcursor(SystemCursor id);
        private static d_sdl_createsystemcursor SDL_CreateSystemCursor = FuncLoader.LoadFunction<d_sdl_createsystemcursor>(NativeLibrary, "SDL_CreateSystemCursor");

        public static IntPtr CreateSystemCursor(SystemCursor id)
        {
            return GetError(SDL_CreateSystemCursor(id));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_freecursor(IntPtr cursor);
        public static d_sdl_freecursor FreeCursor = FuncLoader.LoadFunction<d_sdl_freecursor>(NativeLibrary, "SDL_FreeCursor");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Button d_sdl_getglobalmousestate(out int x, out int y);
        public static d_sdl_getglobalmousestate GetGlobalState = FuncLoader.LoadFunction<d_sdl_getglobalmousestate>(NativeLibrary, "SDL_GetGlobalMouseState");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Button d_sdl_getmousestate(out int x, out int y);
        public static d_sdl_getmousestate GetState = FuncLoader.LoadFunction<d_sdl_getmousestate>(NativeLibrary, "SDL_GetMouseState");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_setcursor(IntPtr cursor);
        public static d_sdl_setcursor SetCursor = FuncLoader.LoadFunction<d_sdl_setcursor>(NativeLibrary, "SDL_SetCursor");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int d_sdl_showcursor(int toggle);
        public static d_sdl_showcursor ShowCursor = FuncLoader.LoadFunction<d_sdl_showcursor>(NativeLibrary, "SDL_ShowCursor");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_warpmouseinwindow(IntPtr window, int x, int y);
        public static d_sdl_warpmouseinwindow WarpInWindow = FuncLoader.LoadFunction<d_sdl_warpmouseinwindow>(NativeLibrary, "SDL_WarpMouseInWindow");
    }

    public static class Drop
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct DropEvent
        {
            public EventType Type;
            public uint Timestamp;
            public IntPtr File;
            public uint WindowId;
        }
    }

    public static class Keyboard
    {
        public struct Keysym
        {
            public int Scancode;
            public int Sym;
            public Keymod Mod;
            public uint Unicode;
        }

        [Flags]
        public enum Keymod : ushort
        {
            None = 0x0000,
            LeftShift = 0x0001,
            RightShift = 0x0002,
            LeftCtrl = 0x0040,
            RightCtrl = 0x0080,
            LeftAlt = 0x0100,
            RightAlt = 0x0200,
            LeftGui = 0x0400,
            RightGui = 0x0800,
            NumLock = 0x1000,
            CapsLock = 0x2000,
            AltGr = 0x4000,
            Reserved = 0x8000,
            Ctrl = (LeftCtrl | RightCtrl),
            Shift = (LeftShift | RightShift),
            Alt = (LeftAlt | RightAlt),
            Gui = (LeftGui | RightGui)
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Event
        {
            public EventType Type;
            public uint TimeStamp;
            public uint WindowId;
            public byte State;
            public byte Repeat;
            private byte padding2;
            private byte padding3;
            public Keysym Keysym;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct TextEditingEvent
        {
            public EventType Type;
            public uint Timestamp;
            public uint WindowId;
            public fixed byte Text[32];
            public int Start;
            public int Length;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct TextInputEvent
        {
            public EventType Type;
            public uint Timestamp;
            public uint WindowId;
            public fixed byte Text[32];
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Keymod d_sdl_getmodstate();
        public static d_sdl_getmodstate GetModState = FuncLoader.LoadFunction<d_sdl_getmodstate>(NativeLibrary, "SDL_GetModState");
    }

    public static class Joystick
    {
        [Flags]
        public enum Hat : byte
        {
            Centered = 0,
            Up = 1 << 0,
            Right = 1 << 1,
            Down = 1 << 2,
            Left = 1 << 3
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DeviceEvent
        {
            public EventType Type;
            public uint TimeStamp;
            public int Which;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_joystickclose(IntPtr joystick);
        public static d_sdl_joystickclose Close = FuncLoader.LoadFunction<d_sdl_joystickclose>(NativeLibrary, "SDL_JoystickClose");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_sdl_joystickfrominstanceid(int joyid);
        private static d_sdl_joystickfrominstanceid SDL_JoystickFromInstanceID = FuncLoader.LoadFunction<d_sdl_joystickfrominstanceid>(NativeLibrary, "SDL_JoystickFromInstanceID");

        public static IntPtr FromInstanceID(int joyid)
        {
            return GetError(SDL_JoystickFromInstanceID(joyid));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate short d_sdl_joystickgetaxis(IntPtr joystick, int axis);
        public static d_sdl_joystickgetaxis GetAxis = FuncLoader.LoadFunction<d_sdl_joystickgetaxis>(NativeLibrary, "SDL_JoystickGetAxis");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate byte d_sdl_joystickgetbutton(IntPtr joystick, int button);
        public static d_sdl_joystickgetbutton GetButton = FuncLoader.LoadFunction<d_sdl_joystickgetbutton>(NativeLibrary, "SDL_JoystickGetButton");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Guid d_sdl_joystickgetguid(IntPtr joystick);
        public static d_sdl_joystickgetguid GetGUID = FuncLoader.LoadFunction<d_sdl_joystickgetguid>(NativeLibrary, "SDL_JoystickGetGUID");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Hat d_sdl_joystickgethat(IntPtr joystick, int hat);
        public static d_sdl_joystickgethat GetHat = FuncLoader.LoadFunction<d_sdl_joystickgethat>(NativeLibrary, "SDL_JoystickGetHat");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int d_sdl_joystickinstanceid(IntPtr joystick);
        public static d_sdl_joystickinstanceid InstanceID = FuncLoader.LoadFunction<d_sdl_joystickinstanceid>(NativeLibrary, "SDL_JoystickInstanceID");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_sdl_joystickopen(int deviceIndex);
        private static d_sdl_joystickopen SDL_JoystickOpen = FuncLoader.LoadFunction<d_sdl_joystickopen>(NativeLibrary, "SDL_JoystickOpen");

        public static IntPtr Open(int deviceIndex)
        {
            return GetError(SDL_JoystickOpen(deviceIndex));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_joysticknumaxes(IntPtr joystick);
        private static d_sdl_joysticknumaxes SDL_JoystickNumAxes = FuncLoader.LoadFunction<d_sdl_joysticknumaxes>(NativeLibrary, "SDL_JoystickNumAxes");

        public static int NumAxes(IntPtr joystick)
        {
            return GetError(SDL_JoystickNumAxes(joystick));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_joysticknumbuttons(IntPtr joystick);
        private static d_sdl_joysticknumbuttons SDL_JoystickNumButtons = FuncLoader.LoadFunction<d_sdl_joysticknumbuttons>(NativeLibrary, "SDL_JoystickNumButtons");

        public static int NumButtons(IntPtr joystick)
        {
            return GetError(SDL_JoystickNumButtons(joystick));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_joysticknumhats(IntPtr joystick);
        private static d_sdl_joysticknumhats SDL_JoystickNumHats = FuncLoader.LoadFunction<d_sdl_joysticknumhats>(NativeLibrary, "SDL_JoystickNumHats");

        public static int NumHats(IntPtr joystick)
        {
            return GetError(SDL_JoystickNumHats(joystick));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_numjoysticks();
        private static d_sdl_numjoysticks SDL_NumJoysticks = FuncLoader.LoadFunction<d_sdl_numjoysticks>(NativeLibrary, "SDL_NumJoysticks");

        public static int NumJoysticks()
        {
            return GetError(SDL_NumJoysticks());
        }
    }

    public static class GameController
    {
        public enum Axis
        {
            Invalid = -1,
            LeftX,
            LeftY,
            RightX,
            RightY,
            TriggerLeft,
            TriggerRight,
            Max,
        }

        public enum Button
        {
            Invalid = -1,
            A,
            B,
            X,
            Y,
            Back,
            Guide,
            Start,
            LeftStick,
            RightStick,
            LeftShoulder,
            RightShoulder,
            DpadUp,
            DpadDown,
            DpadLeft,
            DpadRight,
            Max,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DeviceEvent
        {
            public EventType Type;
            public uint TimeStamp;
            public int Which;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int d_sdl_gamecontrolleraddmapping(string mappingString);
        public static d_sdl_gamecontrolleraddmapping AddMapping = FuncLoader.LoadFunction<d_sdl_gamecontrolleraddmapping>(NativeLibrary, "SDL_GameControllerAddMapping");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int d_sdl_gamecontrolleraddmappingsfromrw(IntPtr rw, int freew);
        public static d_sdl_gamecontrolleraddmappingsfromrw AddMappingFromRw = FuncLoader.LoadFunction<d_sdl_gamecontrolleraddmappingsfromrw>(NativeLibrary, "SDL_GameControllerAddMappingsFromRW");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_gamecontrollerclose(IntPtr gamecontroller);
        public static d_sdl_gamecontrollerclose Close = FuncLoader.LoadFunction<d_sdl_gamecontrollerclose>(NativeLibrary, "SDL_GameControllerClose");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_sdl_joystickfrominstanceid(int joyid);
        private static d_sdl_joystickfrominstanceid SDL_GameControllerFromInstanceID = FuncLoader.LoadFunction<d_sdl_joystickfrominstanceid>(NativeLibrary, "SDL_JoystickFromInstanceID");

        public static IntPtr FromInstanceID(int joyid)
        {
            return GetError(SDL_GameControllerFromInstanceID(joyid));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate short d_sdl_gamecontrollergetaxis(IntPtr gamecontroller, Axis axis);
        public static d_sdl_gamecontrollergetaxis GetAxis = FuncLoader.LoadFunction<d_sdl_gamecontrollergetaxis>(NativeLibrary, "SDL_GameControllerGetAxis");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate byte d_sdl_gamecontrollergetbutton(IntPtr gamecontroller, Button button);
        public static d_sdl_gamecontrollergetbutton GetButton = FuncLoader.LoadFunction<d_sdl_gamecontrollergetbutton>(NativeLibrary, "SDL_GameControllerGetButton");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_sdl_gamecontrollergetjoystick(IntPtr gamecontroller);
        private static d_sdl_gamecontrollergetjoystick SDL_GameControllerGetJoystick = FuncLoader.LoadFunction<d_sdl_gamecontrollergetjoystick>(NativeLibrary, "SDL_GameControllerGetJoystick");

        public static IntPtr GetJoystick(IntPtr gamecontroller)
        {
            return GetError(SDL_GameControllerGetJoystick(gamecontroller));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate byte d_sdl_isgamecontroller(int joystickIndex);
        public static d_sdl_isgamecontroller IsGameController = FuncLoader.LoadFunction<d_sdl_isgamecontroller>(NativeLibrary, "SDL_IsGameController");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_sdl_gamecontrollermapping(IntPtr gamecontroller);
        private static d_sdl_gamecontrollermapping SDL_GameControllerMapping = FuncLoader.LoadFunction<d_sdl_gamecontrollermapping>(NativeLibrary, "SDL_GameControllerMapping");

        public static string GetMapping(IntPtr gamecontroller)
        {
            return GetString(SDL_GameControllerMapping(gamecontroller));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_sdl_gamecontrolleropen(int joystickIndex);
        private static d_sdl_gamecontrolleropen SDL_GameControllerOpen = FuncLoader.LoadFunction<d_sdl_gamecontrolleropen>(NativeLibrary, "SDL_GameControllerOpen");

        public static IntPtr Open(int joystickIndex)
        {
            return GetError(SDL_GameControllerOpen(joystickIndex));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_sdl_gamecontrollername(IntPtr gamecontroller);
        private static d_sdl_gamecontrollername SDL_GameControllerName = FuncLoader.LoadFunction<d_sdl_gamecontrollername>(NativeLibrary, "SDL_GameControllerName");

        public static string GetName(IntPtr gamecontroller)
        {
            return GetString(SDL_GameControllerName(gamecontroller));
        }
    }

    public static class Haptic
    {
        public const uint Infinity = 4292967295U;

        public enum EffectId : ushort
        {
            LeftRight = (1 << 2),
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LeftRight
        {
            public EffectId Type;
            public uint Length;
            public ushort LargeMagnitude;
            public ushort SmallMagnitude;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct Effect
        {
            [FieldOffset(0)] public EffectId type;
            [FieldOffset(0)] public LeftRight leftright;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void d_sdl_hapticclose(IntPtr haptic);
        public static d_sdl_hapticclose Close = FuncLoader.LoadFunction<d_sdl_hapticclose>(NativeLibrary, "SDL_HapticClose");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int d_sdl_hapticeffectsupported(IntPtr haptic, ref Effect effect);
        public static d_sdl_hapticeffectsupported EffectSupported = FuncLoader.LoadFunction<d_sdl_hapticeffectsupported>(NativeLibrary, "SDL_HapticEffectSupported");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int d_sdl_joystickishaptic(IntPtr joystick);
        public static d_sdl_joystickishaptic IsHaptic = FuncLoader.LoadFunction<d_sdl_joystickishaptic>(NativeLibrary, "SDL_JoystickIsHaptic");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_hapticneweffect(IntPtr haptic, ref Effect effect);
        private static d_sdl_hapticneweffect SDL_HapticNewEffect = FuncLoader.LoadFunction<d_sdl_hapticneweffect>(NativeLibrary, "SDL_HapticNewEffect");

        public static void NewEffect(IntPtr haptic, ref Effect effect)
        {
            GetError(SDL_HapticNewEffect(haptic, ref effect));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_sdl_hapticopen(int device_index);
        public static d_sdl_hapticopen Open = FuncLoader.LoadFunction<d_sdl_hapticopen>(NativeLibrary, "SDL_HapticOpen");

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_sdl_hapticopenfromjoystick(IntPtr joystick);
        private static d_sdl_hapticopenfromjoystick SDL_HapticOpenFromJoystick = FuncLoader.LoadFunction<d_sdl_hapticopenfromjoystick>(NativeLibrary, "SDL_HapticOpenFromJoystick");

        public static IntPtr OpenFromJoystick(IntPtr joystick)
        {
            return GetError(SDL_HapticOpenFromJoystick(joystick));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_hapticrumbleinit(IntPtr haptic);
        private static d_sdl_hapticrumbleinit SDL_HapticRumbleInit = FuncLoader.LoadFunction<d_sdl_hapticrumbleinit>(NativeLibrary, "SDL_HapticRumbleInit");

        public static void RumbleInit(IntPtr haptic)
        {
            GetError(SDL_HapticRumbleInit(haptic));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_hapticrumbleplay(IntPtr haptic, float strength, uint length);
        private static d_sdl_hapticrumbleplay SDL_HapticRumblePlay = FuncLoader.LoadFunction<d_sdl_hapticrumbleplay>(NativeLibrary, "SDL_HapticRumblePlay");

        public static void RumblePlay(IntPtr haptic, float strength, uint length)
        {
            GetError(SDL_HapticRumblePlay(haptic, strength, length));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_hapticrumblesupported(IntPtr haptic);
        private static d_sdl_hapticrumblesupported SDL_HapticRumbleSupported = FuncLoader.LoadFunction<d_sdl_hapticrumblesupported>(NativeLibrary, "SDL_HapticRumbleSupported");

        public static int RumbleSupported(IntPtr haptic)
        {
            return GetError(SDL_HapticRumbleSupported(haptic));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_hapticruneffect(IntPtr haptic, int effect, uint iterations);
        private static d_sdl_hapticruneffect SDL_HapticRunEffect = FuncLoader.LoadFunction<d_sdl_hapticruneffect>(NativeLibrary, "SDL_HapticRunEffect");

        public static void RunEffect(IntPtr haptic, int effect, uint iterations)
        {
            GetError(SDL_HapticRunEffect(haptic, effect, iterations));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_hapticstopall(IntPtr haptic);
        private static d_sdl_hapticstopall SDL_HapticStopAll = FuncLoader.LoadFunction<d_sdl_hapticstopall>(NativeLibrary, "SDL_HapticStopAll");

        public static void StopAll(IntPtr haptic)
        {
            GetError(SDL_HapticStopAll(haptic));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int d_sdl_hapticupdateeffect(IntPtr haptic, int effect, ref Effect data);
        private static d_sdl_hapticupdateeffect SDL_HapticUpdateEffect = FuncLoader.LoadFunction<d_sdl_hapticupdateeffect>(NativeLibrary, "SDL_HapticUpdateEffect");

        public static void UpdateEffect(IntPtr haptic, int effect, ref Effect data)
        {
            GetError(SDL_HapticUpdateEffect(haptic, effect, ref data));
        }
    }
}
