using System.Collections.Generic;

namespace Cake.Storm.Fluent.Interfaces
{
	public interface IApplicationConfiguration : IConfiguration
	{
		IReadOnlyDictionary<string, ITargetConfiguration> Targets { get; }

		void AddTarget(string name, ITargetConfiguration targetConfiguration);
	}
}