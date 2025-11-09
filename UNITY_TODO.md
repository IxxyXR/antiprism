# Unity Plugin TODO List

## CRITICAL - Core Integration Tasks

### Integrate All Standalone Programs into Unity Plugin
**Priority: HIGHEST** - Every standalone program that generates or transforms geometry should be accessible through the C API and Unity plugin.

**Excluded:** Viewers (antiview), format converters (obj2off, off2obj, off2pov, off2vrml, off2dae, off2crds), and text output tools (rep_print).

#### Phase 1: Critical Polyhedra Generators (NOT YET INTEGRATED)
- [ ] **symmetro** - Symmetrohedra using Kaplan-Hart notation (T,O,I symmetries with l,m,n multipliers)
- [ ] **stellate** - Stellation operations on polyhedra
- [ ] **canonical** - Canonical/normalized forms (planarize, center, unitize)
- [ ] **wythoff** - Wythoff construction (may overlap with uniform resources)
- [ ] **waterman** - Waterman polyhedra from lattices
- [ ] **poly_kscope** - Kaleidoscopic polyhedra
- [ ] **unitile2d** - Uniform 2D tilings
- [ ] **leonardo** - Leonardo-style polyhedra

#### Phase 2: Important Transformations & Operations
- [ ] **planar** - Make polyhedra planar (planarization algorithms)
- [ ] **canonical** - Canonicalization operations
- [ ] **repel** - Vertex repelling/optimization for better geometry
- [ ] **off_align** - Alignment operations (align to axes, center, etc.)
- [ ] **pol_recip** - Polar reciprocation (beyond basic dual - already have dual)
- [ ] **to_nfold** - Transform to n-fold rotational symmetry
- [ ] **bravais** - Bravais lattice polyhedra
- [ ] **miller** - Miller polyhedra from lattices

#### Phase 3: Utilities & Advanced Operations
- [ ] **off_color** - Advanced coloring operations
- [ ] **off_normals** - Normal vector operations
- [ ] **poly_form** - Polygon formation operations
- [ ] **sph_rings** - Spherical rings
- [ ] **iso_delta** - Isohedral deltahedra
- [ ] **iso_kite** - Isohedral kite polyhedra
- [ ] **kcycle** - K-cycle operations
- [ ] **n_icons** - N-icons construction
- [ ] **tetra59** - Special tetrahedra constructions

#### Phase 4: Query & Analysis (Lower Priority for Unity)
- [ ] **off_query** - Query polyhedron properties
- [ ] **off_report** - Generate reports on polyhedra
- [ ] **off_util** - General utilities

**Total standalone programs: 46** | **Currently integrated: ~8** | **Remaining: ~38**

### Other Major Gaps - Base Library Features Not Yet Exposed

#### Core Library Features Missing from C API
- [ ] **Tiling operations** (tiling.h) - Wythoff-style tiling, uniform tilings
- [ ] **Advanced coloring** (coloring.h, colormap.h) - Color by symmetry, map-based coloring
- [ ] **Symmetry queries** (symmetry.h) - Full symmetry group detection and operations
- [ ] **Planar operations** (planar.h) - Make faces planar, check planarity
- [ ] **Geometric info queries** (geometryinfo.h) - Volume, surface area, angles, etc.
- [ ] **Advanced transformations** (trans3d.h, trans4d.h) - 3D/4D transformation matrices
- [ ] **Iteration utilities** (iteration.h) - Iterative refinement operations

#### Missing Modifier Operations
- [ ] **Chamfer** - Edge/vertex chamfering
- [ ] **Whirl** - Face whirling
- [ ] **Propeller** - Propeller operation
- [ ] **Loft** - Face lofting
- [ ] **Quinto** - Quinto operation
- [ ] **Lace** - Lace operation
- [ ] **Stake** - Stake operation
- [ ] **Medial** - Medial operation

**Note:** Many Conway operators are implemented, but there are additional specialized operations in Antiprism that aren't Conway notation.

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
- [x] Add 35+ new polyhedra types to Unity example (first batch):
  - [x] 4 missing Archimedean solids
  - [x] 4 additional prisms (7, 8, 9, 12 sides)
  - [x] 3 additional antiprisms (6, 8, 10 sides)
  - [x] 4 pyramids
  - [x] 6 dipyramids
  - [x] 4 Kepler-Poinsot polyhedra (stellated)
  - [x] 3 geodesic spheres
  - [x] 7 Johnson solids
- [x] Move PolyhedronType, ModifierType enums and GetResourceName to AntiprismPlugin (public API)
- [x] Add 50+ additional polyhedra types (second batch):
  - [x] 3 more geodesic spheres (5, 6, 8)
  - [x] 24 more Johnson solids (cupolae, bicupolae, elongated variants)
  - [x] 9 more uniform polyhedra (stellated and special forms)
  - [x] 5 uniform compounds
  - [x] 6 Wenninger stellations
  - [x] 4 miscellaneous special polyhedra
- [x] **Total polyhedra types now available: 110+**
- [x] Add C API for custom N-sided polygon-based polyhedra:
  - [x] anti_make_prism(n) - Generate N-sided prism
  - [x] anti_make_antiprism(n) - Generate N-sided antiprism
  - [x] anti_make_pyramid(n) - Generate N-sided pyramid
  - [x] anti_make_dipyramid(n) - Generate N-sided dipyramid
  - [x] anti_make_cupola(n) - Generate N-sided cupola
- [x] Add Unity wrappers for polygon generators (CreatePrism, CreateAntiprism, etc.)

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
- [x] Add custom N-sided prism/antiprism/pyramid/dipyramid with inspector parameter - COMPLETED (C API + Unity wrappers)
- [x] Add more geodesic subdivisions (5, 6, 8) - COMPLETED
- [x] Add cupola variants (elongated, gyroelongated) - COMPLETED (added j18-j20, j36-j38)
- [x] Add more Johnson solids (92 available total) - COMPLETED (added 24 more: j7, j9, j11, j18-j20, j27-j38, j85, j92)
- [x] Add more uniform polyhedra (80 available total) - COMPLETED (added 9 more: u4, u15, u17, u18, u21, u37, u38, u54, u55)
- [x] Add uniform compounds (75 available) - COMPLETED (added 5 famous ones: uc1-uc5)
- [x] Add Wenninger stellations - COMPLETED (added 6: w1, w2, w3, w9, w20, w22)
- [x] Add zonohedra generators - COMPLETED (C API already existed, Unity wrappers already present)

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
