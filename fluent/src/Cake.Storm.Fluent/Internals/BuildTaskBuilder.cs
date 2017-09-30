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

			public string DeployTaskName => $"deploy-{ApplicationName}-{TargetName}-{PlatformName}";
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
					foreach (IStep preBuild in target.Configuration.StepsOf<PreBuildStepAttribute>())
					{
						preBuild.Execute(target.Configuration);
					}

					foreach (IStep build in target.Configuration.StepsOf<BuildStepAttribute>())
					{
						build.Execute(target.Configuration);
					}

					foreach (IStep postBuild in target.Configuration.StepsOf<PostBuildStepAttribute>())
					{
						postBuild.Execute(target.Configuration);
					}
				});

				_parameters.Context.Task(target.ReleaseTaskName)
					.IsDependentOn(CleanTaskBuilder.TASK_NAME)
					.IsDependentOn(target.BuildTaskName)
					.Does(() =>
					{
						foreach (IStep preRelease in target.Configuration.StepsOf<PreReleaseStepAttribute>())
						{
							preRelease.Execute(target.Configuration);
						}

						foreach (IStep release in target.Configuration.StepsOf<ReleaseStepAttribute>())
						{
							release.Execute(target.Configuration);
						}

						foreach (IStep postRelease in target.Configuration.StepsOf<PostReleaseStepAttribute>())
						{
							postRelease.Execute(target.Configuration);
						}
					});
				
				_parameters.Context.Task(target.DeployTaskName)
					.IsDependentOn(CleanTaskBuilder.TASK_NAME)
					.IsDependentOn(target.ReleaseTaskName)
					.Does(() =>
					{
						foreach (IStep preRelease in target.Configuration.StepsOf<PreDeployStepAttribute>())
						{
							preRelease.Execute(target.Configuration);
						}

						foreach (IStep release in target.Configuration.StepsOf<DeployStepAttribute>())
						{
							release.Execute(target.Configuration);
						}

						foreach (IStep postRelease in target.Configuration.StepsOf<PostDeployStepAttribute>())
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

			CakeTaskBuilder<ActionTask> buildTask = _parameters.Context.Task("build");
			CakeTaskBuilder<ActionTask> releaseTask = _parameters.Context.Task("release");
			CakeTaskBuilder<ActionTask> deployTask = _parameters.Context.Task("deploy");

			foreach (Target target in targets)
			{
				buildTask.IsDependentOn(target.BuildTaskName);
				releaseTask.IsDependentOn(target.ReleaseTaskName);
				deployTask.IsDependentOn(target.DeployTaskName);
			}

			buildTask.Does(() => { });
			releaseTask.Does(() => { });
			deployTask.Does(() => { });
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
			logger.Information("deploy: deploy all, see below for more restrictive versions");
			targets.ForEach(t => logger.Information($"\t{t.DeployTaskName}"));
			logger.Information("");
			targets.GroupBy(x => x.ApplicationName).SelectMany(x => x.GroupBy(y => y.TargetName).Select(y => y.First())).ForEach(t => logger.Information($"\tdeploy-{t.ApplicationName}-{t.TargetName}"));
			logger.Information("");
			targets.GroupBy(x => x.ApplicationName).SelectMany(x => x.GroupBy(y => y.PlatformName).Select(y => y.First())).ForEach(t => logger.Information($"\tdeploy-{t.ApplicationName}-{t.PlatformName}"));
			logger.Information("");
			targets.GroupBy(x => x.TargetName).SelectMany(x => x.GroupBy(y => y.PlatformName).Select(y => y.First())).ForEach(t => logger.Information($"\tdeploy-{t.TargetName}-{t.PlatformName}"));
			logger.Information("");
			targets.GroupBy(x => x.ApplicationName).Select(y => y.First()).ForEach(t => logger.Information($"\tdeploy-{t.ApplicationName}"));
			logger.Information("");
			targets.GroupBy(x => x.TargetName).Select(y => y.First()).ForEach(t => logger.Information($"\tdeploy-{t.TargetName}"));
			logger.Information("");
			targets.GroupBy(x => x.PlatformName).Select(y => y.First()).ForEach(t => logger.Information($"\tdeploy-{t.PlatformName}"));
			logger.Information("");
		}

		private void GenerateTasks(string taskName, IEnumerable<Target> targets)
		{
			CakeTaskBuilder<ActionTask> buildTask = _parameters.Context.Task($"build-{taskName}");
			CakeTaskBuilder<ActionTask> releaseTask = _parameters.Context.Task($"release-{taskName}");
			CakeTaskBuilder<ActionTask> deployTask = _parameters.Context.Task($"deploy-{taskName}");

			foreach (Target target in targets)
			{
				buildTask.IsDependentOn(target.BuildTaskName);
				releaseTask.IsDependentOn(target.ReleaseTaskName);
				deployTask.IsDependentOn(target.DeployTaskName);
			}

			buildTask.Does(() => { });
			releaseTask.Does(() => { });
			deployTask.Does(() => { });
		}

		private List<Target> MergeTargets()
		{
			List<Target> targets = new List<Target>();
			foreach (KeyValuePair<string, IApplicationConfiguration> applicationItem in _parameters.ApplicationsConfiguration)
			{
				IReadOnlyDictionary<string, ITargetConfiguration> targetsForApplication;
				bool mergeTargetItem;
				if (applicationItem.Value.Targets.Count > 0)
				{
					targetsForApplication = applicationItem.Value.Targets;
					mergeTargetItem = true;
				}
				else //use all targets if none has been specified
				{
					targetsForApplication = _parameters.TargetsConfiguration;
					mergeTargetItem = false;
				}

				foreach (KeyValuePair<string, ITargetConfiguration> targetItem in targetsForApplication)
				{
					IReadOnlyDictionary<string, IPlatformConfiguration> platformsForTarget;
					bool mergePlatformItem;
					if (targetItem.Value.Platforms.Count > 0)
					{
						platformsForTarget = targetItem.Value.Platforms;
						mergePlatformItem = true;
					}
					else
					{
						platformsForTarget = _parameters.PlatformsConfiguration;
						mergePlatformItem = false;
					}

					foreach (KeyValuePair<string, IPlatformConfiguration> platformItem in platformsForTarget)
					{
						IConfiguration rootLevel = _parameters.RootConfiguration;
						IConfiguration platformLevel = _parameters.PlatformsConfiguration[platformItem.Key];
						ITargetConfiguration targetConfiguration = _parameters.TargetsConfiguration[targetItem.Key];
						IConfiguration targetLevel = targetConfiguration;
						if (targetConfiguration.Platforms.TryGetValue(platformItem.Key, out IPlatformConfiguration targetPlatformConfiguration))
						{
							targetLevel = targetConfiguration.Merge(targetPlatformConfiguration);
						}
						//merge application sub configuration if needed (not from list of all)
						IConfiguration applicationLevel = null;
						if (mergeTargetItem)
						{
							if (mergePlatformItem)
							{
								applicationLevel = platformItem.Value;
							}
							applicationLevel = applicationLevel == null ? targetItem.Value : targetItem.Value.Merge(applicationLevel);
						}

						applicationLevel = applicationLevel == null ? applicationItem.Value : applicationItem.Value.Merge(applicationLevel);


						IConfiguration configuration = rootLevel.Merge(platformLevel.Merge(targetLevel.Merge(applicationLevel)));
						targets.Add(new Target(applicationItem.Key, targetItem.Key, platformItem.Key, configuration));
					}
				}
			}
			return targets;
		}
	}
}