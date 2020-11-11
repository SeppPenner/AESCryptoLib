dotnet nuget push "src\AESCryptoLib\bin\Release\HaemmerElectronics.SeppPenner.AESCryptoLib.*.nupkg" -s "github" --skip-duplicate
dotnet nuget push "src\AESCryptoLib\bin\Release\HaemmerElectronics.SeppPenner.AESCryptoLib.*.nupkg" -s "nuget.org" --skip-duplicate -k "%NUGET_API_KEY%"
PAUSE