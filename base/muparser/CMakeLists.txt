# CMakeLists.txt for muparser library

set(MUPARSER_SOURCES
    muParser.cpp
    muParserBase.cpp
    muParserBytecode.cpp
    muParserCallback.cpp
    muParserError.cpp
    muParserInt.cpp
    muParserTokenReader.cpp
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
