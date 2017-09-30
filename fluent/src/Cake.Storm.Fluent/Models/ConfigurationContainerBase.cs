using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Models
{
	public abstract class ConfigurationContainerBase : IConfigurationContainer
	{
		public IConfiguration Configuration { get; }
		
		protected ConfigurationContainerBase(IConfiguration configuration)
		{
			Configuration = configuration;
		}
	}
}