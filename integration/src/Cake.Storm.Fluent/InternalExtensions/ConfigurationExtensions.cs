using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using Cake.Storm.Fluent.Common;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Models;

namespace Cake.Storm.Fluent.InternalExtensions
{
    public static class ConfigurationExtensions
    {
	    public static TValue GetSimple<TValue>(this IConfiguration configuration, string key)
	    {
		    if(configuration.TryGet<SimpleConfigurationItem<TValue>>(key, out var configurationItem))
		    {
			    return configurationItem.Value;
		    }
		    configuration.Context.CakeContext.LogAndThrow($"Key {key} does not exists");
		    return default(TValue);
	    }

	    public static string AddRootDirectory(this IConfiguration configuration, string path)
	    {
		    return configuration.GetSimple<DirectoryPath>(ConfigurationConstants.ROOT_PATH_KEY).Combine(path).FullPath;
	    }

	    public static string GetSolutionPath(this IConfiguration configuration)
	    {
		    return configuration.AddRootDirectory(configuration.GetSimple<string>(ConfigurationConstants.SOLUTION_KEY));
	    }

	    public static string GetProjectPath(this IConfiguration configuration)
	    {
			return configuration.AddRootDirectory(configuration.GetSimple<string>(ConfigurationConstants.PROJECT_KEY));
		}

		public static string GetApplicationName(this IConfiguration configuration) => configuration.GetSimple<string>(ConfigurationConstants.APPLICATION_NAME_KEY);

	    public static string GetTargetName(this IConfiguration configuration) => configuration.GetSimple<string>(ConfigurationConstants.TARGET_NAME_KEY);

	    public static string GetPlatformName(this IConfiguration configuration) => configuration.GetSimple<string>(ConfigurationConstants.PLATFORM_NAME_KEY);

		public static DirectoryPath GetArtifactsPath(this IConfiguration configuration)
	    {
		    DirectoryPath artifactsRoot = configuration.GetSimple<DirectoryPath>(ConfigurationConstants.ARTIFACTS_PATH_KEY);
		    DirectoryPath result = artifactsRoot.Combine(configuration.GetApplicationName())
			    .Combine(configuration.GetTargetName()).Combine(configuration.GetPlatformName());

			configuration.Context.CakeContext.EnsureDirectoryExists(result);
		    return result;
	    }
    }
}
