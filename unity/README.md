# Antiprism Unity Plugin

This directory provides a Unity native plugin that links against the Antiprism
library. Individual command-line tools are compiled into the static library
`libantiprism_unity` and exposed through an `antiprism_command` C interface.

The plugin executes Antiprism tools directly without spawning external
processes or copying command-line executables. Build the library using CMake
and then link `unity_plugin.cc` into a shared library suitable for Unity.
