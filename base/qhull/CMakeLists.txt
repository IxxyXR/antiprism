# CMakeLists.txt for qhull library

set(QHULL_SOURCES
    geom2_r.cc
    geom_r.cc
    global_r.cc
    io_r.cc
    libqhull_r.cc
    mem_r.cc
    merge_r.cc
    poly2_r.cc
    poly_r.cc
    qset_r.cc
    random_r.cc
    rboxlib_r.cc
    stat_r.cc
    usermem_r.cc
    userprintf_rbox_r.cc
    userprintf_r.cc
    user_r.cc
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
