﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Utilities;
using System.IO;

namespace MonoGame.OpenAL
{
    internal enum ALFormat
    {
        Mono8 = 0x1100,
        Mono16 = 0x1101,
        Stereo8 = 0x1102,
        Stereo16 = 0x1103,
        MonoIma4 = 0x1300,
        StereoIma4 = 0x1301,
        MonoMSAdpcm = 0x1302,
        StereoMSAdpcm = 0x1303,
        MonoFloat32 = 0x10010,
        StereoFloat32 = 0x10011,
    }

    internal enum ALError
    {
        NoError = 0,
        InvalidName = 0xA001,
        InvalidEnum = 0xA002,
        InvalidValue = 0xA003,
        InvalidOperation = 0xA004,
        OutOfMemory = 0xA005,
    }

    internal enum ALGetString
    {
        Extensions = 0xB004,
    }

    internal enum ALBufferi
    {
        UnpackBlockAlignmentSoft = 0x200C,
        LoopSoftPointsExt = 0x2015,
    }

    internal enum ALGetBufferi
    {
        Bits = 0x2002,
        Channels = 0x2003,
        Size = 0x2004,
    }

    internal enum ALSourceb
    {
        Looping = 0x1007,
    }

    internal enum ALSourcei
    {
        SourceRelative = 0x202,
        Buffer = 0x1009,
        EfxDirectFilter = 0x20005,
        EfxAuxilarySendFilter = 0x20006,
    }

    internal enum ALSourcef
    {
        Pitch = 0x1003,
        Gain = 0x100A,
        ReferenceDistance = 0x1020
    }

    internal enum ALGetSourcei
    {
        SampleOffset = 0x1025,
        SourceState = 0x1010,
        BuffersQueued = 0x1015,
        BuffersProcessed = 0x1016,
    }

    internal enum ALSourceState
    {
        Initial = 0x1011,
        Playing = 0x1012,
        Paused = 0x1013,
        Stopped = 0x1014,
    }

    internal enum ALListener3f
    {
        Position = 0x1004,
    }

    internal enum ALSource3f
    {
        Position = 0x1004,
        Velocity = 0x1006,
    }

    internal enum ALDistanceModel
    {
        None = 0,
        InverseDistanceClamped = 0xD002,
    }

    internal enum AlcError
    {
        NoError = 0,
    }

    internal enum AlcGetString
    {
        CaptureDeviceSpecifier = 0x0310,
        CaptureDefaultDeviceSpecifier = 0x0311,
        Extensions = 0x1006,
    }

    internal enum AlcGetInteger
    {
        CaptureSamples = 0x0312,
    }

    internal enum EfxFilteri
    {
        FilterType = 0x8001,
    }

    internal enum EfxFilterf
    {
        LowpassGain = 0x0001,
        LowpassGainHF = 0x0002,
        HighpassGain = 0x0001,
        HighpassGainLF = 0x0002,
        BandpassGain = 0x0001,
        BandpassGainLF = 0x0002,
        BandpassGainHF = 0x0003,
    }

    internal enum EfxFilterType
    {
        None = 0x0000,
        Lowpass = 0x0001,
        Highpass = 0x0002,
        Bandpass = 0x0003,
    }

    internal enum EfxEffecti
    {
        EffectType = 0x8001,
        SlotEffect = 0x0001,
    }

    internal enum EfxEffectSlotf
    {
        EffectSlotGain = 0x0002,
    }

    internal enum EfxEffectf
    {
        EaxReverbDensity = 0x0001,
        EaxReverbDiffusion = 0x0002,
        EaxReverbGain = 0x0003,
        EaxReverbGainHF = 0x0004,
        EaxReverbGainLF = 0x0005,
        DecayTime = 0x0006,
        DecayHighFrequencyRatio = 0x0007,
        DecayLowFrequencyRation = 0x0008,
        EaxReverbReflectionsGain = 0x0009,
        EaxReverbReflectionsDelay = 0x000A,
        ReflectionsPain = 0x000B,
        LateReverbGain = 0x000C,
        LateReverbDelay = 0x000D,
        LateRevertPain = 0x000E,
        EchoTime = 0x000F,
        EchoDepth = 0x0010,
        ModulationTime = 0x0011,
        ModulationDepth = 0x0012,
        AirAbsorbsionHighFrequency = 0x0013,
        EaxReverbHFReference = 0x0014,
        EaxReverbLFReference = 0x0015,
        RoomRolloffFactor = 0x0016,
        DecayHighFrequencyLimit = 0x0017,
    }

