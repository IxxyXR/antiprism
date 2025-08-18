# Unity Plugin

The Unity wrapper builds a shared library named `libantiprism_unity`.
After a successful build it will be placed in `unity/.libs/` with a
platform‑specific extension:

- Windows: `libantiprism_unity.dll`
- macOS: `libantiprism_unity.dylib`
- Linux: `libantiprism_unity.so`

## Building

From the project root run:

```
./bootstrap
./configure --disable-antiview
make -j2
```

## Using with Unity

1. Copy the library from `unity/.libs/` into your Unity project’s
   `Assets/Plugins/x86_64/` directory (on macOS use `Assets/Plugins/`).
2. Copy the Antiprism command‑line executables you intend to call
   (e.g. `off_align`, `conway`) next to the library or ensure they are on
   your `PATH`.
3. Import the functions in C# and call `antiprism_command` with the desired
   command line. Free the returned buffer with `antiprism_free`.

A minimal example script is provided in `AntiprismExample.cs`:

```csharp
[DllImport("antiprism_unity", CallingConvention = CallingConvention.Cdecl)]
static extern int antiprism_command(string cmd, out IntPtr output);

[DllImport("antiprism_unity", CallingConvention = CallingConvention.Cdecl)]
static extern void antiprism_free(IntPtr buffer);
```

## Platform Support

The wrapper library is built and tested on Windows (MSYS2/MinGW), macOS,
and Linux. Use the appropriate file extension for your target platform and
place it in the corresponding `Assets/Plugins` subdirectory required by
Unity.
