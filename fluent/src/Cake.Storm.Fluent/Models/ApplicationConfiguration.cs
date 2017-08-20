using System.Collections.Generic;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Models
{
	internal class ApplicationConfiguration : Configuration, IApplicationConfiguration
	{
		private readonly Dictionary<string, ITargetConfiguration> _targetConfigurations = new Dictionary<string, ITargetConfiguration>();

		public IReadOnlyDictionary<string, ITargetConfiguration> Targets => _targetConfigurations;

		public ApplicationConfiguration(IFluentContext context) : base(context)
		{
		}

		public void AddTarget(string name, ITargetConfiguration targetConfiguration)
		{
			_targetConfigurations.Add(name, targetConfiguration);
		}
	}
}