#include <cstdio>
#include <cstdlib>
#include <cstring>
#include <string>

#if defined(_WIN32)
#define popen _popen
#define pclose _pclose
#endif

static int run_cmd(const char *cmd, char **out_text)
{
  FILE *pipe = popen(cmd, "r");
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

extern "C" int antiprism_command(const char *cmd, char **out_text)
{
  return run_cmd(cmd, out_text);
}

extern "C" void antiprism_free(char *p) { free(p); }
