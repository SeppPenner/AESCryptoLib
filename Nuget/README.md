AESCryptoLib
====================================

AESCryptoLib is an assembly/ library to encrypt strings with AES.
The assembly was written and tested in .Net 4.6.2.

[![Build status](https://ci.appveyor.com/api/projects/status/a3ptt999etgpfutv?svg=true)](https://ci.appveyor.com/project/SeppPenner/aescryptolib)

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

Change history
--------------

* **Version 1.0.0.0 (2017-05-16)** : 1.0 release.
