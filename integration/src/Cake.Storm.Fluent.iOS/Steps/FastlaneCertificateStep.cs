using Cake.Storm.Fluent.iOS.Interfaces;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.iOS.Steps
{
	[PreReleaseStep]
	internal class FastlaneCertificateStep : IStep
	{
		private readonly IFastlaneCertificateAction _fastlaneCertificate;

		public FastlaneCertificateStep(IFastlaneCertificateAction fastlaneCertificate)
		{
			_fastlaneCertificate = fastlaneCertificate;
		}

		public void Execute(IConfiguration configuration)
		{
			_fastlaneCertificate.Execute(configuration);
		}
	}
}