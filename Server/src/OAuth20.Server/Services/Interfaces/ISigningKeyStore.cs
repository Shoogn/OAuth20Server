using System.Security.Cryptography;

namespace OAuth20.Server.Services.Interfaces
{
    public interface ISigningKeyStore
    {
        RSA GetPublicKey(string kid);
        RSA GetPrivateKey();
        string KeyId { get; }
    }
}
