using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Models
{
	internal class PlatformConfiguration : Configuration, IPlatformConfiguration
	{
		public PlatformConfiguration(IFluentContext context) : base(context)
		{
		}
	}
}