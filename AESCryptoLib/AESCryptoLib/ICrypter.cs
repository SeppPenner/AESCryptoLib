namespace AESCryptoLib
{
    /// <summary>
    ///     The main class of the crypto algorithm.
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
        /// <returns></returns>
        string Encrypt(AesKeySize aesKeySize, string decryptedString, string password, string salt);

        /// <summary>
        ///     Decrypts a string with a password and salt value.
        /// </summary>
        /// <param name="aesKeySize">The <see cref="AesKeySize" /> to be used in the decryption.</param>
        /// <param name="encryptedString">The string that should be decrypted.</param>
        /// <param name="password">The password to dencrypt the data with.</param>
        /// <param name="salt">The salt value to dencrypt the data with.</param>
        /// <returns></returns>
        string Decrypt(AesKeySize aesKeySize, string encryptedString, string password, string salt);

        /// <summary>
        ///     Returns a random salt value.
        /// </summary>
        /// <returns>A random salt value as string.</returns>
        string GetRandomSalt();
    }
}