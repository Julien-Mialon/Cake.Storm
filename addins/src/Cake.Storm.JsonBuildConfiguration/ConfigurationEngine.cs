using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Storm.JsonBuildConfiguration.Models;

namespace Cake.Storm.JsonBuildConfiguration
{
	public class ConfigurationEngine
	{
		private RootConfiguration _configuration;
		private ICakeContext _context;

		internal ConfigurationEngine(ICakeContext context, RootConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}

		public AndroidConfiguration GetAndroidConfiguration(string appName, string targetName)
		{
			PlatformConfiguration platform = _configuration.Platforms.FirstOrDefault(x => x.Name == PlatformType.Android);
			TargetConfiguration target = _configuration.Targets.FirstOrDefault(x => x.Name == targetName);
			AppConfiguration app = _configuration.Apps.FirstOrDefault(x => x.Name == appName);

			if (app == null)
			{
				_context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"JsonConfiguration: app {appName} not found");
				throw new CakeException($"JsonConfiguration: app {appName} not found");
			}

			TargetConfiguration appTarget = app.Targets.FirstOrDefault(x => x.Name == targetName);

			if (appTarget == null)
			{
				_context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"JsonConfiguration: target {targetName} not found for app {appName}");
				throw new CakeException($"JsonConfiguration: target {targetName} not found for app {appName}");
			}


		}
	}
}
