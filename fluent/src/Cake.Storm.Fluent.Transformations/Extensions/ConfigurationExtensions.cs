using System;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Transformations.Interfaces;
using Cake.Storm.Fluent.Transformations.Models;
using Cake.Storm.Fluent.Transformations.Steps;

namespace Cake.Storm.Fluent.Transformations.Extensions
{
    public static class ConfigurationExtensions
    {
	    public static TConfiguration UseCodeTransformation<TConfiguration>(this TConfiguration configuration, string sourceFile, Action<ICodeTransformation> transformerAction)
			where TConfiguration : IConfiguration
	    {
		    ICodeTransformationAction transformation = new CodeTransformation();
		    transformerAction(transformation);
		    configuration.AddStep(new CodeTransformationStep(sourceFile, transformation));
		    return configuration;
	    }
	    
	    //version with implicit projectFile, it will comes from configuration
	    public static TConfiguration UseCsprojTransformation<TConfiguration>(this TConfiguration configuration, Action<ICsprojTransformation> transformerAction)
		    where TConfiguration : IConfiguration
	    {
		    return configuration.UseCsprojTransformation(null, transformerAction);
	    }
	    
	    public static TConfiguration UseCsprojTransformation<TConfiguration>(this TConfiguration configuration, string projectFile, Action<ICsprojTransformation> transformerAction)
		    where TConfiguration : IConfiguration
	    {
		    ICsprojTransformationAction transformation = new CsprojTransformation();
		    transformerAction(transformation);
		    configuration.AddStep(new CsprojTransformationStep(projectFile, transformation));
		    return configuration;
	    }

	    public static TConfiguration UseFileTransformation<TConfiguration>(this TConfiguration configuration, string file, Action<IFileTransformation> transformerAction)
		    where TConfiguration : IConfiguration
	    {
			IFileTransformationAction transformation = new FileTransformation();
		    transformerAction(transformation);
		    configuration.AddStep(new FileTransformationStep(file, transformation));
		    return configuration;
	    }

	    public static TConfiguration UseCopyFile<TConfiguration>(this TConfiguration configuration, string sourceFile, string targetFile)
	    	where TConfiguration : IConfiguration
	    {
		    configuration.AddStep(new CopyFilesStep(sourceFile, PathItemType.File, targetFile, PathItemType.File));
		    return configuration;
	    }
	    
	    public static TConfiguration UseCopyFileToDirectory<TConfiguration>(this TConfiguration configuration, string sourceFile, string targetDirectory)
		    where TConfiguration : IConfiguration
	    {
		    configuration.AddStep(new CopyFilesStep(sourceFile, PathItemType.File, targetDirectory, PathItemType.Directory));
		    return configuration;
	    }
	    
	    public static TConfiguration UseCopyFilesToDirectory<TConfiguration>(this TConfiguration configuration, string sourceDirectory, string targetDirectory)
		    where TConfiguration : IConfiguration
	    {
		    configuration.AddStep(new CopyFilesStep(sourceDirectory, PathItemType.Directory, targetDirectory, PathItemType.Directory));
		    return configuration;
	    }
	    
	    public static TConfiguration UseCopyFilesWithPatternToDirectory<TConfiguration>(this TConfiguration configuration, string filePattern, string targetDirectory)
		    where TConfiguration : IConfiguration
	    {
		    configuration.AddStep(new CopyFilesStep(filePattern, PathItemType.Pattern, targetDirectory, PathItemType.Directory));
		    return configuration;
	    }
    }
}
