#include <unistd.h>
#include <fcntl.h>
#include <sstream>
#include <vector>
#include <string>
#include <cstring>

extern "C" {
int off_align_main(int, char**);
int off_color_main(int, char**);
int off_color_radial_main(int, char**);
int off_normals_main(int, char**);
int off_query_main(int, char**);
int off_report_main(int, char**);
int off_trans_main(int, char**);
int off_util_main(int, char**);
int dome_layer_main(int, char**);
int jitterbug_main(int, char**);
int lat_grid_main(int, char**);
int mmop_origami_main(int, char**);
int poly_weave_main(int, char**);
int rotegrity_main(int, char**);
int spidron_main(int, char**);
int string_art_main(int, char**);
int sweep_edges_main(int, char**);
}

static std::string run_program(int (*prog)(int,char**),
                               const std::vector<std::string> &args,
                               const std::string &input)
{
  int out_pipe[2];
  int in_pipe[2];
  pipe(out_pipe);
  pipe(in_pipe);

  int old_stdout = dup(STDOUT_FILENO);
  int old_stdin = dup(STDIN_FILENO);
  dup2(out_pipe[1], STDOUT_FILENO);
  dup2(in_pipe[0], STDIN_FILENO);
  close(out_pipe[1]);
  close(in_pipe[0]);

  std::vector<char*> argv;
  for (const auto &s : args)
    argv.push_back(const_cast<char*>(s.c_str()));
  argv.push_back(nullptr);

  if (!input.empty())
    write(in_pipe[1], input.data(), input.size());
  close(in_pipe[1]);

  prog(static_cast<int>(args.size()), argv.data());

  fflush(stdout);
  dup2(old_stdout, STDOUT_FILENO);
  dup2(old_stdin, STDIN_FILENO);
  close(old_stdout);
  close(old_stdin);

  std::ostringstream oss;
  char buf[4096];
  ssize_t len;
  while ((len = read(out_pipe[0], buf, sizeof(buf))) > 0)
    oss.write(buf, len);
  close(out_pipe[0]);
  return oss.str();
}

extern "C" const char *antiprism_command(const char *cmd_in)
{
  static std::string out;
  std::string cmd = cmd_in ? cmd_in : "";
  size_t nl = cmd.find('\n');
  std::string line = cmd.substr(0, nl);
  std::string input = (nl == std::string::npos) ? std::string() : cmd.substr(nl + 1);

  std::istringstream iss(line);
  std::vector<std::string> args;
  std::string tok;
  while (iss >> tok)
    args.push_back(tok);
  if (args.empty()) {
    out.clear();
    return out.c_str();
  }
  std::string prog = args[0];
  args.erase(args.begin());

  int (*fn)(int,char**) = nullptr;
  if (prog == "off_align") fn = off_align_main;
  else if (prog == "off_color") fn = off_color_main;
  else if (prog == "off_color_radial") fn = off_color_radial_main;
  else if (prog == "off_normals") fn = off_normals_main;
  else if (prog == "off_query") fn = off_query_main;
  else if (prog == "off_report") fn = off_report_main;
  else if (prog == "off_trans") fn = off_trans_main;
  else if (prog == "off_util") fn = off_util_main;
  else if (prog == "dome_layer") fn = dome_layer_main;
  else if (prog == "jitterbug") fn = jitterbug_main;
  else if (prog == "lat_grid") fn = lat_grid_main;
  else if (prog == "mmop_origami") fn = mmop_origami_main;
  else if (prog == "poly_weave") fn = poly_weave_main;
  else if (prog == "rotegrity") fn = rotegrity_main;
  else if (prog == "spidron") fn = spidron_main;
  else if (prog == "string_art") fn = string_art_main;
  else if (prog == "sweep_edges") fn = sweep_edges_main;
  if (!fn) {
    out = "Unknown command";
    return out.c_str();
  }

  out = run_program(fn, args, input);
  return out.c_str();
}
