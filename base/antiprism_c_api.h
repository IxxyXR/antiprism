/*
   Copyright (c) 2003-2025, Adrian Rossiter

   Antiprism - http://www.antiprism.com

   Permission is hereby granted, free of charge, to any person obtaining a
   copy of this software and associated documentation files (the "Software"),
   to deal in the Software without restriction, including without limitation
   the rights to use, copy, modify, merge, publish, distribute, sublicense,
   and/or sell copies of the Software, and to permit persons to whom the
   Software is furnished to do so, subject to the following conditions:

      The above copyright notice and this permission notice shall be included
      in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
  IN THE SOFTWARE.
*/

/*!\file antiprism_c_api.h
 * \brief C API for Antiprism library (Unity/FFI compatible)
 *
 * This header provides a C-compatible API wrapper around the Antiprism C++
 * library, suitable for use with Unity (via P/Invoke), Android builds,
 * and other FFI scenarios.
 */

#ifndef ANTIPRISM_C_API_H
#define ANTIPRISM_C_API_H

#ifdef __cplusplus
extern "C" {
#endif

/* Platform-specific exports */
#if defined(_WIN32) || defined(__CYGWIN__)
  #ifdef ANTIPRISM_BUILD_DLL
    #define ANTIPRISM_API __declspec(dllexport)
  #else
    #define ANTIPRISM_API __declspec(dllimport)
  #endif
#else
  #if __GNUC__ >= 4
    #define ANTIPRISM_API __attribute__((visibility("default")))
  #else
    #define ANTIPRISM_API
  #endif
#endif

/* Opaque handle types */
typedef void* AntiGeometryHandle;
typedef void* AntiVec3dHandle;

/* Status codes */
typedef enum {
  ANTI_OK = 0,
  ANTI_ERROR_MEMORY = -1,
  ANTI_ERROR_INVALID_HANDLE = -2,
  ANTI_ERROR_INVALID_INDEX = -3,
  ANTI_ERROR_PARSE = -4,
  ANTI_ERROR_FILE = -5,
  ANTI_ERROR_UNKNOWN = -99
} AntiStatus;

/*---------------------------------------------------------------------------
 * Library Information
 *---------------------------------------------------------------------------*/

/** Get library version string */
ANTIPRISM_API const char* anti_get_version(void);

/** Get library info/description */
ANTIPRISM_API const char* anti_get_info(void);

/*---------------------------------------------------------------------------
 * Geometry Creation/Destruction
 *---------------------------------------------------------------------------*/

/** Create a new empty geometry object
 * @return Handle to geometry object, or NULL on error */
ANTIPRISM_API AntiGeometryHandle anti_geometry_create(void);

/** Destroy a geometry object and free its memory
 * @param geom Handle to geometry object */
ANTIPRISM_API void anti_geometry_destroy(AntiGeometryHandle geom);

/** Clear all elements from geometry
 * @param geom Handle to geometry object
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_clear(AntiGeometryHandle geom);

/*---------------------------------------------------------------------------
 * Geometry Import/Export
 *---------------------------------------------------------------------------*/

/** Load geometry from OFF format string
 * @param geom Handle to geometry object
 * @param off_data String containing OFF format data
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_read_off_string(AntiGeometryHandle geom,
                                                        const char* off_data);

/** Export geometry to OFF format string
 * @param geom Handle to geometry object
 * @param buffer Output buffer (caller must free with anti_free_string)
 * @param sig_digits Number of significant digits (default: 17)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_write_off_string(AntiGeometryHandle geom,
                                                         char** buffer,
                                                         int sig_digits);

/** Load a built-in polyhedron by name
 * @param geom Handle to geometry object
 * @param name Resource name (e.g., "cube", "tet", "ico", "dodec")
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_read_resource(AntiGeometryHandle geom,
                                                      const char* name);

/** Free a string allocated by the library
 * @param str String to free */
ANTIPRISM_API void anti_free_string(char* str);

/*---------------------------------------------------------------------------
 * Geometry Query - Element Counts
 *---------------------------------------------------------------------------*/

/** Get number of vertices
 * @param geom Handle to geometry object
 * @return Number of vertices, or -1 on error */
ANTIPRISM_API int anti_geometry_num_verts(AntiGeometryHandle geom);

/** Get number of edges
 * @param geom Handle to geometry object
 * @return Number of edges, or -1 on error */
ANTIPRISM_API int anti_geometry_num_edges(AntiGeometryHandle geom);

/** Get number of faces
 * @param geom Handle to geometry object
 * @return Number of faces, or -1 on error */
ANTIPRISM_API int anti_geometry_num_faces(AntiGeometryHandle geom);

/*---------------------------------------------------------------------------
 * Geometry Query - Vertex Data
 *---------------------------------------------------------------------------*/

/** Get vertex coordinates
 * @param geom Handle to geometry object
 * @param v_idx Vertex index
 * @param x Output for x coordinate
 * @param y Output for y coordinate
 * @param z Output for z coordinate
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_get_vert(AntiGeometryHandle geom,
                                                 int v_idx,
                                                 double* x, double* y, double* z);

/** Get all vertex coordinates as flat array
 * @param geom Handle to geometry object
 * @param coords Output array (size must be >= 3 * num_verts)
 * @param max_verts Maximum vertices to copy
 * @return Number of vertices copied, or -1 on error */
ANTIPRISM_API int anti_geometry_get_all_verts(AntiGeometryHandle geom,
                                               double* coords,
                                               int max_verts);

/*---------------------------------------------------------------------------
 * Geometry Query - Face Data
 *---------------------------------------------------------------------------*/

/** Get number of vertices in a face
 * @param geom Handle to geometry object
 * @param f_idx Face index
 * @return Number of vertices in face, or -1 on error */
ANTIPRISM_API int anti_geometry_face_num_verts(AntiGeometryHandle geom,
                                                int f_idx);

/** Get vertex indices for a face
 * @param geom Handle to geometry object
 * @param f_idx Face index
 * @param indices Output array for vertex indices
 * @param max_indices Maximum indices to copy
 * @return Number of indices copied, or -1 on error */
ANTIPRISM_API int anti_geometry_get_face(AntiGeometryHandle geom,
                                          int f_idx,
                                          int* indices,
                                          int max_indices);

/** Get all faces as flat array (format: face0_size, v0, v1, ..., face1_size, ...)
 * @param geom Handle to geometry object
 * @param buffer Output buffer
 * @param buffer_size Size of output buffer
 * @return Total number of integers written, or -1 on error */
ANTIPRISM_API int anti_geometry_get_all_faces(AntiGeometryHandle geom,
                                               int* buffer,
                                               int buffer_size);

/*---------------------------------------------------------------------------
 * Geometry Modification - Add Elements
 *---------------------------------------------------------------------------*/

/** Add a vertex
 * @param geom Handle to geometry object
 * @param x X coordinate
 * @param y Y coordinate
 * @param z Z coordinate
 * @return Index of added vertex, or -1 on error */
ANTIPRISM_API int anti_geometry_add_vert(AntiGeometryHandle geom,
                                          double x, double y, double z);

/** Add a face
 * @param geom Handle to geometry object
 * @param indices Array of vertex indices
 * @param num_indices Number of vertices in face
 * @return Index of added face, or -1 on error */
ANTIPRISM_API int anti_geometry_add_face(AntiGeometryHandle geom,
                                          const int* indices,
                                          int num_indices);

/*---------------------------------------------------------------------------
 * Geometry Transformations
 *---------------------------------------------------------------------------*/

/** Translate geometry
 * @param geom Handle to geometry object
 * @param dx Translation in x
 * @param dy Translation in y
 * @param dz Translation in z
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_translate(AntiGeometryHandle geom,
                                                  double dx, double dy, double dz);

/** Scale geometry uniformly
 * @param geom Handle to geometry object
 * @param scale Scale factor
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_scale(AntiGeometryHandle geom,
                                              double scale);

/** Scale geometry non-uniformly
 * @param geom Handle to geometry object
 * @param sx Scale in x
 * @param sy Scale in y
 * @param sz Scale in z
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_scale_xyz(AntiGeometryHandle geom,
                                                  double sx, double sy, double sz);

/** Rotate geometry around X axis
 * @param geom Handle to geometry object
 * @param angle Rotation angle in radians
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_rotate_x(AntiGeometryHandle geom,
                                                 double angle);

/** Rotate geometry around Y axis
 * @param geom Handle to geometry object
 * @param angle Rotation angle in radians
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_rotate_y(AntiGeometryHandle geom,
                                                 double angle);

/** Rotate geometry around Z axis
 * @param geom Handle to geometry object
 * @param angle Rotation angle in radians
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_rotate_z(AntiGeometryHandle geom,
                                                 double angle);

/** Transform geometry to unit radius sphere (centered at origin)
 * @param geom Handle to geometry object
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_unitize(AntiGeometryHandle geom);

/*---------------------------------------------------------------------------
 * Geometry Operations
 *---------------------------------------------------------------------------*/

/** Calculate convex hull
 * @param geom Handle to geometry object
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_convex_hull(AntiGeometryHandle geom);

/** Triangulate all faces
 * @param geom Handle to geometry object
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_triangulate(AntiGeometryHandle geom);

/** Orient faces consistently
 * @param geom Handle to geometry object
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_orient(AntiGeometryHandle geom);

/** Reverse face orientations
 * @param geom Handle to geometry object
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_orient_reverse(AntiGeometryHandle geom);

/** Canonicalize geometry (adjust vertices for more uniform edge lengths)
 * @param geom Handle to geometry object
 * @param num_iters Maximum number of iterations (0 for default 1000)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_canonicalize(AntiGeometryHandle geom,
                                                     int num_iters);

/** Create dual polyhedron
 * @param geom Handle to geometry object (will be replaced with its dual)
 * @param recip_rad Reciprocation radius (0 for automatic)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_dual(AntiGeometryHandle geom,
                                             double recip_rad);

/** Truncate vertices
 * @param geom Handle to geometry object
 * @param ratio Truncation ratio (0.0-1.0, typically 0.3-0.5)
 * @param order Truncate only vertices with this vertex order (0 for all)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_truncate(AntiGeometryHandle geom,
                                                 double ratio,
                                                 int order);

/** Kis operation (place pyramid on each face)
 * @param geom Handle to geometry object
 * @param n Only kis faces with n sides (0 for all faces)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_kis(AntiGeometryHandle geom, int n);

/** Ambo operation (create vertices at edge midpoints)
 * @param geom Handle to geometry object
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_ambo(AntiGeometryHandle geom);

/** Gyro operation (rotate and subdivide faces)
 * @param geom Handle to geometry object
 * @param n Gyro parameter (default 1)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_gyro(AntiGeometryHandle geom, int n);

/** Join operation (dual of ambo)
 * @param geom Handle to geometry object
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_join(AntiGeometryHandle geom);

/** Needle operation (elongated kis)
 * @param geom Handle to geometry object
 * @param height Height multiplier for needle points
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_needle(AntiGeometryHandle geom, double height);

/** Zip operation (dual of kis)
 * @param geom Handle to geometry object
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_zip(AntiGeometryHandle geom);

/** Subdivide operation (subdivide each face into smaller faces)
 * @param geom Handle to geometry object
 * @param n First parameter (default 2)
 * @param m Second parameter (default 0)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_subdivide(AntiGeometryHandle geom, int n, int m);

/** Expand operation (ambo + ambo of dual)
 * @param geom Handle to geometry object
 * @param n First parameter (default 2)
 * @param m Second parameter (default 0)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_expand(AntiGeometryHandle geom, int n, int m);

/** Meta operation (kis + dual)
 * @param geom Handle to geometry object
 * @param n Meta parameter (default 2)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_meta(AntiGeometryHandle geom, int n);

/** Bevel operation (truncate + ambo)
 * @param geom Handle to geometry object
 * @param n Bevel parameter (default 2)
 * @param ratio Truncation ratio
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_bevel(AntiGeometryHandle geom, int n, double ratio);

/** Snub operation (dual + gyro)
 * @param geom Handle to geometry object
 * @param n Snub parameter (default 2)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_snub(AntiGeometryHandle geom, int n);

/** Ortho operation (combination of operations)
 * @param geom Handle to geometry object
 * @param n First parameter (default 2)
 * @param m Second parameter (default 0)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_ortho(AntiGeometryHandle geom, int n, int m);

/*---------------------------------------------------------------------------
 * Polyhedra Generators (Conway Notation)
 *---------------------------------------------------------------------------*/

/** Generate polyhedron from Conway notation
 * @param geom Handle to geometry object
 * @param notation Conway notation string (e.g., "kT" for kis tetrahedron)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_conway_notation(AntiGeometryHandle geom,
                                               const char* notation);

/*---------------------------------------------------------------------------
 * Zonohedra Generators
 *---------------------------------------------------------------------------*/

/** Generate a zonohedron from a star of vectors
 * A zonohedron is formed by the Minkowski sum of line segments
 * @param geom Handle to geometry object
 * @param star_vectors Array of vectors (x,y,z coordinates flattened)
 * @param num_vectors Number of vectors in the star
 * @return Status code */
ANTIPRISM_API AntiStatus anti_make_zonohedron(AntiGeometryHandle geom,
                                               const double* star_vectors,
                                               int num_vectors);

/** Generate a polar zonohedron from an ordered star
 * @param geom Handle to geometry object
 * @param star_vectors Array of ordered vectors (x,y,z coordinates flattened)
 * @param num_vectors Number of vectors in the star
 * @param step Step this many places to get to next vector (default: 1)
 * @param spiral_step Step between ridges of spirallohedron, 0 for regular (default: 0)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_make_polar_zonohedron(AntiGeometryHandle geom,
                                                     const double* star_vectors,
                                                     int num_vectors,
                                                     int step,
                                                     int spiral_step);

/*---------------------------------------------------------------------------
 * Polygon-Based Polyhedra Generators
 *---------------------------------------------------------------------------*/

/** Generate an N-sided prism
 * @param geom Handle to geometry object
 * @param n Number of sides (must be >= 3)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_make_prism(AntiGeometryHandle geom, int n);

/** Generate an N-sided antiprism
 * @param geom Handle to geometry object
 * @param n Number of sides (must be >= 3)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_make_antiprism(AntiGeometryHandle geom, int n);

/** Generate an N-sided pyramid
 * @param geom Handle to geometry object
 * @param n Number of sides (must be >= 3)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_make_pyramid(AntiGeometryHandle geom, int n);

/** Generate an N-sided dipyramid (bipyramid)
 * @param geom Handle to geometry object
 * @param n Number of sides (must be >= 3)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_make_dipyramid(AntiGeometryHandle geom, int n);

/** Generate an N-sided cupola
 * @param geom Handle to geometry object
 * @param n Number of sides (must be >= 2)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_make_cupola(AntiGeometryHandle geom, int n);

/** Generate a geodesic sphere
 * @param geom Handle to geometry object
 * @param frequency Subdivision frequency (1-10 recommended)
 * @param method Base polyhedron: 0=icosahedron, 1=octahedron, 2=tetrahedron
 * @return Status code */
ANTIPRISM_API AntiStatus anti_make_geodesic(AntiGeometryHandle geom, int frequency, int method);

/*---------------------------------------------------------------------------
 * Symmetrohedra Generators (Kaplan-Hart notation)
 *---------------------------------------------------------------------------*/

/** Generate a symmetrohedron using Kaplan-Hart notation (matches CLI: -k sym,mult0,mult1,mult2)
 * @param geom Handle to geometry object
 * @param sym Symmetry type: 'T' (tetrahedral), 'O' (octahedral), 'I' (icosahedral)
 * @param mult0 Multiplier for primary axis (0 = skip this axis)
 * @param mult1 Multiplier for secondary axis (0 = skip this axis)
 * @param mult2 Multiplier for tertiary axis (0 = skip this axis)
 * @return Status code
 * @note Axis orders: T=[3,3,2], O=[4,3,2], I=[5,3,2]. p,q calculated from non-zero multipliers. */
ANTIPRISM_API AntiStatus anti_make_symmetro_kaplan_hart(
    AntiGeometryHandle geom, char sym, int mult0, int mult1, int mult2);

/** Generate a symmetrohedron with advanced parameters
 * @param geom Handle to geometry object
 * @param sym Symmetry type: 'T', 'O', 'I', 'D', 'S', 'C', 'V', 'H'
 * @param p First Schläfli parameter
 * @param q Second Schläfli parameter
 * @param l Multiplier for first axis
 * @param m Multiplier for second axis
 * @param d0 D value for first axis (default 1)
 * @param d1 D value for second axis (default 1)
 * @param rotation Rotation angle in degrees (default 0)
 * @param sym_id Symmetry ID number (typically 1)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_make_symmetro_advanced(
    AntiGeometryHandle geom, char sym, int p, int q, int l, int m,
    int d0, int d1, double rotation, int sym_id);

/*---------------------------------------------------------------------------
 * 2D Tiling Generators (unitile2d)
 *---------------------------------------------------------------------------*/

/** Generate a uniform 2D tiling on a surface
 * @param geom Handle to geometry object
 * @param pattern Pattern number (1-11):
 *                1=4,4,4,4  2=3,3,3,3,3,3  3=6,6,6  4=3,6,3,6  5=3,3,3,4,4
 *                6=3,3,4,3,4  7=3,3,3,3,6  8=3,12,12  9=4,8,8  10=3,4,6,4  11=4,6,12
 * @param surface_type Surface to tile on:
 *                     0=plane, 1=torus, 2=klein_bottle, 3=mobius_strip
 * @param width Width of tiling (number of pattern repeats)
 * @param height Height of tiling (number of pattern repeats, 0 = use width)
 * @param minor_radius Minor radius for torus/klein/mobius (tube/strip width)
 * @param major_radius Major radius for torus/klein/mobius (ring radius)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_make_unitile2d(
    AntiGeometryHandle geom, int pattern, int surface_type,
    double width, double height, double minor_radius, double major_radius);

/*---------------------------------------------------------------------------
 * Kite-Faced Polyhedra Generators (iso_kite)
 *---------------------------------------------------------------------------*/

/** Generate an isohedral kite-faced polyhedron from a Schwarz triangle model
 * @param geom Handle to geometry object
 * @param model_name Schwarz triangle model name:
 *                   T1, T2 (tetrahedral), O1, O2, O2B (octahedral),
 *                   I1-I10 (icosahedral variants)
 * @param height_a Height of kite apex on OA (0 = calculate automatically)
 * @param height_b Height of kite apex on OB (0 = calculate automatically)
 * @param height_c Height of kite side vertex on OC (0 = calculate automatically)
 * @return Status code */
ANTIPRISM_API AntiStatus anti_make_iso_kite(
    AntiGeometryHandle geom, const char* model_name,
    double height_a, double height_b, double height_c);

/** Generate a trapezohedron (kite-faced dipyramid)
 * @param geom Handle to geometry object
 * @param n Numerator of fraction (n >= 2)
 * @param d Denominator of fraction (0 < d < n)
 * @param height_a Height of kite apex on OA (0 = use default 1.0)
 * @param height_b Height of kite apex on OB (0 = use default 1.0)
 * @return Status code
 * @note Creates a trapezohedron based on fraction n/d */
ANTIPRISM_API AntiStatus anti_make_trapezohedron(
    AntiGeometryHandle geom, int n, int d,
    double height_a, double height_b);

/*---------------------------------------------------------------------------
 * Geometry Information
 *---------------------------------------------------------------------------*/

/** Get centroid of geometry
 * @param geom Handle to geometry object
 * @param x Output for x coordinate
 * @param y Output for y coordinate
 * @param z Output for z coordinate
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_centroid(AntiGeometryHandle geom,
                                                 double* x, double* y, double* z);

/** Get volume of geometry
 * @param geom Handle to geometry object
 * @param volume Output for volume
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_volume(AntiGeometryHandle geom,
                                               double* volume);

/** Get face color
 * @param geom Handle to geometry object
 * @param face_idx Face index
 * @param r Output for red component (0-255)
 * @param g Output for green component (0-255)
 * @param b Output for blue component (0-255)
 * @param a Output for alpha component (0-255)
 * @return Status code (ANTI_OK if color is set, ANTI_ERROR_INVALID_INDEX if no color) */
ANTIPRISM_API AntiStatus anti_geometry_get_face_color(AntiGeometryHandle geom,
                                                       int face_idx,
                                                       int* r, int* g, int* b, int* a);

/** Check if geometry is oriented
 * @param geom Handle to geometry object
 * @return 1 if oriented, 0 if not, -1 on error */
ANTIPRISM_API int anti_geometry_is_oriented(AntiGeometryHandle geom);

/** Get all face normals
 * @param geom Handle to geometry object
 * @param normals Output array (size must be >= 3 * num_faces)
 * @param max_faces Maximum faces to process
 * @return Number of faces processed, or -1 on error */
ANTIPRISM_API int anti_geometry_get_face_normals(AntiGeometryHandle geom,
                                                  double* normals,
                                                  int max_faces);

/** Get normal for a single face
 * @param geom Handle to geometry object
 * @param f_idx Face index
 * @param nx Output for x component of normal
 * @param ny Output for y component of normal
 * @param nz Output for z component of normal
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_get_face_normal(AntiGeometryHandle geom,
                                                        int f_idx,
                                                        double* nx, double* ny, double* nz);

/** Get all vertex normals (averaged from surrounding face normals)
 * @param geom Handle to geometry object
 * @param normals Output array (size must be >= 3 * num_verts)
 * @param max_verts Maximum vertices to process
 * @return Number of vertices processed, or -1 on error */
ANTIPRISM_API int anti_geometry_get_vertex_normals(AntiGeometryHandle geom,
                                                    double* normals,
                                                    int max_verts);

/** Get normal for a single vertex
 * @param geom Handle to geometry object
 * @param v_idx Vertex index
 * @param nx Output for x component of normal
 * @param ny Output for y component of normal
 * @param nz Output for z component of normal
 * @return Status code */
ANTIPRISM_API AntiStatus anti_geometry_get_vertex_normal(AntiGeometryHandle geom,
                                                          int v_idx,
                                                          double* nx, double* ny, double* nz);

#ifdef __cplusplus
}
#endif

#endif /* ANTIPRISM_C_API_H */
