# EigeneEdition (custom edition)

Adds a custom-built profile edition to the selection shown when creating a profile, alongside Standard, Unheard, etc.

Existing profiles are left untouched; the edition only applies to newly created profiles.

## Requirements

- SPT 4.1.6 (server).

## Installation

1. Download `EigeneEdition-1.0.0.zip` from the [Releases](https://github.com/Kaanori/spt-eigeneedition/releases) page.
2. Extract it **into your SPT game folder** (the folder that contains `EscapeFromTarkov.exe`).
   The ZIP already has the right layout; the mod ends up in `SPT_Runtime/user/mods/EigeneEdition/`.
   On Windows, make sure "Extract All" does not add an extra subfolder.
3. Restart the SPT server.

## Uninstall

Delete the mod's files from `SPT_Runtime/user/mods/EigeneEdition/` (settings: `BepInEx/config/` for client mods) and restart the SPT server.

## Building from source

Needs the .NET 10 SDK and an SPT 4.1.6 installation to build against.

```bash
./build.sh             # Linux: build only
./build.sh --install   # build and copy into the game folder
```
```powershell
.\build.ps1            # Windows
.\build.ps1 -Install
```

The game folder is set in `Directory.Build.props` (Windows default `C:/SPT`)
or via the environment variable `SPT_DIR`.

## Support

Bugs and questions: [GitHub issues](https://github.com/Kaanori/spt-eigeneedition/issues).

## License

MIT — see [LICENSE](LICENSE).
