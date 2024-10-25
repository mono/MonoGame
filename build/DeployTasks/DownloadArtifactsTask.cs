﻿
namespace BuildScripts;

[TaskName("DownloadArtifacts")]
public sealed class DownloadArtifactsTask : AsyncFrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) => context.BuildSystem().IsRunningOnGitHubActions;

    public override async Task RunAsync(BuildContext context)
    {
        context.CreateDirectory("nugets");
        foreach (var os in new[] { "windows", "macos", "linux" })
            await context.GitHubActions().Commands.DownloadArtifact($"nuget-{os}", "nugets");

        context.CreateDirectory("vsix");
        await context.GitHubActions().Commands.DownloadArtifact("MonoGame.Templates.VSExtension.vsix", "vsix");

        // Confirm for testing purposes the fil is there.
        context.Information("Files in 'vsix' directory:");
        foreach (var file in context.GetFiles("vsix/*"))
        {
            context.Information(file.FullPath);
        }
    }
}
