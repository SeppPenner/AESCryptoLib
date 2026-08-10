// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrypterTests.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   A class to test the <see cref="Crypter" /> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AESCryptoLib.Tests;

/// <summary>
/// A class to test the <see cref="Crypter"/> class.
/// </summary>
[TestClass]
public class CrypterTests
{
    /// <summary>
    /// The crypter under test.
    /// </summary>
    private readonly ICrypter crypter = new Crypter();

    /// <summary>
    /// Checks whether a string encrypted with <see cref="AesKeySize.Strong"/> is returned unchanged by the
    /// decryption.
    /// </summary>
    [TestMethod]
    public void EncryptAndDecryptWithTheStrongKeySizeReturnsThePlainText()
    {
        var encryptedString = this.crypter.Encrypt(
            AesKeySize.Strong,
            TestDataProvider.PlainText,
            TestDataProvider.Password,
            TestDataProvider.Salt);
        var decryptedString = this.crypter.Decrypt(
            AesKeySize.Strong,
            encryptedString,
            TestDataProvider.Password,
            TestDataProvider.Salt);

        Assert.AreEqual(TestDataProvider.PlainText, decryptedString);
    }

    /// <summary>
    /// Checks whether a string encrypted with <see cref="AesKeySize.Weak"/> is returned unchanged by the decryption.
    /// </summary>
    [TestMethod]
    public void EncryptAndDecryptWithTheWeakKeySizeReturnsThePlainText()
    {
        var encryptedString = this.crypter.Encrypt(
            AesKeySize.Weak,
            TestDataProvider.PlainText,
            TestDataProvider.Password,
            TestDataProvider.Salt);
        var decryptedString = this.crypter.Decrypt(
            AesKeySize.Weak,
            encryptedString,
            TestDataProvider.Password,
            TestDataProvider.Salt);

        Assert.AreEqual(TestDataProvider.PlainText, decryptedString);
    }

    /// <summary>
    /// Checks whether an empty input survives the roundtrip. The padding makes the encrypted result one full block
    /// long, so an empty string is not a special case for the caller.
    /// </summary>
    [TestMethod]
    public void EncryptAndDecryptReturnsAnEmptyStringForAnEmptyInput()
    {
        var encryptedString = this.crypter.Encrypt(
            AesKeySize.Strong,
            string.Empty,
            TestDataProvider.Password,
            TestDataProvider.Salt);
        var decryptedString = this.crypter.Decrypt(
            AesKeySize.Strong,
            encryptedString,
            TestDataProvider.Password,
            TestDataProvider.Salt);

        Assert.AreEqual(16, Convert.FromBase64String(encryptedString).Length);
        Assert.AreEqual(string.Empty, decryptedString);
    }

    /// <summary>
    /// Checks whether characters outside the basic multilingual plane survive the roundtrip. The payload is
    /// converted with <see cref="System.Text.Encoding.UTF32"/>, so a surrogate pair must come back as one character.
    /// </summary>
    [TestMethod]
    public void EncryptAndDecryptKeepsCharactersOutsideTheBasicMultilingualPlane()
    {
        const string PlainText = "Schlüssel 🔐 fertig";

        var encryptedString = this.crypter.Encrypt(
            AesKeySize.Strong,
            PlainText,
            TestDataProvider.Password,
            TestDataProvider.Salt);
        var decryptedString = this.crypter.Decrypt(
            AesKeySize.Strong,
            encryptedString,
            TestDataProvider.Password,
            TestDataProvider.Salt);

        Assert.AreEqual(PlainText, decryptedString);
    }

    /// <summary>
    /// Checks whether the encrypted result is base64 and a whole number of AES blocks long.
    /// </summary>
    [TestMethod]
    public void EncryptReturnsBase64OfWholeBlocks()
    {
        var encryptedString = this.crypter.Encrypt(
            AesKeySize.Strong,
            TestDataProvider.PlainText,
            TestDataProvider.Password,
            TestDataProvider.Salt);

        var encryptedBytes = Convert.FromBase64String(encryptedString);

        Assert.AreEqual(0, encryptedBytes.Length % 16);
        Assert.IsTrue(encryptedBytes.Length > 0);
    }

