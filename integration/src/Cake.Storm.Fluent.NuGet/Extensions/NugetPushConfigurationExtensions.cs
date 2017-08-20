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
			configuration.Configuration.AddSimple(NuGetConstants.NUGET_PUSH_APIKEY_KEY, apiKey);
			return configuration;
		}
		
		public static INugetPushConfiguration WithApiKeyFromEnvironment(this INugetPushConfiguration configuration)
		{
			configuration.Configuration.AddSimple(NuGetConstants.NUGET_PUSH_APIKEY_KEY, NuGetConstants.ENVIRONMENT_PARAMETER);
			return configuration;
		}
	}
}