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

#include "antiprism_c_api.h"
#include "antiprism.h"
#include "geometryutils.h"
#include "symmetro_wrapper.h"

#include <cstring>
#include <cstdlib>
#include <sstream>
#include <cmath>

using namespace anti;

/* Internal helpers */
namespace {
  inline Geometry* to_geom(AntiGeometryHandle h) {
    return static_cast<Geometry*>(h);
  }

  inline AntiGeometryHandle from_geom(Geometry* g) {
    return static_cast<AntiGeometryHandle>(g);
  }
}

/*---------------------------------------------------------------------------
 * Library Information
 *---------------------------------------------------------------------------*/

ANTIPRISM_API const char* anti_get_version() {
  return "Antiprism 0.32.99 (C API)";
}

ANTIPRISM_API const char* anti_get_info() {
  return "Antiprism - Polyhedra Manipulation Library\n"
         "http://www.antiprism.com";
}

/*---------------------------------------------------------------------------
 * Geometry Creation/Destruction
 *---------------------------------------------------------------------------*/

ANTIPRISM_API AntiGeometryHandle anti_geometry_create() {
  try {
    Geometry* geom = new Geometry();
    return from_geom(geom);
  }
  catch (...) {
    return nullptr;
  }
}

ANTIPRISM_API void anti_geometry_destroy(AntiGeometryHandle geom) {
  if (geom) {
    delete to_geom(geom);
  }
}

ANTIPRISM_API AntiStatus anti_geometry_clear(AntiGeometryHandle geom) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    to_geom(geom)->clear_all();
    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

/*---------------------------------------------------------------------------
 * Geometry Import/Export
 *---------------------------------------------------------------------------*/

