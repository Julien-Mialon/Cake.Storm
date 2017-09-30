using System;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Models;

namespace Cake.Storm.Fluent.Extensions
{
    public static class ApplicationConfigurationExtensions
    {
	    public static TConfiguration UseTarget<TConfiguration>(this TConfiguration configuration, string name, Action<ITargetConfiguration> action = null)
		    where TConfiguration : IApplicationConfiguration
	    {
		    ITargetConfiguration targetConfiguration = new TargetConfiguration(configuration.Context);
		    action?.Invoke(targetConfiguration);
		    configuration.AddTarget(name, targetConfiguration);

		    return configuration;
	    }
	}
}
