AESCryptoLib
====================================

AESCryptoLib is an assembly/ library to encrypt strings with AES.
The assembly was written and tested in .Net 4.8 and .Net Standard 2.0.

[![Build status](https://ci.appveyor.com/api/projects/status/a3ptt999etgpfutv?svg=true)](https://ci.appveyor.com/project/SeppPenner/aescryptolib)
[![GitHub issues](https://img.shields.io/github/issues/SeppPenner/AESCryptoLib.svg)](https://github.com/SeppPenner/AESCryptoLib/issues)
[![GitHub forks](https://img.shields.io/github/forks/SeppPenner/AESCryptoLib.svg)](https://github.com/SeppPenner/AESCryptoLib/network)
[![GitHub stars](https://img.shields.io/github/stars/SeppPenner/AESCryptoLib.svg)](https://github.com/SeppPenner/AESCryptoLib/stargazers)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://raw.githubusercontent.com/SeppPenner/AESCryptoLib/master/License.txt)
[![Nuget](https://img.shields.io/badge/AESCryptoLib-Nuget-brightgreen.svg)](https://www.nuget.org/packages/HaemmerElectronics.SeppPenner.AESCryptoLib/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/HaemmerElectronics.SeppPenner.AESCryptoLib.svg)](https://www.nuget.org/packages/HaemmerElectronics.SeppPenner.AESCryptoLib/)
[![Known Vulnerabilities](https://snyk.io/test/github/SeppPenner/AESCryptoLib/badge.svg)](https://snyk.io/test/github/SeppPenner/AESCryptoLib)
[![Gitter](https://badges.gitter.im/AESCryptoLib/community.svg)](https://gitter.im/AESCryptoLib/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

## Available for
* NetFramework 4.5
* NetFramework 4.6
* NetFramework 4.6.2
* NetFramework 4.7
* NetFramework 4.7.2
* NetFramework 4.8
* NetStandard 2.0
* NetStandard 2.1
* NetCore 2.2
* NetCore 3.0

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

* **Version 1.0.8.0 (2019-11-08)** : Updated nuget packages.
* **Version 1.0.6.0 (2019-10-28)** : Updated nuget packages, added GitVersionTask, fixed bugs with package.
* **Version 1.0.5.0 (2019-06-23)** : Added icon to the nuget package.
* **Version 1.0.0.4 (2019-05-05)** : Updated .Net version to 4.8.
* **Version 1.0.0.3 (2018-03-15)** : Fixed bug with different .Net versions in the Nuget package.
* **Version 1.0.0.2 (2018-02-11)** : Switched to .Net to 4.7.
* **Version 1.0.0.1 (2017-08-16)** : Updated nuget project file and added documentation.
* **Version 1.0.0.0 (2017-05-16)** : 1.0 release.
