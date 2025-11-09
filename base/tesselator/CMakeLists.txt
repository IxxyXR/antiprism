# CMakeLists.txt for tesselator library

set(TESS_SOURCES
    dict.cc
    geom.cc
    memalloc.cc
    mesh.cc
    normal.cc
    priorityq.cc
    render.cc
    sweep.cc
    tess.cc
    tessmono.cc
)

add_library(tesselator STATIC ${TESS_SOURCES})

target_include_directories(tesselator
    PUBLIC ${CMAKE_CURRENT_SOURCE_DIR}
)

# Disable warnings for third-party code
if(MSVC)
    target_compile_options(tesselator PRIVATE /W0)
else()
    target_compile_options(tesselator PRIVATE -w)
endif()
