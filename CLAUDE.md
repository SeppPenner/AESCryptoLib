# Project rules for Claude

## What this is

AESCryptoLib is a small class library that encrypts and decrypts strings with AES. It is published as
the NuGet package
[HaemmerElectronics.SeppPenner.AESCryptoLib](https://www.nuget.org/packages/HaemmerElectronics.SeppPenner.AESCryptoLib/).
`GeneratePackageOnBuild` is enabled, so **every** build writes a `.nupkg` and a `.snupkg` into
`src/AESCryptoLib/bin/Release`. There is no application and no installer in this repository, the
library is the product.

One solution `src/AESCryptoLib.sln` with exactly two projects:

- `src/AESCryptoLib/AESCryptoLib.csproj`, the library, `TargetFrameworks` `net8.0;net10.0`, the only
  packable project.
- `src/AESCryptoLib.Tests/AESCryptoLib.Tests.csproj`, MSTest, `net10.0`, `IsPackable` `false`, added
  in version 1.2.3.0.

Layout inside `src/AESCryptoLib`:

- `Crypter.cs` plus `ICrypter.cs`: the public entry point with `Encrypt`, `Decrypt` and
  `GetRandomSalt`. The private static helper `CreateAes` derives the key and the initialization
  vector and hands back the configured `Aes` instance, both public methods use it. Keep new logic in
  that shape.
- `AesKeySize.cs`: the enum with `Strong = 256` and `Weak = 128`.
- `GlobalUsings.cs`: all usings of the project.

Layout inside `src/AESCryptoLib.Tests`:

- `CrypterTests.cs`: the roundtrip for both key sizes, the empty string, characters outside the basic
  multilingual plane, the base64 shape, the missing randomness of the result, the default parameters,
  a wrong password, a wrong salt, a wrong iteration count, a wrong hash algorithm, an input that is
  no base64, the two ciphertexts of version 1.2.2.0 and the three properties of `GetRandomSalt`.
- `TestDataProvider.cs`: the password, the salt, the plain text and the two ciphertexts that version
  1.2.2.0 produced from them. Those two strings are the compatibility proof for the key derivation.
  Do not regenerate them, that would defeat their purpose.
- `GlobalUsings.cs`: all usings of the test project.

`src/Directory.Build.props` holds nothing but `GenerateDocumentationFile`, and it applies to both
projects.

Repository root: `README.md` (the only user documentation, with the usage sample), `Changelog.md`,
`Updating.md` (the release steps), `License.txt` (MIT), `Icon.png` (the package icon), `Secure.ico`,
`BuildAndPushPackage.bat`, `Delete-BIN-OBJ-Folders.bat` and `.gitattributes`. There is no
`HowToUse.md` and no screenshots.

## Build

```powershell
dotnet build src/AESCryptoLib.sln -c Release
```

```powershell
dotnet test src/AESCryptoLib.sln -c Release
```

- Two target frameworks `net8.0;net10.0` in the library, `net10.0` in the test project, no
  `RuntimeIdentifiers`. Nothing in the code is Windows specific.
- All build properties live directly in the two `.csproj` files and are duplicated there.
  `src/Directory.Build.props` adds `GenerateDocumentationFile` on top, for the test project as well.
  A `cref` that cannot be resolved therefore breaks the **test** build too, and the test project
  imports fewer namespaces than the library.
- `TreatWarningsAsErrors` is enabled in both projects, so every warning breaks the build, NuGet
  warnings (`NU****`) from restore included. A clean build reports zero warnings, keep it that way.
- `NU1803` (HTTP source usage during restore) is the one warning suppressed via `NoWarn`. It is
  needed because the machine has an HTTP feed configured globally next to nuget.org. Fix warnings
  instead of extending that list. `NuGetAudit` and `NuGetAuditMode=all` are on in both projects, so
  a vulnerable transitive package fails the build too.
- Versions come from GitVersion.MsBuild out of the git tags, for example `1.2.3-2` for the second
  commit after tag `1.2.2`. Never edit a version property or an assembly version by hand.
- Restore needs nuget.org. If a private feed is configured globally on the machine and answers 404
  for public packages, restore fails with `NU1301`. Then build with an explicit source:
  `dotnet build src/AESCryptoLib.sln --source https://api.nuget.org/v3/index.json`.
- Tests are MSTest, in the single test project `src/AESCryptoLib.Tests`, which follows the same
  package set as the sibling repositories: `Microsoft.NET.Test.Sdk`, `MSTest.TestAdapter`,
  `MSTest.TestFramework`, `coverlet.collector` and `GitVersion.MsBuild`. `dotnet test` runs 18 tests,
  they need no network and no fixture outside the repository. Never claim a test run happened without
  running it.
- The tests only ever run against `net10.0`, `net8.0` is compile verified. Executing them on `net8.0`
  needs a .NET 8 runtime, which is not installed on the development machine.

## Code conventions

Follow the surrounding code, it is consistent throughout every file:

- File header comment block with `<copyright file="..." company="Hämmer Electronics">` and a
  `<summary>`, then the file-scoped namespace.
- XML doc comments on every type and every member, private members included, no exceptions.
  Implementations of an interface member additionally carry `<inheritdoc cref="..."/>` and
  `<seealso cref="..."/>` pointing at that interface. The existing files write those two tags with an
  explicit closing tag (`<inheritdoc cref="ICrypter"></inheritdoc>`), leave that alone.
- `Nullable`, `ImplicitUsings` and `LangVersion latest` are enabled.
- New `using` directives go into the `GlobalUsings.cs` of the respective project, inside the
  existing `#pragma warning disable IDE0065` block, never at the top of a file. The editorconfig
  requires usings inside the namespace (`csharp_using_directive_placement=inside_namespace:warning`),
  which global usings cannot satisfy, that is what the pragma is for. Do not add other pragmas. The
  comment text in that block is German because Visual Studio generated it, leave it alone.
- Fields, properties, methods and events are always accessed with `this.` qualification
  (`dotnet_style_qualification_for_*` at severity `warning`).
- `src/.editorconfig` also enforces braces everywhere, no multiple blank lines, four spaces, CRLF,
  UTF-8, file scoped namespaces, `System` usings sorted first and `IDE0005` as warning. Analyzer
  warnings are fixed, not silenced.

## Known quirks

Do not silently "clean up" these, they are existing behaviour:

- **UTF-32 everywhere.** The plain text and the salt string are both converted with
  `Encoding.UTF32`, the encrypted result is base64. Switching either one to UTF-8 changes the derived
  key and makes every string encrypted by an earlier version unreadable.
- **The key and the initialization vector come from one PBKDF2 output.** The key takes the first
  `KeySize / 8` bytes, the initialization vector the next 16. Up to version 1.2.2.0 that was two
  `GetBytes` calls on an `Rfc2898DeriveBytes` instance, since 1.2.3.0 it is one
  `Rfc2898DeriveBytes.Pbkdf2` call over the sum of both lengths, which yields the same bytes. The
  change was not optional: on `net10.0` the constructors are obsolete (`SYSLIB0060`) and every
  warning is an error here. The two ciphertexts in `TestDataProvider` are what proves the derivation
  did not change, one of them with SHA512 where key and vector share the first hash block, one with
  SHA256 where they span two.
- **There is no random initialization vector.** The vector is derived from the password and the salt,
  so the same password with the same salt always produces the same ciphertext. That is why
  `GetRandomSalt` exists and why a salt value must not be reused for a second secret.
- **`GetRandomSalt` returned an empty string up to version 1.2.2.0.** It asked `alg?.Hash is null`
  before calling `ComputeHash`, and `Hash` is `null` on a freshly created `SHA512`, so the method
  returned `string.Empty` on every call and everyone following the readme encrypted without a salt.
  Since 1.2.3.0 it returns 64 bytes from `RandomNumberGenerator` in the same shape as before, upper
  case hex pairs separated by dashes, 191 characters.
- **The `AesKeySize` values are the key length in bits** and are cast straight to `Aes.KeySize`.
  Renaming the members is safe, changing the numbers is not.
- **The package id is not the assembly name.** Package `HaemmerElectronics.SeppPenner.AESCryptoLib`,
  assembly and root namespace `AESCryptoLib`.
- **The package carries four files from the repository root.** `Icon.png`, `License.txt`, `README.md`
  and `Changelog.md` are packed via `None Include` with `Pack=true`, `README.md` additionally as
  `PackageReadmeFile`. Renaming or moving one of them breaks the pack step.
- **`PackageRequireLicenseAcceptance` is `true`** although the license is MIT. NuGet shows the
  acceptance prompt because of it. It has been that way since the first release.
- **`Secure.ico`** is tracked in the root and referenced nowhere, not by a project, not by the
  package. Leave it alone.
- **`BuildAndPushPackage.bat` ends with `PAUSE`** and starts with `cd .\src`, so it has to be
  started from the repository root and it blocks until a key is pressed. Non interactively it needs
  its standard input redirected from `nul`, otherwise it never returns. It deletes every `bin` and
  `obj` folder below `src` first, then restores, builds `Release` and pushes both the `.nupkg` and
  the `.snupkg` to the sources named `nuget.org` and `github`, using `%NUGET_API_KEY%` and
  `%GITHUB_API_KEY%` and `--skip-duplicate`. Both source names have to exist in the machine wide
  NuGet configuration.
- **The release notes are duplicated.** `PackageReleaseNotes` in the `.csproj` repeats the top entry
  of `Changelog.md` in a slightly different format. Both have to be updated for a release.
- **AppVeyor badge without CI in the repository.** `README.md` links an AppVeyor build that is
  configured outside of this repository. There is no `.github` folder and no pipeline file here.
- **`src/AESCryptoLib.sln.DotSettings`** is tracked and holds nothing but a ReSharper user dictionary
  (`Crypter`, `Haemmer`, `H_00E4mmer`). Leave it alone.
- **`.gitattributes` sets `* text=auto`** and every rule of the Visual Studio template below it is
  commented out. Any binary file added later needs its own rule.

## Releasing

1. Make the change.
2. Add an entry at the top of `Changelog.md` in the existing format:
   `* **Version 1.2.3.0 (2026-08-10)** : Short description.`
3. Put the same text into `PackageReleaseNotes` in `src/AESCryptoLib/AESCryptoLib.csproj`, in the
   format used there: `Version 1.2.3.0 (2026-08-10): Short description.`
4. Commit that.
5. Tag the commit with the plain version number, no `v` prefix (`1.2.2`, `1.2.1`, ...). The existing
   tags are lightweight tags, create new ones the same way.
6. Push the commits and the tag.
7. Run `BuildAndPushPackage.bat` from the repository root.

The version in the `Changelog.md` has four parts (`1.2.3.0`), the tag has three (`1.2.3`).
GitVersion turns the tag into the assembly version and into the version of the package file, so the
tag has to exist **before** the package build. Without it the package is called `1.2.3-2.nupkg` and
carries a prerelease version.

## Git

- **Never amend a commit.** No `git commit --amend`, not for a typo in the message, not to add a
  forgotten file, not even when the commit is still local. Write a follow-up commit instead. The
  release versions come from tags on exact commits, an amended commit leaves its tag pointing at a
  commit that no longer exists in the branch.

## Writing style

- Commit messages are written **in English only**: short, precise subject line, explanatory body
  when needed.
- Code comments and comments in project files such as `.csproj` are **always English**, regardless
  of the language used in the conversation.
- **No em dashes or en dashes** (`—`, `–`), neither in prose, commit messages, code comments nor
  documentation. Use a regular hyphen, comma, colon, parentheses or a separate sentence.
- German texts (documentation, chat replies) always use real umlauts and ß, never ASCII
  transliterations such as `ae`, `oe`, `ue` or `ss`. Identifiers, file names and configuration keys
  stay unchanged where umlauts are technically undesirable.
