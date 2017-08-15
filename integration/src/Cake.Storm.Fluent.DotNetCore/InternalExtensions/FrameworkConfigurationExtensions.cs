using System;
using System.Linq;
using Cake.Storm.Fluent.DotNetCore.Common;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Models;

namespace Cake.Storm.Fluent.DotNetCore.InternalExtensions
{
	internal static class FrameworkConfigurationExtensions
	{
		public static void RunOnConfiguredTargetFramework(this IConfiguration configuration, Action<string> onFramework)
		{
			if (configuration.Has(DotNetCoreConstants.TARGET_FRAMEWORK_KEY))
			{
				ListConfigurationItem<string> frameworks = configuration.Get<ListConfigurationItem<string>>(DotNetCoreConstants.TARGET_FRAMEWORK_KEY);

				foreach (var framework in frameworks.Values.Distinct())
				{
					onFramework(framework);
				}
			}
			else
			{
				onFramework(null);
			}
		}
	}
}