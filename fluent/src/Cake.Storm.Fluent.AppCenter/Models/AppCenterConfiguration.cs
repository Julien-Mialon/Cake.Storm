using Cake.Storm.Fluent.AppCenter.Interfaces;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Models;

namespace Cake.Storm.Fluent.AppCenter.Models
{
	public class AppCenterConfiguration : ConfigurationContainerBase, IAppCenterConfiguration
	{
		public AppCenterConfiguration(IConfiguration configuration) : base(configuration)
		{
		}
	}
}