    /// <summary>
    /// Checks whether the same parameters produce the same result. There is no random initialization vector, the
    /// vector is derived from the password and the salt value, which is why the salt has to be a new one per secret.
    /// </summary>
    [TestMethod]
    public void EncryptWithTheSameParametersReturnsTheSameResult()
    {
        var firstResult = this.crypter.Encrypt(
            AesKeySize.Strong,
            TestDataProvider.PlainText,
            TestDataProvider.Password,
            TestDataProvider.Salt);
        var secondResult = this.crypter.Encrypt(
            AesKeySize.Strong,
            TestDataProvider.PlainText,
            TestDataProvider.Password,
            TestDataProvider.Salt);

        Assert.AreEqual(firstResult, secondResult);
    }

    /// <summary>
    /// Checks whether a different salt value produces a different result for the same plain text and password.
    /// </summary>
    [TestMethod]
    public void EncryptWithADifferentSaltReturnsADifferentResult()
    {
        var firstResult = this.crypter.Encrypt(
            AesKeySize.Strong,
            TestDataProvider.PlainText,
            TestDataProvider.Password,
            TestDataProvider.Salt);
        var secondResult = this.crypter.Encrypt(
            AesKeySize.Strong,
            TestDataProvider.PlainText,
            TestDataProvider.Password,
            TestDataProvider.Salt + "00");

        Assert.AreNotEqual(firstResult, secondResult);
    }

    /// <summary>
    /// Checks whether the optional parameters default to 50000 iterations and SHA256, and whether a
    /// <see langword="null"/> hash algorithm name is treated the same way as SHA256.
    /// </summary>
    [TestMethod]
    public void TheDefaultParametersAre50000IterationsAndSha256()
    {
        var withDefaults = this.crypter.Encrypt(
            AesKeySize.Strong,
            TestDataProvider.PlainText,
            TestDataProvider.Password,
            TestDataProvider.Salt);
        var withExplicitValues = this.crypter.Encrypt(
            AesKeySize.Strong,
            TestDataProvider.PlainText,
            TestDataProvider.Password,
            TestDataProvider.Salt,
            50000,
            HashAlgorithmName.SHA256);
        var withNullHashAlgorithm = this.crypter.Encrypt(
            AesKeySize.Strong,
            TestDataProvider.PlainText,
            TestDataProvider.Password,
            TestDataProvider.Salt,
            50000,
            null);

        Assert.AreEqual(withExplicitValues, withDefaults);
        Assert.AreEqual(withExplicitValues, withNullHashAlgorithm);
    }

    /// <summary>
    /// Checks whether a wrong password is reported instead of returning garbage.
    /// </summary>
    [TestMethod]
    public void DecryptWithAWrongPasswordThrowsACryptographicException()
    {
        var encryptedString = this.crypter.Encrypt(
            AesKeySize.Strong,
            TestDataProvider.PlainText,
            TestDataProvider.Password,
            TestDataProvider.Salt);

        Assert.ThrowsExactly<CryptographicException>(() => this.crypter.Decrypt(
            AesKeySize.Strong,
            encryptedString,
            "WrongPassword",
            TestDataProvider.Salt));
    }

    /// <summary>
    /// Checks whether a wrong salt value is reported instead of returning garbage.
    /// </summary>
    [TestMethod]
    public void DecryptWithAWrongSaltThrowsACryptographicException()
    {
        var encryptedString = this.crypter.Encrypt(
            AesKeySize.Strong,
            TestDataProvider.PlainText,
            TestDataProvider.Password,
            TestDataProvider.Salt);

        Assert.ThrowsExactly<CryptographicException>(() => this.crypter.Decrypt(
            AesKeySize.Strong,
            encryptedString,
            TestDataProvider.Password,
            "11-22-33-44-55-66-77-88"));
    }

    /// <summary>
    /// Checks whether the number of iterations is part of the key, so that decrypting with another count fails.
    /// </summary>
    [TestMethod]
    public void DecryptWithAnotherIterationCountThrowsACryptographicException()
    {
        var encryptedString = this.crypter.Encrypt(
            AesKeySize.Strong,
            TestDataProvider.PlainText,
            TestDataProvider.Password,
            TestDataProvider.Salt,
            50000,
            HashAlgorithmName.SHA256);

        Assert.ThrowsExactly<CryptographicException>(() => this.crypter.Decrypt(
            AesKeySize.Strong,
            encryptedString,
            TestDataProvider.Password,
            TestDataProvider.Salt,
            50001,
            HashAlgorithmName.SHA256));
    }

