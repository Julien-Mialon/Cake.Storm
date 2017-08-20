using System.Collections.Generic;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Models
{
	internal class TargetConfiguration : Configuration, ITargetConfiguration
	{
		private readonly Dictionary<string, IPlatformConfiguration> _platformConfigurations = new Dictionary<string, IPlatformConfiguration>();

		public IReadOnlyDictionary<string, IPlatformConfiguration> Platforms => _platformConfigurations;

		public TargetConfiguration(IFluentContext context) : base(context)
		{
		}

		public void AddPlatform(string name, IPlatformConfiguration platformConfiguration)
		{
			_platformConfigurations.Add(name, platformConfiguration);
		}
	}
}