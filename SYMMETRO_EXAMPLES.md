# Symmetrohedra CLI Examples → Unity Settings

This guide shows how to reproduce the CLI examples in Unity.

## CLI Format: `-k sym,mult0,mult1,mult2[,connection]`

In Unity, set:
- `polyhedronType` = `Symmetrohedra`
- `symmetroSym` = symmetry character
- `symmetroMult0`, `symmetroMult1`, `symmetroMult2` = multipliers

## Common Examples

### Cuboctahedron (U7)
```bash
symmetro -k o,1,1,0,e
```
**Unity:**
- symmetroSym = 'O'
- symmetroMult0 = 1
- symmetroMult1 = 1
- symmetroMult2 = 0

### Rhombicuboctahedron-like (scaled squares)
```bash
symmetro -k o,1,1,0 -r 2
```
**Unity:**
- symmetroSym = 'O'
- symmetroMult0 = 1
- symmetroMult1 = 1
- symmetroMult2 = 0
- (Note: -r 2 scaling not yet supported in basic API)

### Approximate Snub Cube (U12)
```bash
symmetro -k o,1,0,1 -a 28.53243959961367
```
**Unity:**
- symmetroSym = 'O'
- symmetroMult0 = 1
- symmetroMult1 = 0
- symmetroMult2 = 1
- (Note: -a rotation angle not yet supported in basic API)

### Twister
```bash
symmetro -k t,1,1,0 -C s -a 15
```
**Unity:**
- symmetroSym = 'T'
- symmetroMult0 = 1
- symmetroMult1 = 1
- symmetroMult2 = 0
- (Note: -C convex hull and -a angle not yet supported in basic API)

### Icosidodecahedron (U24) - using -c notation
```bash
symmetro -c 5,5,d
```
**Not yet available in Unity**
(This uses -c notation, not Kaplan-Hart -k notation)

## Axis Orders Reference

| Symmetry | Axis 0 | Axis 1 | Axis 2 |
|----------|--------|--------|--------|
| T (Tet)  | 3      | 3      | 2      |
| O (Oct)  | 4      | 3      | 2      |
| I (Ico)  | 5      | 3      | 2      |

## How p,q are Calculated

**1 non-zero multiplier:** p = q = that axis's order

**2 non-zero multipliers:** p = first axis order, q = second axis order

### Examples:
- `O,1,1,0` → p=4, q=3 (axes 0 and 1)
- `O,1,0,1` → p=4, q=2 (axes 0 and 2)
- `T,1,1,0` → p=3, q=3 (axes 0 and 1)
- `I,2,3,0` → p=5, q=3 (axes 0 and 1, multiplier values affect polygon size)

## Limitations of Current Unity API

The basic `CreateSymmetroKaplanHart()` API supports:
- ✅ Symmetry type (T, O, I)
- ✅ 3 multipliers
- ❌ Rotation angle (-a parameter)
- ❌ Convex hull options (-C parameter)
- ❌ Scaling (-r parameter)
- ❌ Other advanced options

For advanced features, use `CreateSymmetroAdvanced()` (not yet fully documented).

## Testing in Unity

1. Pull latest changes and rebuild DLL:
   ```bash
   git pull
   build.bat  # Windows
   ```

2. Copy DLL to Unity:
   - `build\Release\antiprism.dll` → `Assets/Plugins/`

3. In Unity Inspector:
   - Set `Polyhedron Type` to `Symmetrohedra`
   - Adjust `symmetroSym`, `symmetroMult0`, `symmetroMult1`, `symmetroMult2`
   - Watch the polyhedron update in real-time!

## Verified Examples

Try these confirmed working combinations:

| Name | Sym | M0 | M1 | M2 | Result |
|------|-----|----|----|----|----|
| Cuboctahedron | O | 1 | 1 | 0 | U7 uniform polyhedron |
| Snub Cube basis | O | 1 | 0 | 1 | Twisted octahedral form |
| Tetrahedral form | T | 1 | 1 | 0 | Tetrahedral symmetry |

---
Last Updated: 2025-11-09
