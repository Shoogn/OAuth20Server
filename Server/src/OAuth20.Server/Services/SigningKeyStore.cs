using Microsoft.Extensions.Configuration;
using OAuth20.Server.Services.Interfaces;
using System;
using System.IO;
using System.Security.Cryptography;
using static OAuth20.Server.Constants;

namespace OAuth20.Server.Services
{
    public class SigningKeyStore : ISigningKeyStore, IDisposable
    {
        private readonly RSA _privateKey;
        private readonly RSA _publicKey;
        public SigningKeyStore(IConfiguration configuration)
        {
            // Load the private key from configuration
            string privateKeyPath = configuration["JWTSigningKeys:PrivateKeyPath"]
                ?? throw new InvalidOperationException(  "Private key path missing");

            string publicKeyPath = configuration["JWTSigningKeys:PublicKeyPath"]
                ?? throw new InvalidOperationException( "Public key path missing");

            _privateKey = RSA.Create();
            _privateKey.ImportFromPem(  File.ReadAllText(privateKeyPath));

            _publicKey = RSA.Create();
            _publicKey.ImportFromPem( File.ReadAllText(publicKeyPath));
        }

        public string KeyId => OAuth2JwtConstants.CurrentKeyId;

        public void Dispose()
        {
            _privateKey?.Dispose();
            _publicKey?.Dispose();
        }

        public RSA GetPrivateKey()
        {
            return _privateKey;
        }

        public RSA GetPublicKey(string kid)
        {
           if (kid != OAuth2JwtConstants.CurrentKeyId)
               throw new InvalidOperationException("Invalid key ID");

           return _publicKey;
        }
    }
}
