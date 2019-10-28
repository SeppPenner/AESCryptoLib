dotnet build AESCryptoLib.sln -c Release
xcopy /s .\AESCryptoLib\bin\Release ..\Nuget\Source\
pause