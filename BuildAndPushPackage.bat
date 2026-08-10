cd .\src

@ECHO off
cls

FOR /d /r . %%d in (bin,obj) DO (
	IF EXIST "%%d" (		 	 
		ECHO %%d | FIND /I "\node_modules\" > Nul && ( 
			ECHO.Skipping: %%d
		) || (
			ECHO.Deleting: %%d
			rd /s/q "%%d"
		)
	)
)

@ECHO on
@ECHO.Building solution...
@dotnet restore
@dotnet build -c Release
@cd .\AESCryptoLib\bin\Release
@ECHO.Build successful.
@REM The pushes stay unechoed on purpose, ECHO on would print the expanded API keys to the console.
@dotnet nuget push *.nupkg -s "nuget.org" --skip-duplicate -k "%NUGET_API_KEY%"
@dotnet nuget push *.snupkg -s "nuget.org" --skip-duplicate -k "%NUGET_API_KEY%"
@dotnet nuget push *.nupkg -s "github" --skip-duplicate --api-key "%GITHUB_API_KEY%"
@dotnet nuget push *.snupkg -s "github" --skip-duplicate --api-key "%GITHUB_API_KEY%"
@ECHO.Upload success. Press any key to exit.
PAUSE