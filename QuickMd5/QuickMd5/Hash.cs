using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace QuickMd5
{
    public static class Hash
    {
        public static string GetMd5Hash(string filename)
        {
            byte[] hashBytes;

            using (var md5Hash = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    hashBytes = md5Hash.ComputeHash(stream);
                }
            }

            var hashBuilder = new StringBuilder();

            foreach (var b in hashBytes)
            {
                hashBuilder.Append(b.ToString("X2"));
            }

            return hashBuilder.ToString();
        }

        public static string GetSha256Hash(string filename)
        {
            byte[] hashBytes;

            using (var sha256Hash = SHA256.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    hashBytes = sha256Hash.ComputeHash(stream);
                }
            }

            var hashBuilder = new StringBuilder();

            foreach (var b in hashBytes)
            {
                hashBuilder.Append(b.ToString("x2"));
            }

            return hashBuilder.ToString();
        }
    }
}
