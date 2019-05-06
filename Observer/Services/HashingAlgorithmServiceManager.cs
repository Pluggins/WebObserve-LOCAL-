using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Observer.Services
{
    public static class HashingAlgorithmServiceManager
    {
        public static string GenerateSHA256(byte[] plainText, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            byte[] plainTextWithSaltBytes =
              new byte[plainText.Length + salt.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            byte[] hash = algorithm.ComputeHash(plainTextWithSaltBytes);
            
            return Convert.ToBase64String(hash);
        }

        public static string GenerateSHA1(byte[] plainText)
        {
            byte[] hash;
            SHA1 shaM = new SHA1Managed();
            hash = shaM.ComputeHash(plainText);

            return Convert.ToBase64String(hash);
        }
    }
}