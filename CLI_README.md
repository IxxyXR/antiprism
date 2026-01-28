# Antiprism CLI (Binary Distribution)

This package is a self-contained CLI bundle for Antiprism.

## macOS note (quarantine)
If you downloaded this zip/tar from the internet, macOS may quarantine the files.
If the executables refuse to launch, remove quarantine on the extracted folder:

```bash
xattr -dr com.apple.quarantine "/path/to/antiprism-cli-macos"
```

The macOS package may include `unquarantine.command` you can double-click to
run this automatically.

## Usage
The executables are in `bin/`. Example:

```bash
bin/zono -P 7
bin/symmetro -t "I[5,3],1,2" -m "map_blue:purple:red:orange" -r 1.07046626932,1 | bin/antiview
```

The `data/` folder is bundled and will be used automatically (no need to set
`ANTIPRISM_DATA`).
