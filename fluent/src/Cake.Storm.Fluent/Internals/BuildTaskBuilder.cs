using System;
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
			List<Target> targets = MergeTargets(_parameters);

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
			List<Target> targets = MergeTargets(_parameters);

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

		private static void ValidateParameters(BuilderParameters parameters)
		{
			List<string> logs = new List<string>();
			if (parameters.PlatformsConfiguration.Count == 0)
			{
				logs.Add("Error: No platforms defined");
			}
			if (parameters.TargetsConfiguration.Count == 0)
			{
				logs.Add("Error: No targets defined");
			}
			if (parameters.ApplicationsConfiguration.Count == 0)
			{
				logs.Add("Error: No applications defined");
			}

			logs.AddRange(
				parameters.PlatformsConfiguration.Keys
					.Where(platformName => !ValidateNames(platformName))
					.Select(platformName => $"Error: platform {platformName} has invalid name, only [A-Za-z0-9_-] allowed"));

			logs.AddRange(
				parameters.TargetsConfiguration.Keys
					.Where(targetName => !ValidateNames(targetName))
					.Select(targetName => $"Error: target {targetName} has invalid name, only [A-Za-z0-9_-] allowed"));

			logs.AddRange(
				parameters.ApplicationsConfiguration.Keys
					.Where(applicationName => !ValidateNames(applicationName))
					.Select(applicationName => $"Error: application {applicationName} has invalid name, only [A-Za-z0-9_-] allowed"));

			//check if all platform used in targets are defined in global list
			logs.AddRange(
				parameters.TargetsConfiguration.SelectMany(target => 
					target.Value.Platforms.Keys
						.Where(platformName => !parameters.PlatformsConfiguration.ContainsKey(platformName))
						.Select(platformName => $"Error: platform {platformName} used in target {target.Key} does not exists in platforms list")
					)
				);
			
			//check if all targets used in application are defined in global list 
			logs.AddRange(
				parameters.ApplicationsConfiguration.SelectMany(application => 
					application.Value.Targets.Keys
						.Where(targetName => !parameters.TargetsConfiguration.ContainsKey(targetName))
						.Select(targetName => $"Error: target {targetName} used in application {application.Key} does not exists in targets list")
					)
				);
			
			//check if all platforms used in targets in application are defined for this targets
			logs.AddRange(
				parameters.ApplicationsConfiguration.SelectMany(application => 
					application.Value.Targets
						//check if all platforms used are defined in global list
						.SelectMany(target => 
							target.Value.Platforms.Keys
								.Where(platformName => !parameters.PlatformsConfiguration.ContainsKey(platformName))
								.Select(platformName => $"Error: platform {platformName} used in application {application.Key} and target {target.Key} does not exists in platforms list")
						)
						//check if all platforms used are also defined in the corresponding target platforms list
						.Concat(application.Value.Targets
							.Where(target => parameters.TargetsConfiguration.ContainsKey(target.Key))
							.Select(target => Tuple.Create(target, parameters.TargetsConfiguration[target.Key]))
							.Where(targets => targets.Item2.Platforms.Count > 0)
							.SelectMany(targets => 
								targets.Item1.Value.Platforms.Keys
									.Where(platformName => !targets.Item2.Platforms.ContainsKey(platformName))
									.Select(platformName => $"Error: platform {platformName} used in application {application.Key} and target {targets.Item1.Key} does not exists in the target {targets.Item1.Key} platforms list")
							)
						)
					)
				);
			
			if (logs.Count > 0)
			{
				parameters.Context.CakeContext.LogAndThrow(string.Join(Environment.NewLine, logs));
			}
		}

		private static bool ValidateNames(string name)
		{
			return !string.IsNullOrWhiteSpace(name) &&
			       name.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_');
		}

		private static List<Target> MergeTargets(BuilderParameters parameters)
		{
			ValidateParameters(parameters);
			List<Target> result = new List<Target>();

			foreach (KeyValuePair<string, IApplicationConfiguration> applicationItem in parameters.ApplicationsConfiguration)
			{
				IApplicationConfiguration applicationConfiguration = applicationItem.Value;
				
				IReadOnlyDictionary<string, ITargetConfiguration> applicationTargets = null;
				IReadOnlyDictionary<string, ITargetConfiguration> targets;
				
				if (applicationConfiguration.Targets.Count > 0) // does the application define specific targets or not ?
				{
					applicationTargets = applicationConfiguration.Targets;
					targets = FilterTargets(parameters.TargetsConfiguration, applicationTargets); //filter global list of targets to be restricted to the ones accepted by the application
				}
				else //if not, use all defined target in the global configuration
				{
					targets = parameters.TargetsConfiguration;
				}
				
				//on each targets 
				foreach (KeyValuePair<string, ITargetConfiguration> targetItem in targets)
				{
					string targetName = targetItem.Key;
					ITargetConfiguration targetConfiguration = targetItem.Value;
					ITargetConfiguration applicationTargetConfiguration = applicationTargets?[targetName];
					
					IReadOnlyDictionary<string, IPlatformConfiguration> applicationTargetPlatforms = null;
					IReadOnlyDictionary<string, IPlatformConfiguration> targetPlatforms = null;
					IReadOnlyDictionary<string, IPlatformConfiguration> platforms;

					if (applicationTargetConfiguration != null && applicationTargetConfiguration.Platforms.Count > 0)
					{
						applicationTargetPlatforms = applicationTargetConfiguration.Platforms;
						if (targetConfiguration.Platforms.Count > 0)
						{
							targetPlatforms = FilterPlatforms(targetConfiguration.Platforms, applicationTargetPlatforms); //filter list of platforms in target to the ones defined by the application
						}
						platforms = FilterPlatforms(parameters.PlatformsConfiguration, applicationTargetPlatforms); //filter global list of platforms to the ones defined in the application
					}
					else if(targetConfiguration.Platforms.Count > 0)
					{
						targetPlatforms = targetConfiguration.Platforms;
						platforms = FilterPlatforms(parameters.PlatformsConfiguration, targetPlatforms); //filter global list of platforms to be restricted to the ones accepted by the target
					}
					else
					{
						platforms = parameters.PlatformsConfiguration;
					}
					
					foreach (KeyValuePair<string, IPlatformConfiguration> platformItem in platforms)
					{
						string platformName = platformItem.Key;
						IPlatformConfiguration platformConfiguration = platformItem.Value;
						IPlatformConfiguration targetPlatformConfiguration = targetPlatforms?[platformName];
						IPlatformConfiguration applicationTargetPlatformConfiguration = applicationTargetPlatforms?[platformName];
						
						IConfiguration rootLevel = parameters.RootConfiguration;
						IConfiguration platformLevel = platformConfiguration;
						IConfiguration targetLevel = targetConfiguration;
						IConfiguration applicationLevel = applicationConfiguration;
						
						// merge platform specific detail in target level
						if (targetPlatformConfiguration != null)
						{
							targetLevel = targetLevel.Merge(targetPlatformConfiguration);
						}
						
						// merge target and platform specific detail in application level
						if (applicationTargetConfiguration != null)
						{
							IConfiguration applicationTargetLevel = applicationTargetConfiguration;
							if (applicationTargetPlatformConfiguration != null)
							{
								applicationTargetLevel = applicationTargetLevel.Merge(applicationTargetPlatformConfiguration);
							}
							applicationLevel = applicationLevel.Merge(applicationTargetLevel);
						}

						IConfiguration configuration = rootLevel.Merge(platformLevel.Merge(targetLevel.Merge(applicationLevel)));
						result.Add(new Target(applicationItem.Key, targetItem.Key, platformItem.Key, configuration));
					}
				}
			}

			return result;
		}

		private static IReadOnlyDictionary<string, ITargetConfiguration> FilterTargets(IReadOnlyDictionary<string, ITargetConfiguration> globalTargets, IReadOnlyDictionary<string, ITargetConfiguration> applicationTargets)
		{
			return applicationTargets.Keys
				.Select(targetName => Tuple.Create(targetName, globalTargets[targetName]))
				.ToDictionary(x => x.Item1, x => x.Item2);
		}
		
		private static IReadOnlyDictionary<string, IPlatformConfiguration> FilterPlatforms(IReadOnlyDictionary<string, IPlatformConfiguration> globalPlatforms, IReadOnlyDictionary<string, IPlatformConfiguration> targetPlatforms)
		{
			return targetPlatforms.Keys
				.Select(platformName => Tuple.Create(platformName, globalPlatforms[platformName]))
				.ToDictionary(x => x.Item1, x => x.Item2);
		}
	}
}