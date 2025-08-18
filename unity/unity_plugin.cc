#include <cstdio>
#include <cstdlib>
#include <cstring>
#include <string>

#if defined(_WIN32)
#define popen _popen
#define pclose _pclose
#endif

static int run_cmd(const char *exe, const char *args, char **out_text)
{
  std::string cmd = exe;
  if (args && *args) {
    cmd += " ";
    cmd += args;
  }
  FILE *pipe = popen(cmd.c_str(), "r");
  if (!pipe)
    return -1;
  std::string output;
  char buffer[4096];
  while (fgets(buffer, sizeof(buffer), pipe))
    output += buffer;
  int ret = pclose(pipe);
  *out_text = (char *)malloc(output.size() + 1);
  if (!*out_text)
    return -1;
  memcpy(*out_text, output.c_str(), output.size() + 1);
  return ret;
}

extern "C" void antiprism_free(char *p) { free(p); }

#define AP_WRAP(cmd) \
  extern "C" int cmd##_string(const char *args, char **out_text) { \
    return run_cmd(#cmd, args, out_text); \
  }

AP_WRAP(off2pov)
AP_WRAP(off2vrml)
AP_WRAP(off2crds)
AP_WRAP(off2obj)
AP_WRAP(obj2off)
AP_WRAP(off2dae)
AP_WRAP(off_color)
AP_WRAP(off_util)
AP_WRAP(off_trans)
AP_WRAP(off_align)
AP_WRAP(poly_kscope)
AP_WRAP(polygon)
AP_WRAP(zono)
AP_WRAP(conv_hull)
AP_WRAP(pol_recip)
AP_WRAP(geodesic)
AP_WRAP(poly_form)
AP_WRAP(sph_rings)
AP_WRAP(off_report)
AP_WRAP(off_query)
AP_WRAP(kcycle)
AP_WRAP(unitile2d)
AP_WRAP(repel)
AP_WRAP(lat_util)
AP_WRAP(canonical)
AP_WRAP(conway)
AP_WRAP(n_icons)
AP_WRAP(iso_delta)
AP_WRAP(bravais)
AP_WRAP(waterman)
AP_WRAP(col_util)
AP_WRAP(planar)
AP_WRAP(off_normals)
AP_WRAP(leonardo)
AP_WRAP(iso_kite)
AP_WRAP(to_nfold)
AP_WRAP(symmetro)
AP_WRAP(stellate)
AP_WRAP(miller)
AP_WRAP(wythoff)
AP_WRAP(off_color_radial)
AP_WRAP(tetra59)
AP_WRAP(spidron)
AP_WRAP(dome_layer)
AP_WRAP(poly_weave)
AP_WRAP(mmop_origami)
AP_WRAP(jitterbug)
AP_WRAP(string_art)
AP_WRAP(rotegrity)
AP_WRAP(sweep_edges)
AP_WRAP(lat_grid)

#undef AP_WRAP
