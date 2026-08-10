// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestDataProvider.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   A class to provide the test data used in the tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AESCryptoLib.Tests;

/// <summary>
/// A class to provide the test data used in the tests.
/// </summary>
public static class TestDataProvider
{
    /// <summary>
    /// The password used in all tests. It is the password from the usage sample in the readme file.
    /// </summary>
    public const string Password = "Password";

    /// <summary>
    /// A fixed salt value. The tests need a salt that does not change between runs, because the encrypted result
    /// only stays comparable as long as the key derivation gets the same input.
    /// </summary>
    public const string Salt = "AA-BB-CC-DD-EE-FF-00-11";

    /// <summary>
    /// The string that is encrypted in the tests. It carries a German umlaut so that the encoding of the payload is
    /// part of every roundtrip.
    /// </summary>
    public const string PlainText = "Hello Wörld";

    /// <summary>
    /// The <see cref="PlainText"/> encrypted by version 1.2.2.0 with <see cref="AesKeySize.Strong"/>, 50000
    /// iterations and SHA256, using <see cref="Password"/> and <see cref="Salt"/>. Version 1.2.3.0 replaced the
    /// obsolete Rfc2898DeriveBytes constructor with the static Pbkdf2 method, this value proves that the key
    /// derivation did not change. Do not regenerate it, that would defeat its purpose.
    /// </summary>
    public const string EncryptedByVersion122Strong = "52p1xqEjqa1Gne1/XsYOWpfzMU86TPsEjKDomy2A48jLAfmxWJPEmI9zWF5Cb7lI";

    /// <summary>
    /// The <see cref="PlainText"/> encrypted by version 1.2.2.0 with <see cref="AesKeySize.Weak"/>, 1000 iterations
    /// and SHA512, using <see cref="Password"/> and <see cref="Salt"/>. The parameters differ from
    /// <see cref="EncryptedByVersion122Strong"/> on purpose: with SHA512 the key and the initialization vector both
    /// come from the first hash block, with SHA256 they span two blocks. Do not regenerate it.
    /// </summary>
    public const string EncryptedByVersion122Weak = "EoELqsZ+DfXv/9OONo4nvz6ZniiRLSpeg55Z84jCBr0BKPb4/Sge8uIwK1tRylTW";
}