ANTIPRISM_API AntiStatus anti_geometry_read_off_string(AntiGeometryHandle geom,
                                                        const char* off_data) {
  if (!geom || !off_data)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    // Create a temporary file in memory or use a string stream approach
    // For simplicity, we'll write to a temp file
    const char* temp_file = "/tmp/antiprism_temp.off";
    FILE* f = fopen(temp_file, "w");
    if (!f)
      return ANTI_ERROR_FILE;

    fprintf(f, "%s", off_data);
    fclose(f);

    Status stat = to_geom(geom)->read(temp_file);
    remove(temp_file);

    if (!stat.is_ok())
      return ANTI_ERROR_PARSE;

    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_write_off_string(AntiGeometryHandle geom,
                                                         char** buffer,
                                                         int sig_digits) {
  if (!geom || !buffer)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    // Write to temp file then read back
    const char* temp_file = "/tmp/antiprism_temp_out.off";
    Status stat = to_geom(geom)->write(temp_file, sig_digits);
    if (!stat.is_ok())
      return ANTI_ERROR_FILE;

    // Read file contents
    FILE* f = fopen(temp_file, "r");
    if (!f)
      return ANTI_ERROR_FILE;

    fseek(f, 0, SEEK_END);
    long size = ftell(f);
    fseek(f, 0, SEEK_SET);

    char* data = (char*)malloc(size + 1);
    if (!data) {
      fclose(f);
      return ANTI_ERROR_MEMORY;
    }

    fread(data, 1, size, f);
    data[size] = '\0';
    fclose(f);
    remove(temp_file);

    *buffer = data;
    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_read_resource(AntiGeometryHandle geom,
                                                      const char* name) {
  if (!geom || !name)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Status stat = to_geom(geom)->read_resource(name);
    if (!stat.is_ok())
      return ANTI_ERROR_PARSE;

    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API void anti_free_string(char* str) {
  free(str);
}

/*---------------------------------------------------------------------------
 * Geometry Query - Element Counts
 *---------------------------------------------------------------------------*/

ANTIPRISM_API int anti_geometry_num_verts(AntiGeometryHandle geom) {
  if (!geom)
    return -1;

  try {
    return (int)to_geom(geom)->verts().size();
  }
  catch (...) {
    return -1;
  }
}

ANTIPRISM_API int anti_geometry_num_edges(AntiGeometryHandle geom) {
  if (!geom)
    return -1;

  try {
    return (int)to_geom(geom)->edges().size();
  }
  catch (...) {
    return -1;
  }
}

ANTIPRISM_API int anti_geometry_num_faces(AntiGeometryHandle geom) {
  if (!geom)
    return -1;

  try {
    return (int)to_geom(geom)->faces().size();
  }
  catch (...) {
    return -1;
  }
}

/*---------------------------------------------------------------------------
 * Geometry Query - Vertex Data
 *---------------------------------------------------------------------------*/

ANTIPRISM_API AntiStatus anti_geometry_get_vert(AntiGeometryHandle geom,
                                                 int v_idx,
                                                 double* x, double* y, double* z) {
  if (!geom || !x || !y || !z)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Geometry* g = to_geom(geom);
    if (v_idx < 0 || v_idx >= (int)g->verts().size())
      return ANTI_ERROR_INVALID_INDEX;

    const Vec3d& v = g->verts(v_idx);
    *x = v[0];
    *y = v[1];
    *z = v[2];

    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API int anti_geometry_get_all_verts(AntiGeometryHandle geom,
                                               double* coords,
                                               int max_verts) {
  if (!geom || !coords)
    return -1;

  try {
    Geometry* g = to_geom(geom);
    const auto& verts = g->verts();
    int num_to_copy = (max_verts < (int)verts.size()) ? max_verts : (int)verts.size();

    for (int i = 0; i < num_to_copy; i++) {
      coords[i * 3 + 0] = verts[i][0];
      coords[i * 3 + 1] = verts[i][1];
      coords[i * 3 + 2] = verts[i][2];
    }

    return num_to_copy;
  }
  catch (...) {
    return -1;
  }
}

/*---------------------------------------------------------------------------
 * Geometry Query - Face Data
 *---------------------------------------------------------------------------*/

ANTIPRISM_API int anti_geometry_face_num_verts(AntiGeometryHandle geom,
                                                int f_idx) {
  if (!geom)
    return -1;

  try {
    Geometry* g = to_geom(geom);
    if (f_idx < 0 || f_idx >= (int)g->faces().size())
      return -1;

    return (int)g->faces(f_idx).size();
  }
  catch (...) {
    return -1;
  }
}

ANTIPRISM_API int anti_geometry_get_face(AntiGeometryHandle geom,
                                          int f_idx,
                                          int* indices,
                                          int max_indices) {
  if (!geom || !indices)
    return -1;

  try {
    Geometry* g = to_geom(geom);
    if (f_idx < 0 || f_idx >= (int)g->faces().size())
      return -1;

    const auto& face = g->faces(f_idx);
    int num_to_copy = (max_indices < (int)face.size()) ? max_indices : (int)face.size();

    for (int i = 0; i < num_to_copy; i++) {
      indices[i] = face[i];
    }

    return num_to_copy;
  }
  catch (...) {
    return -1;
  }
}

ANTIPRISM_API int anti_geometry_get_all_faces(AntiGeometryHandle geom,
                                               int* buffer,
                                               int buffer_size) {
  if (!geom || !buffer)
    return -1;

  try {
    Geometry* g = to_geom(geom);
    const auto& faces = g->faces();

    int pos = 0;
    for (size_t i = 0; i < faces.size(); i++) {
      const auto& face = faces[i];
      int face_size = (int)face.size();

      // Check if we have room for size + all indices
      if (pos + 1 + face_size > buffer_size)
        break;

      buffer[pos++] = face_size;
      for (int j = 0; j < face_size; j++) {
        buffer[pos++] = face[j];
      }
    }

    return pos;
  }
  catch (...) {
    return -1;
  }
}

/*---------------------------------------------------------------------------
 * Geometry Modification - Add Elements
 *---------------------------------------------------------------------------*/

ANTIPRISM_API int anti_geometry_add_vert(AntiGeometryHandle geom,
                                          double x, double y, double z) {
  if (!geom)
    return -1;

  try {
    return to_geom(geom)->add_vert(Vec3d(x, y, z));
  }
  catch (...) {
    return -1;
  }
}

ANTIPRISM_API int anti_geometry_add_face(AntiGeometryHandle geom,
                                          const int* indices,
                                          int num_indices) {
  if (!geom || !indices || num_indices < 3)
    return -1;

  try {
    std::vector<int> face(indices, indices + num_indices);
    return to_geom(geom)->add_face(face);
  }
  catch (...) {
    return -1;
  }
}

/*---------------------------------------------------------------------------
 * Geometry Transformations
 *---------------------------------------------------------------------------*/

ANTIPRISM_API AntiStatus anti_geometry_translate(AntiGeometryHandle geom,
                                                  double dx, double dy, double dz) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Trans3d trans = Trans3d::translate(Vec3d(dx, dy, dz));
    to_geom(geom)->transform(trans);
    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_scale(AntiGeometryHandle geom,
                                              double scale) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Trans3d trans = Trans3d::scale(scale);
    to_geom(geom)->transform(trans);
    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_scale_xyz(AntiGeometryHandle geom,
                                                  double sx, double sy, double sz) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Trans3d trans = Trans3d::scale(sx, sy, sz);
    to_geom(geom)->transform(trans);
    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_rotate_x(AntiGeometryHandle geom,
                                                 double angle) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Trans3d trans = Trans3d::rotate(Vec3d::X, angle);
    to_geom(geom)->transform(trans);
    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_rotate_y(AntiGeometryHandle geom,
                                                 double angle) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Trans3d trans = Trans3d::rotate(Vec3d::Y, angle);
    to_geom(geom)->transform(trans);
    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_rotate_z(AntiGeometryHandle geom,
                                                 double angle) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Trans3d trans = Trans3d::rotate(Vec3d::Z, angle);
    to_geom(geom)->transform(trans);
    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_unitize(AntiGeometryHandle geom) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Geometry* g = to_geom(geom);
    Vec3d cent = g->centroid();

    // Center at origin
    Trans3d trans = Trans3d::translate(-cent);
    g->transform(trans);

    // Find max distance from origin
    double max_dist = 0.0;
    for (const auto& v : g->verts()) {
      double dist = v.len();
      if (dist > max_dist)
        max_dist = dist;
    }

    // Scale to unit sphere
    if (max_dist > 1e-10) {
      trans = Trans3d::scale(1.0 / max_dist);
      g->transform(trans);
    }

    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

/*---------------------------------------------------------------------------
 * Geometry Operations
 *---------------------------------------------------------------------------*/

ANTIPRISM_API AntiStatus anti_geometry_convex_hull(AntiGeometryHandle geom) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Status stat = to_geom(geom)->set_hull();
    if (!stat.is_ok())
      return ANTI_ERROR_UNKNOWN;

    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_triangulate(AntiGeometryHandle geom) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    to_geom(geom)->triangulate();
    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_orient(AntiGeometryHandle geom) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    to_geom(geom)->orient(1); // Positive orientation
    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_orient_reverse(AntiGeometryHandle geom) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    to_geom(geom)->orient_reverse();
    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_dual(AntiGeometryHandle geom,
                                             double recip_rad) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Geometry dual;
    get_dual(dual, *to_geom(geom), recip_rad);
    *to_geom(geom) = dual;
    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_truncate(AntiGeometryHandle geom,
                                                 double ratio,
                                                 int order) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    truncate_verts(*to_geom(geom), ratio, order);
    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_kis(AntiGeometryHandle geom, int n) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Geometry& g = *to_geom(geom);

    // Get face centers
    std::vector<Vec3d> centers;
    g.face_cents(centers);

    // Build new faces
    std::vector<std::vector<int>> faces_new;
    std::vector<int> face;

    const std::vector<std::vector<int>>& faces = g.faces();
    int num_verts = g.verts().size();

    for (unsigned int i = 0; i < faces.size(); i++) {
      // Skip faces that don't match the size filter
      if (n > 0 && (int)faces[i].size() != n) {
        faces_new.push_back(faces[i]);
        continue;
      }

      // Add center vertex
      g.add_vert(centers[i]);
      int center_idx = num_verts++;

      // Create triangular faces from center to each edge
      for (unsigned int j = 0; j < faces[i].size(); j++) {
        face.push_back(center_idx);
        face.push_back(faces[i][j]);
        face.push_back(faces[i][(j + 1) % faces[i].size()]);
        faces_new.push_back(face);
        face.clear();
      }
    }

    // Replace faces
    if (faces_new.size() > 0) {
      g.clear(FACES);
      for (const auto& f : faces_new)
        g.add_face(f);
    }

    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_ambo(AntiGeometryHandle geom) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Geometry& g = *to_geom(geom);

    std::vector<std::vector<int>> &faces = g.raw_faces();
    std::vector<Vec3d> &verts = g.raw_verts();

    std::map<std::pair<int,int>, int> vert_map;
    std::vector<Vec3d> verts_new;

    // Create vertices at edge midpoints
    int vert_num = 0;
    for (const auto& face : faces) {
      for (unsigned int j = 0; j < face.size(); j++) {
        int v1 = face[j];
        int v2 = face[(j + 1) % face.size()];

        std::pair<int,int> edge = std::make_pair(std::min(v1, v2), std::max(v1, v2));

        if (vert_map.find(edge) == vert_map.end()) {
          vert_map[edge] = vert_num++;
          verts_new.push_back((verts[v1] + verts[v2]) * 0.5);
        }
      }
    }

    // Build new faces (one for each original face, one for each original vertex)
    std::vector<std::vector<int>> faces_new;

    // Faces from original faces
    for (const auto& face : faces) {
      std::vector<int> new_face;
      for (unsigned int j = 0; j < face.size(); j++) {
        int v1 = face[j];
        int v2 = face[(j + 1) % face.size()];
        std::pair<int,int> edge = std::make_pair(std::min(v1, v2), std::max(v1, v2));
        new_face.push_back(vert_map[edge]);
      }
      faces_new.push_back(new_face);
    }

    // Faces from original vertices
    std::map<int, std::vector<std::pair<int,int>>> vert_edges;
    for (const auto& kv : vert_map) {
      vert_edges[kv.first.first].push_back(std::make_pair(kv.first.second, kv.second));
      vert_edges[kv.first.second].push_back(std::make_pair(kv.first.first, kv.second));
    }

    for (auto& kv : vert_edges) {
      // Sort edges around vertex to create proper face ordering
      if (kv.second.size() >= 3) {
        std::vector<int> new_face;
        for (const auto& edge : kv.second)
          new_face.push_back(edge.second);
        faces_new.push_back(new_face);
      }
    }

    // Replace geometry
    g.clear_all();
    verts = verts_new;
    for (const auto& f : faces_new)
      g.add_face(f);

    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

static AntiStatus gyro_single(Geometry& g) {
  std::vector<std::vector<int>> &faces = g.raw_faces();
  std::vector<Vec3d> &verts = g.raw_verts();

  std::map<std::pair<int,int>, int> edge_vert_map;
  std::vector<Vec3d> verts_new;
  int vert_num = 0;

  // Create new vertices: 1/3 along each edge from both ends
  for (const auto& face : faces) {
    for (unsigned int j = 0; j < face.size(); j++) {
      int v1 = face[j];
      int v2 = face[(j + 1) % face.size()];

      std::pair<int,int> edge_fwd = std::make_pair(v1, v2);

      if (edge_vert_map.find(edge_fwd) == edge_vert_map.end()) {
        edge_vert_map[edge_fwd] = vert_num++;
        verts_new.push_back(verts[v1] + (verts[v2] - verts[v1]) * (1.0/3.0));
      }
    }
  }

  // Build new faces
  std::vector<std::vector<int>> faces_new;

  for (unsigned int i = 0; i < faces.size(); i++) {
    const auto& face = faces[i];

    // Central n-gon (rotated)
    std::vector<int> center_face;
    for (unsigned int j = 0; j < face.size(); j++) {
      int v1 = face[j];
      int v2 = face[(j + 1) % face.size()];
      center_face.push_back(edge_vert_map[std::make_pair(v1, v2)]);
    }
    faces_new.push_back(center_face);

    // Triangles at each vertex
    for (unsigned int j = 0; j < face.size(); j++) {
      int v1 = face[j];
      int v2 = face[(j + 1) % face.size()];
      int v0 = face[(j + face.size() - 1) % face.size()];

      std::vector<int> tri;
      tri.push_back(edge_vert_map[std::make_pair(v0, v1)]);
      tri.push_back(edge_vert_map[std::make_pair(v1, v2)]);
      tri.push_back(edge_vert_map[std::make_pair(v2, v1)]); // reverse edge

      faces_new.push_back(tri);
    }
  }

  // Replace geometry
  g.clear_all();
  verts = verts_new;
  for (const auto& f : faces_new)
    g.add_face(f);

  return ANTI_OK;
}

ANTIPRISM_API AntiStatus anti_geometry_gyro(AntiGeometryHandle geom, int n) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Geometry& g = *to_geom(geom);
    // Note: n parameter reserved for future use (gyro ratio control)
    return gyro_single(g);
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_join(AntiGeometryHandle geom) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  // Join = dual of ambo (jC = daC)
  AntiStatus status = anti_geometry_ambo(geom);
  if (status != ANTI_OK)
    return status;
  return anti_geometry_dual(geom, 1.0);
}

ANTIPRISM_API AntiStatus anti_geometry_needle(AntiGeometryHandle geom, double height) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Geometry& g = *to_geom(geom);

    // Get face centers and scale them outward
    std::vector<Vec3d> centers;
    g.face_cents(centers);

    std::vector<std::vector<int>> faces_new;
    std::vector<int> face;

    const std::vector<std::vector<int>>& faces = g.faces();
    int num_verts = g.verts().size();

    for (unsigned int i = 0; i < faces.size(); i++) {
      // Add needle point (scaled outward from face center)
      Vec3d needle_point = centers[i] * height;
      g.add_vert(needle_point);
      int needle_idx = num_verts++;

      // Create triangular faces from needle point to each edge
      for (unsigned int j = 0; j < faces[i].size(); j++) {
        face.push_back(needle_idx);
        face.push_back(faces[i][j]);
        face.push_back(faces[i][(j + 1) % faces[i].size()]);
        faces_new.push_back(face);
        face.clear();
      }
    }

    // Replace faces
    if (faces_new.size() > 0) {
      g.clear(FACES);
      for (const auto& f : faces_new)
        g.add_face(f);
    }

    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_zip(AntiGeometryHandle geom) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  // Zip = dual of kis (zC = dkC)
  AntiStatus status = anti_geometry_kis(geom, 0);
  if (status != ANTI_OK)
    return status;
  return anti_geometry_dual(geom, 1.0);
}

ANTIPRISM_API AntiStatus anti_geometry_subdivide(AntiGeometryHandle geom, int n, int m) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  // Note: n and m parameters reserved for future use (subdivision control)
  try {
    Geometry& g = *to_geom(geom);
    std::vector<std::vector<int>> &faces = g.raw_faces();
    std::vector<Vec3d> &verts = g.raw_verts();

    std::map<std::pair<int,int>, int> edge_verts;
    std::vector<Vec3d> verts_new = verts;
    int num_verts = verts.size();

    for (const auto& face : faces) {
      for (unsigned int j = 0; j < face.size(); j++) {
        int v1 = face[j];
        int v2 = face[(j + 1) % face.size()];
        std::pair<int,int> edge = std::make_pair(std::min(v1, v2), std::max(v1, v2));

        if (edge_verts.find(edge) == edge_verts.end()) {
          edge_verts[edge] = num_verts++;
          verts_new.push_back((verts[v1] + verts[v2]) * 0.5);
        }
      }
    }

    std::vector<Vec3d> centers;
    g.face_cents(centers);
    int center_start = num_verts;
    for (const auto& center : centers) {
      verts_new.push_back(center);
    }

    std::vector<std::vector<int>> faces_new;
    for (unsigned int i = 0; i < faces.size(); i++) {
      const auto& face = faces[i];
      int face_center = center_start + i;

      for (unsigned int j = 0; j < face.size(); j++) {
        int v1 = face[j];
        int v2 = face[(j + 1) % face.size()];

        std::pair<int,int> edge1 = std::make_pair(std::min(v1, v2), std::max(v1, v2));
        std::pair<int,int> edge2 = std::make_pair(std::min(v2, face[(j + 2) % face.size()]),
                                                   std::max(v2, face[(j + 2) % face.size()]));

        std::vector<int> quad;
        quad.push_back(edge_verts[edge1]);
        quad.push_back(v2);
        quad.push_back(edge_verts[edge2]);
        quad.push_back(face_center);
        faces_new.push_back(quad);
      }
    }

    g.clear_all();
    verts = verts_new;
    for (const auto& f : faces_new)
      g.add_face(f);

    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_expand(AntiGeometryHandle geom, int n, int m) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  // Note: n and m parameters reserved for future use
  // Expand = ambo + ambo
  AntiStatus status = anti_geometry_ambo(geom);
  if (status != ANTI_OK)
    return status;
  return anti_geometry_ambo(geom);
}

ANTIPRISM_API AntiStatus anti_geometry_meta(AntiGeometryHandle geom, int n) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  // Note: n parameter reserved for future use
  // Meta = kis + dual
  AntiStatus status = anti_geometry_kis(geom, 0);
  if (status != ANTI_OK)
    return status;
  return anti_geometry_dual(geom, 1.0);
}

ANTIPRISM_API AntiStatus anti_geometry_bevel(AntiGeometryHandle geom, int n, double ratio) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  // Note: n parameter reserved for future use
  // Bevel = truncate + ambo
  AntiStatus status = anti_geometry_truncate(geom, ratio, 0);
  if (status != ANTI_OK)
    return status;
  return anti_geometry_ambo(geom);
}

