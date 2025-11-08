# CMakeLists.txt for qhull library

set(QHULL_SOURCES
    geom2.c
    geom.c
    global.c
    io.c
    libqhull.c
    mem.c
    merge.c
    poly2.c
    poly.c
    qset.c
    stat.c
    user.c
)

add_library(qhull STATIC ${QHULL_SOURCES})

target_include_directories(qhull
    PUBLIC ${CMAKE_CURRENT_SOURCE_DIR}
)

# Disable warnings for third-party code
if(MSVC)
    target_compile_options(qhull PRIVATE /W0)
else()
    target_compile_options(qhull PRIVATE -w)
endif()
