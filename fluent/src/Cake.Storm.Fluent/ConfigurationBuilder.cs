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
		private readonly Dictionary<string, SwitchBuilder> _switches = new Dictionary<string, SwitchBuilder>();

		private bool _rootConfigurationDefined;

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

		public ConfigurationBuilder AddConfiguration(Action<IConfiguration> action = null)
		{
			if (_rootConfigurationDefined)
			{
				_context.CakeContext.LogAndThrow("Root configuration has already been defined");
			}
			
			action?.Invoke(_rootConfiguration);
			_rootConfigurationDefined = true;
			return this;
		}

		public ConfigurationBuilder AddPlatform(string name, Action<IPlatformConfiguration> action = null)
		{
			if (_platforms.ContainsKey(name))
			{
				_context.CakeContext.LogAndThrow($"Platform {name} already exists");
			}
			
			IPlatformConfiguration platformConfiguration = new PlatformConfiguration(_context);
			action?.Invoke(platformConfiguration);
			_platforms.Add(name, platformConfiguration);
			return this;
		}

		public ConfigurationBuilder AddTarget(string name, Action<ITargetConfiguration> action = null)
		{
			if (_targets.ContainsKey(name))
			{
				_context.CakeContext.LogAndThrow($"Target {name} already exists");
			}
			
			ITargetConfiguration targetConfiguration = new TargetConfiguration(_context);
			action?.Invoke(targetConfiguration);
			_targets.Add(name, targetConfiguration);
			return this;
		}

		public ConfigurationBuilder AddApplication(string name, Action<IApplicationConfiguration> action = null)
		{
			if (_applications.ContainsKey(name))
			{
				_context.CakeContext.LogAndThrow($"Application {name} already exists");
			}
			
			IApplicationConfiguration applicationConfiguration = new ApplicationConfiguration(_context);
			action?.Invoke(applicationConfiguration);
			_applications.Add(name, applicationConfiguration);
			return this;
		}

		public ConfigurationBuilder AddConfigurationSwitch(string name, Action<ISwitchBuilder> action = null)
		{
			if (_switches.ContainsKey(name))
			{
				_context.CakeContext.LogAndThrow($"Switch {name} already exists");
			}

			SwitchBuilder switchBuilder = new SwitchBuilder(_context, name);
			action?.Invoke(switchBuilder);
			_switches.Add(name, switchBuilder);
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
			 *  - deploy
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

			IConfiguration switchConfiguration = MergeSwitches();
			
			BuilderParameters parameters = new BuilderParameters(rootPath, buildPath, artifactsPath, _context, _rootConfiguration, switchConfiguration, _platforms, _targets, _applications);
			new Builder(parameters).Build();
		}

		private IConfiguration MergeSwitches()
		{
			IConfiguration configuration = new Configuration(_context);
			foreach (SwitchBuilder switchBuilder in _switches.Values)
			{
				configuration = configuration.Merge(switchBuilder.GetSelectedConfiguration());
			}
			return configuration;
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
