# Antiprism Unity Plugin

This directory provides a Unity native plugin that links against the Antiprism
library. Individual command-line tools are compiled into the static library
`libantiprism_unity` and exposed through an `antiprism_command` C interface.
The plugin executes Antiprism tools directly without spawning external
processes or copying command-line executables. Commands are invoked with
separate parameters for the tool name, OFF input text, and command-line
options, so no temporary files are required.

## Building a standalone plug-in

1. **Build the Antiprism core as static libraries**
   ```sh
   ./bootstrap
   ./configure --disable-shared
   make
   ```
   After `make` completes, the static libraries are placed in
   `base/.libs/` and its subdirectories (`libantiprism.a`, `libqhull.a`,
   `libtess.a`, `libmuparser.a`).

2. **Build the wrapper library that contains the command-line tools**
   ```sh
   cmake -S unity/libantiprism_unity -B build
   cmake --build build
   ```
   This creates `build/libantiprism_unity.a`.

3. **Link everything into a shared library for Unity**
   ```sh
   c++ -std=c++17 -fPIC -shared unity/unity_plugin.cc \
       build/libantiprism_unity.a \
       base/.libs/libantiprism.a \
       base/qhull/.libs/libqhull.a \
       base/tesselator/.libs/libtess.a \
       base/muparser/.libs/libmuparser.a \
       -I base -I src -I src_extra \
       -o antiprism_unity.dll   # use .so on Linux, .dylib on macOS
   ```
   The resulting library is self‑contained and can be dropped into a
   Unity project without additional C or C++ code.

## Using from Unity

Place the generated library in `Assets/Plugins` and call the function via
P/Invoke:

```csharp
using System.Runtime.InteropServices;

public static class Antiprism
{
    [DllImport("antiprism_unity")] 
    private static extern System.IntPtr antiprism_command(
        string command, string input, string options);

    public static string Run(string command, string input = "", string options = "")
    {
        return Marshal.PtrToStringAnsi(antiprism_command(command, input, options));
    }
}
```

You can now run any exposed tool directly from C#. Pass command-line arguments
in `options` exactly as you would on the command line. If a command expects an
OFF file, include `-` in `options` and supply the OFF text via the `input`
parameter:

```csharp
string offData = "OFF\n0 0 0\n"; // minimal example
string result = Antiprism.Run("off_align", offData, "-");
```
