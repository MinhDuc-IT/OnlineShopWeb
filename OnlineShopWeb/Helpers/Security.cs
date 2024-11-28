using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace OnlineShopWeb.Helpers
{
    public class Security
    {
        private static readonly string EncryptKey = "TestOnlineShopWeb";

        private static byte[] GetValidKey(string key)
        {
            const int keySize = 32; 
            var keyBytes = Encoding.UTF8.GetBytes(key);

            if (keyBytes.Length == keySize)
                return keyBytes;

            Array.Resize(ref keyBytes, keySize);
            return keyBytes;
        }

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText), "The input text cannot be null or empty.");

            using (var aes = Aes.Create())
            {
                aes.Key = GetValidKey(EncryptKey);
                aes.IV = new byte[16]; 

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var writer = new StreamWriter(cs))
                    {
                        writer.Write(plainText);
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException(nameof(cipherText), "The cipher text cannot be null or empty.");

            //if (!IsBase64String(cipherText))
            //    throw new FormatException("The input is not a valid Base64 string.");

            using (var aes = Aes.Create())
            {
                aes.Key = GetValidKey(EncryptKey);
                aes.IV = new byte[16];

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var reader = new StreamReader(cs))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        //private static bool IsBase64String(string input)
        //{
        //    if (string.IsNullOrEmpty(input) || input.Length % 4 != 0)
        //        return false;
        //    try
        //    {
        //        Convert.FromBase64String(input);
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
    }
}
