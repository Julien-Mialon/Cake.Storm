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
			ICodeTransformation transformation = new CodeTransformation();
		    transformerAction(transformation);
		    configuration.AddStep(new CodeTransformationStep(sourceFile, transformation));
		    return configuration;
	    }
    }
}
