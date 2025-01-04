using System.Diagnostics;

namespace BuildScripts;

[TaskName("Build Native DesktopVK")]
public sealed class BuildNativeDesktopVKTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (context.IsRunningOnWindows())
            BuildWindows(context);
        else if (context.IsRunningOnLinux())
            BuildLinux(context);
        else if (context.IsRunningOnMacOs())
            BuildMacOS(context);
        else
            throw new PlatformNotSupportedException("Current platform is not supported for native builds");
    }

    private void BuildWindows(BuildContext context)
    {
        int exit;

        {
            var buildDir = "native/monogame/external/sdl2/sdl/build";
            context.CreateDirectory(buildDir);

            exit = context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildDir, Arguments = "-A x64 -D CMAKE_MSVC_RUNTIME_LIBRARY=MultiThreaded ../" });
            if (exit != 0)
                throw new Exception($"SDL2 Cmake failed! {exit}");

            exit = context.StartProcess("msbuild", new ProcessSettings { WorkingDirectory = buildDir, Arguments = "SDL2.sln /p:Configuration=Release /p:Platform=x64" });
            if (exit != 0)
                throw new Exception($"SDL2 build failed! {exit}");
        }

        // Generate the native projects.
        exit = context.StartProcess("premake5", new ProcessSettings { WorkingDirectory = "native/monogame", Arguments = "clean" });
        if (exit != 0)
            throw new Exception($"Premake clean failed! {exit}");
        exit = context.StartProcess("premake5", new ProcessSettings { WorkingDirectory = "native/monogame", Arguments = "--os=windows --verbose vs2022" });
        if (exit != 0)
            throw new Exception($"Premake generation failed! {exit}");

        // Build it.
        exit = context.StartProcess("msbuild", new ProcessSettings { WorkingDirectory = "native/monogame", Arguments = "monogame.sln /p:Configuration=Release /p:Platform=x64" });
        if (exit != 0)
            throw new Exception($"Native build failed! {exit}");
    }

    private void BuildLinux(BuildContext context)
    {
        int exit;

        // Get number of processors for parallel build
        int processorCount;
        try
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "nproc",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            using var process = Process.Start(processInfo);
            var result = process?.StandardOutput.ReadToEnd().Trim();
            process?.WaitForExit();
            processorCount = int.TryParse(result, out int count) ? count : 1;
        }
        catch
        {
            processorCount = 1;
        }

        // Build SDL2 first (keeping the CMake build for SDL2)
        {
            var sdlBuildDir = "native/monogame/external/sdl2/sdl/build";
            context.CreateDirectory(sdlBuildDir);

            context.Log.Information($"Building SDL2 in {sdlBuildDir}");

            exit = context.StartProcess("cmake", new ProcessSettings
            {
                WorkingDirectory = sdlBuildDir,
                Arguments = "-DCMAKE_BUILD_TYPE=Release -DCMAKE_POSITION_INDEPENDENT_CODE=ON ../"
            });
            if (exit != 0)
                throw new Exception($"SDL2 Cmake failed! {exit}");

            exit = context.StartProcess("make", new ProcessSettings
            {
                WorkingDirectory = sdlBuildDir,
                Arguments = $"-j{processorCount}"
            });
            if (exit != 0)
                throw new Exception($"SDL2 build failed! {exit}");
        }

        // Generate and build native project using Premake (similar to Windows and macOS)
        context.Log.Information("Generating native project with Premake...");

        // Use the existing GenerateNativeProject helper method
        GenerateNativeProject(context, "linux");

        // Set environment variables for the build
        var buildSettings = new ProcessSettings
        {
            WorkingDirectory = "native/monogame",
            Arguments = $"config=release -j{processorCount}"
        };

        // Add Vulkan SDK to environment if available
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("VULKAN_SDK")))
        {
            buildSettings.EnvironmentVariables = new Dictionary<string, string>
            {
                { "VULKAN_SDK", Environment.GetEnvironmentVariable("VULKAN_SDK") ?? "" }
            };
        }

        // Build the project
        context.Log.Information("Building native project...");
        exit = context.StartProcess("make", buildSettings);
        if (exit != 0)
            throw new Exception($"Native build failed! {exit}");

        context.Log.Information("Native build completed successfully.");
    }

    private void BuildMacOS(BuildContext context)
    {
        int exit;

        // Build SDL2
        {
            var buildDir = "native/monogame/external/sdl2/sdl/build";
            context.CreateDirectory(buildDir);

            // Build for x64
            exit = context.StartProcess("cmake", new ProcessSettings {
                WorkingDirectory = buildDir,
                Arguments = "-DCMAKE_BUILD_TYPE=Release -DCMAKE_OSX_ARCHITECTURES=x86_64 ../"
            });
            if (exit != 0)
                throw new Exception($"SDL2 Cmake (x64) failed! {exit}");

            exit = context.StartProcess("make", new ProcessSettings {
                WorkingDirectory = buildDir
            });
            if (exit != 0)
                throw new Exception($"SDL2 build (x64) failed! {exit}");

            // Clean and rebuild for ARM64
            context.CleanDirectory(buildDir);

            exit = context.StartProcess("cmake", new ProcessSettings {
                WorkingDirectory = buildDir,
                Arguments = "-DCMAKE_BUILD_TYPE=Release -DCMAKE_OSX_ARCHITECTURES=arm64 ../"
            });
            if (exit != 0)
                throw new Exception($"SDL2 Cmake (ARM64) failed! {exit}");

            exit = context.StartProcess("make", new ProcessSettings {
                WorkingDirectory = buildDir
            });
            if (exit != 0)
                throw new Exception($"SDL2 build (ARM64) failed! {exit}");

            // Create universal binary
            exit = context.StartProcess("lipo", new ProcessSettings {
                WorkingDirectory = buildDir,
                Arguments = "-create -output libSDL2.dylib x86_64/libSDL2.dylib arm64/libSDL2.dylib"
            });
            if (exit != 0)
                throw new Exception($"SDL2 universal binary creation failed! {exit}");
        }

        // Generate and build native project
        GenerateNativeProject(context, "macosx");

        // Build for both architectures and create universal binary
        exit = context.StartProcess("make", new ProcessSettings {
            WorkingDirectory = "native/monogame",
            Arguments = "config=release_x64"
        });
        if (exit != 0)
            throw new Exception($"Native build (x64) failed! {exit}");

        exit = context.StartProcess("make", new ProcessSettings {
            WorkingDirectory = "native/monogame",
            Arguments = "config=release_arm64"
        });
        if (exit != 0)
            throw new Exception($"Native build (ARM64) failed! {exit}");

        // Create universal binary for the final library
        exit = context.StartProcess("lipo", new ProcessSettings {
            WorkingDirectory = "native/monogame/bin",
            Arguments = "-create -output monogame.native.dylib x86_64/monogame.native.dylib arm64/monogame.native.dylib"
        });
        if (exit != 0)
            throw new Exception($"Native universal binary creation failed! {exit}");
    }

    private void GenerateNativeProject(BuildContext context, string os)
    {
        context.Log.Information($"Cleaning previous premake files for {os}...");
        int exit = context.StartProcess("premake5", new ProcessSettings {
            WorkingDirectory = "native/monogame",
            Arguments = "clean"
        });
        if (exit != 0)
            throw new Exception($"Premake clean failed! {exit}");

        context.Log.Information($"Generating new premake files for {os}...");
        exit = context.StartProcess("premake5", new ProcessSettings {
            WorkingDirectory = "native/monogame",
            Arguments = $"--os={os} --verbose gmake2"
        });
        if (exit != 0)
            throw new Exception($"Premake generation failed! {exit}");

        // Print the generated makefile configuration
        if (System.IO.File.Exists("native/monogame/Makefile"))
        {
            context.Log.Information("Generated Makefile contents:");
            context.Log.Information(System.IO.File.ReadAllText("native/monogame/Makefile").Substring(0, 500) + "..."); // Show first 500 chars
        }
    }
}
