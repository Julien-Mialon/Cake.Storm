using System;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Models;

namespace Cake.Storm.Fluent.Extensions
{
    public static class ApplicationConfigurationExtensions
    {
	    public static TConfiguration UseTarget<TConfiguration>(this TConfiguration configuration, string name, Action<ITargetConfiguration> action)
		    where TConfiguration : IApplicationConfiguration
	    {
		    ITargetConfiguration targetConfiguration = new TargetConfiguration(configuration.Context);
		    action(targetConfiguration);
		    configuration.AddTarget(name, targetConfiguration);

		    return configuration;
	    }
	}
}
