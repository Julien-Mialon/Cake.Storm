using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Models
{
	internal class SwitchConfiguration : Configuration, ISwitchConfiguration
	{
		public SwitchConfiguration(IFluentContext context) : base(context)
		{
		}
	}
}