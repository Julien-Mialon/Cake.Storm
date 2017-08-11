using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Storm.Fluent.iOS.Models;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.iOS.Interfaces
{
    public interface IFastlaneCertificate
    {
		string UserName { get; }

		string TeamName { get; }

		string ProvisioningProfileName { get; }

		CertificateType CertificateType { get; }

	    IFastlaneCertificate WithUserName(string userName);

	    IFastlaneCertificate WithTeamName(string teamName);

	    IFastlaneCertificate WithProvisioningProfileName(string provisioningProfileName);

	    IFastlaneCertificate WithCertificateType(CertificateType certificateType);

	    void Execute(IConfiguration configuration);
    }
}
