# GitHub Actions CI for Unity Plugin Builds

This repository includes automated builds for all Unity platforms using GitHub Actions.

## What Gets Built

Every push to main/develop branches or pull request triggers builds for:

- **Windows**: x86 and x64 DLLs
- **Linux**: x86_64 shared library
- **macOS**: Universal binary (x86_64 + arm64)
- **Android**: All architectures (armeabi-v7a, arm64-v8a, x86, x86_64)
- **iOS**: Device (arm64) and Simulator (arm64)

## How to Use

### Option 1: Download from Actions Tab

1. Go to the **Actions** tab in GitHub
2. Click on the latest successful workflow run
3. Scroll down to **Artifacts** section
4. Download `antiprism-unity-plugin.zip` or `antiprism-unity-plugin.tar.gz`

This package contains:
```
antiprism-unity-plugin/
├── Plugins/
│   ├── x86/antiprism.dll              (Windows 32-bit)
│   ├── x86_64/antiprism.dll           (Windows 64-bit)
│   ├── x86_64/libantiprism.so         (Linux)
│   ├── macOS/libantiprism.bundle      (macOS Universal)
│   ├── Android/libs/
│   │   ├── armeabi-v7a/libantiprism.so
│   │   ├── arm64-v8a/libantiprism.so
│   │   ├── x86/libantiprism.so
│   │   └── x86_64/libantiprism.so
│   └── iOS/libantiprism.a
├── Scripts/
│   ├── AntiprismPlugin.cs
│   └── PolyhedronExample.cs
└── Documentation...
```

### Option 2: Download Individual Platform Builds

In the Artifacts section, you can also download individual platform builds:
- `windows-x64`
- `windows-Win32`
- `linux-x86_64`
- `macos-x86_64`
- `macos-arm64`
- `android-armeabi-v7a`
- `android-arm64-v8a`
- `android-x86`
- `android-x86_64`
- `ios-OS64`
- `ios-SIMULATORARM64`

### Option 3: Create a Release

To create a downloadable release:

1. Create and push a git tag:
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```

2. The workflow automatically creates a GitHub Release with the plugin package attached

## Manual Trigger

You can manually trigger a build:

1. Go to **Actions** tab
2. Click **Build Unity Plugin** workflow
3. Click **Run workflow**
4. Select branch and click **Run workflow**

## Build Status

Check the build status badge (add to main README.md):

```markdown
![Build Unity Plugin](https://github.com/YOUR_USERNAME/antiprism/actions/workflows/build-unity-plugin.yml/badge.svg)
```

## Troubleshooting

**Build fails on Windows:**
- Check MSVC compatibility in the Windows job logs
- Verify all platform-specific code compiles

**Android build fails:**
- Check NDK version compatibility (currently using r25c)
- Verify CMake toolchain configuration

**iOS build fails:**
- Check Xcode version on macOS runner
- Verify deployment target (iOS 11.0+)

**macOS universal binary creation fails:**
- Ensure both x86_64 and arm64 builds completed successfully
- Check lipo command in packaging job

## Customization

### Change NDK Version

Edit `.github/workflows/build-unity-plugin.yml`:
```yaml
- name: Setup Android NDK
  uses: nttld/setup-ndk@v1
  with:
    ndk-version: r26c  # Change this
```

### Add More Platforms

Add new jobs following the existing pattern:
```yaml
build-webgl:
  name: Build WebGL
  runs-on: ubuntu-latest
  steps:
    # ... custom build steps
```

### Change Trigger Branches

Edit the `on:` section:
```yaml
on:
  push:
    branches: [ main, custom-branch ]
```

## Local Testing

Test the workflow locally using [act](https://github.com/nektos/act):

```bash
# Install act
brew install act  # macOS
# or
choco install act  # Windows

# Run specific job
act -j build-windows

# Run entire workflow
act push
```
