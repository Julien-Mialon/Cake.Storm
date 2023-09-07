#l "../src/scripts/bootstrapper.csx"

const string MODULE_VERSION = "1.0.1";

Configure()
	.UseRootDirectory("..")
	.UseBuildDirectory("build")
	.UseArtifactsDirectory("artifacts")
	.AddConfiguration(configuration => configuration
		.WithSolution("Cake.Storm.Fluent.sln")
        .WithTargetFrameworks("netstandard2.0")
		.WithBuildParameter("Configuration", "Release")
		.WithBuildParameter("Platform", "Any CPU")
		.UseDefaultTooling()
		.UseDotNetCoreTooling()
        .WithDotNetCoreOutputType(OutputType.Copy)
        .UseFilesTransformation(transformation => transformation
            .OnFile("misc/nuspecs/Cake.Storm.Fluent.Android.nuspec")
            .OnFile("misc/nuspecs/Cake.Storm.Fluent.AppCenter.nuspec")
            .OnFile("misc/nuspecs/Cake.Storm.Fluent.DotNetCore.nuspec")
            .OnFile("misc/nuspecs/Cake.Storm.Fluent.iOS.nuspec")
            .OnFile("misc/nuspecs/Cake.Storm.Fluent.NuGet.nuspec")
            .OnFile("misc/nuspecs/Cake.Storm.Fluent.nuspec")
            .OnFile("misc/nuspecs/Cake.Storm.Fluent.Transformations.nuspec")

            .Replace("{cake.storm.fluent}", MODULE_VERSION)
        )
	)
	//platforms configuration
	.AddPlatform("dotnet")
	//targets configuration
	.AddTarget("pack", configuration => configuration
        .UseCsprojTransformation(transformations => transformations.UpdatePackageVersionFromParameter())
        .UseNugetPack(nugetConfiguration => nugetConfiguration
            .WithAuthor("Julien Mialon")
            .AddAllFilesFromArtifacts("lib")
			.AddFile("../logo/icon.png")
			.WithDependenciesFromProject()
        )
	)
    .AddTarget("push", configuration => configuration
        .UseCsprojTransformation(transformations => transformations.UpdatePackageVersionFromParameter())
        .UseNugetPack(nugetConfiguration => nugetConfiguration
            .WithAuthor("Julien Mialon")
            .AddAllFilesFromArtifacts("lib")
			.AddFile("../logo/icon.png")
			.WithDependenciesFromProject()
        )
        .UseNugetPush(pushConfiguration => pushConfiguration.WithApiKeyFromEnvironment())
    )
    //applications configuration
	.AddApplication("fluent-core", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent/Cake.Storm.Fluent.csproj")
        .WithVersion(MODULE_VERSION)
        .UseNugetPack(nugetConfiguration => nugetConfiguration
            .WithNuspec("misc/nuspecs/Cake.Storm.Fluent.nuspec")
            .WithPackageId("Cake.Storm.Fluent")
            .WithReleaseNotesFile("misc/release_notes/Cake.Storm.Fluent.md")
            .AddFile("misc/scripts/Cake.Storm.Fluent.cake", "scripts")
        )
    )
    .AddApplication("fluent-android", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent.Android/Cake.Storm.Fluent.Android.csproj")
        .WithVersion(MODULE_VERSION)
        .UseNugetPack(nugetConfiguration => nugetConfiguration
            .WithNuspec("misc/nuspecs/Cake.Storm.Fluent.Android.nuspec")
            .WithPackageId("Cake.Storm.Fluent.Android")
            .WithReleaseNotesFile("misc/release_notes/Cake.Storm.Fluent.Android.md")
            .AddFile("misc/scripts/Cake.Storm.Fluent.Android.cake", "scripts")
        )
    )
    .AddApplication("fluent-dotnetcore", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent.DotNetCore/Cake.Storm.Fluent.DotNetCore.csproj")
        .WithVersion(MODULE_VERSION)
        .UseNugetPack(nugetConfiguration => nugetConfiguration
            .WithNuspec("misc/nuspecs/Cake.Storm.Fluent.DotNetCore.nuspec")
            .WithPackageId("Cake.Storm.Fluent.DotNetCore")
            .WithReleaseNotesFile("misc/release_notes/Cake.Storm.Fluent.DotNetCore.md")
            .AddFile("misc/scripts/Cake.Storm.Fluent.DotNetCore.cake", "scripts")
        )
    )
    .AddApplication("fluent-ios", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent.iOS/Cake.Storm.Fluent.iOS.csproj")
        .WithVersion(MODULE_VERSION)
        .UseNugetPack(nugetConfiguration => nugetConfiguration
            .WithNuspec("misc/nuspecs/Cake.Storm.Fluent.iOS.nuspec")
            .WithPackageId("Cake.Storm.Fluent.iOS")
            .WithReleaseNotesFile("misc/release_notes/Cake.Storm.Fluent.iOS.md")
            .AddFile("misc/scripts/Cake.Storm.Fluent.iOS.cake", "scripts")
        )
    )
    .AddApplication("fluent-transformations", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent.Transformations/Cake.Storm.Fluent.Transformations.csproj")
        .WithVersion(MODULE_VERSION)
        .UseNugetPack(nugetConfiguration => nugetConfiguration
            .WithNuspec("misc/nuspecs/Cake.Storm.Fluent.Transformations.nuspec")
            .WithPackageId("Cake.Storm.Fluent.Transformations")
            .WithReleaseNotesFile("misc/release_notes/Cake.Storm.Fluent.Transformations.md")
            .AddFile("misc/scripts/Cake.Storm.Fluent.Transformations.cake", "scripts")
        )
    )
    .AddApplication("fluent-nuget", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent.NuGet/Cake.Storm.Fluent.NuGet.csproj")
        .WithVersion(MODULE_VERSION)
        .UseNugetPack(nugetConfiguration => nugetConfiguration
            .WithNuspec("misc/nuspecs/Cake.Storm.Fluent.NuGet.nuspec")
            .WithPackageId("Cake.Storm.Fluent.NuGet")
            .WithReleaseNotesFile("misc/release_notes/Cake.Storm.Fluent.NuGet.md")
            .AddFile("misc/scripts/Cake.Storm.Fluent.NuGet.cake", "scripts")
        )
    )
    .AddApplication("fluent-appcenter", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent.AppCenter/Cake.Storm.Fluent.AppCenter.csproj")
        .WithVersion(MODULE_VERSION)
        .UseNugetPack(nugetConfiguration => nugetConfiguration
            .WithNuspec("misc/nuspecs/Cake.Storm.Fluent.AppCenter.nuspec")
            .WithPackageId("Cake.Storm.Fluent.AppCenter")
            .WithReleaseNotesFile("misc/release_notes/Cake.Storm.Fluent.AppCenter.md")
            .AddFile("misc/scripts/Cake.Storm.Fluent.AppCenter.cake", "scripts")
        )
    )
	.Build();

RunTarget(Argument("target", "help"));