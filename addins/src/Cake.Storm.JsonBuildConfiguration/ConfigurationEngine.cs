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

		public List<string> GetPlatforms()
		{
			return _configuration.Platforms.Select(x => x.Name.ToString().ToLower()).ToList();
		}

		public List<string> GetApps()
		{
			return _configuration.Apps.Select(x => x.Name).ToList();
		}

		public List<string> GetTargets(string appName)
		{
			return _configuration.Apps.FirstOrDefault(x => x.Name == appName)?.Targets.Select(x => x.Name).ToList();
		}

		public AndroidBuildConfiguration GetAndroid(string appName, string targetName)
		{
			return GetBuildConfiguration(PlatformType.Android, appName, targetName) as AndroidBuildConfiguration;
		}

		public iOSBuildConfiguration GetiOS(string appName, string targetName)
		{
			return GetBuildConfiguration(PlatformType.iOS, appName, targetName) as iOSBuildConfiguration;
		}

		public DotNetBuildConfiguration GetDotNet(string appName, string targetName)
		{
			return GetBuildConfiguration(PlatformType.Dotnet, appName, targetName) as DotNetBuildConfiguration;
		}

		private Configuration GetBuildConfiguration(PlatformType platformType, string appName, string targetName)
		{
			// all three methods will throw exception if any is not found
			PlatformConfiguration platform = GetPlatform(platformType);
			TargetConfiguration target = GetTarget(targetName);
			AppConfiguration app = GetApp(appName);

			platform = MergePlatformConfiguration(platform, target, app);
			target = MergeTargetConfiguration(target, app);

			BuildConfiguration build = MergeBuildConfiguration(_configuration.Build, platform, target, app);

			switch (platformType)
			{
				case PlatformType.Android:
					return new AndroidBuildConfiguration(build, platform, target, app);
				case PlatformType.iOS:
					return new iOSBuildConfiguration(build, platform, target, app);
				case PlatformType.Dotnet:
					return new DotNetBuildConfiguration(build, platform, target, app);
				case PlatformType.Undefined:
				default:
					throw new ArgumentOutOfRangeException(nameof(platformType));
			}
		}

		#region Private tool methods

		private BuildConfiguration MergeBuildConfiguration(BuildConfiguration rootBuild, PlatformConfiguration rootPlatform, TargetConfiguration rootTarget, AppConfiguration app)
		{
			List<BuildConfiguration> builds = new List<BuildConfiguration>
			{
				rootBuild,
				rootPlatform.Build,
				rootTarget.Build,
				rootTarget.Platforms?.FirstOrDefault(x => x.Name == rootPlatform.Name)?.Build,
				app.Build,
				app.Platforms?.FirstOrDefault(x => x.Name == rootPlatform.Name)?.Build,
				app.Targets?.FirstOrDefault(x => x.Name == rootTarget.Name)?.Build,
				app.Targets?.FirstOrDefault(x => x.Name == rootTarget.Name)?.Platforms?.FirstOrDefault(x => x.Name == rootPlatform.Name)?.Build
			};

			BuildConfiguration conf = builds.First();
			foreach (BuildConfiguration build in builds.Skip(1))
			{
				conf = BuildConfiguration.Merge(conf, build);
			}

			return conf;
		}

		private TargetConfiguration MergeTargetConfiguration(TargetConfiguration rootTarget, AppConfiguration app)
		{
			return TargetConfiguration.Merge(rootTarget, app.Targets?.FirstOrDefault(x => x.Name == rootTarget.Name));
		}

		private PlatformConfiguration MergePlatformConfiguration(PlatformConfiguration rootPlatform, TargetConfiguration rootTarget, AppConfiguration app)
		{
			List<PlatformConfiguration> platforms = new List<PlatformConfiguration>
			{
				rootPlatform,
				rootTarget.Platforms?.FirstOrDefault(x => x.Name == rootPlatform.Name),
				app.Platforms?.FirstOrDefault(x => x.Name == rootPlatform.Name),
				app.Targets?.FirstOrDefault(x => x.Name == rootTarget.Name)?.Platforms?.FirstOrDefault(x => x.Name == rootPlatform.Name)
			};

			PlatformConfiguration conf = platforms.FirstOrDefault();
			foreach (PlatformConfiguration platform in platforms.Skip(1))
			{
				conf = PlatformConfiguration.Merge(conf, platform);
			}

			return conf;
		}

		private PlatformConfiguration GetPlatform(PlatformType platformType)
		{
			PlatformConfiguration platform = _configuration.Platforms.FirstOrDefault(x => x.Name == platformType);

			if (platform == null)
			{
				_context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"JsonConfiguration: platform {platformType} not found at root level");
				throw new CakeException($"JsonConfiguration: platform {platformType} not found at root level");
			}

			return platform;
		}

		private TargetConfiguration GetTarget(string targetName)
		{
			TargetConfiguration target = _configuration.Targets.FirstOrDefault(x => x.Name == targetName);

			if (target == null)
			{
				_context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"JsonConfiguration: target {targetName} not found at root level");
				throw new CakeException($"JsonConfiguration: target {targetName} not found at root level");
			}

			return target;
		}

		private AppConfiguration GetApp(string appName)
		{
			AppConfiguration app = _configuration.Apps.FirstOrDefault(x => x.Name == appName);

			if (app == null)
			{
				_context.Log.Write(Verbosity.Quiet, LogLevel.Error, $"JsonConfiguration: app {appName} not found");
				throw new CakeException($"JsonConfiguration: app {appName} not found");
			}

			return app;
		}

		#endregion
	}
}
