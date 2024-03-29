using System.IO;
using UnityEngine;

namespace Mirror.SimpleWeb
{
    internal class SslConfigLoader
    {        
        internal struct Cert
        {
            public string path;
            public string password;
        }
        internal static SslConfig Load(SimpleWebTransport transport)
        {
            Debug.Log("sslConfigLoader.Load: is started!");
            // don't need to load anything if ssl is not enabled
            if (!transport.sslEnabled)
                return default;

            string certJsonPath = transport.sslCertJson;

            Cert cert = LoadCertJson(certJsonPath);

            Debug.Log("sslConfigLoader.Load: transport.sslProtocols=" + transport.sslProtocols + ", cert.path=" + cert.path);
            Debug.Log("sslConfigLoader.Load: is ended!");

            return new SslConfig(
                enabled: transport.sslEnabled,
                sslProtocols: transport.sslProtocols,
                certPath: cert.path,
                certPassword: cert.password
            );            
        }

        internal static Cert LoadCertJson(string certJsonPath)
        {
            Debug.Log("sslConfigLoader.LoadCertJson: is started!");

            string json = File.ReadAllText(certJsonPath);
            Cert cert = JsonUtility.FromJson<Cert>(json);

            if (string.IsNullOrWhiteSpace(cert.path))
            {
                throw new InvalidDataException("Cert Json didn't not contain \"path\"");
            }
            if (string.IsNullOrWhiteSpace(cert.password))
            {
                // password can be empty
                cert.password = string.Empty;
            }

            Debug.Log("sslConfigLoader.LoadCertJson: is ended!");
            return cert;
        }
    }
}
