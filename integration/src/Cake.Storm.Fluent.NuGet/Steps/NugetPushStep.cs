using Cake.Common.Tools.NuGet;
using Cake.Common.Tools.NuGet.Push;
using Cake.Core;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.NuGet.Common;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.NuGet.Steps
{
	[DeployStep]
	public class NugetPushStep : IStep
	{
		public void Execute(IConfiguration configuration)
		{
			if (!configuration.TryGetSimple(NuGetConstants.NUGET_PACK_OUTPUT_FILE_KEY, out string nugetPackageFile))
			{
				configuration.LogAndThrow("Unable to find package to push");
			}

			configuration.FileExistsOrThrow(nugetPackageFile);

			if (!configuration.TryGetSimple(NuGetConstants.NUGET_PUSH_SOURCE_KEY, out string nugetSource))
			{
				nugetSource = "https://www.nuget.org/api/v2/package";
			}

			if (!configuration.TryGetSimple(NuGetConstants.NUGET_PUSH_APIKEY_KEY, out string nugetApiKey)
			    || nugetApiKey == NuGetConstants.ENVIRONMENT_PARAMETER)
			{
				nugetApiKey = configuration.Context.CakeContext.Environment.GetEnvironmentVariable("NUGET_API_KEY");
				if (string.IsNullOrEmpty(nugetApiKey))
				{
					configuration.LogAndThrow("Unable to find nuget api key in environment variable NUGET_API_KEY");
				}
			}
			
			if (string.IsNullOrEmpty(nugetApiKey))
			{
				configuration.LogAndThrow("Empty value for nuget api key");
			}
			
			configuration.Context.CakeContext.NuGetPush(nugetPackageFile, new NuGetPushSettings
			{
				Source = nugetSource,
				ApiKey = nugetApiKey
			});
		}
	}
}