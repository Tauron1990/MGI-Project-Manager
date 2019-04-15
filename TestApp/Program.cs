﻿using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace TestApp
{
    static class Programm
    {

        static readonly SecureRandom secureRandom = new SecureRandom();

        static AsymmetricCipherKeyPair GenerateRsaKeyPair(int length)
        {
            var keygenParam = new KeyGenerationParameters(secureRandom, length);

            var keyGenerator = new RsaKeyPairGenerator();
            keyGenerator.Init(keygenParam);
            return keyGenerator.GenerateKeyPair();
        }

        static AsymmetricCipherKeyPair GenerateEcKeyPair(string curveName)
        {
            var ecParam = SecNamedCurves.GetByName(curveName);
            var ecDomain = new ECDomainParameters(ecParam.Curve, ecParam.G, ecParam.N);
            var keygenParam = new ECKeyGenerationParameters(ecDomain, secureRandom);

            var keyGenerator = new ECKeyPairGenerator();
            keyGenerator.Init(keygenParam);
            return keyGenerator.GenerateKeyPair();
        }

        static X509Certificate GenerateCertificate(
            X509Name issuer, X509Name subject,
            AsymmetricKeyParameter issuerPrivate,
            AsymmetricKeyParameter subjectPublic)
        {
            ISignatureFactory signatureFactory;
            if (issuerPrivate is ECPrivateKeyParameters)
            {
                signatureFactory = new Asn1SignatureFactory(
                    X9ObjectIdentifiers.ECDsaWithSha256.ToString(),
                    issuerPrivate);
            }
            else
            {
                signatureFactory = new Asn1SignatureFactory(
                    PkcsObjectIdentifiers.Sha256WithRsaEncryption.ToString(),
                    issuerPrivate);
            }

            var certGenerator = new X509V3CertificateGenerator();
            certGenerator.SetIssuerDN(issuer);
            certGenerator.SetSubjectDN(subject);
            certGenerator.SetSerialNumber(BigInteger.ValueOf(1));
            certGenerator.SetNotAfter(DateTime.UtcNow.AddHours(1));
            certGenerator.SetNotBefore(DateTime.UtcNow);
            certGenerator.SetPublicKey(subjectPublic);
            return certGenerator.Generate(signatureFactory);
        }

        static bool ValidateSelfSignedCert(X509Certificate cert, ICipherParameters pubKey)
        {
            cert.CheckValidity(DateTime.UtcNow);
            var tbsCert = cert.GetTbsCertificate();
            var sig = cert.GetSignature();

            var signer = SignerUtilities.GetSigner(cert.SigAlgName);
            signer.Init(false, pubKey);
            signer.BlockUpdate(tbsCert, 0, tbsCert.Length);
            return signer.VerifySignature(sig);
        }

        static void Main()
        {
            var caName = new X509Name("CN=MgiProjectManagerCA");
            var eeName = new X509Name("CN=MgiProjectManagerEE");
            var caKey = GenerateEcKeyPair("secp256r1");
            var eeKey = GenerateRsaKeyPair(2048);

            var caCert = GenerateCertificate(caName, caName, caKey.Private, caKey.Public);
            var eeCert = GenerateCertificate(caName, eeName, caKey.Private, eeKey.Public);
            var caOk = ValidateSelfSignedCert(caCert, caKey.Public);
            var eeOk = ValidateSelfSignedCert(eeCert, caKey.Public);

            //DotNetUtilities.ToX509Certificate(caCert).Export(X509ContentType.Pkcs12)

            using (var f = File.OpenWrite("ca.cer"))
            {
                var buf = caCert.GetEncoded();
                f.Write(buf, 0, buf.Length);
            }

            using (var f = File.OpenWrite("ee.cer"))
            {
                var buf = eeCert.GetEncoded();
                f.Write(buf, 0, buf.Length);
            }
        }
    }
}