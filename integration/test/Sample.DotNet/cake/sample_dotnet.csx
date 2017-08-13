
Configure() //this must be embedded in a script to pass needed parameters (like Task delegate to create new entry points)
			//generic configuration
	.UseRootDirectory("..")
	.UseBuildDirectory("build")
	.UseArtifactsDirectory("artifacts")
	.AddConfiguration(configuration => configuration
		.WithSolution("Sample.DotNet.sln")
		.WithBuildParameter("Configuration", "Release")
		.UseDefaultTooling()
	)
	//platforms configuration
	.AddPlatform("dotnet", configuration => configuration
		.WithProject("Sample.DotNet/Sample.DotNet.csproj")
		.UseDotNetCoreTooling()
	)
	.AddPlatform("iOS", configuration => configuration
		.WithProject("Sample.DotNet/Sample.DotNet.csproj")
		.UseiOSTooling()
	)
	.AddPlatform("android", configuration => configuration
		.WithProject("Sample.Android/Sample.Android.csproj")
		.UseAndroidTooling()
	)
	//targets configuration
	.AddTarget("dev", configuration => configuration
		.WithBuildParameter("Platform", "Any CPU")
		.UseCodeTransformation("Sample.DotNet/Program.cs", transformations => transformations
			.UpdateVariable("TEXT", "Hello world from (dev) build")
			.UpdateVariable("NUMBER", 73)
		)
	)
	.AddApplication("sample")
	.Build();

RunTarget(Argument("target", "help"));