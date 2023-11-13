//--------------------------------------------------------------------------------------------------------------------
// <copyright file="ICrypter.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The main interface of the crypto algorithm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AESCryptoLib;

/// <summary>
///     The main interface of the crypto algorithm.
/// </summary>
public interface ICrypter
{
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
    string Encrypt(
        AesKeySize aesKeySize,
        string decryptedString,
        string password,
        string salt,
        int iterations = 50000,
        HashAlgorithmName? hashAlgorithmName = null);

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
    string Decrypt(
        AesKeySize aesKeySize,
        string encryptedString,
        string password,
        string salt,
        int iterations = 50000,
        HashAlgorithmName? hashAlgorithmName = null);

    /// <summary>
    ///     Returns a random salt value.
    /// </summary>
    /// <returns>A random salt value as string.</returns>
    string GetRandomSalt();
}