    internal enum EfxEffectType
    {
        Reverb = 0x8000,
    }

    internal class AL
    {
        internal static void LoadEntryPoints()
        {
            IntPtr nativeLibrary = GetNativeLibrary();

            if (nativeLibrary == IntPtr.Zero)
            {
                throw new DllNotFoundException("Couldn't load the OpenAL library, it appears to be missing.");
            }

            Enable = FuncLoader.LoadFunction<d_alenable>(nativeLibrary, "alEnable");
            alBufferData = FuncLoader.LoadFunction<d_albufferdata>(nativeLibrary, "alBufferData");
            alDeleteBuffers = FuncLoader.LoadFunction<d_aldeletebuffers>(nativeLibrary, "alDeleteBuffers");
            Bufferi = FuncLoader.LoadFunction<d_albufferi>(nativeLibrary, "alBufferi");
            GetBufferi = FuncLoader.LoadFunction<d_algetbufferi>(nativeLibrary, "alGetBufferi");
            Bufferiv = FuncLoader.LoadFunction<d_albufferiv>(nativeLibrary, "alBufferiv");
            alGenBuffers = FuncLoader.LoadFunction<d_algenbuffers>(nativeLibrary, "alGenBuffers");
            alGenSources = FuncLoader.LoadFunction<d_algensources>(nativeLibrary, "alGenSources");
            GetError = FuncLoader.LoadFunction<d_algeterror>(nativeLibrary, "alGetError");
            alIsBuffer = FuncLoader.LoadFunction<d_alisbuffer>(nativeLibrary, "alIsBuffer");
            alSourcePause = FuncLoader.LoadFunction<d_alsourcepause>(nativeLibrary, "alSourcePause");
            alSourcePlay = FuncLoader.LoadFunction<d_alsourceplay>(nativeLibrary, "alSourcePlay");
            IsSource = FuncLoader.LoadFunction<d_alissource>(nativeLibrary, "alIsSource");
            alDeleteSources = FuncLoader.LoadFunction<d_aldeletesources>(nativeLibrary, "alDeleteSources");
            SourceStop = FuncLoader.LoadFunction<d_alsourcestop>(nativeLibrary, "alSourceStop");
            alSourcei = FuncLoader.LoadFunction<d_alsourcei>(nativeLibrary, "alSourcei");
            alSource3i = FuncLoader.LoadFunction<d_alsource3i>(nativeLibrary, "alSource3i");
            alSourcef = FuncLoader.LoadFunction<d_alsourcef>(nativeLibrary, "alSourcef");
            alSource3f = FuncLoader.LoadFunction<d_alsource3f>(nativeLibrary, "alSource3f");
            GetSource = FuncLoader.LoadFunction<d_algetsourcei>(nativeLibrary, "alGetSourcei");
            GetListener = FuncLoader.LoadFunction<d_algetlistener3f>(nativeLibrary, "alGetListener3f");
            DistanceModel = FuncLoader.LoadFunction<d_aldistancemodel>(nativeLibrary, "alDistanceModel");
            DopplerFactor = FuncLoader.LoadFunction<d_aldopplerfactor>(nativeLibrary, "alDopplerFactor");
            alSourceQueueBuffers = FuncLoader.LoadFunction<d_alsourcequeuebuffers>(nativeLibrary, "alSourceQueueBuffers");
            alSourceUnqueueBuffers = FuncLoader.LoadFunction<d_alsourceunqueuebuffers>(nativeLibrary, "alSourceUnqueueBuffers");
            alSourceUnqueueBuffers2 = FuncLoader.LoadFunction<d_alsourceunqueuebuffers2>(nativeLibrary, "alSourceUnqueueBuffers");
            alGetEnumValue = FuncLoader.LoadFunction<d_algetenumvalue>(nativeLibrary, "alGetEnumValue");
            IsExtensionPresent = FuncLoader.LoadFunction<d_alisextensionpresent>(nativeLibrary, "alIsExtensionPresent");
            alGetProcAddress = FuncLoader.LoadFunction<d_algetprocaddress>(nativeLibrary, "alGetProcAddress");
            alGetString = FuncLoader.LoadFunction<d_algetstring>(nativeLibrary, "alGetString");

            Alc.CreateContext = FuncLoader.LoadFunction<Alc.d_alccreatecontext>(nativeLibrary, "alcCreateContext");
            Alc.GetErrorForDevice = FuncLoader.LoadFunction<Alc.d_alcgeterror>(nativeLibrary, "alcGetError");
            Alc.alcGetIntegerv = FuncLoader.LoadFunction<Alc.d_alcgetintegerv>(nativeLibrary, "alcGetIntegerv");
            Alc.GetCurrentContext = FuncLoader.LoadFunction<Alc.d_alcgetcurrentcontext>(nativeLibrary, "alcGetCurrentContext");
            Alc.MakeContextCurrent = FuncLoader.LoadFunction<Alc.d_alcmakecontextcurrent>(nativeLibrary, "alcMakeContextCurrent");
            Alc.DestroyContext = FuncLoader.LoadFunction<Alc.d_alcdestroycontext>(nativeLibrary, "alcDestroyContext");
            Alc.CloseDevice = FuncLoader.LoadFunction<Alc.d_alcclosedevice>(nativeLibrary, "alcCloseDevice");
            Alc.OpenDevice = FuncLoader.LoadFunction<Alc.d_alcopendevice>(nativeLibrary, "alcOpenDevice");
            Alc.alcCaptureOpenDevice = FuncLoader.LoadFunction<Alc.d_alccaptureopendevice>(nativeLibrary, "alcCaptureOpenDevice");
            Alc.CaptureStart = FuncLoader.LoadFunction<Alc.d_alccapturestart>(nativeLibrary, "alcCaptureStart");
            Alc.CaptureSamples = FuncLoader.LoadFunction<Alc.d_alccapturesamples>(nativeLibrary, "alcCaptureSamples");
            Alc.CaptureStop = FuncLoader.LoadFunction<Alc.d_alccapturestop>(nativeLibrary, "alcCaptureStop");
            Alc.CaptureCloseDevice = FuncLoader.LoadFunction<Alc.d_alccaptureclosedevice>(nativeLibrary, "alcCaptureCloseDevice");
            Alc.IsExtensionPresent = FuncLoader.LoadFunction<Alc.d_alcisextensionpresent>(nativeLibrary, "alcIsExtensionPresent");
            Alc.alcGetString = FuncLoader.LoadFunction<Alc.d_alcgetstring>(nativeLibrary, "alcGetString");

#if IOS
            Alc.SuspendContext = FuncLoader.LoadFunction<Alc.d_alcsuspendcontext>(nativeLibrary, "alcSuspendContext");
            Alc.ProcessContext = FuncLoader.LoadFunction<Alc.d_alcprocesscontext>(nativeLibrary, "alcProcessContext");
#endif

#if ANDROID
            Alc.DevicePause = FuncLoader.LoadFunction<Alc.d_alcdevicepausesoft>(nativeLibrary, "alcDevicePauseSOFT");
            Alc.DeviceResume = FuncLoader.LoadFunction<Alc.d_alcdeviceresumesoft>(nativeLibrary, "alcDeviceResumeSOFT");
#endif
        }

