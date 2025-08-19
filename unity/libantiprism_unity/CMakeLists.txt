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
    ${SRC_DIR}/rep_print.cc
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

set_source_files_properties(${SRC_DIR}/off_align.cc         PROPERTIES COMPILE_DEFINITIONS "main=off_align_main")
set_source_files_properties(${SRC_DIR}/off_color.cc         PROPERTIES COMPILE_DEFINITIONS "main=off_color_main")
set_source_files_properties(${SRC_DIR}/off_color_radial.cc  PROPERTIES COMPILE_DEFINITIONS "main=off_color_radial_main")
set_source_files_properties(${SRC_DIR}/off_normals.cc       PROPERTIES COMPILE_DEFINITIONS "main=off_normals_main")
set_source_files_properties(${SRC_DIR}/off_query.cc         PROPERTIES COMPILE_DEFINITIONS "main=off_query_main")
set_source_files_properties(${SRC_DIR}/off_report.cc        PROPERTIES COMPILE_DEFINITIONS "main=off_report_main")
set_source_files_properties(${SRC_DIR}/off_trans.cc         PROPERTIES COMPILE_DEFINITIONS "main=off_trans_main")
set_source_files_properties(${SRC_DIR}/off_util.cc          PROPERTIES COMPILE_DEFINITIONS "main=off_util_main")
set_source_files_properties(${SRC_EXTRA_DIR}/dome_layer.cc  PROPERTIES COMPILE_DEFINITIONS "main=dome_layer_main")
set_source_files_properties(${SRC_EXTRA_DIR}/jitterbug.cc   PROPERTIES COMPILE_DEFINITIONS "main=jitterbug_main")
set_source_files_properties(${SRC_EXTRA_DIR}/lat_grid.cc    PROPERTIES COMPILE_DEFINITIONS "main=lat_grid_main")
set_source_files_properties(${SRC_EXTRA_DIR}/mmop_origami.cc PROPERTIES COMPILE_DEFINITIONS "main=mmop_origami_main")
set_source_files_properties(${SRC_EXTRA_DIR}/poly_weave.cc  PROPERTIES COMPILE_DEFINITIONS "main=poly_weave_main")
set_source_files_properties(${SRC_EXTRA_DIR}/rotegrity.cc   PROPERTIES COMPILE_DEFINITIONS "main=rotegrity_main")
set_source_files_properties(${SRC_EXTRA_DIR}/spidron.cc     PROPERTIES COMPILE_DEFINITIONS "main=spidron_main")
set_source_files_properties(${SRC_EXTRA_DIR}/string_art.cc  PROPERTIES COMPILE_DEFINITIONS "main=string_art_main")
set_source_files_properties(${SRC_EXTRA_DIR}/sweep_edges.cc PROPERTIES COMPILE_DEFINITIONS "main=sweep_edges_main")

target_include_directories(antiprism_unity PRIVATE
    ${CMAKE_CURRENT_SOURCE_DIR}/../../base
    ${SRC_DIR}
    ${SRC_EXTRA_DIR}
)

target_link_libraries(antiprism_unity antiprism)
