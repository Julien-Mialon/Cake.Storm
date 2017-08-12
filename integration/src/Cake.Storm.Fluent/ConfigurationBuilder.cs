using System;
using System.Collections.Generic;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Internals;
using Cake.Storm.Fluent.Models;

namespace Cake.Storm.Fluent
{
	public class ConfigurationBuilder
	{
		private readonly IFluentContext _context;

		private readonly PathContainer _pathContainer;
		private readonly IConfiguration _rootConfiguration;
		private readonly Dictionary<string, IPlatformConfiguration> _platforms = new Dictionary<string, IPlatformConfiguration>();
		private readonly Dictionary<string, ITargetConfiguration> _targets = new Dictionary<string, ITargetConfiguration>();
		private readonly Dictionary<string, IApplicationConfiguration> _applications = new Dictionary<string, IApplicationConfiguration>();

		public ConfigurationBuilder(IFluentContext context)
		{
			_context = context;
			_pathContainer = new PathContainer();
			_rootConfiguration = new Configuration(context);
		}

		public ConfigurationBuilder UseRootDirectory(string path)
		{
			_pathContainer.Root = path;
			return this;
		}

		public ConfigurationBuilder UseBuildDirectory(string path)
		{
			_pathContainer.Build = path;
			return this;
		}

		public ConfigurationBuilder UseArtifactsDirectory(string path)
		{
			_pathContainer.Artifacts = path;
			return this;
		}

		public ConfigurationBuilder AddConfiguration(Action<IConfiguration> action)
		{
			action(_rootConfiguration);
			return this;
		}

		public ConfigurationBuilder AddPlatform(string name, Action<IPlatformConfiguration> action = null)
		{
			IPlatformConfiguration platformConfiguration = new PlatformConfiguration(_context);
			action?.Invoke(platformConfiguration);
			_platforms.Add(name, platformConfiguration);
			return this;
		}

		public ConfigurationBuilder AddTarget(string name, Action<ITargetConfiguration> action = null)
		{
			ITargetConfiguration targetConfiguration = new TargetConfiguration(_context);
			action?.Invoke(targetConfiguration);
			_targets.Add(name, targetConfiguration);
			return this;
		}

		public ConfigurationBuilder AddApplication(string name, Action<IApplicationConfiguration> action = null)
		{
			IApplicationConfiguration applicationConfiguration = new ApplicationConfiguration(_context);
			action?.Invoke(applicationConfiguration);
			_applications.Add(name, applicationConfiguration);
			return this;
		}

		public void Build()
		{
			/*
			 * must generate tasks : 
			 *	- clean
			 *	- build
			 *	- rebuild
			 *	- release
			 *	- help/default
			 *	
			 *	every task with prefix build/rebuild/release
			 *		- platform-target-application
			 *		- platform-application
			 *		- target-application
			 *		- platform-target 
			 *		- platform
			 *		- target
			 *		- application
			 */

			DirectoryPath rootPath = _context.CakeContext.Directory(_pathContainer.Root);
			if (!_context.CakeContext.DirectoryExists(rootPath))
			{
				_context.CakeContext.LogAndThrow($"Root directory {rootPath.FullPath} does not exists");
			}

			DirectoryPath buildPath = rootPath.Combine(_pathContainer.Build);
			DirectoryPath artifactsPath = rootPath.Combine(_pathContainer.Artifacts);

			BuilderParameters parameters = new BuilderParameters(rootPath, buildPath, artifactsPath, _context, _rootConfiguration, _platforms, _targets, _applications);
			new Builder(parameters).Build();
		}

		private class PathContainer
		{
			public string Root { get; set; }

			public string Build { get; set; }

			public string Artifacts { get; set; }

			public PathContainer()
			{
				Root = ".";
				Build = "build";
				Artifacts = "artifacts";
			}
		}
	}
}
