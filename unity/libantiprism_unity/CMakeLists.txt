cmake_minimum_required(VERSION 3.10)
project(antiprism_unity)

set(SRC_DIR ${CMAKE_CURRENT_SOURCE_DIR}/../../src)
set(SRC_EXTRA_DIR ${CMAKE_CURRENT_SOURCE_DIR}/../../src_extra)

add_library(antiprism_unity STATIC
    ${SRC_DIR}/off_align.cc
    ${SRC_DIR}/off_color.cc
    ${SRC_DIR}/off_color_radial.cc
    ${SRC_DIR}/off_normals.cc
    ${SRC_DIR}/off_query.cc
    ${SRC_DIR}/off_report.cc
    ${SRC_DIR}/off_trans.cc
    ${SRC_DIR}/off_util.cc
    ${SRC_EXTRA_DIR}/dome_layer.cc
    ${SRC_EXTRA_DIR}/jitterbug.cc
    ${SRC_EXTRA_DIR}/lat_grid.cc
    ${SRC_EXTRA_DIR}/mmop_origami.cc
    ${SRC_EXTRA_DIR}/poly_weave.cc
    ${SRC_EXTRA_DIR}/rotegrity.cc
    ${SRC_EXTRA_DIR}/spidron.cc
    ${SRC_EXTRA_DIR}/string_art.cc
    ${SRC_EXTRA_DIR}/sweep_edges.cc
)

target_include_directories(antiprism_unity PRIVATE
    ${CMAKE_CURRENT_SOURCE_DIR}/../../base
    ${SRC_DIR}
    ${SRC_EXTRA_DIR}
)

target_link_libraries(antiprism_unity antiprism)
