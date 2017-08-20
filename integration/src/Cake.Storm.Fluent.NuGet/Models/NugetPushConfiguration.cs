using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Models;
using Cake.Storm.Fluent.NuGet.Interfaces;

namespace Cake.Storm.Fluent.NuGet.Models
{
	internal class NugetPushConfiguration : ConfigurationContainerBase, INugetPushConfiguration
	{
		public NugetPushConfiguration(IConfiguration configuration) : base(configuration)
		{
		}
	}
}