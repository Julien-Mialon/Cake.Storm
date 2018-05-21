<p align="center"><img src="logo/horizontalversion.png" alt="Cake.Storm" height="150px"></p>

# Cake Fluent
Cake is a wonderful tool to build .Net project (or others) using a DSL in C#.
But most of the time, I copy/paste the same scripts on all project with minor changes for configuration (solution path, project path...).
Here comes the Cake.Storm.Fluent package, the goal : only write configuration, it will generate everything else.

The base package only contains configuration to handle tasks generation and orchestration for Cake, you will probably need some other addin for your specific need.

Fluent addins : 
- Cake.Storm.Fluent.Android : Xamarin android tooling
- Cake.Storm.Fluent.iOS : Xamarin iOS tooling
- Cake.Storm.Fluent.NuGet : create and publish nuget packages from your projects
- Cake.Storm.Fluent.DotNetCore : build .net project using dotnetcore tools
- Cake.Storm.Fluent.Transformations : update source files before building (eg: change some variables values, no more #if DEBUG with many build configurations)

## Remarks

All addins must be loaded using a load instruction instead of addin because they add a small scripts to load necessary tooling and import namespaces in cake.
```
#load "nuget:?package=Cake.Storm.Fluent.XYZ"
```

## Sample

Here comes a sample script for building, packaging and publishing a .Net project on nuget
```csharp
#load "nuget:?package=Cake.Storm.Fluent"
#load "nuget:?package=Cake.Storm.Fluent.DotNetCore"
#load "nuget:?package=Cake.Storm.Fluent.NuGet"

Configure()
	.UseRootDirectory("..")
	.UseBuildDirectory("build")
	.UseArtifactsDirectory("artifacts")
	.AddConfiguration(configuration => configuration
		.WithSolution("Cake.Storm.Fluent.Sample.sln")
        .WithTargetFrameworks("net46", "netstandard1.6")
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
        .UseNugetPack(nugetConfiguration => nugetConfiguration.WithAuthor("Julien Mialon"))
	)
    .AddTarget("push", configuration => configuration
        .UseNugetPack(nugetConfiguration => nugetConfiguration.WithAuthor("Julien Mialon"))
        .UseNugetPush()
    )
    //applications configuration
	.AddApplication("fluent-sample", configuration => configuration
        .WithProject("src/Cake.Storm.Fluent.Sample/Cake.Storm.Fluent.Sample.csproj")
        .WithVersion("1.0.0")
        .UseNugetPack(nugetConfiguration => nugetConfiguration
            .WithNuspec("Cake.Storm.Fluent.Sample.nuspec")
            .WithPackageId("Cake.Storm.Fluent.Sample")
            .WithReleaseNotesFile("Cake.Storm.Fluent.Sample.md")
        )
    )
	.Build();

RunTarget(Argument("target", "help"));
```

## Does all of this really work !?

Does it works: yes, I'm using it to deploy all packages in this repository.


Does it fit all your needs: if you are using really standard things, it could, if you have custom process, probably not. It's up to you to write another addins to fit all your needs ;)
