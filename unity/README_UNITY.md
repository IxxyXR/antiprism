# Antiprism Unity Plugin

This directory contains the Unity integration for the Antiprism polyhedra library, allowing you to use Antiprism's powerful polyhedra generation and manipulation capabilities directly in Unity, including Android builds.

## Overview

The Antiprism Unity plugin provides:
- C FFI (Foreign Function Interface) wrapper for the C++ library
- C# classes for easy Unity integration
- Support for all platforms including Android, iOS, Windows, macOS, and Linux
- Example scripts demonstrating usage

## Files

- `antiprism_c_api.h` - C API header (in ../base/)
- `antiprism_c_api.cc` - C API implementation (in ../base/)
- `AntiprismPlugin.cs` - C# wrapper for Unity
- `PolyhedronExample.cs` - Example Unity MonoBehaviour script
- `README_UNITY.md` - This file

## Building the Library

### Desktop Platforms (Linux, macOS, Windows)

1. **Configure the build:**
   ```bash
   cd /path/to/antiprism
   ./configure
   ```

2. **Build the library:**
   ```bash
   make
   ```

3. **Install (optional):**
   ```bash
   sudo make install
   ```

4. **The library will be:**
   - Linux: `base/.libs/libantiprism.so`
   - macOS: `base/.libs/libantiprism.dylib`
   - Windows: `base/.libs/libantiprism.dll` (or `antiprism.dll`)

### Android

For Android builds, you need to cross-compile for each architecture:

1. **Install Android NDK** (if not already installed)

2. **Set up NDK environment:**
   ```bash
   export ANDROID_NDK=/path/to/android-ndk
   export TOOLCHAIN=$ANDROID_NDK/toolchains/llvm/prebuilt/linux-x86_64
   ```

3. **Build for each architecture:**

   **ARMv7:**
   ```bash
   ./configure --host=armv7a-linux-androideabi21 \
       CC=$TOOLCHAIN/bin/armv7a-linux-androideabi21-clang \
       CXX=$TOOLCHAIN/bin/armv7a-linux-androideabi21-clang++ \
       --disable-shared --enable-static
   make clean && make
   cp base/.libs/libantiprism.a build/android/armeabi-v7a/
   ```

   **ARM64:**
   ```bash
   ./configure --host=aarch64-linux-android21 \
       CC=$TOOLCHAIN/bin/aarch64-linux-android21-clang \
       CXX=$TOOLCHAIN/bin/aarch64-linux-android21-clang++ \
       --disable-shared --enable-static
   make clean && make
   cp base/.libs/libantiprism.a build/android/arm64-v8a/
   ```

   **x86:**
   ```bash
   ./configure --host=i686-linux-android21 \
       CC=$TOOLCHAIN/bin/i686-linux-android21-clang \
       CXX=$TOOLCHAIN/bin/i686-linux-android21-clang++ \
       --disable-shared --enable-static
   make clean && make
   cp base/.libs/libantiprism.a build/android/x86/
   ```

   **x86_64:**
   ```bash
   ./configure --host=x86_64-linux-android21 \
       CC=$TOOLCHAIN/bin/x86_64-linux-android21-clang \
       CXX=$TOOLCHAIN/bin/x86_64-linux-android21-clang++ \
       --disable-shared --enable-static
   make clean && make
   cp base/.libs/libantiprism.a build/android/x86_64/
   ```

### iOS

For iOS, use Xcode and the iOS toolchain:

```bash
./configure --host=arm-apple-darwin \
    CC=$(xcrun --sdk iphoneos --find clang) \
    CXX=$(xcrun --sdk iphoneos --find clang++) \
    CFLAGS="-arch arm64 -mios-version-min=11.0" \
    CXXFLAGS="-arch arm64 -mios-version-min=11.0" \
    --disable-shared --enable-static
make clean && make
```

## Unity Integration

### 1. Copy Files to Unity Project

Copy the compiled library to your Unity project's Plugins folder:

```
YourUnityProject/
├── Assets/
│   ├── Plugins/
│   │   ├── Android/
│   │   │   ├── arm64-v8a/
│   │   │   │   └── libantiprism.so
│   │   │   ├── armeabi-v7a/
│   │   │   │   └── libantiprism.so
│   │   │   ├── x86/
│   │   │   │   └── libantiprism.so
│   │   │   └── x86_64/
│   │   │       └── libantiprism.so
│   │   ├── iOS/
│   │   │   └── libantiprism.a
│   │   ├── x86_64/
│   │   │   └── libantiprism.bundle (macOS)
│   │   └── x86/
│   │       └── antiprism.dll (Windows)
│   ├── Scripts/
│   │   ├── Antiprism/
│   │   │   ├── AntiprismPlugin.cs
│   │   │   └── PolyhedronExample.cs
```

### 2. Configure Plugin Import Settings

In Unity, select each library file and configure the import settings:

**For Android libraries:**
- Platform: Android
- CPU: Set to appropriate architecture (ARMv7, ARM64, x86, x86_64)

**For iOS libraries:**
- Platform: iOS
- Check "Add to Embedded Binaries"

**For desktop libraries:**
- Platform: Set to appropriate platform (Standalone Windows, macOS, Linux)

### 3. Use in Your Scripts

