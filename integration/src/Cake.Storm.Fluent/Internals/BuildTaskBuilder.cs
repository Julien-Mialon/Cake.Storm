using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Storm.Fluent.Common;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Models;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.Internals
{
	public class BuildTaskBuilder : IBuilder
	{
		private readonly BuilderParameters _parameters;

		private class Target
		{
			public Target(string applicationName, string targetName, string platformName, IConfiguration configuration)
			{
				ApplicationName = applicationName;
				TargetName = targetName;
				PlatformName = platformName;
				Configuration = configuration;
				Configuration.Add(ConfigurationConstants.PLATFORM_NAME_KEY, new SimpleConfigurationItem<string>(PlatformName));
				Configuration.Add(ConfigurationConstants.TARGET_NAME_KEY, new SimpleConfigurationItem<string>(targetName));
				Configuration.Add(ConfigurationConstants.APPLICATION_NAME_KEY, new SimpleConfigurationItem<string>(applicationName));
			}

			public string ApplicationName { get; }

			public string TargetName { get; }

			public string PlatformName { get; }

			public IConfiguration Configuration { get; }

			public string BuildTaskName => $"build-{ApplicationName}-{TargetName}-{PlatformName}";

			public string ReleaseTaskName => $"release-{ApplicationName}-{TargetName}-{PlatformName}";
		}

		public BuildTaskBuilder(BuilderParameters parameters)
		{
			_parameters = parameters;
		}

		public void Build()
		{
			List<Target> targets = MergeTargets();

			//generate application-target-platform
			foreach (Target target in targets)
			{
				//generate application-target-platform
				_parameters.Context.Task(target.BuildTaskName).Does(() =>
				{
					foreach (var preBuild in target.Configuration.StepsOf<PreBuildStepAttribute>())
					{
						preBuild.Execute(target.Configuration);
					}

					foreach (var build in target.Configuration.StepsOf<BuildStepAttribute>())
					{
						build.Execute(target.Configuration);
					}

					foreach (var postBuild in target.Configuration.StepsOf<PostBuildStepAttribute>())
					{
						postBuild.Execute(target.Configuration);
					}
				});

				_parameters.Context.Task(target.ReleaseTaskName)
					.IsDependentOn(CleanTaskBuilder.TASK_NAME)
					.IsDependentOn(target.BuildTaskName)
					.Does(() =>
					{
						foreach (var preRelease in target.Configuration.StepsOf<PreReleaseStepAttribute>())
						{
							preRelease.Execute(target.Configuration);
						}

						foreach (var release in target.Configuration.StepsOf<ReleaseStepAttribute>())
						{
							release.Execute(target.Configuration);
						}

						foreach (var postRelease in target.Configuration.StepsOf<PostReleaseStepAttribute>())
						{
							postRelease.Execute(target.Configuration);
						}
					});
			}

			//generate application-target
			foreach (IList<Target> grouping in targets.GroupBy(x => x.ApplicationName).SelectMany(x => x.GroupBy(y => y.TargetName).Select(y => y.ToList())))
			{
				Target sample = grouping.First();
				GenerateTasks($"{sample.ApplicationName}-{sample.TargetName}", grouping);
			}

			//generate application-platform
			foreach (IList<Target> grouping in targets.GroupBy(x => x.ApplicationName).SelectMany(x => x.GroupBy(y => y.PlatformName).Select(y => y.ToList())))
			{
				Target sample = grouping.First();
				GenerateTasks($"{sample.ApplicationName}-{sample.PlatformName}", grouping);
			}

			//generate target-platform
			foreach (IList<Target> grouping in targets.GroupBy(x => x.TargetName).SelectMany(x => x.GroupBy(y => y.PlatformName).Select(y => y.ToList())))
			{
				Target sample = grouping.First();
				GenerateTasks($"{sample.TargetName}-{sample.PlatformName}", grouping);
			}

			//generate application
			foreach (IList<Target> grouping in targets.GroupBy(x => x.ApplicationName).Select(y => y.ToList()))
			{
				Target sample = grouping.First();
				GenerateTasks($"{sample.ApplicationName}", grouping);
			}

			//generate target
			foreach (IList<Target> grouping in targets.GroupBy(x => x.TargetName).Select(y => y.ToList()))
			{
				Target sample = grouping.First();
				GenerateTasks($"{sample.TargetName}", grouping);
			}

			//generate platform
			foreach (IList<Target> grouping in targets.GroupBy(x => x.PlatformName).Select(y => y.ToList()))
			{
				Target sample = grouping.First();
				GenerateTasks($"{sample.PlatformName}", grouping);
			}

			var buildTask = _parameters.Context.Task($"build");
			var releaseTask = _parameters.Context.Task($"release");

			foreach (Target target in targets)
			{
				buildTask.IsDependentOn(target.BuildTaskName);
				releaseTask.IsDependentOn(target.ReleaseTaskName);
			}

			buildTask.Does(() => { });
			releaseTask.Does(() => { });
		}

		public void Help()
		{
			List<Target> targets = MergeTargets();

			ICakeLog logger = _parameters.Context.CakeContext.Log;

			logger.Information("build: build all, see below for more restrictive versions");
			targets.ForEach(t => logger.Information($"\t{t.BuildTaskName}"));
			logger.Information("");
			targets.GroupBy(x => x.ApplicationName).SelectMany(x => x.GroupBy(y => y.TargetName).Select(y => y.First())).ForEach(t => logger.Information($"\tbuild-{t.ApplicationName}-{t.TargetName}"));
			logger.Information("");
			targets.GroupBy(x => x.ApplicationName).SelectMany(x => x.GroupBy(y => y.PlatformName).Select(y => y.First())).ForEach(t => logger.Information($"\tbuild-{t.ApplicationName}-{t.PlatformName}"));
			logger.Information("");
			targets.GroupBy(x => x.TargetName).SelectMany(x => x.GroupBy(y => y.PlatformName).Select(y => y.First())).ForEach(t => logger.Information($"\tbuild-{t.TargetName}-{t.PlatformName}"));
			logger.Information("");
			targets.GroupBy(x => x.ApplicationName).Select(y => y.First()).ForEach(t => logger.Information($"\tbuild-{t.ApplicationName}"));
			logger.Information("");
			targets.GroupBy(x => x.TargetName).Select(y => y.First()).ForEach(t => logger.Information($"\tbuild-{t.TargetName}"));
			logger.Information("");
			targets.GroupBy(x => x.PlatformName).Select(y => y.First()).ForEach(t => logger.Information($"\tbuild-{t.PlatformName}"));
			logger.Information("");
			logger.Information("");
			logger.Information("release: release all, see below for more restrictive versions");
			targets.ForEach(t => logger.Information($"\t{t.ReleaseTaskName}"));
			logger.Information("");
			targets.GroupBy(x => x.ApplicationName).SelectMany(x => x.GroupBy(y => y.TargetName).Select(y => y.First())).ForEach(t => logger.Information($"\trelease-{t.ApplicationName}-{t.TargetName}"));
			logger.Information("");
			targets.GroupBy(x => x.ApplicationName).SelectMany(x => x.GroupBy(y => y.PlatformName).Select(y => y.First())).ForEach(t => logger.Information($"\trelease-{t.ApplicationName}-{t.PlatformName}"));
			logger.Information("");
			targets.GroupBy(x => x.TargetName).SelectMany(x => x.GroupBy(y => y.PlatformName).Select(y => y.First())).ForEach(t => logger.Information($"\trelease-{t.TargetName}-{t.PlatformName}"));
			logger.Information("");
			targets.GroupBy(x => x.ApplicationName).Select(y => y.First()).ForEach(t => logger.Information($"\trelease-{t.ApplicationName}"));
			logger.Information("");
			targets.GroupBy(x => x.TargetName).Select(y => y.First()).ForEach(t => logger.Information($"\trelease-{t.TargetName}"));
			logger.Information("");
			targets.GroupBy(x => x.PlatformName).Select(y => y.First()).ForEach(t => logger.Information($"\trelease-{t.PlatformName}"));
			logger.Information("");
		}

		private void GenerateTasks(string taskName, IEnumerable<Target> targets)
		{
			var buildTask = _parameters.Context.Task($"build-{taskName}");
			var releaseTask = _parameters.Context.Task($"release-{taskName}");

			foreach (Target target in targets)
			{
				buildTask.IsDependentOn(target.BuildTaskName);
				releaseTask.IsDependentOn(target.ReleaseTaskName);
			}

			buildTask.Does(() => { });
			releaseTask.Does(() => { });
		}

		private List<Target> MergeTargets()
		{
			List<Target> targets = new List<Target>();
			foreach (var applicationItem in _parameters.ApplicationsConfiguration)
			{
				if (applicationItem.Value.Targets.Any())
				{
					foreach (var targetItem in applicationItem.Value.Targets)
					{
						if (targetItem.Value.Platforms.Any())
						{
							foreach (var platformItem in targetItem.Value.Platforms)
							{
								IConfiguration rootLevel = _parameters.RootConfiguration;
								IConfiguration platformLevel = _parameters.PlatformsConfiguration[platformItem.Key];
								IConfiguration targetLevel = _parameters.TargetsConfiguration[targetItem.Key]
									.Merge(_parameters.TargetsConfiguration[targetItem.Key].Platforms[platformItem.Key]);
								IConfiguration applicationLevel = applicationItem.Value.Merge(
									targetItem.Value.Merge(
										platformItem.Value
									)
								);

								IConfiguration configuration = rootLevel.Merge(platformLevel.Merge(targetLevel.Merge(applicationLevel)));
								targets.Add(new Target(applicationItem.Key, targetItem.Key, platformItem.Key, configuration));
							}
						}
					}
				}
			}
			return targets;
		}
	}
}
