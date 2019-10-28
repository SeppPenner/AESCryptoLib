dotnet build AesCryptoLib.sln -c Release
xcopy /s .\AesCryptoLib\bin\Release ..\Nuget\Source\
pause