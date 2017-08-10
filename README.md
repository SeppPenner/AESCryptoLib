AESCryptoLib
====================================

AESCryptoLib is an assembly/ library to encrypt strings with AES.
The assembly was written and tested in .Net 4.6.2.

[![Build status](https://ci.appveyor.com/api/projects/status/a3ptt999etgpfutv?svg=true)](https://ci.appveyor.com/project/SeppPenner/aescryptolib)
[![GitHub issues](https://img.shields.io/github/issues/SeppPenner/AESCryptoLib.svg)](https://github.com/SeppPenner/AESCryptoLib/issues)
[![GitHub forks](https://img.shields.io/github/forks/SeppPenner/AESCryptoLib.svg)](https://github.com/SeppPenner/AESCryptoLib/network)
[![GitHub stars](https://img.shields.io/github/stars/SeppPenner/AESCryptoLib.svg)](https://github.com/SeppPenner/AESCryptoLib/stargazers)
[![GitHub license](https://img.shields.io/badge/license-AGPL-blue.svg)](https://raw.githubusercontent.com/SeppPenner/AESCryptoLib/master/License.txt)
[![Nuget](https://img.shields.io/badge/AESCryptoLib-Nuget-brightgreen.svg)](https://www.nuget.org/packages/HaemmerElectronics.SeppPenner.AESCryptoLib/)

## Basic usage:
```csharp
public void Test()
{
	ICrypter cryptor = new Crypter();
	string salt = cryptor.GetRandomSalt();
	string encrypted = cryptor.Encrypt(AesKeySize.Strong, "Test", "Password", salt);
	string decrypted = cryptor.Decrypt(AesKeySize.Strong, encrypted, "Password", salt);
}
```

The project can be found on [nuget](https://www.nuget.org/packages/HaemmerElectronics.SeppPenner.AESCryptoLib/).

Change history
--------------

* **Version 1.0.0.0 (2017-05-16)** : 1.0 release.
