using Cake.Storm.Fluent.Android.Interfaces;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Models;

namespace Cake.Storm.Fluent.Android.Models
{
	internal class KeystoreConfiguration : ConfigurationContainerBase, IKeystoreConfiguration
	{
		public KeystoreConfiguration(IConfiguration configuration) : base(configuration)
		{
		}
	}
}