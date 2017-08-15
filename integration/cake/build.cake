#l "../src/scripts/bootstrapper.csx"

Configure()
	.UseRootDirectory("..")
	.UseBuildDirectory("build")
	.UseArtifactsDirectory("artifacts")
	.AddConfiguration(configuration => configuration
		.WithSolution("Cake.Storm.Fluent.sln")
		.WithBuildParameter("Configuration", "Release")
		.UseDefaultTooling()
	)
	//platforms configuration
	.AddPlatform("dotnet", configuration => configuration
		.UseDotNetCoreTooling()
	)
	//targets configuration
	.AddTarget("nuget", configuration => configuration
		.WithBuildParameter("Platform", "Any CPU")
		.UseCodeTransformation("Sample.DotNet/Program.cs", transformations => transformations
			.UpdateVariable("TEXT", "Hello world from (dev) build")
			.UpdateVariable("NUMBER", 73)
		)
	)
    //applications configuration
	.AddApplication("fluent-core", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent/Cake.Storm.Fluent.csproj")
    )
    .AddApplication("fluent-android", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent.Android/Cake.Storm.Fluent.Android.csproj")
    )
    .AddApplication("fluent-dotnetcore", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent.DotNetCore/Cake.Storm.Fluent.DotNetCore.csproj")
    )
    .AddApplication("fluent-ios", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent.iOS/Cake.Storm.Fluent.iOS.csproj")
    )
    .AddApplication("fluent-transformations", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent.Transformations/Cake.Storm.Fluent.Transformations.csproj")
    )
	.Build();

RunTarget(Argument("target", "help"));