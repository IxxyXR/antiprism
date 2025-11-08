# Building Antiprism as a Unity Plugin

This guide covers building Antiprism as a shared library with C FFI for use as a Unity plugin, including Android builds.

## Overview

Antiprism can now be built as a DLL/shared library with a C-compatible API, making it suitable for Unity and other game engines. The library includes:

- C FFI (Foreign Function Interface) wrapper around the C++ core
- Support for all major platforms (Windows, macOS, Linux, Android, iOS)
- Unity C# wrapper for easy integration
- Example scripts

## Prerequisites

### All Platforms
- C++14 compatible compiler (GCC 4.9+, Clang 3.4+, MSVC 2015+)
- Make or CMake build system

### Linux
```bash
sudo apt-get install build-essential automake autoconf libtool
```

### macOS
```bash
xcode-select --install
brew install automake autoconf libtool
```

### Windows
- Visual Studio 2015 or later with C++ tools
- Or MinGW-w64

### Android
- Android NDK r21 or later
- CMake 3.10+

### iOS
- Xcode with iOS SDK
- Command Line Tools

## Build Methods

Antiprism supports two build systems:

1. **Autotools** (./configure && make) - Traditional, well-tested
2. **CMake** - Modern, better for cross-platform builds

Choose the method that works best for your workflow.

---

## Method 1: Autotools Build

### Linux / macOS

1. **Configure:**
   ```bash
   cd /path/to/antiprism
   ./autogen.sh  # Only if building from git
   ./configure
   ```

2. **Build:**
   ```bash
   make -j$(nproc)
   ```

3. **Find the library:**
   - Linux: `base/.libs/libantiprism.so`
   - macOS: `base/.libs/libantiprism.dylib`

### Windows (MinGW/MSYS2)

1. **Install MSYS2** from https://www.msys2.org/

2. **Install dependencies:**
   ```bash
   pacman -S base-devel mingw-w64-x86_64-gcc mingw-w64-x86_64-autotools
   ```

3. **Build:**
   ```bash
   ./configure
   make
   ```

4. **Find the library:**
   - `base/.libs/antiprism.dll` or `base/.libs/libantiprism.dll`

---

## Method 2: CMake Build (Recommended for Unity)

CMake provides better cross-platform support and is easier for Unity plugin builds.

### Desktop Build (Windows/macOS/Linux)

1. **Create build directory:**
   ```bash
   cd /path/to/antiprism
   mkdir build
   cd build
   ```

2. **Configure:**
   ```bash
   # Linux/macOS
   cmake .. -DBUILD_SHARED_LIBS=ON -DBUILD_UNITY_PLUGIN=ON

   # Windows (Visual Studio)
   cmake .. -G "Visual Studio 16 2019" -A x64 -DBUILD_SHARED_LIBS=ON -DBUILD_UNITY_PLUGIN=ON

   # Windows (MinGW)
   cmake .. -G "MinGW Makefiles" -DBUILD_SHARED_LIBS=ON -DBUILD_UNITY_PLUGIN=ON
   ```

3. **Build:**
   ```bash
   cmake --build . --config Release
   ```

4. **Find the library:**
   - Linux: `build/libantiprism.so`
   - macOS: `build/libantiprism.dylib`
   - Windows: `build/Release/antiprism.dll`

### Android Build

Android requires building for multiple architectures. Here's a complete script:

**build_android.sh:**
```bash
#!/bin/bash

# Set your NDK path
export ANDROID_NDK=/path/to/android-ndk
export MIN_API=21

# Output directory
OUTPUT_DIR="build/android"
mkdir -p $OUTPUT_DIR

# Architectures to build
ARCHS=("armeabi-v7a" "arm64-v8a" "x86" "x86_64")

for ARCH in "${ARCHS[@]}"; do
    echo "Building for $ARCH..."

    BUILD_DIR="build/android-$ARCH"
    mkdir -p $BUILD_DIR
    cd $BUILD_DIR

    cmake ../.. \
        -DCMAKE_SYSTEM_NAME=Android \
        -DCMAKE_ANDROID_NDK=$ANDROID_NDK \
        -DCMAKE_ANDROID_ARCH_ABI=$ARCH \
        -DCMAKE_ANDROID_API=$MIN_API \
        -DCMAKE_BUILD_TYPE=Release \
        -DBUILD_SHARED_LIBS=ON \
        -DBUILD_UNITY_PLUGIN=ON \
        -DBUILD_EXECUTABLES=OFF

    cmake --build . --config Release

    # Copy to output
    mkdir -p ../$ARCH
    cp libantiprism.so ../$ARCH/

    cd ../..
done

echo "Android libraries built in $OUTPUT_DIR"
```

