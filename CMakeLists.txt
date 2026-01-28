# CMakeLists.txt for Antiprism Library
# Alternative build system to autotools for easier cross-platform builds
# Particularly useful for Unity plugin builds (Android, iOS, etc.)

cmake_minimum_required(VERSION 3.10)
project(antiprism VERSION 0.32.99 LANGUAGES CXX C)

# C++ Standard
set(CMAKE_CXX_STANDARD 14)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

# Options
option(BUILD_SHARED_LIBS "Build shared libraries" ON)
option(BUILD_EXECUTABLES "Build command-line executables" OFF)
option(BUILD_UNITY_PLUGIN "Build for Unity plugin (includes C API)" ON)

# Platform-specific settings
if(ANDROID)
    set(BUILD_SHARED_LIBS OFF)  # Use static libs for Android
    set(BUILD_EXECUTABLES OFF)
    set(BUILD_UNITY_PLUGIN ON)
endif()

if(IOS)
    set(BUILD_SHARED_LIBS OFF)
    set(BUILD_EXECUTABLES OFF)
    set(BUILD_UNITY_PLUGIN ON)
endif()

# Source files for the core library
set(ANTIPRISM_SOURCES
    base/off_read.cc
    base/off_write.cc
    base/crds_read.cc
    base/displaypoly.cc
    base/geometry.cc
    base/geometryutils.cc
    base/colormap.cc
    base/color.cc
    base/dual.cc
    base/programopts.cc
    base/status.cc
    base/vec3d.cc
    base/trans3d.cc
    base/vec4d.cc
    base/trans4d.cc
    base/vec_utils.cc
    base/vec_utils_norm.cc
    base/vec_utils_cent.cc
    base/utils.cc
    base/utils_parser.cc
    base/getopt.cc
    base/mathutils.cc
    base/normal.cc
    base/c_hull.cc
    base/triangulate.cc
    base/iteration.cc
    base/symmetry.cc
    base/sort_merge.cc
    base/boundbox.cc
    base/geometryinfo.cc
    base/coloring.cc
    base/prop_col.cc
    base/named_cols.cc
    base/geodesic.cc
    base/zonohedron.cc
    base/johnson.cc
    base/uniform.cc
    base/std_polys.cc
    base/skilling.cc
    base/stellations.cc
    base/timer.cc
    base/polygon.cc
    base/povwriter.cc
    base/scene.cc
    base/canonical.cc
    base/trans.cc
    base/faces.cc
    base/vrmlwriter.cc
    base/wythoff.cc
    base/wythoff_tiling.cc
    base/wythoff_ops.cc
    base/planar.cc
)

# Add C API for Unity plugin
if(BUILD_UNITY_PLUGIN)
    list(APPEND ANTIPRISM_SOURCES
        base/antiprism_c_api.cc
        base/symmetro_wrapper.cc
    )
endif()

# Public headers
set(ANTIPRISM_HEADERS
    base/antiprism.h
    base/boundbox.h
    base/elemprops.h
    base/colormap.h
    base/coloring.h
    base/color.h
    base/const.h
    base/displaypoly.h
    base/geometry.h
    base/geometryutils.h
    base/geometryinfo.h
    base/iteration.h
    base/trans3d.h
    base/trans4d.h
    base/mathutils.h
    base/normal.h
    base/polygon.h
    base/povwriter.h
    base/programopts.h
    base/random.h
    base/scene.h
    base/status.h
    base/symmetry.h
    base/tiling.h
    base/timer.h
    base/utils.h
    base/getopt.h
    base/vec3d.h
    base/vec4d.h
    base/vec_utils.h
    base/vrmlwriter.h
    base/planar.h
)

if(BUILD_UNITY_PLUGIN)
    list(APPEND ANTIPRISM_HEADERS
        base/antiprism_c_api.h
        base/symmetro_wrapper.h
    )
endif()

# Subdirectories for embedded libraries
add_subdirectory(base/qhull)
add_subdirectory(base/tesselator)
add_subdirectory(base/muparser)

# Main library target
add_library(antiprism ${ANTIPRISM_SOURCES})

# Include directories
target_include_directories(antiprism
    PUBLIC
        $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}/base>
        $<INSTALL_INTERFACE:include>
    PRIVATE
        ${CMAKE_CURRENT_SOURCE_DIR}/base
)

# Link embedded libraries
target_link_libraries(antiprism
    PRIVATE
        qhull
        tesselator
        muparser
)

# Compiler definitions
target_compile_definitions(antiprism PRIVATE
    SUPDIR="${CMAKE_INSTALL_PREFIX}/share/antiprism"
    VERSION="${PROJECT_VERSION}"
)

if(BUILD_UNITY_PLUGIN)
    target_compile_definitions(antiprism PUBLIC ANTIPRISM_BUILD_DLL)
endif()

# MSVC-specific definitions
if(MSVC)
    target_compile_definitions(antiprism PUBLIC _USE_MATH_DEFINES)
    target_compile_definitions(antiprism PRIVATE _CRT_SECURE_NO_WARNINGS)
endif()

# Platform-specific timer definitions
if(WIN32)
    # Windows uses timeGetTime and Sleep
    target_compile_definitions(antiprism PRIVATE UTIMER=2 USLEEP=2)
    target_link_libraries(antiprism PRIVATE winmm)  # For timeGetTime
else()
    # Unix uses gettimeofday and usleep
    target_compile_definitions(antiprism PRIVATE UTIMER=1 USLEEP=1)
endif()

# Platform-specific libraries
if(UNIX AND NOT APPLE AND NOT ANDROID)
    target_link_libraries(antiprism PRIVATE m)  # Math library
endif()

# Set library output name
set_target_properties(antiprism PROPERTIES
    OUTPUT_NAME "antiprism"
    VERSION ${PROJECT_VERSION}
    SOVERSION 0
)

# Install targets
if(NOT ANDROID AND NOT IOS)
    include(GNUInstallDirs)

    install(TARGETS antiprism
        EXPORT antiprism-targets
        LIBRARY DESTINATION ${CMAKE_INSTALL_LIBDIR}
        ARCHIVE DESTINATION ${CMAKE_INSTALL_LIBDIR}
        RUNTIME DESTINATION ${CMAKE_INSTALL_BINDIR}
    )

    install(FILES ${ANTIPRISM_HEADERS}
        DESTINATION ${CMAKE_INSTALL_INCLUDEDIR}/antiprism
    )

    install(EXPORT antiprism-targets
        FILE antiprism-config.cmake
        NAMESPACE antiprism::
        DESTINATION ${CMAKE_INSTALL_LIBDIR}/cmake/antiprism
    )
endif()

# Build executables if requested
if(BUILD_EXECUTABLES)
    # Add executable targets here
    # Example:
    # add_executable(off_util src/off_util.cc)
    # target_link_libraries(off_util antiprism)
endif()

# Print configuration summary
message(STATUS "")
message(STATUS "Antiprism Configuration Summary:")
message(STATUS "  Version:              ${PROJECT_VERSION}")
message(STATUS "  Build type:           ${CMAKE_BUILD_TYPE}")
message(STATUS "  Shared libraries:     ${BUILD_SHARED_LIBS}")
message(STATUS "  Build executables:    ${BUILD_EXECUTABLES}")
message(STATUS "  Unity plugin mode:    ${BUILD_UNITY_PLUGIN}")
message(STATUS "  Install prefix:       ${CMAKE_INSTALL_PREFIX}")
message(STATUS "")