ANTIPRISM_API AntiStatus anti_geometry_snub(AntiGeometryHandle geom, int n) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  // Note: n parameter reserved for future use
  // Snub = dual + gyro
  AntiStatus status = anti_geometry_dual(geom, 1.0);
  if (status != ANTI_OK)
    return status;
  return anti_geometry_gyro(geom, n);
}

ANTIPRISM_API AntiStatus anti_geometry_ortho(AntiGeometryHandle geom, int n, int m) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  // Note: n and m parameters reserved for future use
  // Ortho = join + join
  AntiStatus status = anti_geometry_join(geom);
  if (status != ANTI_OK)
    return status;
  return anti_geometry_join(geom);
}

/*---------------------------------------------------------------------------
 * Polyhedra Generators (Conway Notation)
 *---------------------------------------------------------------------------*/

// Note: This is a placeholder. Full Conway notation support would require
// linking against the conway program's logic, which is in src/conway.cc
// For now, we'll provide basic built-in shapes via read_resource
ANTIPRISM_API AntiStatus anti_conway_notation(AntiGeometryHandle geom,
                                               const char* notation) {
  if (!geom || !notation)
    return ANTI_ERROR_INVALID_HANDLE;

  // Simple mapping of basic Conway seeds to resource names
  if (strcmp(notation, "T") == 0) {
    return anti_geometry_read_resource(geom, "tet");
  }
  else if (strcmp(notation, "C") == 0) {
    return anti_geometry_read_resource(geom, "cube");
  }
  else if (strcmp(notation, "O") == 0) {
    return anti_geometry_read_resource(geom, "oct");
  }
  else if (strcmp(notation, "I") == 0) {
    return anti_geometry_read_resource(geom, "ico");
  }
  else if (strcmp(notation, "D") == 0) {
    return anti_geometry_read_resource(geom, "dodec");
  }

  // For more complex notation, this would need the full Conway implementation
  return ANTI_ERROR_PARSE;
}