```csharp
using UnityEngine;
using Antiprism;

public class MyPolyhedron : MonoBehaviour
{
    void Start()
    {
        // Create a geometry
        using (var geom = new Geometry())
        {
            // Load a built-in polyhedron
            geom.LoadResource("ico");  // icosahedron

            // Transform it
            geom.Unitize();  // Normalize to unit sphere
            geom.Scale(2.0f);  // Scale to radius 2

            // Apply to Unity mesh
            Mesh mesh = new Mesh();
            geom.ApplyToMesh(mesh);

            GetComponent<MeshFilter>().mesh = mesh;
        }
    }
}
```

## Available Polyhedra

The `LoadResource()` method supports many built-in polyhedra:

### Platonic Solids
- `"tet"` - Tetrahedron
- `"cube"` - Cube
- `"oct"` - Octahedron
- `"ico"` - Icosahedron
- `"dodec"` - Dodecahedron

### Other Common Polyhedra
- `"tru_oct"` - Truncated octahedron
- `"tru_ico"` - Truncated icosahedron (soccer ball)
- `"rhomb_dodec"` - Rhombic dodecahedron
- Many more! (See Antiprism documentation for full list)

## API Reference

### AntiprismPlugin Class

Static methods for library information:

```csharp
string GetVersion()  // Returns library version
string GetInfo()     // Returns library information
```

### Geometry Class

Polyhedron manipulation class:

```csharp
// Construction
Geometry()  // Create empty geometry

// Loading
Status LoadResource(string name)  // Load built-in polyhedron

// Query
int VertexCount { get; }  // Number of vertices
int FaceCount { get; }    // Number of faces
Vector3[] GetVertices()   // Get all vertices
int[] GetTriangles()      // Get triangulated face indices

// Transformations
Status Scale(float scale)
Status Translate(Vector3 offset)
Status RotateX(float angle)  // Angle in radians
Status RotateY(float angle)
Status RotateZ(float angle)
Status Unitize()  // Normalize to unit sphere

// Operations
Status ConvexHull()  // Calculate convex hull
Status Orient()      // Orient faces consistently

// Unity Integration
void ApplyToMesh(Mesh mesh)  // Apply geometry to Unity mesh
```

## Example: Procedural Polyhedron Generator

```csharp
using UnityEngine;
using Antiprism;

public class ProceduralPolyhedron : MonoBehaviour
{
    public string[] polyhedronTypes = { "tet", "cube", "oct", "ico", "dodec" };
    public float spacing = 3.0f;

    void Start()
    {
        for (int i = 0; i < polyhedronTypes.Length; i++)
        {
            CreatePolyhedron(polyhedronTypes[i], new Vector3(i * spacing, 0, 0));
        }
    }

    void CreatePolyhedron(string type, Vector3 position)
    {
        GameObject go = new GameObject(type);
        go.transform.position = position;

        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Standard"));

        using (var geom = new Geometry())
        {
            geom.LoadResource(type);
            geom.Unitize();

            Mesh mesh = new Mesh();
            geom.ApplyToMesh(mesh);
            mf.mesh = mesh;
        }
    }
}
```

## Troubleshooting

### Library Not Found

**Error:** `DllNotFoundException: antiprism`

**Solution:**
1. Ensure the library is in the correct Plugins folder
2. Check the import settings for the library
3. For Android, ensure you have libraries for all required architectures
4. Try renaming the library (e.g., `antiprism.dll` vs `libantiprism.dll`)

### Android Build Errors

**Error:** `UnsatisfiedLinkError`

**Solution:**
1. Ensure you built for all required architectures (arm64-v8a, armeabi-v7a)
2. Check that libraries are in the correct subfolders
3. In Unity, go to Player Settings → Android → Other Settings and ensure "Target Architectures" matches your libraries

### Memory Issues

Always use `using` statements with Geometry objects:

```csharp
// Good
using (var geom = new Geometry()) {
    // Use geometry
}

// Bad - may leak memory
var geom = new Geometry();
// ... forgot to dispose
```

## Performance Tips

1. **Reuse Geometry objects** when generating multiple meshes with transformations
2. **Cache meshes** instead of regenerating every frame
3. **Use Unitize()** to normalize polyhedra before scaling for consistent sizing
4. **Triangulate in Antiprism** rather than Unity for better performance

## Advanced Usage

### Custom Transformations

```csharp
using (var geom = new Geometry())
{
    geom.LoadResource("ico");

    // Multiple transformations
    geom.RotateX(Mathf.PI / 4);
    geom.RotateY(Mathf.PI / 6);
    geom.Scale(2.0f);
    geom.Translate(new Vector3(0, 5, 0));

    // Apply to mesh
    Mesh mesh = new Mesh();
    geom.ApplyToMesh(mesh);
}
```

### Dynamic Mesh Updates

```csharp
public class AnimatedPolyhedron : MonoBehaviour
{
    private Mesh mesh;
    private float time;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    void Update()
    {
        time += Time.deltaTime;

        using (var geom = new Geometry())
        {
            geom.LoadResource("ico");
            geom.Unitize();
            geom.Scale(1.0f + Mathf.Sin(time) * 0.2f);
            geom.ApplyToMesh(mesh);
        }
    }
}
```

## License

Antiprism is licensed under the MIT License. See the main LICENSE file for details.

## Support

For issues, questions, or contributions:
- Antiprism website: http://www.antiprism.com
- GitHub: https://github.com/antiprism/antiprism

## Credits

- Antiprism library by Adrian Rossiter
- Unity C API wrapper and integration
