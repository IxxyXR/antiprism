#if defined(_WIN32)
#include <io.h>
#include <fcntl.h>
#if defined(_MSC_VER)
#include <BaseTsd.h>
typedef SSIZE_T ssize_t;
#endif
#define pipe(fds) _pipe(fds, 4096, _O_BINARY)
#define dup _dup
#define dup2 _dup2
#define close _close
#define read _read
#define write _write
#ifndef STDIN_FILENO
#define STDIN_FILENO 0
#endif
#ifndef STDOUT_FILENO
#define STDOUT_FILENO 1
#endif
#else
#include <unistd.h>
#include <fcntl.h>
#endif
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

static std::string run_program(int (*prog)(int, char **),
                               const std::string &prog_name,
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

  std::vector<std::string> argv_store;
  argv_store.push_back(prog_name);
  argv_store.insert(argv_store.end(), args.begin(), args.end());

  std::vector<char *> argv;
  for (auto &s : argv_store)
    argv.push_back(const_cast<char *>(s.c_str()));
  argv.push_back(nullptr);

  if (!input.empty())
    write(in_pipe[1], input.data(), input.size());
  close(in_pipe[1]);

  prog(static_cast<int>(argv_store.size()), argv.data());

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

extern "C" const char *antiprism_command(const char *command_name,
                                         const char *input_data,
                                         const char *options)
{
  static std::string out;
  std::string prog = command_name ? command_name : "";
  if (prog.empty()) {
    out.clear();
    return out.c_str();
  }

  std::vector<std::string> args;
  if (options && *options) {
    std::istringstream iss(options);
    std::string tok;
    while (iss >> tok)
      args.push_back(tok);
  }

  int (*fn)(int, char **) = nullptr;
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

  std::string input = input_data ? input_data : "";
  out = run_program(fn, prog, args, input);
  return out.c_str();
}
