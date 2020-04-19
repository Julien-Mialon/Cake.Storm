﻿using System;
using Cake.Storm.Fluent.Common.Steps;
using Cake.Storm.Fluent.DotNetCore.Common;
using Cake.Storm.Fluent.DotNetCore.Models;
using Cake.Storm.Fluent.DotNetCore.Steps;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Models;

namespace Cake.Storm.Fluent.DotNetCore.Extensions
{
	public static class DotNetExtensions
	{
		public static TConfiguration UseDotNetCoreTooling<TConfiguration>(this TConfiguration configuration, RestoreType restoreType = RestoreType.OnlySolution)
			where TConfiguration : IConfiguration
		{
			switch (restoreType)
			{
				case RestoreType.NoRestore:
					break;
				case RestoreType.OnlySolution:
					configuration.AddStep(new DotNetRestoreStep());
					break;
				default:
					throw new NotSupportedException($"RestoreType : {restoreType} not supported on DotNetCore");
			}

			configuration.AddStep(new DotNetBuildStep());
			configuration.AddStep(new DotNetReleaseStep());
			return configuration;
		}

		//TODO: add a custom output type where user can specify which file he wants to copy from output directory
		public static TConfiguration WithDotNetCoreOutputType<TConfiguration>(this TConfiguration configuration, OutputType outputType)
			where TConfiguration : IConfiguration
		{
			configuration.Add(DotNetCoreConstants.DOTNETCORE_OUTPUT_TYPE_KEY, new SimpleConfigurationItem<OutputType>(outputType));
			return configuration;
		}

		public static TConfiguration WithTargetFramework<TConfiguration>(this TConfiguration configuration, string targetFramework)
			where TConfiguration : IConfiguration
		{
			if (configuration.Has(DotNetCoreConstants.TARGET_FRAMEWORK_KEY))
			{
				ListConfigurationItem<string> frameworksConfiguration = configuration.Get<ListConfigurationItem<string>>(DotNetCoreConstants.TARGET_FRAMEWORK_KEY);
				frameworksConfiguration.Values.Add(targetFramework);
			}
			else
			{
				configuration.Add(DotNetCoreConstants.TARGET_FRAMEWORK_KEY, new ListConfigurationItem<string>(targetFramework));
			}

			return configuration;
		}

		public static TConfiguration WithTargetFrameworks<TConfiguration>(this TConfiguration configuration, params string[] targetFrameworks)
			where TConfiguration : IConfiguration
		{
			if (configuration.Has(DotNetCoreConstants.TARGET_FRAMEWORK_KEY))
			{
				ListConfigurationItem<string> frameworksConfiguration = configuration.Get<ListConfigurationItem<string>>(DotNetCoreConstants.TARGET_FRAMEWORK_KEY);
				frameworksConfiguration.Values.AddRange(targetFrameworks);
			}
			else
			{
				configuration.Add(DotNetCoreConstants.TARGET_FRAMEWORK_KEY, new ListConfigurationItem<string>(targetFrameworks));
			}

			return configuration;
		}
	}
}