    /// <summary>
    /// Checks whether the hash algorithm is part of the key, so that decrypting with another one fails.
    /// </summary>
    [TestMethod]
    public void DecryptWithAnotherHashAlgorithmThrowsACryptographicException()
    {
        var encryptedString = this.crypter.Encrypt(
            AesKeySize.Strong,
            TestDataProvider.PlainText,
            TestDataProvider.Password,
            TestDataProvider.Salt,
            50000,
            HashAlgorithmName.SHA256);

        Assert.ThrowsExactly<CryptographicException>(() => this.crypter.Decrypt(
            AesKeySize.Strong,
            encryptedString,
            TestDataProvider.Password,
            TestDataProvider.Salt,
            50000,
            HashAlgorithmName.SHA512));
    }

    /// <summary>
    /// Checks whether an input that is no base64 string is reported by the conversion.
    /// </summary>
    [TestMethod]
    public void DecryptWithAStringThatIsNoBase64ThrowsAFormatException()
    {
        Assert.ThrowsExactly<FormatException>(() => this.crypter.Decrypt(
            AesKeySize.Strong,
            "This is not base64!",
            TestDataProvider.Password,
            TestDataProvider.Salt));
    }

    /// <summary>
    /// Checks whether a string encrypted by version 1.2.2.0 with <see cref="AesKeySize.Strong"/> can still be
    /// decrypted. Here the key fills the first SHA256 block and the initialization vector starts the second one.
    /// </summary>
    [TestMethod]
    public void DecryptReadsTheStrongCipherTextOfVersion122()
    {
        var decryptedString = this.crypter.Decrypt(
            AesKeySize.Strong,
            TestDataProvider.EncryptedByVersion122Strong,
            TestDataProvider.Password,
            TestDataProvider.Salt,
            50000,
            HashAlgorithmName.SHA256);

        Assert.AreEqual(TestDataProvider.PlainText, decryptedString);
    }

    /// <summary>
    /// Checks whether a string encrypted by version 1.2.2.0 with <see cref="AesKeySize.Weak"/> can still be
    /// decrypted. Here the key and the initialization vector both come from the first SHA512 block.
    /// </summary>
    [TestMethod]
    public void DecryptReadsTheWeakCipherTextOfVersion122()
    {
        var decryptedString = this.crypter.Decrypt(
            AesKeySize.Weak,
            TestDataProvider.EncryptedByVersion122Weak,
            TestDataProvider.Password,
            TestDataProvider.Salt,
            1000,
            HashAlgorithmName.SHA512);

        Assert.AreEqual(TestDataProvider.PlainText, decryptedString);
    }

    /// <summary>
    /// Checks whether a generated salt value carries 64 random bytes written as upper case hex pairs separated by
    /// dashes. Up to version 1.2.2.0 the method always returned an empty string.
    /// </summary>
    [TestMethod]
    public void GetRandomSaltReturns64BytesAsHexPairs()
    {
        var salt = this.crypter.GetRandomSalt();

        var parts = salt.Split('-');

        Assert.AreEqual(191, salt.Length);
        Assert.AreEqual(64, parts.Length);
        Assert.IsTrue(parts.All(part => part.Length == 2), "Every part has to be one byte written as two hex digits.");
        Assert.IsTrue(
            parts.All(part => byte.TryParse(part, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _)),
            "Every part has to be a hex number.");
        Assert.AreEqual(salt, salt.ToUpperInvariant());
    }

    /// <summary>
    /// Checks whether two generated salt values differ. That is the whole point of the method and it was broken up
    /// to version 1.2.2.0, where every call returned an empty string.
    /// </summary>
    [TestMethod]
    public void GetRandomSaltReturnsADifferentValueOnEveryCall()
    {
        var salts = Enumerable.Range(0, 100).Select(_ => this.crypter.GetRandomSalt()).ToList();

        Assert.AreEqual(100, salts.Distinct().Count());
        Assert.IsFalse(salts.Any(string.IsNullOrEmpty));
    }

    /// <summary>
    /// Checks whether a generated salt value works as the salt of a roundtrip, which is what the usage sample in the
    /// readme file does.
    /// </summary>
    [TestMethod]
    public void GetRandomSaltCanBeUsedForARoundtrip()
    {
        var salt = this.crypter.GetRandomSalt();

        var encryptedString = this.crypter.Encrypt(
            AesKeySize.Strong,
            TestDataProvider.PlainText,
            TestDataProvider.Password,
            salt);
        var decryptedString = this.crypter.Decrypt(
            AesKeySize.Strong,
            encryptedString,
            TestDataProvider.Password,
            salt);

        Assert.AreEqual(TestDataProvider.PlainText, decryptedString);
    }
}