/*---------------------------------------------------------------------------
 * Zonohedra Generators
 *---------------------------------------------------------------------------*/

ANTIPRISM_API AntiStatus anti_make_zonohedron(AntiGeometryHandle geom,
                                               const double* star_vectors,
                                               int num_vectors) {
  if (!geom || !star_vectors || num_vectors < 1)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    // Convert flat array to vector of Vec3d
    std::vector<Vec3d> star;
    star.reserve(num_vectors);
    for (int i = 0; i < num_vectors; i++) {
      star.push_back(Vec3d(
        star_vectors[i * 3 + 0],
        star_vectors[i * 3 + 1],
        star_vectors[i * 3 + 2]
      ));
    }

    // Generate zonohedron
    Status stat = make_zonohedron(*to_geom(geom), star);
    return stat.is_ok() ? ANTI_OK : ANTI_ERROR_UNKNOWN;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_make_polar_zonohedron(AntiGeometryHandle geom,
                                                     const double* star_vectors,
                                                     int num_vectors,
                                                     int step,
                                                     int spiral_step) {
  if (!geom || !star_vectors || num_vectors < 1)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    // Convert flat array to vector of Vec3d
    std::vector<Vec3d> star;
    star.reserve(num_vectors);
    for (int i = 0; i < num_vectors; i++) {
      star.push_back(Vec3d(
        star_vectors[i * 3 + 0],
        star_vectors[i * 3 + 1],
        star_vectors[i * 3 + 2]
      ));
    }

    // Generate polar zonohedron
    Status stat = make_polar_zonohedron(*to_geom(geom), star, step, spiral_step);
    return stat.is_ok() ? ANTI_OK : ANTI_ERROR_UNKNOWN;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

/*---------------------------------------------------------------------------
 * Polygon-Based Polyhedra Generators
 *---------------------------------------------------------------------------*/

ANTIPRISM_API AntiStatus anti_make_prism(AntiGeometryHandle geom, int n) {
  if (!geom || n < 3)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Polygon pgon(n, 1, Polygon::prism, Polygon::sub_default);
    Status stat = pgon.make_poly(*to_geom(geom));
    return stat.is_ok() ? ANTI_OK : ANTI_ERROR_UNKNOWN;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_make_antiprism(AntiGeometryHandle geom, int n) {
  if (!geom || n < 3)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Polygon pgon(n, 1, Polygon::antiprism, Polygon::sub_default);
    Status stat = pgon.make_poly(*to_geom(geom));
    return stat.is_ok() ? ANTI_OK : ANTI_ERROR_UNKNOWN;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_make_pyramid(AntiGeometryHandle geom, int n) {
  if (!geom || n < 3)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Polygon pgon(n, 1, Polygon::pyramid, Polygon::sub_default);
    Status stat = pgon.make_poly(*to_geom(geom));
    return stat.is_ok() ? ANTI_OK : ANTI_ERROR_UNKNOWN;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_make_dipyramid(AntiGeometryHandle geom, int n) {
  if (!geom || n < 3)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Polygon pgon(n, 1, Polygon::dipyramid, Polygon::sub_default);
    Status stat = pgon.make_poly(*to_geom(geom));
    return stat.is_ok() ? ANTI_OK : ANTI_ERROR_UNKNOWN;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_make_cupola(AntiGeometryHandle geom, int n) {
  if (!geom || n < 2)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Polygon pgon(n, 1, Polygon::cupola, Polygon::sub_default);
    Status stat = pgon.make_poly(*to_geom(geom));
    return stat.is_ok() ? ANTI_OK : ANTI_ERROR_UNKNOWN;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_make_geodesic(AntiGeometryHandle geom, int frequency, int method) {
  if (!geom || frequency < 1)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Geometry* out_geom = to_geom(geom);

    // Create base polyhedron based on method
    Geometry base;
    switch (method) {
      case 0:  // Icosahedron
        base.read_resource("ico");
        break;
      case 1:  // Octahedron
        base.read_resource("oct");
        break;
      case 2:  // Tetrahedron
        base.read_resource("tet");
        break;
      default:
        return ANTI_ERROR_INVALID_HANDLE;
    }

    // Generate geodesic sphere using Class I pattern (m=0, n=frequency)
    bool success = make_geodesic_sphere(*out_geom, base, 0, frequency);

    return success ? ANTI_OK : ANTI_ERROR_UNKNOWN;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

/*---------------------------------------------------------------------------
 * Symmetrohedra Generators
 *---------------------------------------------------------------------------*/

ANTIPRISM_API AntiStatus anti_make_symmetro_kaplan_hart(
    AntiGeometryHandle geom, char sym, int mult0, int mult1, int mult2) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  // Validate symmetry type
  if (sym != 'T' && sym != 'O' && sym != 'I')
    return ANTI_ERROR_INVALID_HANDLE;

  // Validate multipliers (at least one must be non-zero, at most two can be non-zero)
  if (mult0 < 0 || mult1 < 0 || mult2 < 0)
    return ANTI_ERROR_INVALID_HANDLE;

  int num_multipliers = (mult0 > 0 ? 1 : 0) + (mult1 > 0 ? 1 : 0) + (mult2 > 0 ? 1 : 0);
  if (num_multipliers == 0 || num_multipliers == 3)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    // Calculate p and q from multipliers (same logic as CLI tool)
    // Axis orders for each symmetry: T=[3,3,2], O=[4,3,2], I=[5,3,2]
    int orders[3];
    orders[0] = (sym == 'T') ? 3 : ((sym == 'O') ? 4 : 5);
    orders[1] = 3;
    orders[2] = 2;

    int p, q;
    if (num_multipliers == 1) {
      // Single axis - both p and q are same order
      if (mult0 > 0) {
        p = q = orders[0];
      } else if (mult1 > 0) {
        p = q = orders[1];
      } else { // mult2 > 0
        p = q = orders[2];
      }
    } else { // num_multipliers == 2
      // Two axes - p and q are different orders
      if (mult0 > 0 && mult1 > 0) {
        p = orders[0];
        q = orders[1];
      } else if (mult0 > 0 && mult2 > 0) {
        p = orders[0];
        q = orders[2];
      } else { // mult1 > 0 && mult2 > 0
        p = orders[1];
        q = orders[2];
      }
    }

    // Create symmetro object
    symmetro symm;

    // Set symmetry
    symm.setSym(sym, p, q, 0, 1);  // sym_id always 1 for basic usage

    // Build index array of non-zero multipliers (matching CLI tool behavior)
    // setMult expects indices in order: setMult(0, first_nonzero_mult), setMult(1, second_nonzero_mult)
    std::vector<int> idx;
    std::vector<int> mults = {mult0, mult1, mult2};
    for (int i = 0; i < 3; i++) {
      if (mults[i] > 0) {
        idx.push_back(i);
      }
    }

    // If only one multiplier, duplicate the index (same polygon on both axes)
    if (idx.size() == 1) {
      idx.push_back(idx[0]);
    }

    // Set multipliers using the non-zero indices
    for (size_t i = 0; i < idx.size(); i++) {
      symm.setMult(i, mults[idx[i]]);
    }

    // Fill symmetry vectors (Kaplan-Hart mode)
    std::string error_msg;
    int err = symm.fill_sym_vec('k', &error_msg);
    if (err != 0) {
      // Error in fill_sym_vec - invalid p,q combination for this symmetry
      return ANTI_ERROR_PARSE;  // More specific error than UNKNOWN
    }

    // Calculate polygons
    double angle_between_axes = NAN;  // Let it calculate automatically
    std::vector<Geometry> pgeoms = symm.calc_polygons(
        'k',      // mode: Kaplan-Hart
        0.0,      // rotation
        0.0,      // rotation_multiplier
        false,    // add_pi
        false,    // swap_axes
        0.0,      // offset
        false,    // verbose
        angle_between_axes,
        &error_msg
    );

    // Check if generation failed
    if (!error_msg.empty()) {
      // Error in calc_polygons
      return ANTI_ERROR_PARSE;  // More specific error than UNKNOWN
    }

    // Check if polygons were generated
    if (pgeoms.empty()) {
      return ANTI_ERROR_PARSE;  // No polygons generated
    }

    // Combine the two generated polygons into the output geometry
    Geometry* out_geom = to_geom(geom);
    out_geom->clear_all();

    for (const auto& pg : pgeoms) {
      if (pg.verts().size() > 0) {
        out_geom->append(pg);
      }
    }

    // Verify we have valid geometry
    if (out_geom->verts().size() == 0) {
      return ANTI_ERROR_PARSE;  // Empty geometry generated
    }

    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_make_symmetro_advanced(
    AntiGeometryHandle geom, char sym, int p, int q, int l, int m,
    int d0, int d1, double rotation, int sym_id) {
  if (!geom)
    return ANTI_ERROR_INVALID_HANDLE;

  // Validate symmetry type
  if (sym != 'T' && sym != 'O' && sym != 'I' && sym != 'D' &&
      sym != 'S' && sym != 'C' && sym != 'V' && sym != 'H')
    return ANTI_ERROR_INVALID_HANDLE;

  // Validate parameters
  if (p < 2 || q < 2 || l < 0 || m < 0 || sym_id < 1)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    // Create symmetro object
    symmetro symm;

    // Set symmetry
    symm.setSym(sym, p, q, 0, sym_id);

    // Set multipliers for each axis
    symm.setMult(0, l);
    symm.setMult(1, m);

    // Set d values
    symm.setD(0, d0);
    symm.setD(1, d1);

    // Determine mode based on symmetry type
    char mode = 'k';  // Default to Kaplan-Hart
    if (sym == 'S' || sym == 'C' || sym == 'V' || sym == 'H') {
      mode = 'c';  // s/c symmetry mode
    }

    // Fill symmetry vectors
    std::string error_msg;
    int err = symm.fill_sym_vec(mode, &error_msg);
    if (err != 0) {
      return ANTI_ERROR_UNKNOWN;
    }

    // Calculate polygons
    double angle_between_axes = NAN;  // Let it calculate automatically
    std::vector<Geometry> pgeoms = symm.calc_polygons(
        mode,     // mode
        rotation, // rotation
        0.0,      // rotation_multiplier
        false,    // add_pi
        false,    // swap_axes
        0.0,      // offset
        false,    // verbose
        angle_between_axes,
        &error_msg
    );

    // Check if generation failed
    if (!error_msg.empty()) {
      return ANTI_ERROR_UNKNOWN;
    }

    // Combine the two generated polygons into the output geometry
    Geometry* out_geom = to_geom(geom);
    out_geom->clear_all();

    for (const auto& pg : pgeoms) {
      if (pg.verts().size() > 0) {
        out_geom->append(pg);
      }
    }

    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

/*---------------------------------------------------------------------------
 * Geometry Information
 *---------------------------------------------------------------------------*/

ANTIPRISM_API AntiStatus anti_geometry_centroid(AntiGeometryHandle geom,
                                                 double* x, double* y, double* z) {
  if (!geom || !x || !y || !z)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Vec3d cent = to_geom(geom)->centroid();
    *x = cent[0];
    *y = cent[1];
    *z = cent[2];
    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_volume(AntiGeometryHandle geom,
                                               double* volume) {
  if (!geom || !volume)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    GeometryInfo info = to_geom(geom)->get_info();
    *volume = info.volume();
    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_get_face_color(AntiGeometryHandle geom,
                                                       int face_idx,
                                                       int* r, int* g, int* b, int* a) {
  if (!geom || !r || !g || !b || !a)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Geometry* geo = to_geom(geom);

    // Check if face index is valid
    if (face_idx < 0 || face_idx >= (int)geo->faces().size())
      return ANTI_ERROR_INVALID_INDEX;

    // Get face color - FACES = 2 in const.h
    Color col = geo->get_cols()[2].get(face_idx);

    // Check if color is set
    if (!col.is_set())
      return ANTI_ERROR_INVALID_INDEX;

    // Get RGBA values (0-255)
    *r = col[0];
    *g = col[1];
    *b = col[2];
    *a = col[3];

    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API int anti_geometry_is_oriented(AntiGeometryHandle geom) {
  if (!geom)
    return -1;

  try {
    return to_geom(geom)->is_oriented() ? 1 : 0;
  }
  catch (...) {
    return -1;
  }
}

ANTIPRISM_API int anti_geometry_get_face_normals(AntiGeometryHandle geom,
                                                  double* normals,
                                                  int max_faces) {
  if (!geom || !normals)
    return -1;

  try {
    Geometry* g = to_geom(geom);
    std::vector<Vec3d> norms;
    g->face_norms(norms);

    int num_faces = (int)norms.size();
    if (num_faces > max_faces)
      num_faces = max_faces;

    for (int i = 0; i < num_faces; i++) {
      normals[i * 3 + 0] = norms[i][0];
      normals[i * 3 + 1] = norms[i][1];
      normals[i * 3 + 2] = norms[i][2];
    }

    return num_faces;
  }
  catch (...) {
    return -1;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_get_face_normal(AntiGeometryHandle geom,
                                                        int f_idx,
                                                        double* nx, double* ny, double* nz) {
  if (!geom || !nx || !ny || !nz)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Geometry* g = to_geom(geom);
    if (f_idx < 0 || f_idx >= (int)g->faces().size())
      return ANTI_ERROR_INVALID_INDEX;

    Vec3d norm = g->face_norm(f_idx).unit();
    *nx = norm[0];
    *ny = norm[1];
    *nz = norm[2];
    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}

ANTIPRISM_API int anti_geometry_get_vertex_normals(AntiGeometryHandle geom,
                                                    double* normals,
                                                    int max_verts) {
  if (!geom || !normals)
    return -1;

  try {
    Geometry* g = to_geom(geom);
    GeometryInfo info(*g);
    const std::vector<Vec3d>& v_norms = info.get_vert_norms(true);

    int num_verts = (int)v_norms.size();
    if (num_verts > max_verts)
      num_verts = max_verts;

    for (int i = 0; i < num_verts; i++) {
      normals[i * 3 + 0] = v_norms[i][0];
      normals[i * 3 + 1] = v_norms[i][1];
      normals[i * 3 + 2] = v_norms[i][2];
    }

    return num_verts;
  }
  catch (...) {
    return -1;
  }
}

ANTIPRISM_API AntiStatus anti_geometry_get_vertex_normal(AntiGeometryHandle geom,
                                                          int v_idx,
                                                          double* nx, double* ny, double* nz) {
  if (!geom || !nx || !ny || !nz)
    return ANTI_ERROR_INVALID_HANDLE;

  try {
    Geometry* g = to_geom(geom);
    if (v_idx < 0 || v_idx >= (int)g->verts().size())
      return ANTI_ERROR_INVALID_INDEX;

    GeometryInfo info(*g);
    const std::vector<Vec3d>& v_norms = info.get_vert_norms(true);

    if (v_idx >= (int)v_norms.size())
      return ANTI_ERROR_INVALID_INDEX;

    *nx = v_norms[v_idx][0];
    *ny = v_norms[v_idx][1];
    *nz = v_norms[v_idx][2];
    return ANTI_OK;
  }
  catch (...) {
    return ANTI_ERROR_UNKNOWN;
  }
}
