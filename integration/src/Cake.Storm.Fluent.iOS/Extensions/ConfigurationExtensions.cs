using System;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.iOS.Interfaces;
using Cake.Storm.Fluent.iOS.Models;
using Cake.Storm.Fluent.iOS.Steps;

namespace Cake.Storm.Fluent.iOS.Extensions
{
    public static class ConfigurationExtensions
    {
	    public static TConfiguration UsePListTransformation<TConfiguration>(this TConfiguration configuration, string sourceFile, Action<IPListTransformation> transformerAction)
			where TConfiguration : IConfiguration
	    {
		    IPListTransformation transformation = new PListTransformation();
		    transformerAction(transformation);
			configuration.AddStep(new PListTransformationStep(sourceFile, transformation));
		    return configuration;
	    }
    }
}