        private static IntPtr GetNativeLibrary()
        {
            var ret = IntPtr.Zero;

#if DESKTOPGL
            // Load bundled library
            var assemblyLocation = Path.GetDirectoryName((new Uri(typeof(AL).Assembly.CodeBase)).LocalPath);
            if (CurrentPlatform.OS == OS.Windows && Environment.Is64BitProcess)
                ret = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "x64/soft_oal.dll"));
            else if (CurrentPlatform.OS == OS.Windows && !Environment.Is64BitProcess)
                ret = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "x86/soft_oal.dll"));
            else if (CurrentPlatform.OS == OS.Linux && Environment.Is64BitProcess)
                ret = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "x64/libopenal.so.1"));
            else if (CurrentPlatform.OS == OS.Linux && !Environment.Is64BitProcess)
                ret = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "x86/libopenal.so.1"));
            else if (CurrentPlatform.OS == OS.MacOSX)
                ret = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "libopenal.1.dylib"));

            // Load system library
            if (ret == IntPtr.Zero)
            {
                if (CurrentPlatform.OS == OS.Windows)
                    ret = FuncLoader.LoadLibrary("soft_oal.dll");
                else if (CurrentPlatform.OS == OS.Linux)
                    ret = FuncLoader.LoadLibrary("libopenal.so.1");
                else
                    ret = FuncLoader.LoadLibrary("libopenal.1.dylib");
            }
