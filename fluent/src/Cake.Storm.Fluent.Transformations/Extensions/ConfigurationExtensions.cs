﻿using System;
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
    }
}
