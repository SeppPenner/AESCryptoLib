// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AesKeySize.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   The <see cref="AesKeySize" /> to be set for the encryption method.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AESCryptoLib;

/// <summary>
///     The <see cref="AesKeySize" /> to be set for the encryption method.
/// </summary>
public enum AesKeySize
{
    /// <summary>
    /// The strong encryption type.
    /// </summary>
    Strong = 256,

    /// <summary>
    /// The weaker encryption type.
    /// </summary>
    Weak = 128
}
