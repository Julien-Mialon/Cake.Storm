#l "../src/scripts/bootstrapper.csx"

Configure()
	.UseRootDirectory("..")
	.UseBuildDirectory("build")
	.UseArtifactsDirectory("artifacts")
	.AddConfiguration(configuration => configuration
		.WithSolution("Cake.Storm.Fluent.sln")
        .WithTargetFrameworks("net45", "net46", "netstandard1.6")
		.WithBuildParameter("Configuration", "Release")
		.WithBuildParameter("Platform", "Any CPU")
		.UseDefaultTooling()
		.UseDotNetCoreTooling()
        .WithDotNetCoreOutputType(OutputType.Copy)
	)
	//platforms configuration
	.AddPlatform("dotnet")
	//targets configuration
	.AddTarget("pack", configuration => configuration
        .UseCsprojTransformation(transformations => transformations.UpdatePackageVersionFromParameter())
        .UseNugetPack(nugetConfiguration => nugetConfiguration.WithAuthor("Julien Mialon"))
	)
    .AddTarget("push", configuration => configuration
        .UseCsprojTransformation(transformations => transformations.UpdatePackageVersionFromParameter())
        .UseNugetPack(nugetConfiguration => nugetConfiguration.WithAuthor("Julien Mialon"))
        .UseNugetPush()
    )
    //applications configuration
	.AddApplication("fluent-core", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent/Cake.Storm.Fluent.csproj")
        .WithVersion("0.1.0")
        .UseNugetPack(nugetConfiguration => nugetConfiguration
            .WithNuspec("nuspecs/Cake.Storm.Fluent.nuspec")
            .WithPackageId("Cake.Storm.Fluent")
            .WithReleaseNotesFile("release_notes/Cake.Storm.Fluent.md")
            .AddFile("scripts/Cake.Storm.Fluent.cake", "scripts")
        )
    )
    .AddApplication("fluent-android", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent.Android/Cake.Storm.Fluent.Android.csproj")
        .WithVersion("0.1.0")
        .UseNugetPack(nugetConfiguration => nugetConfiguration
            .WithNuspec("nuspecs/Cake.Storm.Fluent.Android.nuspec")
            .WithPackageId("Cake.Storm.Fluent.Android")
            .WithReleaseNotesFile("release_notes/Cake.Storm.Fluent.Android.md")
            .AddFile("scripts/Cake.Storm.Fluent.Android.cake", "scripts")
        )
    )
    .AddApplication("fluent-dotnetcore", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent.DotNetCore/Cake.Storm.Fluent.DotNetCore.csproj")
        .WithVersion("0.1.0")
        .UseNugetPack(nugetConfiguration => nugetConfiguration
            .WithNuspec("nuspecs/Cake.Storm.Fluent.DotNetCore.nuspec")
            .WithPackageId("Cake.Storm.Fluent.DotNetCore")
            .WithReleaseNotesFile("release_notes/Cake.Storm.Fluent.DotNetCore.md")
            .AddFile("scripts/Cake.Storm.Fluent.DotNetCore.cake", "scripts")
        )
    )
    .AddApplication("fluent-ios", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent.iOS/Cake.Storm.Fluent.iOS.csproj")
        .WithVersion("0.1.0")
        .UseNugetPack(nugetConfiguration => nugetConfiguration
            .WithNuspec("nuspecs/Cake.Storm.Fluent.iOS.nuspec")
            .WithPackageId("Cake.Storm.Fluent.iOS")
            .WithReleaseNotesFile("release_notes/Cake.Storm.Fluent.iOS.md")
            .AddFile("scripts/Cake.Storm.Fluent.iOS.cake", "scripts")
        )
    )
    .AddApplication("fluent-transformations", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent.Transformations/Cake.Storm.Fluent.Transformations.csproj")
        .WithVersion("0.1.0")
        .UseNugetPack(nugetConfiguration => nugetConfiguration
            .WithNuspec("nuspecs/Cake.Storm.Fluent.Transformations.nuspec")
            .WithPackageId("Cake.Storm.Fluent.Transformations")
            .WithReleaseNotesFile("release_notes/Cake.Storm.Fluent.Transformations.md")
            .AddFile("scripts/Cake.Storm.Fluent.Transformations.cake", "scripts")
        )
    )
	.Build();

RunTarget(Argument("target", "help"));