Usage:
```bash
chmod +x build_android.sh
./build_android.sh
```

**Alternative: Using android-cmake toolchain**

```bash
# For each architecture
for ARCH in armeabi-v7a arm64-v8a x86 x86_64; do
    mkdir -p build/android-$ARCH
    cd build/android-$ARCH

    cmake ../.. \
        -DCMAKE_TOOLCHAIN_FILE=$ANDROID_NDK/build/cmake/android.toolchain.cmake \
        -DANDROID_ABI=$ARCH \
        -DANDROID_PLATFORM=android-21 \
        -DBUILD_SHARED_LIBS=ON \
        -DBUILD_UNITY_PLUGIN=ON

    make -j$(nproc)
    cd ../..
done
```

### iOS Build

**build_ios.sh:**
```bash
#!/bin/bash

BUILD_DIR="build/ios"
mkdir -p $BUILD_DIR
cd $BUILD_DIR

cmake ../.. \
    -G Xcode \
    -DCMAKE_SYSTEM_NAME=iOS \
    -DCMAKE_OSX_DEPLOYMENT_TARGET=11.0 \
    -DCMAKE_OSX_ARCHITECTURES="arm64" \
    -DBUILD_SHARED_LIBS=OFF \
    -DBUILD_UNITY_PLUGIN=ON \
    -DBUILD_EXECUTABLES=OFF

cmake --build . --config Release

echo "iOS library built: libantiprism.a"
```

---

## Installing to Unity

After building, copy the libraries to your Unity project:

### Directory Structure

```
YourUnityProject/
├── Assets/
│   ├── Plugins/
│   │   ├── Android/
│   │   │   ├── libs/
│   │   │   │   ├── armeabi-v7a/
│   │   │   │   │   └── libantiprism.so
│   │   │   │   ├── arm64-v8a/
│   │   │   │   │   └── libantiprism.so
│   │   │   │   ├── x86/
│   │   │   │   │   └── libantiprism.so
│   │   │   │   └── x86_64/
│   │   │   │       └── libantiprism.so
│   │   ├── iOS/
│   │   │   └── libantiprism.a
│   │   ├── x86_64/
│   │   │   └── libantiprism.bundle (macOS)
│   │   ├── x86/
│   │   │   └── antiprism.dll (Windows 32-bit)
│   │   └── x86_64/
│   │       └── antiprism.dll (Windows 64-bit)
│   └── Scripts/
│       └── Antiprism/
│           ├── AntiprismPlugin.cs
│           └── PolyhedronExample.cs
```

### Copy Script

**install_to_unity.sh:**
```bash
#!/bin/bash

UNITY_PROJECT="/path/to/YourUnityProject"
PLUGINS_DIR="$UNITY_PROJECT/Assets/Plugins"

# Create directories
mkdir -p "$PLUGINS_DIR/Android/libs/armeabi-v7a"
mkdir -p "$PLUGINS_DIR/Android/libs/arm64-v8a"
mkdir -p "$PLUGINS_DIR/Android/libs/x86"
mkdir -p "$PLUGINS_DIR/Android/libs/x86_64"
mkdir -p "$PLUGINS_DIR/iOS"
mkdir -p "$PLUGINS_DIR/x86_64"

# Copy Android libraries
cp build/android/armeabi-v7a/libantiprism.so "$PLUGINS_DIR/Android/libs/armeabi-v7a/"
cp build/android/arm64-v8a/libantiprism.so "$PLUGINS_DIR/Android/libs/arm64-v8a/"
cp build/android/x86/libantiprism.so "$PLUGINS_DIR/Android/libs/x86/"
cp build/android/x86_64/libantiprism.so "$PLUGINS_DIR/Android/libs/x86_64/"

# Copy iOS library
cp build/ios/Release/libantiprism.a "$PLUGINS_DIR/iOS/"

# Copy desktop libraries (adjust paths as needed)
# cp build/libantiprism.so "$PLUGINS_DIR/x86_64/"  # Linux
# cp build/libantiprism.dylib "$PLUGINS_DIR/x86_64/"  # macOS
# cp build/Release/antiprism.dll "$PLUGINS_DIR/x86_64/"  # Windows

echo "Libraries copied to Unity project"
```

---

## Configuring in Unity

### 1. Android Library Settings

For each `.so` file in `Assets/Plugins/Android/libs/`:

