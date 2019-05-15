//using System;
//using System.IO;
//using System.IO.Compression;
//using System.Security.Cryptography.X509Certificates;
//using System.Text;
//using Org.BouncyCastle.Asn1.Pkcs;
//using Org.BouncyCastle.Asn1.Sec;
//using Org.BouncyCastle.Asn1.X509;
//using Org.BouncyCastle.Asn1.X9;
//using Org.BouncyCastle.Crypto;
//using Org.BouncyCastle.Crypto.Generators;
//using Org.BouncyCastle.Crypto.Operators;
//using Org.BouncyCastle.Crypto.Parameters;
//using Org.BouncyCastle.Math;
//using Org.BouncyCastle.Security;
//using Org.BouncyCastle.X509;
//using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using LibGit2Sharp;

namespace TestApp
{
    static class Programm
    {
        //static readonly SecureRandom secureRandom = new SecureRandom();

        //static AsymmetricCipherKeyPair GenerateRsaKeyPair(int length)
        //{
        //    var keygenParam = new KeyGenerationParameters(secureRandom, length);

        //    var keyGenerator = new RsaKeyPairGenerator();
        //    keyGenerator.Init(keygenParam);
        //    return keyGenerator.GenerateKeyPair();
        //}

        //static AsymmetricCipherKeyPair GenerateEcKeyPair(string curveName)
        //{
        //    var ecParam = SecNamedCurves.GetByName(curveName);
        //    var ecDomain = new ECDomainParameters(ecParam.Curve, ecParam.G, ecParam.N);
        //    var keygenParam = new ECKeyGenerationParameters(ecDomain, secureRandom);

        //    var keyGenerator = new ECKeyPairGenerator();
        //    keyGenerator.Init(keygenParam);
        //    return keyGenerator.GenerateKeyPair();
        //}

        //static X509Certificate GenerateCertificate(
        //    X509Name issuer, X509Name subject,
        //    AsymmetricKeyParameter issuerPrivate,
        //    AsymmetricKeyParameter subjectPublic)
        //{
        //    ISignatureFactory signatureFactory;
        //    if (issuerPrivate is ECPrivateKeyParameters)
        //    {
        //        signatureFactory = new Asn1SignatureFactory(
        //            X9ObjectIdentifiers.ECDsaWithSha256.ToString(),
        //            issuerPrivate);
        //    }
        //    else
        //    {
        //        signatureFactory = new Asn1SignatureFactory(
        //            PkcsObjectIdentifiers.Sha256WithRsaEncryption.ToString(),
        //            issuerPrivate);
        //    }

        //    var certGenerator = new X509V3CertificateGenerator();
        //    certGenerator.SetIssuerDN(issuer);
        //    certGenerator.SetSubjectDN(subject);
        //    certGenerator.SetSerialNumber(BigInteger.ValueOf(1));
        //    certGenerator.SetNotAfter(DateTime.UtcNow.AddHours(1));
        //    certGenerator.SetNotBefore(DateTime.UtcNow);
        //    certGenerator.SetPublicKey(subjectPublic);
        //    return certGenerator.Generate(signatureFactory);
        //}

        //static bool ValidateSelfSignedCert(X509Certificate cert, ICipherParameters pubKey)
        //{
        //    cert.CheckValidity(DateTime.UtcNow);
        //    var tbsCert = cert.GetTbsCertificate();
        //    var sig = cert.GetSignature();

        //    var signer = SignerUtilities.GetSigner(cert.SigAlgName);
        //    signer.Init(false, pubKey);
        //    signer.BlockUpdate(tbsCert, 0, tbsCert.Length);
        //    return signer.VerifySignature(sig);
        //}

        static void Main()
        {
            using (var repo = new Repository(@"C:\Users\PC\Desktop\test\MGI\TestApp\bin\Debug\Test"))
            {
                var result = Commands.Pull(repo, new Signature(new Identity("TestApp", "test@TestApp.de"), DateTimeOffset.Now), new PullOptions());
            }

            //var test2 = Repository.Clone(@"https://github.com/Tauron1990/MGI-Project-Manager.git",
            //    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test"));

            //string test = Environment.GetEnvironmentVariable("Path").Split(';').First(p => p.Contains("dotnet"));

            ////dotnet publish Tauron.MgiProjectManager.Server.csproj

            ////* Create your Process
            //Process process = new Process
            //{
            //    StartInfo =
            //    {
            //        FileName = Path.Combine(test, "dotnet.exe"),
            //        Arguments =
            //            $"publish {Path.Combine(@"C:\Users\PC\Desktop\test\MGI\Alt\Tauron.MgiProjectManager.Server", "Tauron.MgiProjectManager.Server.csproj")}",
            //        UseShellExecute = false,
            //        RedirectStandardOutput = true,
            //        RedirectStandardError = true
            //    }
            //};
            ////* Set your output and error (asynchronous) handlers
            //process.OutputDataReceived += OutputHandler;
            //process.ErrorDataReceived += OutputHandler;
            ////* Start process and handlers
            //process.Start();
            //process.BeginOutputReadLine();
            //process.BeginErrorReadLine();
            //process.WaitForExit();

            //Console.WriteLine();
            //Console.WriteLine(process.ExitCode);

            //Console.ReadKey();
            //using (var server = Microsoft.Web.Administration.ServerManager.OpenRemote("http://192.168.105.18"))
            //{
            //    foreach (var serverSite in server.Sites)
            //    {
            //        Console.WriteLine(serverSite.Name);
            //    }
            //}

            //Console.ReadKey();
            //const string path = @"G:\ForTest";

            //foreach (var filePath in Directory.EnumerateFiles(path))
            //{
            //    using (var sourceFile = new FileStream(filePath, FileMode.Open))
            //    {
            //        using (var output = new FileStream(Path.GetFileName(filePath) + ".bin", FileMode.Create))
            //        {
            //            using (var zip = new GZipStream(output, CompressionLevel.Optimal))
            //            {
            //                sourceFile.CopyTo(zip);
            //            }
            //        }
            //    }
            //}

            //var caName = new X509Name("CN=MgiProjectManagerCA");
            //var eeName = new X509Name("CN=MgiProjectManagerEE");
            //var caKey = GenerateEcKeyPair("secp256r1");
            //var eeKey = GenerateRsaKeyPair(2048);

            //var caCert = GenerateCertificate(caName, caName, caKey.Private, caKey.Public);
            //var eeCert = GenerateCertificate(caName, eeName, caKey.Private, eeKey.Public);
            //var caOk = ValidateSelfSignedCert(caCert, caKey.Public);
            //var eeOk = ValidateSelfSignedCert(eeCert, caKey.Public);

            ////DotNetUtilities.ToX509Certificate(caCert).Export(X509ContentType.Pkcs12)

            //using (var f = File.OpenWrite("ca.cer"))
            //{
            //    var buf = caCert.GetEncoded();
            //    f.Write(buf, 0, buf.Length);
            //}

            //using (var f = File.OpenWrite("ee.cer"))
            //{
            //    var buf = eeCert.GetEncoded();
            //    f.Write(buf, 0, buf.Length);
            //}
        }

        static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            Console.WriteLine(outLine.Data);
        }
    }
}