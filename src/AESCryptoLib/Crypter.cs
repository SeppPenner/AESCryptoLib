//--------------------------------------------------------------------------------------------------------------------
// <copyright file="Crypter.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The main class of the crypto algorithm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AESCryptoLib;

/// <inheritdoc cref="ICrypter"></inheritdoc>
/// <summary>
///     The main class of the crypto algorithm.
/// </summary>
/// <seealso cref="ICrypter"></seealso>
public class Crypter : ICrypter
{
    /// <summary>
    /// The random generator.
    /// </summary>
    private readonly Random random = new();

    /// <inheritdoc cref="ICrypter"></inheritdoc>
    /// <summary>
    ///     Decrypts a string with a password and salt value.
    /// </summary>
    /// <param name="aesKeySize">The <see cref="AesKeySize" /> to be used in the decryption.</param>
    /// <param name="encryptedString">The string that should be decrypted.</param>
    /// <param name="password">The password to decrypt the data with.</param>
    /// <param name="salt">The salt value to decrypt the data with.</param>
    /// <param name="iterations">The number of iterations.</param>
    /// <param name="hashAlgorithmName">The hash algorithm name.</param>
    /// <returns>A decrypted <see cref="string"/>.</returns>
    /// <seealso cref="ICrypter"></seealso>
    public string Decrypt(
        AesKeySize aesKeySize,
        string encryptedString,
        string password,
        string salt,
        int iterations = 50000,
        HashAlgorithmName? hashAlgorithmName = null)
    {
        var saltValue = Encoding.UTF32.GetBytes(salt);
        var generatedKey = new Rfc2898DeriveBytes(password, saltValue, iterations, hashAlgorithmName ?? HashAlgorithmName.SHA256);
        var aes = Aes.Create();
        aes.BlockSize = 128;
        aes.KeySize = (int)aesKeySize;
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

    /// <inheritdoc cref="ICrypter"></inheritdoc>
    /// <summary>
    ///     Encrypts a string with a password and salt value.
    /// </summary>
    /// <param name="aesKeySize">The <see cref="AesKeySize" /> to be used in the encryption.</param>
    /// <param name="decryptedString">The string that should be encrypted.</param>
    /// <param name="password">The password to encrypt the data with.</param>
    /// <param name="salt">The salt value to encrypt the data with.</param>
    /// <param name="iterations">The number of iterations.</param>
    /// <param name="hashAlgorithmName">The hash algorithm name.</param>
    /// <returns>An encrypted <see cref="string"/>.</returns>
    /// <seealso cref="ICrypter"></seealso>
    public string Encrypt(
        AesKeySize aesKeySize,
        string decryptedString,
        string password,
        string salt,
        int iterations = 50000,
        HashAlgorithmName? hashAlgorithmName = null)
    {
        var saltValue = Encoding.UTF32.GetBytes(salt);
        var generatedKey = new Rfc2898DeriveBytes(password, saltValue, iterations, hashAlgorithmName ?? HashAlgorithmName.SHA256);
        var aes = Aes.Create();
        aes.BlockSize = 128;
        aes.KeySize = (int)aesKeySize;
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

    /// <inheritdoc cref="ICrypter"></inheritdoc>
    /// <summary>
    ///     Returns a random salt value.
    /// </summary>
    /// <returns>A random salt value as string.</returns>
    /// <seealso cref="ICrypter"></seealso>
    public string GetRandomSalt()
    {
        var alg = SHA512.Create();

        if (alg?.Hash is null)
        {
            return string.Empty;
        }

        alg.ComputeHash(
            Encoding.UTF32.GetBytes(DateTime.Now.ToLongDateString() + this.random.Next(int.MaxValue) + Guid.NewGuid()));
        return BitConverter.ToString(alg.Hash);
    }
}
