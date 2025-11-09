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
   Name: symmetro_wrapper.cc
   Description: Wrapper implementation for symmetro - extracted from src/symmetro.cc
                This is a wrapper-only file that does NOT modify the original source.
   Project: Antiprism Unity Plugin - http://www.antiprism.com
*/

#include "symmetro_wrapper.h"

#include <algorithm>
#include <cmath>
#include <cstdio>

using std::string;
using std::swap;
using std::vector;

namespace anti {

void symmetro::debug(const char mode)
{
  fprintf(stderr, "\n");

  if (strchr("kt", mode)) {
    fprintf(stderr, "symmetry = %c%s[%d,%d]%d\n", sym,
            (sym == 'D' ? std::to_string(dihedral_n).c_str() : ""), p, q,
            sym_id_no);
    fprintf(stderr, "\n");
  }

  fprintf(stderr, "vector axis 0: %.17lf %.17lf %.17lf\n", sym_vec[0][0],
          sym_vec[0][1], sym_vec[0][2]);
  fprintf(stderr, "vector axis 1: %.17lf %.17lf %.17lf\n", sym_vec[1][0],
          sym_vec[1][1], sym_vec[1][2]);
  fprintf(stderr, "\n");

  for (int i = 0; i < 2; i++)
    fprintf(stderr, "axis %d: mult = %d  scale = %.17lf\n", i, mult[i],
            scale[i]);
  fprintf(stderr, "\n");

  for (int i = 0; i < 2; i++) {
    string buffer = "";
    if (d_substitute[i] && (d_substitute[i] != d[i]))
      buffer = "(then substituted with " + std::to_string(getN(i)) + "/" +
               std::to_string(d_substitute[i]) + "-gon)";
    if (mult[i])
      fprintf(stderr, "axis %d polygon: %d/%d-gon %s\n", i, getN(i), d[i],
              buffer.c_str());
  }
  fprintf(stderr, "\n");
}

void symmetro::setSym(const char s, const int psym, const int qsym,
                      const int dih_n, const int id_no)
{
  sym = s;
  p = psym;
  q = qsym;
  dihedral_n = dih_n;
  sym_id_no = id_no;
}

void symmetro::setMult(const int a, const int m) { mult[a] = m; }

void symmetro::setScale(const int a, const double s) { scale[a] = s; }

void symmetro::setD(const int a, const int dee) { d[a] = dee; }

void symmetro::setD_substitute(const int a, const int dee)
{
  d_substitute[a] = dee;
}

int symmetro::getOrder(const int a)
{
  switch (a) {
  case 0:
    return p;
  case 1:
    return q;
  default:
    return 0;
  }
}

int symmetro::getN(const int a) { return (getOrder(a) * mult[a]); }

double symmetro::axis_angle(const int n, const int d)
{
  double nn = double(n);
  double dd = double(d);
  return (acos(1.0 / tan(M_PI * dd / nn) / tan(M_PI * (nn - dd) / (2.0 * nn))));
}

double symmetro::getAngleBetweenAxesSin(const int axis1, const int axis2)
{
  double sin_angle_between_axes =
      vcross(sym_vec[axis1].unit(), sym_vec[axis2].unit()).len();
  if (fabs(sin_angle_between_axes) > 1.0) {
    sin_angle_between_axes = (sin_angle_between_axes < 0.0) ? -1.0 : 1.0;
  }
  return (asin(sin_angle_between_axes));
}

void symmetro::swap_vecs(Vec3d &a, Vec3d &b)
{
  a = -a;
  b = -b;
  swap(a, b);
}

int symmetro::fill_sym_vec(const char mode, string *error_msg)
{
  int err_no = 0; // 1 - wrong p,q  2 - wrong sym_id_no  3 - wrong sym

  if (sym == 'T') {
    if (p == 3 && q == 3) { // K-H mode +120 degrees
      if (sym_id_no == 1) {
        sym_vec[0] = Vec3d(1, 1, 1);
        sym_vec[1] = Vec3d(-1, -1, 1);
      }
    }
    else if ((p == 3 && q == 2) || (p == 2 && q == 3)) {
      if (sym_id_no == 1) {
        sym_vec[0] = Vec3d(1, 1, 1);
        sym_vec[1] = Vec3d(0, 0, 1);
      }
    }
    else if (p == 2 && q == 2) {
      if (sym_id_no == 1) {
        sym_vec[0] = Vec3d(0, 0, 1);
        sym_vec[1] = Vec3d(1, 0, 0);
      }
    }
    else
      err_no = 1;

    if (p < q)
      swap_vecs(sym_vec[0], sym_vec[1]);
  }
  else if (sym == 'O') {
    if (p == 4 && q == 4) {
      if (sym_id_no == 1) {
        sym_vec[0] = Vec3d(0, 0, 1);
        sym_vec[1] = Vec3d(1, 0, 0);
      }
    }
    else if ((p == 4 && q == 3) || (p == 3 && q == 4)) {
      if (sym_id_no == 1) {
        sym_vec[0] = Vec3d(0, 0, 1);
        sym_vec[1] = Vec3d(1, 1, 1);
      }
    }
    else if ((p == 4 && q == 2) || (p == 2 && q == 4)) {
      if (sym_id_no == 1) {
        sym_vec[0] = Vec3d(0, 0, 1);
        sym_vec[1] = Vec3d(0, 1, 1);
      }
      else if (sym_id_no == 2) {
        sym_vec[0] = Vec3d(0, 0, 1);
        sym_vec[1] = Vec3d(1, 1, 0);
      }
    }
    else if (p == 3 && q == 3) {
      if (sym_id_no == 1) {
        sym_vec[0] = Vec3d(1, 1, 1);
        sym_vec[1] = Vec3d(1, -1, 1);
      }
    }
    else if ((p == 3 && q == 2) || (p == 2 && q == 3)) {
      if (sym_id_no == 1) {
        sym_vec[0] = Vec3d(1, 1, 1);
        sym_vec[1] = Vec3d(0, -1, -1);
      }
      else if (sym_id_no == 2) {
        sym_vec[0] = Vec3d(1, 1, 1);
        sym_vec[1] = Vec3d(1, 0, -1);
      }
    }
    else if (p == 2 && q == 2) {
      if (sym_id_no == 1) {
        sym_vec[0] = Vec3d(0, 1, 1);
        sym_vec[1] = Vec3d(1, 0, 1);
      }
      else if (sym_id_no == 2) {
        sym_vec[0] = Vec3d(0, 1, 1);
        sym_vec[1] = Vec3d(0, 1, -1);
      }
    }
    else
      err_no = 1;

    if (p < q)
      swap_vecs(sym_vec[0], sym_vec[1]);
  }
  else if (sym == 'I') {
    if (p == 5 && q == 5) {
      if (sym_id_no == 1) {
        sym_vec[0] = Vec3d(0, 1, phi);
        sym_vec[1] = Vec3d(0, 1, -phi);
      }
    }
    else if ((p == 5 && q == 3) || (p == 3 && q == 5)) {
      if (sym_id_no == 1) {
        sym_vec[0] = Vec3d(0, 1, phi);
        sym_vec[1] = Vec3d(1, 1, 1);
      }
      else if (sym_id_no == 2) {
        sym_vec[0] = Vec3d(0, 1, phi);
        sym_vec[1] = Vec3d(phi, -1 / phi, 0);
      }
    }
    else if ((p == 5 && q == 2) || (p == 2 && q == 5)) {
      if (sym_id_no == 1) {
        sym_vec[0] = Vec3d(0, 1, phi);
        sym_vec[1] = Vec3d(0, 0, -1);
      }
      else if (sym_id_no == 2) {
        sym_vec[0] = Vec3d(0, 1, phi);
        sym_vec[1] = Vec3d(1, 1 / phi, -phi);
      }
      else if (sym_id_no == 3) {
        sym_vec[0] = Vec3d(0, 1, phi);
        sym_vec[1] = Vec3d(1, 0, 0);
      }
    }
    else if (p == 3 && q == 3) {
      if (sym_id_no == 1) {
        sym_vec[0] = Vec3d(1, 1, 1);
        sym_vec[1] = Vec3d(-1 / phi, 0, -phi);
      }
      else if (sym_id_no == 2) {
        sym_vec[0] = Vec3d(1, 1, 1);
        sym_vec[1] = Vec3d(1, -1, -1);
      }
    }
    else if ((p == 3 && q == 2) || (p == 2 && q == 3)) {
      if (sym_id_no == 1) {
        sym_vec[0] = Vec3d(1, 1, 1);
        sym_vec[1] = Vec3d(-1, -1 / phi, -phi);
      }
      else if (sym_id_no == 2) {
        sym_vec[0] = Vec3d(1, 1, 1);
        sym_vec[1] = Vec3d(-1, 0, 0);
      }
      else if (sym_id_no == 3) {
        sym_vec[0] = Vec3d(1, 1, 1);
        sym_vec[1] = Vec3d(1, -1 / phi, -phi);
      }
      else if (sym_id_no == 4) {
        sym_vec[0] = Vec3d(1, 1, 1);
        sym_vec[1] = Vec3d(1, 1 / phi, -phi);
      }
    }
    else if (p == 2 && q == 2) {
      if (sym_id_no == 1) {
        sym_vec[0] = Vec3d(0, 0, 1);
        sym_vec[1] = Vec3d(1, 1 / phi, phi);
      }
      else if (sym_id_no == 2) {
        sym_vec[0] = Vec3d(0, 0, 1);
        sym_vec[1] = Vec3d(1 / phi, phi, 1);
      }
      else if (sym_id_no == 3) {
        sym_vec[0] = Vec3d(0, 0, 1);
        sym_vec[1] = Vec3d(phi, 1, 1 / phi);
      }
      else if (sym_id_no == 4) {
        sym_vec[0] = Vec3d(0, 0, 1);
        sym_vec[1] = Vec3d(1, 0, 0);
      }
    }
    else
      err_no = 1;

    if (p < q)
      swap_vecs(sym_vec[0], sym_vec[1]);
  }
  else if (sym == 'D' && mode == 't') {
    int p_tmp = (p < q) ? q : p;

    if (p_tmp == 2 && (sym_id_no <= (dihedral_n / 2))) {
      double a = sym_id_no * M_PI / dihedral_n;
      sym_vec[0] = Vec3d(1, 0, 0);
      sym_vec[1] = Vec3d(cos(a), sin(a), 0);
    }
    else if ((p_tmp == dihedral_n) && (sym_id_no == 1)) {
      sym_vec[0] = Vec3d(0, 0, 1);
      sym_vec[1] = Vec3d(1, 0, 0);
    }
    else
      err_no = 2;

    if (p < q)
      swap_vecs(sym_vec[0], sym_vec[1]);
  }
  else
    // D symmetry of option -c included
    if (strchr("SCHVD", sym)) {
      double a = axis_angle(p, d[0]);
      sym_vec[0] = Vec3d(0, 0, 1);
      sym_vec[1] = Vec3d(sin(a), 0, cos(a));
    }
    else
      err_no = 3;

  // sym_vec will only not be set if no id_no was found
  if (!err_no)
    err_no = (sym_vec[0].is_set()) ? 0 : 2;

  if (err_no == 1) {
    if (error_msg)
      *error_msg = msg_str("invalid p,q values: %d,%d", p, q);
  }
  else if (err_no == 2) {
    if (error_msg)
      *error_msg = msg_str("invalid symmetry id no: %d", sym_id_no);
  }
  else if (err_no == 3) {
    if (error_msg)
      *error_msg = msg_str("invalid symmetry: %c", sym);
  }

  return err_no;
}

double symmetro::angle(const int n, const int d)
{
  return ((2.0 * M_PI * double(d) / double(n)));
}

double symmetro::circumradius(const int n, const int d)
{
  double edge_len = 1.0;
  return (edge_len / (2.0 * sin(angle(n, d) / 2.0)));
}

void symmetro::substitute_polygon(Geometry &geom, const int axis_no)
{
  // Make one convex regular polygon
  geom.set_hull("");

  // if star polygon, rethread face
  if (d_substitute[axis_no] > 1) {
    // there will only be one face after convex hull
    vector<int> face = geom.faces(0);
    int n = face.size();
    int d = d_substitute[axis_no];

    // handle compound polygons
    int d_test = (d <= n / 2) ? d : n - d;
    int parts = (!(n % d_test)) ? d_test : 1;

    vector<vector<int>> faces_new;

    int stop = (parts == 1) ? n : n / d_test;
    int k = 0;
    for (int i = 0; i < parts; i++) {
      vector<int> face_new;
      int k2 = k;
      for (int j = 0; j < stop; j++) {
        face_new.push_back(face[k2]);
        k2 = (k2 + d) % n;
      }
      faces_new.push_back(face_new);
      k++;
    }

    // replace with new n/d face(s)
    geom.clear(FACES);
    for (auto &i : faces_new)
      geom.add_face(i);
  }
}

// angle_between_axes in radians, is modified
vector<Geometry> symmetro::calc_polygons(
    const char mode, const double rotation, const double rotation_multiplier,
    const bool add_pi, const bool swap_axes, const double offset,
    const bool verbose, double &angle_between_axes, string *error_msg)
{
  // there will be two polygons generated in seperate geoms
  vector<Geometry> pgeom(2);

  // the two axes can be swapped if the angle is applied to the second axis
  vector<int> axis(2);
  axis[0] = 0;
  axis[1] = 1;
  if (swap_axes)
    swap(axis[0], axis[1]);

  double r0 = scale[axis[0]] * circumradius(getN(axis[0]), d[axis[0]]);
  double r1 = scale[axis[1]] * circumradius(getN(axis[1]), d[axis[1]]);

  if (!std::isnan(angle_between_axes))
    angle_between_axes = deg2rad(angle_between_axes);
  else
    angle_between_axes = (mode == 's')
                             ? axis_angle(getN(axis[0]), d[axis[0]])
                             : getAngleBetweenAxesSin(axis[0], axis[1]);
  if (verbose)
    fprintf(stderr, "\nangle between axes: radians = %.17lf degrees = %.17lf\n",
            angle_between_axes, rad2deg(angle_between_axes));

  Trans3d rot = Trans3d::rotate(Vec3d(0, 1, 0), angle_between_axes);
  Trans3d rot_inv = Trans3d::rotate(Vec3d(0, 1, 0), -angle_between_axes);

  double ang = deg2rad(rotation);
  if (rotation_multiplier)
    ang += rotation_multiplier * angle(getN(axis[0]), d[axis[0]]) / 2.0;
  if (add_pi)
    ang += M_PI;
  if (verbose)
    fprintf(stderr,
            "turn angle: radians = %.17lf degrees = %.17lf on axis %d\n", ang,
            rad2deg(ang), axis[0]);

  Vec3d V = Trans3d::rotate(Vec3d(0, 0, 1), ang) * Vec3d(r0, 0, 0);
  Vec3d q = rot * V;
  Vec3d u = rot * Vec3d(0, 0, 1);

  double a = u[0] * u[0] + u[1] * u[1];
  double b = 2 * (q[0] * u[0] + q[1] * u[1]);
  double c = q[0] * q[0] + q[1] * q[1] - r1 * r1;

  double disc = b * b - 4 * a * c;
  if (disc < -epsilon) {
    if (error_msg)
      *error_msg = "model is not geometrically constructible";

    return pgeom;
  }
  else if (disc < 0)
    disc = 0;

  double sign_flag = -1.0;
  // modes s and c
  if (mode == 's') {
    // AR - The sign flag, which changes for the range 90 to 270 degrees, allows
    // the model to reverse, otherwise the model breaks apart in this range.
    double turn_angle_test_val = fabs(fmod(fabs(ang), 2.0 * M_PI) - M_PI);
    sign_flag = (turn_angle_test_val > M_PI / 2.0) ? -1.0 : 1.0;
  }
  double t = (-b + sign_flag * sqrt(disc)) / (2 * a);

  Vec3d P = V + Vec3d(0, 0, t);
  Vec3d Q = rot * P;

  if (vdot(sym_vec[axis[0]], sym_vec[axis[1]]) > 0.0) {
    sym_vec[axis[1]] *= -1.0;
  }

  for (unsigned int i = 0; i < pgeom.size(); i++) {
    int j = axis[i];
    int n = getN(j);

    // handle compound polygons
    int d_test = (d[j] <= n / 2) ? d[j] : n - d[j];
    int parts = (!(n % d_test)) ? d_test : 1;
    double bump_ang = angle(n, d[j]) / (double)parts;

    if ((n > 0) && scale[j]) {
      double bump_angle = 0.0;
      int vert_idx = 0;

      for (int k = 0; k < parts; k++) {
        for (int idx = 0; idx < n; idx++) {
          if (i == 0) {
            pgeom[j].add_vert(
                Trans3d::rotate(Vec3d(0, 0, 1),
                                (idx * angle(n, d[j])) + bump_angle) *
                    P +
                Vec3d(0, 0, offset));
          }
          else if (i == 1) {
            pgeom[j].add_vert(
                rot_inv *
                Trans3d::rotate(Vec3d(0, 0, 1),
                                (idx * angle(n, d[j])) + bump_angle) *
                Q);
          }
        }

        vector<int> face;
        for (int idx = 0; idx < n; ++idx)
          face.push_back(vert_idx++);
        pgeom[j].add_face(face);
        face.clear();

        bump_angle += bump_ang;
      }

      if (d_substitute[j])
        substitute_polygon(pgeom[j], j);

      pgeom[j].transform(Trans3d::align(Vec3d(0, 0, 1), Vec3d(1, 0, 0),
                                        sym_vec[axis[0]], sym_vec[axis[1]]));
    }

    if (!scale[j])
      pgeom[j].clear_all();
  }

  return pgeom;
}

} // namespace anti
