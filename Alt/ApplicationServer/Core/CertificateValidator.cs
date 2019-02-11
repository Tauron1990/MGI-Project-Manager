using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;

namespace Tauron.Application.ProjectManager.ApplicationServer.Core
{
    public class CertificateValidator : X509CertificateValidator
    {
        private static readonly X509Certificate2 _root = CrteateRoot();

        private static X509Certificate2 CrteateRoot()
        {
            var pass = new SecureString();

            foreach (var c in "tauron")
            {
                pass.AppendChar(c);
            }

            return new X509Certificate2(Properties.Resources.ca, pass);
        }

        public override void Validate(X509Certificate2 certificate)
        {
            using (var chain = new X509Chain {ChainPolicy = {RevocationMode = X509RevocationMode.NoCheck}})
            {
                chain.ChainPolicy.ExtraStore.Add(_root);
                chain.Build(certificate);
                if (chain.ChainStatus.Length != 1 || chain.ChainStatus.First().Status != X509ChainStatusFlags.UntrustedRoot)
                    throw new SecurityTokenValidationException();

                if (certificate.Issuer == "CN=MGI-Certificate-Authority" && certificate.SubjectName.Name == "CN=MGI-Certificate-Client")
                    return;
            }

            throw new SecurityTokenValidationException();
        }
    }
}