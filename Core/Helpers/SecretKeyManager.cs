using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public class SecretKeyManager
    {
        private static SecretKeyManager? _instance;
        private static readonly object  LockObject = new object();

        private readonly string _secretKey;
        private SecretKeyManager()
        {
            _secretKey = GenerateSecretKey();
        }

        public static SecretKeyManager Instance
        {
            get
            {
                lock (LockObject)
                {
                    return _instance ??= new SecretKeyManager();
                }
            }
        }

        public string GetSecretKey()
        {
            return _secretKey;
        }

        private string GenerateSecretKey()
        {
            byte[] Keybytes = RSA.Create(1024).ExportParameters(true).Modulus!;

            return Convert.ToBase64String(Keybytes);
        }

    }
}
