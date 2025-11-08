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
