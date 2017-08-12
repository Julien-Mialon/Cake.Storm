﻿using System;
using Cake.Storm.Fluent.Common.Steps;
using Cake.Storm.Fluent.iOS.Common;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.iOS.Interfaces;
using Cake.Storm.Fluent.iOS.Models;
using Cake.Storm.Fluent.iOS.Steps;
using Cake.Storm.Fluent.Models;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.iOS.Extensions
{
    public static class ConfigurationExtensions
    {
	    public static TConfiguration UsePListTransformation<TConfiguration>(this TConfiguration configuration, string sourceFile, Action<IPListTransformation> transformerAction = null)
			where TConfiguration : IConfiguration
	    {
		    IPListTransformation transformation = new PListTransformation();
		    transformerAction?.Invoke(transformation);
			configuration.AddStep(new PListTransformationStep(sourceFile, transformation));
		    return configuration;
	    }

	    public static TConfiguration UseFastlane<TConfiguration>(this TConfiguration configuration, Action<IFastlaneCertificate> configurator)
			where TConfiguration : IConfiguration
	    {
			IFastlaneCertificate fastlaneCertificate = new FastlaneCertificate();
			configurator.Invoke(fastlaneCertificate);

		    configuration.AddStep(new FastlaneCertificateStep(fastlaneCertificate));
			return configuration;
	    }

	    public static TConfiguration WithBundleId<TConfiguration>(this TConfiguration configuration, string bundleId)
		    where TConfiguration : IConfiguration
	    {
			configuration.Add(iOSConstants.BUNDLE_ID_KEY, new SimpleConfigurationItem<string>(bundleId));
		    return configuration;
	    }

	    public static TConfiguration WithSignKey<TConfiguration>(this TConfiguration configuration, string signKey)
		    where TConfiguration : IConfiguration
	    {
		    configuration.Add(iOSConstants.IOS_SIGN_KEY, new SimpleConfigurationItem<string>(signKey));
		    return configuration;
	    }

	    public static TConfiguration WithSignProvision<TConfiguration>(this TConfiguration configuration, string signProvision)
		    where TConfiguration : IConfiguration
	    {
		    configuration.Add(iOSConstants.IOS_SIGN_PROVISION, new SimpleConfigurationItem<string>(signProvision));
		    return configuration;
	    }

		public static TConfiguration UseiOSTooling<TConfiguration>(this TConfiguration configuration)
			where TConfiguration : IConfiguration
	    {
			configuration.AddStep(new NugetRestoreAllStep());
			configuration.AddStep(new MSBuildSolutionStep());
			configuration.AddStep(new iOSReleaseStep());

		    return configuration;
	    }
    }
}
