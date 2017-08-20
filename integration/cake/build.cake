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
	.AddTarget("nuget", configuration => configuration
        .UseCsprojTransformation(transformations => transformations
            .UpdatePackageVersionFromParameter()
        )
	)
    //applications configuration
	.AddApplication("fluent-core", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent/Cake.Storm.Fluent.csproj")
        .WithVersion("0.1.0")
        .UseNugetPack(nugetConfiguration => nugetConfiguration
            .WithNuspec("nuspecs/Cake.Storm.Fluent.nuspec")
            .WithPackageId("Cake.Storm.Fluent")
            .WithAuthor("Julien Mialon")
            .WithReleaseNotesFile("release_notes/Cake.Storm.Fluent.md")
            .AddFile("scripts/Cake.Storm.Fluent.cake", "scripts")
        )
    )
    .AddApplication("fluent-android", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent.Android/Cake.Storm.Fluent.Android.csproj")
        .WithVersion("0.9.0")
    )
    .AddApplication("fluent-dotnetcore", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent.DotNetCore/Cake.Storm.Fluent.DotNetCore.csproj")
        .WithVersion("0.9.0")
    )
    .AddApplication("fluent-ios", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent.iOS/Cake.Storm.Fluent.iOS.csproj")
        .WithVersion("0.9.0")
    )
    .AddApplication("fluent-transformations", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent.Transformations/Cake.Storm.Fluent.Transformations.csproj")
        .WithVersion("0.9.0")
    )
	.Build();

RunTarget(Argument("target", "help"));