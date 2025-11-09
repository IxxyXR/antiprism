# Unity Plugin TODO List

## Current Status

### Completed in Previous Sessions
- [x] Fix modifiers (Dual, Truncate, etc.) - replaced placeholder implementations
- [x] Add normal visualization debug mode (color-coded: green=outward, red=inward, cyan=vertex)
- [x] Implement 15 Conway operators with parameter support
- [x] Fix normal direction issues (negated normals to match triangle winding)
- [x] Fix dual operation (recipRadius default changed from 0.0 to 1.0)
- [x] Add parameter controls to Unity inspector (truncateRatio, kisFaceSides, needleHeight, dualRadius)
- [x] Fix P/Invoke declaration location (moved to Geometry class)
- [x] Remove iteration loop interpretation of parameters (parameters are for behavior, not repetition)
- [x] Add Snub and Ortho Conway operators

### Completed in Current Session
- [x] Fix Bevel parameter passing (use named parameter for ratio)
- [x] Add 35+ new polyhedra types to Unity example:
  - [x] 4 missing Archimedean solids
  - [x] 4 additional prisms (7, 8, 9, 12 sides)
  - [x] 3 additional antiprisms (6, 8, 10 sides)
  - [x] 4 pyramids
  - [x] 6 dipyramids
  - [x] 4 Kepler-Poinsot polyhedra (stellated)
  - [x] 3 geodesic spheres
  - [x] 7 Johnson solids

## Pending Tasks - High Priority

### Testing & Verification
- [ ] **CRITICAL: Recompile C++ DLL** using `build.bat` to apply all C API changes
- [ ] **Test all 15 Conway operators** in Unity after DLL rebuild
  - [ ] Dual (with recipRadius parameter)
  - [ ] Truncate (with ratio parameter)
  - [ ] Kis (with faceSides parameter)
  - [ ] Ambo
  - [ ] Gyro
  - [ ] Join
  - [ ] Needle (with height parameter)
  - [ ] Zip
  - [ ] Subdivide
  - [ ] Expand
  - [ ] Meta
  - [ ] Bevel (with ratio parameter)
  - [ ] Snub
  - [ ] Ortho
  - [ ] ConvexHull
- [ ] **Test all 60+ polyhedra types** in Unity
  - [ ] Verify all Platonic solids render correctly
  - [ ] Verify all 13 Archimedean solids render correctly
  - [ ] Verify prisms/antiprisms/pyramids/dipyramids work
  - [ ] Verify Kepler-Poinsot stellated polyhedra render correctly
  - [ ] Verify geodesic spheres render correctly
  - [ ] Verify Johnson solids render correctly
- [ ] **Verify parameter behavior** - ensure reserved parameters don't break anything
- [ ] **Test normal visualization** with various polyhedra and modifiers
- [ ] **Test modifier combinations** (e.g., Truncate + Dual, Kis + Gyro, etc.)

## Future Enhancements

### Additional Polyhedra Generators
- [ ] Add custom N-sided prism/antiprism/pyramid/dipyramid with inspector parameter
- [ ] Add more geodesic subdivisions (5, 6, 8, etc.)
- [ ] Add cupola variants (elongated, gyroelongated)
- [ ] Add more Johnson solids (92 available total)
- [ ] Add more uniform polyhedra (80 available total)
- [ ] Add uniform compounds (75 available)
- [ ] Add Wenninger stellations
- [ ] Add zonohedra generators

### Conway Operator Enhancements
- [ ] Implement actual parameter behavior for operators (not just reserved)
  - [ ] Gyro(n) - rotation angle control
  - [ ] Expand(n,m) - different expansion modes
  - [ ] Meta(n) - different meta modes
  - [ ] Bevel(n) - different bevel modes
  - [ ] Snub(n) - different snub modes
  - [ ] Ortho(n,m) - different ortho modes
- [ ] Add string-based Conway notation parser
  - [ ] Support operator chains (e.g., "kT", "aC", "eD")
  - [ ] Add to Unity inspector as text field

### UI/UX Improvements
- [ ] Add real-time preview in Unity Editor
- [ ] Add dropdown categories for polyhedra types (Platonic, Archimedean, etc.)
- [ ] Add tooltips explaining each polyhedron type
- [ ] Add parameter presets for common modifier combinations
- [ ] Add "Random Polyhedron" button
- [ ] Add export to OBJ/STL functionality

### Performance & Optimization
- [ ] Add mesh caching to avoid regeneration on every inspector change
- [ ] Add LOD (Level of Detail) support for complex polyhedra
- [ ] Profile performance with high subdivision geodesic spheres
- [ ] Add async mesh generation for complex operations

### Android/Mobile
- [ ] Test DLL on Android device
- [ ] Verify all polyhedra render correctly on mobile
- [ ] Add mobile-specific optimizations
- [ ] Test touch controls for rotation

### Documentation
- [ ] Add XML documentation comments to all public C# methods
- [ ] Create Unity package documentation
- [ ] Add example scenes showing different polyhedra
- [ ] Create tutorial for adding new polyhedra types
- [ ] Document Conway notation usage

## Known Issues
- None currently identified (all previous issues fixed)

## Technical Debt
- [ ] Consider refactoring GetResourceName() switch to use attributes/dictionary
- [ ] Consider adding validation for resource names (warn if resource not found)
- [ ] Consider adding error handling for invalid parameter ranges
- [ ] Consider caching Geometry handles for repeated operations

## Architecture Decisions to Document
1. **Parameter semantics**: Reserved int parameters for future behavioral control, not iterations
2. **Default values**: Match Conway notation defaults (gyro n=1, expand n=2, meta n=2, etc.)
3. **P/Invoke location**: Declarations must be in the class that calls them (nested Geometry class)
4. **Normal negation**: Required because Unity uses left-handed coords, Antiprism uses right-handed
5. **Triangle winding**: Reversed for Unity compatibility

## Workflow Reminders
- **ALWAYS** update Unity wrappers when C API changes
- **ALWAYS** recompile DLL after C++ changes with `build.bat`
- **ALWAYS** test in Unity after DLL changes
- **NEVER** use iteration loops for operator parameters

## Resources
- Antiprism resource system: All polyhedra accessible via `anti_geometry_read_resource()`
- Conway notation operators: Defined in `src/conway.cc`
- Uniform polyhedra: 80 types (u1-u80)
- Johnson solids: 92 types (j1-j92)
- Polygon-based: Infinite families (pri{n}, ant{n}, pyr{n}, dip{n}, etc.)

---
Last Updated: 2025-11-09
Session: claude/setup-codebase-usage-011CUvX3GuR5AAAMCtLnVHvx
