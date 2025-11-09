/*
   Copyright (c) 2014-2023, Roger Kaufman, Adrian Rossiter
   Copyright (c) 2025, Antiprism Unity Plugin Wrapper

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

/*
   Name: symmetro_wrapper.h
   Description: Wrapper for symmetro functionality - extracted from src/symmetro.cc
                This is a wrapper-only file that does NOT modify the original source.
   Project: Antiprism Unity Plugin - http://www.antiprism.com
*/

#ifndef SYMMETRO_WRAPPER_H
#define SYMMETRO_WRAPPER_H

#include "antiprism.h"

#include <string>
#include <vector>

namespace anti {

/// Symmetrohedra generator using Kaplan-Hart notation
/// Extracted from src/symmetro.cc for use as a library component
class symmetro {
public:
  /// Constructor - initializes default values
  symmetro()
  {
    for (int i = 0; i < 2; i++) {
      mult.push_back(0);
      sym_vec.push_back(Vec3d());
      d.push_back(1);
      d_substitute.push_back(0);
      scale.push_back(1);
    }
  }

  /// Debug output
  void debug(const char mode);

  /// Set symmetry type
  /// @param s Symmetry character: T (tetrahedral), O (octahedral), I (icosahedral), D (dihedral)
  /// @param psym First symmetry parameter (p in Schläfli {p,q})
  /// @param qsym Second symmetry parameter (q in Schläfli {p,q})
  /// @param dih_n For dihedral symmetry, the n value
  /// @param id_no Symmetry ID number
  void setSym(const char s, const int psym, const int qsym, const int dih_n,
              const int id_no);

  /// Set multiplier for an axis
  /// @param a Axis index (0 or 1)
  /// @param m Multiplier value
  void setMult(const int a, const int m);

  /// Set scale for an axis
  /// @param a Axis index (0 or 1)
  /// @param s Scale value
  void setScale(const int a, const double s);

  /// Set d value for an axis
  /// @param a Axis index (0 or 1)
  /// @param dee D value
  void setD(const int a, const int dee);

  /// Set d substitute value for an axis
  /// @param a Axis index (0 or 1)
  /// @param dee D substitute value
  void setD_substitute(const int a, const int dee);

  /// Get order for an axis
  int getOrder(const int a);

  /// Get N value for an axis
  int getN(const int a);

  /// Calculate axis angle
  double axis_angle(const int n, const int d);

  /// Get angle between axes using sine method
  double getAngleBetweenAxesSin(const int axis1, const int axis2);

  /// Swap two vectors
  void swap_vecs(Vec3d &a, Vec3d &b);

  /// Fill symmetry vectors
  /// @param mode Mode character
  /// @param error_msg Optional error message output
  /// @return Status code (0 = success)
  int fill_sym_vec(const char mode, std::string *error_msg);

  /// Calculate angle for polygon
  double angle(const int n, const int d);

  /// Calculate circumradius for polygon
  double circumradius(const int n, const int d);

  /// Substitute polygon on an axis
  void substitute_polygon(Geometry &geom, const int axis_no);

  /// Calculate and generate polygons
  /// @param mode Mode: 'k' (Kaplan-Hart), 't' (twister), 's'/'c' (s/c symmetry)
  /// @param rotation Rotation angle in degrees
  /// @param rotation_multiplier Rotation multiplier
  /// @param add_pi Add PI to rotation
  /// @param swap_axes Swap axes
  /// @param offset Offset value
  /// @param verbose Verbose output
  /// @param angle_between_axes Angle between axes (in/out parameter)
  /// @param error_msg Optional error message output
  /// @return Vector of generated geometries (typically 2 polygons)
  std::vector<Geometry> calc_polygons(const char mode, const double rotation,
                                      const double rotation_multiplier,
                                      const bool add_pi, const bool swap_axes,
                                      const double offset, const bool verbose,
                                      double &angle_between_axes,
                                      std::string *error_msg);

  /// Destructor
  ~symmetro() = default;

private:
  char sym;           // Symmetry type: T, O, I, D
  int p;              // First Schläfli parameter
  int q;              // Second Schläfli parameter
  int dihedral_n;     // For dihedral symmetry
  int sym_id_no;      // Symmetry ID number

  std::vector<int> mult;           // Multipliers for each axis
  std::vector<Vec3d> sym_vec;      // Symmetry vectors for each axis
  std::vector<int> d;              // D values for each axis
  std::vector<int> d_substitute;   // D substitute values
  std::vector<double> scale;       // Scale values for each axis
};

} // namespace anti

#endif // SYMMETRO_WRAPPER_H
