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
    /// The block size used for the encryption and the decryption in bits.
    /// </summary>
    private const int BlockSizeInBits = 128;

    /// <summary>
    /// The number of random bytes a salt value returned by <see cref="GetRandomSalt"/> is built from.
    /// </summary>
    private const int SaltSizeInBytes = 64;

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
        using var aes = CreateAes(aesKeySize, password, salt, iterations, hashAlgorithmName);
        using var decryptor = aes.CreateDecryptor();
        using var memoryStream = new MemoryStream();

        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write, leaveOpen: true))
        {
            var data = Convert.FromBase64String(encryptedString);
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();
        }

        return Encoding.UTF32.GetString(memoryStream.ToArray());
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
        using var aes = CreateAes(aesKeySize, password, salt, iterations, hashAlgorithmName);
        using var encryptor = aes.CreateEncryptor();
        using var memoryStream = new MemoryStream();

        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write, leaveOpen: true))
        {
            var data = Encoding.UTF32.GetBytes(decryptedString);
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.FlushFinalBlock();
        }

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    /// <inheritdoc cref="ICrypter"></inheritdoc>
    /// <summary>
    ///     Returns a random salt value.
    /// </summary>
    /// <returns>A random salt value as string.</returns>
    /// <seealso cref="ICrypter"></seealso>
    public string GetRandomSalt()
    {
        return BitConverter.ToString(RandomNumberGenerator.GetBytes(SaltSizeInBytes));
    }

    /// <summary>
    /// Creates the <see cref="Aes"/> instance for the given parameters with the key and the initialization vector
    /// derived from the password and the salt value.
    /// </summary>
    /// <param name="aesKeySize">The <see cref="AesKeySize" /> to be used.</param>
    /// <param name="password">The password to derive the key from.</param>
    /// <param name="salt">The salt value to derive the key with.</param>
    /// <param name="iterations">The number of iterations.</param>
    /// <param name="hashAlgorithmName">The hash algorithm name.</param>
    /// <returns>An <see cref="Aes"/> instance ready for the encryption or the decryption.</returns>
    private static Aes CreateAes(
        AesKeySize aesKeySize,
        string password,
        string salt,
        int iterations,
        HashAlgorithmName? hashAlgorithmName)
    {
        var aes = Aes.Create();
        aes.BlockSize = BlockSizeInBits;
        aes.KeySize = (int)aesKeySize;

        var keyLength = aes.KeySize / 8;
        var initializationVectorLength = aes.BlockSize / 8;

        // The key and the initialization vector are the first bytes of one single PBKDF2 output, the key first.
        // The obsolete Rfc2898DeriveBytes constructor produced the same bytes with two consecutive GetBytes calls,
        // so data encrypted by an earlier version of this library stays decryptable.
        var derivedBytes = Rfc2898DeriveBytes.Pbkdf2(
            password,
            Encoding.UTF32.GetBytes(salt),
            iterations,
            hashAlgorithmName ?? HashAlgorithmName.SHA256,
            keyLength + initializationVectorLength);

        aes.Key = derivedBytes[..keyLength];
        aes.IV = derivedBytes[keyLength..];
        return aes;
    }
}