1. Select the file in Unity
2. In Inspector:
   - **Platform**: Android
   - **CPU**: Set to appropriate architecture (ARMv7, ARM64, x86, x86_64)
   - **Load on startup**: Checked

### 2. iOS Library Settings

For `libantiprism.a`:

1. Select the file in Unity
2. In Inspector:
   - **Platform**: iOS
   - **Add to Embedded Binaries**: Checked
   - **Compile flags**: (none needed)

### 3. Desktop Library Settings

For `.dll` / `.dylib` / `.so`:

1. Select the file
2. In Inspector:
   - **Platform**: Standalone (Windows/macOS/Linux)
   - **CPU**: x86_64 (or x86 for 32-bit)
   - **Load on startup**: Checked

---

## Troubleshooting

### Build Errors

**"configure: error: C++ compiler cannot create executables"**
- Install C++ compiler (g++, clang++, or MSVC)
- Ensure compiler is in PATH

**"fatal error: 'cmath' file not found"**
- Install C++ standard library headers
- On Linux: `sudo apt-get install libstdc++-dev`

### Android NDK Issues

**"Could not find CMAKE_ANDROID_NDK"**
- Set ANDROID_NDK environment variable:
  ```bash
  export ANDROID_NDK=/path/to/ndk
  ```

**"undefined reference to __atomic_*"**
- Link with atomic library:
  ```cmake
  target_link_libraries(antiprism PRIVATE atomic)
  ```

### Unity Runtime Errors

**DllNotFoundException: antiprism**
- Check library is in correct Plugins folder
- Verify library name matches (with or without "lib" prefix)
- For Android, ensure target architectures in Player Settings match your libraries

**EntryPointNotFoundException**
- Rebuild library with `-DBUILD_UNITY_PLUGIN=ON`
- Verify C API is included (check for antiprism_c_api.cc in build)

### iOS Build Issues

**"No such file or directory" for system headers**
- Ensure Xcode Command Line Tools are installed:
  ```bash
  xcode-select --install
  ```

**Code signing errors**
- Disable code signing for static libraries
- Use development provisioning profile in Unity

---

## Optimization Tips

### Build Configuration

For best performance in Unity builds:

1. **Use Release configuration:**
   ```bash
   cmake .. -DCMAKE_BUILD_TYPE=Release
   ```

2. **Enable optimizations:**
   ```bash
   # Add to CMakeLists.txt
   if(CMAKE_BUILD_TYPE MATCHES Release)
       if(MSVC)
           target_compile_options(antiprism PRIVATE /O2 /Ob2)
       else()
           target_compile_options(antiprism PRIVATE -O3 -march=native)
       endif()
   endif()
   ```

3. **Strip symbols** (for smaller libraries):
   ```bash
   # After building
   strip -s libantiprism.so  # Linux/Android
   strip -x libantiprism.dylib  # macOS
   ```

### Android-Specific

1. **Use ARM64 only** for modern devices:
   - Only build arm64-v8a
   - Reduces APK size significantly

2. **Enable LTO** (Link-Time Optimization):
   ```bash
   cmake .. -DCMAKE_INTERPROCEDURAL_OPTIMIZATION=ON
   ```

---

## Testing the Build

Create a simple test program:

**test.cpp:**
```cpp
#include "antiprism_c_api.h"
#include <stdio.h>

int main() {
    printf("Antiprism version: %s\n", anti_get_version());

    AntiGeometryHandle geom = anti_geometry_create();
    if (!geom) {
        printf("Failed to create geometry\n");
        return 1;
    }

    AntiStatus status = anti_geometry_read_resource(geom, "cube");
    if (status != ANTI_OK) {
        printf("Failed to load cube: %d\n", status);
        return 1;
    }

    int num_verts = anti_geometry_num_verts(geom);
    int num_faces = anti_geometry_num_faces(geom);

    printf("Cube has %d vertices and %d faces\n", num_verts, num_faces);

    anti_geometry_destroy(geom);
    return 0;
}
```

Compile and run:
```bash
# Linux
g++ test.cpp -I./base -L./build -lantiprism -o test
LD_LIBRARY_PATH=./build ./test

# Expected output:
# Antiprism version: Antiprism 0.32.99 (C API)
# Cube has 8 vertices and 6 faces
```

---

## Next Steps

1. Copy the Unity C# wrapper files from `unity/` directory
2. Copy built libraries to your Unity project
3. See `unity/README_UNITY.md` for Unity integration guide
4. Check `unity/PolyhedronExample.cs` for usage examples

## Support

For issues or questions:
- Antiprism website: http://www.antiprism.com
- GitHub issues: https://github.com/antiprism/antiprism/issues
