using System.Security.Cryptography;
using lib12.Reflection;

namespace nex.Utilities
{
    public enum HashAlgorithms
    {
        [CreateType(typeof(MD5CryptoServiceProvider))]
        MD5,
        [CreateType(typeof(SHA1CryptoServiceProvider))]
        SHA1
    }
}