#elif ANDROID
            ret = FuncLoader.LoadLibrary("libopenal32.so");

            if (ret == IntPtr.Zero)
            {
                var appFilesDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var appDir = Path.GetDirectoryName(appFilesDir);
                var lib = Path.Combine(appDir, "lib", "libopenal32.so");

                ret = FuncLoader.LoadLibrary(lib);
            }
#else
            ret = FuncLoader.LoadLibrary("/System/Library/Frameworks/OpenAL.framework/OpenAL");
#endif

            return ret;
        }

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alenable(int cap);
        internal static d_alenable Enable;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_albufferdata(uint bid, int format, IntPtr data, int size, int freq);
        internal static d_albufferdata alBufferData;

        internal static void BufferData(int bid, ALFormat format, byte[] data, int size, int freq)
        {
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            alBufferData((uint)bid, (int)format, handle.AddrOfPinnedObject(), size, freq);
            handle.Free();
        }

        internal static void BufferData(int bid, ALFormat format, short[] data, int size, int freq)
        {
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            alBufferData((uint)bid, (int)format, handle.AddrOfPinnedObject(), size, freq);
            handle.Free();
        }

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal unsafe delegate void d_aldeletebuffers(int n, int* buffers);
        internal static d_aldeletebuffers alDeleteBuffers;

        internal static void DeleteBuffers(int[] buffers)
        {
            DeleteBuffers(buffers.Length, ref buffers[0]);
        }

        internal unsafe static void DeleteBuffers(int n, ref int buffers)
        {
            fixed (int* pbuffers = &buffers)
            {
                alDeleteBuffers(n, pbuffers);
            }
        }

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_albufferi(int buffer, ALBufferi param, int value);
        internal static d_albufferi Bufferi;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_algetbufferi(int bid, ALGetBufferi param, out int value);
        internal static d_algetbufferi GetBufferi;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_albufferiv(int bid, ALBufferi param, int[] values);
        internal static d_albufferiv Bufferiv;

        internal static void GetBuffer(int bid, ALGetBufferi param, out int value)
        {
            GetBufferi(bid, param, out value);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal unsafe delegate void d_algenbuffers(int count, int* buffers);
        internal static d_algenbuffers alGenBuffers;

        internal unsafe static void GenBuffers(int count, out int[] buffers)
        {
            buffers = new int[count];
            fixed (int* ptr = &buffers[0])
            {
                alGenBuffers(count, ptr);
            }
        }

        internal static void GenBuffers(int count, out int buffer)
        {
            int[] ret;
            GenBuffers(count, out ret);
            buffer = ret[0];
        }

        internal static int[] GenBuffers(int count)
        {
            int[] ret;
            GenBuffers(count, out ret);
            return ret;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_algensources(int n, uint[] sources);
        internal static d_algensources alGenSources;


        internal static void GenSources(int[] sources)
        {
            uint[] temp = new uint[sources.Length];
            alGenSources(temp.Length, temp);
            for (int i = 0; i < temp.Length; i++)
            {
                sources[i] = (int)temp[i];
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate ALError d_algeterror();
        internal static d_algeterror GetError;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool d_alisbuffer(uint buffer);
        internal static d_alisbuffer alIsBuffer;

        internal static bool IsBuffer(int buffer)
        {
            return alIsBuffer((uint)buffer);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alsourcepause(uint source);
        internal static d_alsourcepause alSourcePause;

        internal static void SourcePause(int source)
        {
            alSourcePause((uint)source);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alsourceplay(uint source);
        internal static d_alsourceplay alSourcePlay;

        internal static void SourcePlay(int source)
        {
            alSourcePlay((uint)source);
        }

        internal static string GetErrorString(ALError errorCode)
        {
            return errorCode.ToString();
        }

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool d_alissource(int source);
        internal static d_alissource IsSource;

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_aldeletesources(int n, ref int sources);
        internal static d_aldeletesources alDeleteSources;

        internal static void DeleteSource(int source)
        {
            alDeleteSources(1, ref source);
        }

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alsourcestop(int sourceId);
        internal static d_alsourcestop SourceStop;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alsourcei(int sourceId, int i, int a);
        internal static d_alsourcei alSourcei;

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alsource3i(int sourceId, ALSourcei i, int a, int b, int c);
        internal static d_alsource3i alSource3i;

        internal static void Source(int sourceId, ALSourcei i, int a)
        {
            alSourcei(sourceId, (int)i, a);
        }

        internal static void Source(int sourceId, ALSourceb i, bool a)
        {
            alSourcei(sourceId, (int)i, a ? 1 : 0);
        }

        internal static void Source(int sourceId, ALSource3f i, float x, float y, float z)
        {
            alSource3f(sourceId, i, x, y, z);
        }

        internal static void Source(int sourceId, ALSourcef i, float dist)
        {
            alSourcef(sourceId, i, dist);
        }

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alsourcef(int sourceId, ALSourcef i, float a);
        internal static d_alsourcef alSourcef;

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alsource3f(int sourceId, ALSource3f i, float x, float y, float z);
        internal static d_alsource3f alSource3f;

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_algetsourcei(int sourceId, ALGetSourcei i, out int state);
        internal static d_algetsourcei GetSource;

        internal static ALSourceState GetSourceState(int sourceId)
        {
            int state;
            GetSource(sourceId, ALGetSourcei.SourceState, out state);
            return (ALSourceState)state;
        }

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_algetlistener3f(ALListener3f param, out float value1, out float value2, out float value3);
        internal static d_algetlistener3f GetListener;

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_aldistancemodel(ALDistanceModel model);
        internal static d_aldistancemodel DistanceModel;

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_aldopplerfactor(float value);
        internal static d_aldopplerfactor DopplerFactor;

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal unsafe delegate void d_alsourcequeuebuffers(int sourceId, int numEntries, int* buffers);
        internal static d_alsourcequeuebuffers alSourceQueueBuffers;

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal unsafe delegate void d_alsourceunqueuebuffers(int sourceId, int numEntries, int* salvaged);
        internal static d_alsourceunqueuebuffers alSourceUnqueueBuffers;

        [CLSCompliant(false)]
        internal static unsafe void SourceQueueBuffers(int sourceId, int numEntries, int[] buffers)
        {
            fixed (int* ptr = &buffers[0])
            {
                AL.alSourceQueueBuffers(sourceId, numEntries, ptr);
            }
        }

        internal unsafe static void SourceQueueBuffer(int sourceId, int buffer)
        {
            AL.alSourceQueueBuffers(sourceId, 1, &buffer);
        }

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alsourceunqueuebuffers2(int sid, int numEntries, out int[] bids);
        internal static d_alsourceunqueuebuffers2 alSourceUnqueueBuffers2;

        internal static unsafe int[] SourceUnqueueBuffers(int sourceId, int numEntries)
        {
            if (numEntries <= 0)
            {
                throw new ArgumentOutOfRangeException("numEntries", "Must be greater than zero.");
            }
            int[] array = new int[numEntries];
            fixed (int* ptr = &array[0])
            {
                alSourceUnqueueBuffers(sourceId, numEntries, ptr);
            }
            return array;
        }

        internal static void SourceUnqueueBuffers(int sid, int numENtries, out int[] bids)
        {
            alSourceUnqueueBuffers2(sid, numENtries, out bids);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int d_algetenumvalue(string enumName);
        internal static d_algetenumvalue alGetEnumValue;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool d_alisextensionpresent(string extensionName);
        internal static d_alisextensionpresent IsExtensionPresent;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_algetprocaddress(string functionName);
        internal static d_algetprocaddress alGetProcAddress;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr d_algetstring(int p);
        private static d_algetstring alGetString;

        internal static string GetString(int p)
        {
            return Marshal.PtrToStringAnsi(alGetString(p));
        }

        internal static string Get(ALGetString p)
        {
            return GetString((int)p);
        }
    }

    internal partial class Alc
    {
        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_alccreatecontext(IntPtr device, int[] attributes);
        internal static d_alccreatecontext CreateContext;

        internal static AlcError GetError()
        {
            return GetErrorForDevice(IntPtr.Zero);
        }

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate AlcError d_alcgeterror(IntPtr device);
        internal static d_alcgeterror GetErrorForDevice;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alcgetintegerv(IntPtr device, int param, int size, int[] values);
        internal static d_alcgetintegerv alcGetIntegerv;

        internal static void GetInteger(IntPtr device, AlcGetInteger param, int size, int[] values)
        {
            alcGetIntegerv(device, (int)param, size, values);
        }

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_alcgetcurrentcontext();
        internal static d_alcgetcurrentcontext GetCurrentContext;

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alcmakecontextcurrent(IntPtr context);
        internal static d_alcmakecontextcurrent MakeContextCurrent;

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alcdestroycontext(IntPtr context);
        internal static d_alcdestroycontext DestroyContext;

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alcclosedevice(IntPtr device);
        internal static d_alcclosedevice CloseDevice;

        [CLSCompliant(false)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_alcopendevice(string device);
        internal static d_alcopendevice OpenDevice;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_alccaptureopendevice(string device, uint sampleRate, int format, int sampleSize);
        internal static d_alccaptureopendevice alcCaptureOpenDevice;

        internal static IntPtr CaptureOpenDevice(string device, uint sampleRate, ALFormat format, int sampleSize)
        {
            return alcCaptureOpenDevice(device, sampleRate, (int)format, sampleSize);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_alccapturestart(IntPtr device);
        internal static d_alccapturestart CaptureStart;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alccapturesamples(IntPtr device, IntPtr buffer, int samples);
        internal static d_alccapturesamples CaptureSamples;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_alccapturestop(IntPtr device);
        internal static d_alccapturestop CaptureStop;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_alccaptureclosedevice(IntPtr device);
        internal static d_alccaptureclosedevice CaptureCloseDevice;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool d_alcisextensionpresent(IntPtr device, string extensionName);
        internal static d_alcisextensionpresent IsExtensionPresent;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr d_alcgetstring(IntPtr device, int p);
        internal static d_alcgetstring alcGetString;

        internal static string GetString(IntPtr device, int p)
        {
            return Marshal.PtrToStringAnsi(alcGetString(device, p));
        }

        internal static string GetString(IntPtr device, AlcGetString p)
        {
            return GetString(device, (int)p);
        }

#if IOS
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alcsuspendcontext(IntPtr context);
        internal static d_alcsuspendcontext SuspendContext;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alcprocesscontext(IntPtr context);
        internal static d_alcprocesscontext ProcessContext;
#endif

#if ANDROID
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alcdevicepausesoft(IntPtr device);
        internal static d_alcdevicepausesoft DevicePause;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void d_alcdeviceresumesoft(IntPtr device);
        internal static d_alcdeviceresumesoft DeviceResume;
#endif
    }

    internal class XRamExtension
    {
        internal enum XRamStorage
        {
            Automatic,
            Hardware,
            Accessible
        }

        private int RamSize;
        private int RamFree;
        private int StorageAuto;
        private int StorageHardware;
        private int StorageAccessible;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool SetBufferModeDelegate(int n, ref int buffers, int value);

        private SetBufferModeDelegate setBufferMode;

        internal XRamExtension()
        {
            IsInitialized = false;
            if (!AL.IsExtensionPresent("EAX-RAM"))
            {
                return;
            }
            RamSize = AL.alGetEnumValue("AL_EAX_RAM_SIZE");
            RamFree = AL.alGetEnumValue("AL_EAX_RAM_FREE");
            StorageAuto = AL.alGetEnumValue("AL_STORAGE_AUTOMATIC");
            StorageHardware = AL.alGetEnumValue("AL_STORAGE_HARDWARE");
            StorageAccessible = AL.alGetEnumValue("AL_STORAGE_ACCESSIBLE");
            if (RamSize == 0 || RamFree == 0 || StorageAuto == 0 || StorageHardware == 0 || StorageAccessible == 0)
            {
                return;
            }
            try
            {
                setBufferMode = (XRamExtension.SetBufferModeDelegate)Marshal.GetDelegateForFunctionPointer(AL.alGetProcAddress("EAXSetBufferMode"), typeof(XRamExtension.SetBufferModeDelegate));
            }
            catch (Exception)
            {
                return;
            }
            IsInitialized = true;
        }

        internal bool IsInitialized { get; private set; }

        internal bool SetBufferMode(int i, ref int id, XRamStorage storage)
        {
            if (storage == XRamExtension.XRamStorage.Accessible)
            {
                return setBufferMode(i, ref id, StorageAccessible);
            }
            if (storage != XRamExtension.XRamStorage.Hardware)
            {
                return setBufferMode(i, ref id, StorageAuto);
            }
            return setBufferMode(i, ref id, StorageHardware);
        }
    }

    [CLSCompliant(false)]
    internal class EffectsExtension
    {
        /* Effect API */

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void alGenEffectsDelegate(int n, out uint effect);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void alDeleteEffectsDelegate(int n, ref int effect);
        //[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
        //private delegate bool alIsEffectDelegate (uint effect);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void alEffectfDelegate(uint effect, EfxEffectf param, float value);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void alEffectiDelegate(uint effect, EfxEffecti param, int value);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void alGenAuxiliaryEffectSlotsDelegate(int n, out uint effectslots);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void alDeleteAuxiliaryEffectSlotsDelegate(int n, ref int effectslots);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void alAuxiliaryEffectSlotiDelegate(uint slot, EfxEffecti type, uint effect);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void alAuxiliaryEffectSlotfDelegate(uint slot, EfxEffectSlotf param, float value);

        /* Filter API */
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void alGenFiltersDelegate(int n, [Out] uint* filters);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void alFilteriDelegate(uint fid, EfxFilteri param, int value);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void alFilterfDelegate(uint fid, EfxFilterf param, float value);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void alDeleteFiltersDelegate(int n, [In] uint* filters);


        private alGenEffectsDelegate alGenEffects;
        private alDeleteEffectsDelegate alDeleteEffects;
        //private alIsEffectDelegate alIsEffect;
        private alEffectfDelegate alEffectf;
        private alEffectiDelegate alEffecti;
        private alGenAuxiliaryEffectSlotsDelegate alGenAuxiliaryEffectSlots;
        private alDeleteAuxiliaryEffectSlotsDelegate alDeleteAuxiliaryEffectSlots;
        private alAuxiliaryEffectSlotiDelegate alAuxiliaryEffectSloti;
        private alAuxiliaryEffectSlotfDelegate alAuxiliaryEffectSlotf;
        private alGenFiltersDelegate alGenFilters;
        private alFilteriDelegate alFilteri;
        private alFilterfDelegate alFilterf;
        private alDeleteFiltersDelegate alDeleteFilters;

        internal static IntPtr device;
        static EffectsExtension _instance;
        internal static EffectsExtension Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EffectsExtension();
                return _instance;
            }
        }

        private EffectsExtension()
        {
            IsInitialized = false;
            if (!Alc.IsExtensionPresent(device, "ALC_EXT_EFX"))
            {
                return;
            }

            alGenEffects = (alGenEffectsDelegate)Marshal.GetDelegateForFunctionPointer(AL.alGetProcAddress("alGenEffects"), typeof(alGenEffectsDelegate));
            alDeleteEffects = (alDeleteEffectsDelegate)Marshal.GetDelegateForFunctionPointer(AL.alGetProcAddress("alDeleteEffects"), typeof(alDeleteEffectsDelegate));
            alEffectf = (alEffectfDelegate)Marshal.GetDelegateForFunctionPointer(AL.alGetProcAddress("alEffectf"), typeof(alEffectfDelegate));
            alEffecti = (alEffectiDelegate)Marshal.GetDelegateForFunctionPointer(AL.alGetProcAddress("alEffecti"), typeof(alEffectiDelegate));
            alGenAuxiliaryEffectSlots = (alGenAuxiliaryEffectSlotsDelegate)Marshal.GetDelegateForFunctionPointer(AL.alGetProcAddress("alGenAuxiliaryEffectSlots"), typeof(alGenAuxiliaryEffectSlotsDelegate));
            alDeleteAuxiliaryEffectSlots = (alDeleteAuxiliaryEffectSlotsDelegate)Marshal.GetDelegateForFunctionPointer(AL.alGetProcAddress("alDeleteAuxiliaryEffectSlots"), typeof(alDeleteAuxiliaryEffectSlotsDelegate));
            alAuxiliaryEffectSloti = (alAuxiliaryEffectSlotiDelegate)Marshal.GetDelegateForFunctionPointer(AL.alGetProcAddress("alAuxiliaryEffectSloti"), typeof(alAuxiliaryEffectSlotiDelegate));
            alAuxiliaryEffectSlotf = (alAuxiliaryEffectSlotfDelegate)Marshal.GetDelegateForFunctionPointer(AL.alGetProcAddress("alAuxiliaryEffectSlotf"), typeof(alAuxiliaryEffectSlotfDelegate));

            alGenFilters = (alGenFiltersDelegate)Marshal.GetDelegateForFunctionPointer(AL.alGetProcAddress("alGenFilters"), typeof(alGenFiltersDelegate));
            alFilteri = (alFilteriDelegate)Marshal.GetDelegateForFunctionPointer(AL.alGetProcAddress("alFilteri"), typeof(alFilteriDelegate));
            alFilterf = (alFilterfDelegate)Marshal.GetDelegateForFunctionPointer(AL.alGetProcAddress("alFilterf"), typeof(alFilterfDelegate));
            alDeleteFilters = (alDeleteFiltersDelegate)Marshal.GetDelegateForFunctionPointer(AL.alGetProcAddress("alDeleteFilters"), typeof(alDeleteFiltersDelegate));

            IsInitialized = true;
        }

        internal bool IsInitialized { get; private set; }

        /*

alEffecti (effect, EfxEffecti.FilterType, (int)EfxEffectType.Reverb);
            ALHelper.CheckError ("Failed to set Filter Type.");

        */

        internal void GenAuxiliaryEffectSlots(int count, out uint slot)
        {
            this.alGenAuxiliaryEffectSlots(count, out slot);
            ALHelper.CheckError("Failed to Genereate Aux slot");
        }

        internal void GenEffect(out uint effect)
        {
            this.alGenEffects(1, out effect);
            ALHelper.CheckError("Failed to Generate Effect.");
        }

        internal void DeleteAuxiliaryEffectSlot(int slot)
        {
            alDeleteAuxiliaryEffectSlots(1, ref slot);
        }

        internal void DeleteEffect(int effect)
        {
            alDeleteEffects(1, ref effect);
        }

        internal void BindEffectToAuxiliarySlot(uint slot, uint effect)
        {
            alAuxiliaryEffectSloti(slot, EfxEffecti.SlotEffect, effect);
            ALHelper.CheckError("Failed to bind Effect");
        }

        internal void AuxiliaryEffectSlot(uint slot, EfxEffectSlotf param, float value)
        {
            alAuxiliaryEffectSlotf(slot, param, value);
            ALHelper.CheckError("Failes to set " + param + " " + value);
        }

        internal void BindSourceToAuxiliarySlot(int SourceId, int slot, int slotnumber, int filter)
        {
            AL.alSource3i(SourceId, ALSourcei.EfxAuxilarySendFilter, slot, slotnumber, filter);
        }

        internal void Effect(uint effect, EfxEffectf param, float value)
        {
            alEffectf(effect, param, value);
            ALHelper.CheckError("Failed to set " + param + " " + value);
        }

        internal void Effect(uint effect, EfxEffecti param, int value)
        {
            alEffecti(effect, param, value);
            ALHelper.CheckError("Failed to set " + param + " " + value);
        }

        internal unsafe int GenFilter()
        {
            uint filter = 0;
            this.alGenFilters(1, &filter);
            return (int)filter;
        }
        internal void Filter(int sourceId, EfxFilteri filter, int EfxFilterType)
        {
            this.alFilteri((uint)sourceId, filter, EfxFilterType);
        }
        internal void Filter(int sourceId, EfxFilterf filter, float EfxFilterType)
        {
            this.alFilterf((uint)sourceId, filter, EfxFilterType);
        }
        internal void BindFilterToSource(int sourceId, int filterId)
        {
            AL.Source(sourceId, ALSourcei.EfxDirectFilter, filterId);
        }
        internal unsafe void DeleteFilter(int filterId)
        {
            alDeleteFilters(1, (uint*)&filterId);
        }
    }
}
