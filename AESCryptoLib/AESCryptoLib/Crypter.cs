using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AESCryptoLib
{
    /// <inheritdoc />
    public class Crypter : ICrypter
    {
        private readonly Random _random = new Random();

        /// <inheritdoc />
        public string Decrypt(AesKeySize aesKeySize, string encryptedString, string password, string salt)
        {
            var saltValue = Encoding.UTF32.GetBytes(salt);
            var generatedKey = new Rfc2898DeriveBytes(password, saltValue);
            var aes = new AesManaged
            {
                KeySize = (int) aesKeySize,
                BlockSize = 128
            };
            aes.Key = generatedKey.GetBytes(aes.KeySize / 8);
            aes.IV = generatedKey.GetBytes(aes.BlockSize / 8);
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            var data = Convert.FromBase64String(encryptedString);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            cs.Close();
            var decryptedString = Encoding.UTF32.GetString(ms.ToArray());
            ms.Close();
            aes.Clear();
            return decryptedString;
        }

        /// <inheritdoc />
        public string Encrypt(AesKeySize aesKeySize, string decryptedString, string password, string salt)
        {
            var saltValue = Encoding.UTF32.GetBytes(salt);
            var generatedKey = new Rfc2898DeriveBytes(password, saltValue);
            var aes = new AesManaged
            {
                KeySize = (int) aesKeySize,
                BlockSize = 128
            };
            aes.Key = generatedKey.GetBytes(aes.KeySize / 8);
            aes.IV = generatedKey.GetBytes(aes.BlockSize / 8);
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            var data = Encoding.UTF32.GetBytes(decryptedString);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            cs.Close();
            var encryptedString = Convert.ToBase64String(ms.ToArray());
            ms.Close();
            aes.Clear();
            return encryptedString;
        }

        /// <inheritdoc />
        public string GetRandomSalt()
        {
            var alg = SHA512.Create();
            alg.ComputeHash(
                Encoding.UTF32.GetBytes(DateTime.Now.ToLongDateString() + _random.Next(int.MaxValue) + Guid.NewGuid()));
            return BitConverter.ToString(alg.Hash);
        }
    }
}