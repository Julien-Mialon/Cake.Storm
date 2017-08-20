using Cake.Storm.Fluent.iOS.Interfaces;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Models;

namespace Cake.Storm.Fluent.iOS.Models
{
	internal class FastlaneCertificateConfiguration : ConfigurationContainerBase, IFastlaneCertificateConfiguration
	{
		public FastlaneCertificateConfiguration(IConfiguration configuration) : base(configuration)
		{
		}
	}
}