#!/usr/bin/env bash
set -euo pipefail

# Create a self-contained macOS CLI package (bin/lib/data) from an Autotools build.
# Usage: scripts/packaging/macos-cli-package.sh [out_dir]
# Default output: dist/antiprism-cli-macos

out_dir=${1:-dist/antiprism-cli-macos}
mkdir -p "$out_dir/bin" "$out_dir/lib"

# Copy real binaries from libtool .libs directories
if [ -d src/.libs ]; then
  find src/.libs -maxdepth 1 -type f -perm -111 -print0 | xargs -0 -I{} cp -f "{}" "$out_dir/bin/"
fi
if [ -d aview/.libs ]; then
  find aview/.libs -maxdepth 1 -type f -perm -111 -print0 | xargs -0 -I{} cp -f "{}" "$out_dir/bin/"
fi

# Bundle the shared library
if [ -d base/.libs ]; then
  cp -f base/.libs/libantiprism.0.dylib "$out_dir/lib/"
  if [ -f base/.libs/libantiprism.dylib ]; then
    cp -f base/.libs/libantiprism.dylib "$out_dir/lib/"
  fi
fi

# Bundle data resources
if [ -d data ]; then
  rsync -a data "$out_dir/"
fi

# Add README and a double-clickable unquarantine helper
if [ -f CLI_README.md ]; then
  cp -f CLI_README.md "$out_dir/"
fi
cat > "$out_dir/unquarantine.command" <<'SCRIPT'
#!/usr/bin/env bash
set -euo pipefail
pkg_dir="$(cd "$(dirname "$0")" && pwd)"
xattr -dr com.apple.quarantine "$pkg_dir"
echo "Removed quarantine from: $pkg_dir"
SCRIPT
chmod +x "$out_dir/unquarantine.command"

# Relink binaries to the bundled dylib
if [ -f "$out_dir/lib/libantiprism.0.dylib" ]; then
  install_name_tool -id "@rpath/libantiprism.0.dylib" "$out_dir/lib/libantiprism.0.dylib"
  for bin in "$out_dir"/bin/*; do
    if file "$bin" | grep -q "Mach-O"; then
      install_name_tool -change "/usr/local/lib/libantiprism.0.dylib" "@loader_path/../lib/libantiprism.0.dylib" "$bin" || true
      install_name_tool -add_rpath "@loader_path/../lib" "$bin" 2>/dev/null || true
    fi
  done
fi

echo "Packaged macOS CLI into $out_dir" >&2
