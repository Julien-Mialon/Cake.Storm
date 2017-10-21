using Cake.Common;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.NuGet.Common;
using Cake.Storm.Fluent.NuGet.Interfaces;
using Cake.Storm.Fluent.Resolvers;

namespace Cake.Storm.Fluent.NuGet.Extensions
{
	public static class NugetPushConfigurationExtensions
	{
		public static INugetPushConfiguration WithSource(this INugetPushConfiguration configuration, string source)
		{
			configuration.Configuration.AddSimple(NuGetConstants.NUGET_PUSH_SOURCE_KEY, source);
			return configuration;
		}
		
		public static INugetPushConfiguration WithApiKey(this INugetPushConfiguration configuration, string apiKey)
		{
			configuration.Configuration.AddSimple(NuGetConstants.NUGET_PUSH_APIKEY_KEY, ValueResolver.FromConstant(apiKey));
			return configuration;
		}

		public static INugetPushConfiguration WithApiKeyFromArgument(this INugetPushConfiguration configuration, string argumentName = "nugetApiKey")
		{
			configuration.Configuration.AddSimple(NuGetConstants.NUGET_PUSH_APIKEY_KEY, ValueResolver.FromArgument<string>(argumentName));
			return configuration;
		}
		
		public static INugetPushConfiguration WithApiKeyFromEnvironment(this INugetPushConfiguration configuration, string environmentVariable = "NUGET_API_KEY")
		{
			configuration.Configuration.AddSimple(NuGetConstants.NUGET_PUSH_APIKEY_KEY, ValueResolver.FromEnvironment<string>(environmentVariable));
			return configuration;
		}
	}
}