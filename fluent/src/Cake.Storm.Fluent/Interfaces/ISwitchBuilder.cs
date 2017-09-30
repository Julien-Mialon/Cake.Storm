using System;

namespace Cake.Storm.Fluent.Interfaces
{
	public interface ISwitchBuilder
	{
		ISwitchBuilder Add(string name, Action<ISwitchConfiguration> action = null);
		
		ISwitchBuilder Use(string name);
	}
}