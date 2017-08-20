using System.Collections.Generic;

namespace Cake.Storm.Fluent.Interfaces
{
	public interface ITargetConfiguration : IConfiguration
	{
		IReadOnlyDictionary<string, IPlatformConfiguration> Platforms { get; }
		
		void AddPlatform(string name, IPlatformConfiguration platformConfiguration);
	}
}