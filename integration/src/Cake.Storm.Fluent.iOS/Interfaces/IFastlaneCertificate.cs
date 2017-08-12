using Cake.Storm.Fluent.iOS.Models;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.iOS.Interfaces
{
    public interface IFastlaneCertificate
    {
	    IFastlaneCertificate WithUserName(string userName);

	    IFastlaneCertificate WithTeamName(string teamName);

	    IFastlaneCertificate WithProvisioningProfileName(string provisioningProfileName);

	    IFastlaneCertificate WithCertificateType(CertificateType certificateType);
    }

	internal interface IFastlaneCertificateAction : IFastlaneCertificate
	{
		void Execute(IConfiguration configuration);
	}
}
