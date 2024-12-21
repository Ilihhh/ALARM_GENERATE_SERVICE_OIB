using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class Custom : X509CertificateValidator
    {
        /// <summary>
        /// Implementation of a custom certificate validation on the client side.
        /// Client should consider certificate valid if the given certifiate is not self-signed.
        /// If validation fails, throw an exception with an adequate message.
        /// </summary>
        /// <param name="certificate"> certificate to be validate </param>
        public override void Validate(X509Certificate2 certificate)
        {

            if (certificate == null)
            {
                Console.WriteLine("NIGGA");
                throw new ArgumentNullException("certificate");
            }

            // Proveri CN sertifikata
            if (certificate.Subject != "CN=secservercert")
            {
                Console.WriteLine("GIGANIGGA");
                throw new SecurityTokenValidationException("Sertifikat nije validan.");
            }

            Console.WriteLine("Sertifikat je validan: " + certificate.Subject);
        }
    }
}
