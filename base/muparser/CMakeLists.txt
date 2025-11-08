# CMakeLists.txt for muparser library

set(MUPARSER_SOURCES
    muParser.cc
    muParserBase.cc
    muParserBytecode.cc
    muParserCallback.cc
    muParserDLL.cc
    muParserError.cc
    muParserInt.cc
    muParserTest.cc
    muParserTokenReader.cc
)

add_library(muparser STATIC ${MUPARSER_SOURCES})

target_include_directories(muparser
    PUBLIC ${CMAKE_CURRENT_SOURCE_DIR}
)

# Disable warnings for third-party code
if(MSVC)
    target_compile_options(muparser PRIVATE /W0)
else()
    target_compile_options(muparser PRIVATE -w)
endif()
