cmake_minimum_required (VERSION 3.0)

set(CMAKE_CXX_STANDARD 11)
set(CMAKE_INSTALL_PREFIX $ENV{__CMakeBinDir})
set(RESOURCES)

# Include 'bin/obj' dir since it contains _version.h
include_directories("${CMAKE_BINARY_DIR}/../../")

if(WIN32)
    add_definitions(-DWIN32)
    add_definitions(-D_WIN32=1)
    add_definitions(-DUNICODE -D_UNICODE)
    if(IS_64BIT_BUILD)
        add_definitions(-D_WIN64=1)
    endif()
    add_compile_options($<$<CONFIG:Debug>:-DDEBUG>)
    add_compile_options($<$<CONFIG:Release>:-DNDEBUG>)
    add_compile_options($<$<CONFIG:Debug>:/Od>)
    add_compile_options($<$<CONFIG:Debug>:/MDd>) # /MT will static link the VC runtime library, so it doesn't need to be installed on the target machine
    add_compile_options($<$<CONFIG:Release>:/MT>)
    add_compile_options(/guard:cf)
    add_compile_options(/d2Zi+) # make optimized builds debugging easier
    add_compile_options(/nologo) # Suppress Startup Banner

    add_compile_options(/W3) # set warning level to 3
    add_compile_options(/WX) # treat warnings as errors
    add_compile_options(/wd4267) # warnings from pytorch
    add_compile_options(/wd4244) # warnings from pytorch
    add_compile_options(/wd4996) # warnings from pytorch
    add_compile_options(/wd4251) # warnings from pytorch
    add_compile_options(/wd4275) # warnings from pytorch
    add_compile_options(/wd4190) # warnings from pytorch
    add_compile_options(/wd4305) # warnings from pytorch
    add_compile_options(/wd4522) # warnings from pytorch
    add_compile_options(/wd4018) # warnings from pytorch
    add_compile_options(/wd4273) # warnings from pytorch
    add_compile_options(/wd4067) # warnings from pytorch
    add_compile_options(/wd4624) # warnings from pytorch
    add_compile_options(/wd4805) # warnings from pytorch

    add_compile_options(/Oi) # enable intrinsics
    add_compile_options(/Oy-) # disable suppressing of the creation of frame pointers on the call stack for quicker function calls
    add_compile_options(/GF) # enable read-only string pooling
    add_compile_options(/FC) # use full pathnames in diagnostics
    add_compile_options(/DEBUG)
    add_compile_options(/GS)
    add_compile_options(/Zc:inline)
    add_compile_options(/fp:precise)
    add_compile_options(/EHsc)
    add_compile_options(/MP)

    if ("$ENV{__BuildArch}" STREQUAL "x86")
        add_compile_options(/Gz)
    endif ()

    set(CMAKE_SHARED_LINKER_FLAGS "${CMAKE_SHARED_LINKER_FLAGS} /LARGEADDRESSAWARE") # can handle addresses larger than 2 gigabytes
    set(CMAKE_SHARED_LINKER_FLAGS "${CMAKE_SHARED_LINKER_FLAGS} /DEBUG")
    set(CMAKE_SHARED_LINKER_FLAGS "${CMAKE_SHARED_LINKER_FLAGS} /INCREMENTAL:NO")
    set(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} /DEBUG /PDBCOMPRESS")
    set(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} /STACK:1572864")

    set(CMAKE_SHARED_LINKER_FLAGS "${CMAKE_SHARED_LINKER_FLAGS} /guard:cf")
    set(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} /guard:cf")

    # Debug build specific flags
    set(CMAKE_SHARED_LINKER_FLAGS_DEBUG "/NOVCFEATURE")

    # Release build specific flags
    set(CMAKE_SHARED_LINKER_FLAGS_RELEASE "${CMAKE_SHARED_LINKER_FLAGS_RELEASE} /DEBUG /OPT:REF /OPT:ICF")
    set(CMAKE_STATIC_LINKER_FLAGS_RELEASE "${CMAKE_STATIC_LINKER_FLAGS_RELEASE} /DEBUG /OPT:REF /OPT:ICF")
    set(CMAKE_EXE_LINKER_FLAGS_RELEASE "${CMAKE_EXE_LINKER_FLAGS_RELEASE} /DEBUG /OPT:REF /OPT:ICF")
    set(CMAKE_SHARED_LINKER_FLAGS_RELEASE "${CMAKE_SHARED_LINKER_FLAGS_RELEASE} /NODEFAULTLIB:libucrt.lib /DEFAULTLIB:ucrt.lib")
    set(CMAKE_EXE_LINKER_FLAGS_RELEASE "${CMAKE_EXE_LINKER_FLAGS_RELEASE} /NODEFAULTLIB:libucrt.lib /DEFAULTLIB:ucrt.lib")

else()
    add_definitions(-Werror) # treat warnings as errors
    add_definitions(-D_GLIBCXX_USE_CXX11_ABI=0)
    add_compile_options(-Wno-unused-local-typedef)
    add_compile_options(-fPIC)
    add_compile_options(-fvisibility=hidden)
endif()

# This is required to map a symbol reference to a matching definition local to the module (.so)
# containing the reference instead of using definitions from other modules.
if(${CMAKE_SYSTEM_NAME} MATCHES "Linux")
    set(CMAKE_SHARED_LINKER_FLAGS "${CMAKE_SHARED_LINKER_FLAGS} -Xlinker -Bsymbolic -Bsymbolic-functions")
    set(CMAKE_SHARED_LINKER_FLAGS "${CMAKE_SHARED_LINKER_FLAGS} -Wl,--build-id=sha1")
    set(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} -Wl,--build-id=sha1")
    add_compile_options(-fstack-protector-strong)
elseif(${CMAKE_SYSTEM_NAME} MATCHES "Darwin")
    add_compile_options(-fstack-protector)
endif()

# Experimenting with trying to solve a linker problem
if(${CMAKE_SYSTEM_NAME} MATCHES "Darwin")
    add_compile_options(-v)
#    set(CMAKE_SHARED_LINKER_FLAGS "${CMAKE_SHARED_LINKER_FLAGS} --for-linker=-v")
#    set(CMAKE_LINK_WHAT_YOU_USE FALSE)
endif()

add_definitions(-D_NO_ASYNCRTIMP)
add_definitions(-D_NO_PPLXIMP)
if(${CMAKE_SYSTEM_NAME} MATCHES "Linux")
    add_definitions(-D__LINUX__)
endif()


if(CMAKE_SYSTEM_NAME STREQUAL Linux)
    set(CLR_CMAKE_PLATFORM_UNIX 1)
endif(CMAKE_SYSTEM_NAME STREQUAL Linux)

if(CMAKE_SYSTEM_NAME STREQUAL Darwin)
    set(CLR_CMAKE_PLATFORM_UNIX 1)
endif(CMAKE_SYSTEM_NAME STREQUAL Darwin)

if(CMAKE_SYSTEM_NAME STREQUAL FreeBSD)
    set(CLR_CMAKE_PLATFORM_UNIX 1)
    add_definitions(-D_BSD_SOURCE) # required for getline
endif(CMAKE_SYSTEM_NAME STREQUAL FreeBSD)

if(CMAKE_SYSTEM_NAME STREQUAL OpenBSD)
    set(CLR_CMAKE_PLATFORM_UNIX 1)
endif(CMAKE_SYSTEM_NAME STREQUAL OpenBSD)

if(CMAKE_SYSTEM_NAME STREQUAL NetBSD)
    set(CLR_CMAKE_PLATFORM_UNIX 1)
endif(CMAKE_SYSTEM_NAME STREQUAL NetBSD)

if(CMAKE_SYSTEM_NAME STREQUAL SunOS)
    set(CLR_CMAKE_PLATFORM_UNIX 1)
endif(CMAKE_SYSTEM_NAME STREQUAL SunOS)

if(CLR_CMAKE_PLATFORM_UNIX)
    if(STRIP_SYMBOLS)
        find_program(STRIP strip)
        if(CMAKE_SYSTEM_NAME STREQUAL Darwin)
            find_program(DSYMUTIL dsymutil)
        else(CMAKE_SYSTEM_NAME STREQUAL Darwin)
            find_program(OBJCOPY objcopy)
        endif(CMAKE_SYSTEM_NAME STREQUAL Darwin)
    endif(STRIP_SYMBOLS)
endif(CLR_CMAKE_PLATFORM_UNIX)

function(strip_symbols targetName outputFilename)
    if(CLR_CMAKE_PLATFORM_UNIX)
        if(STRIP_SYMBOLS)
            # On the older version of cmake (2.8.12) used on Ubuntu 14.04 the TARGET_FILE
            # generator expression doesn't work correctly returning the wrong path and on
            # the newer cmake versions the LOCATION property isn't supported anymore.
            if(CMAKE_VERSION VERSION_EQUAL 3.0 OR CMAKE_VERSION VERSION_GREATER 3.0)
                set(strip_source_file $<TARGET_FILE:${targetName}>)
            else()
                get_property(strip_source_file TARGET ${targetName} PROPERTY LOCATION)
            endif()

            if(CMAKE_SYSTEM_NAME STREQUAL Darwin)
                set(strip_destination_file ${strip_source_file}.dwarf)

                add_custom_command(
                    TARGET ${targetName}
                    POST_BUILD
                    VERBATIM
                    COMMAND ${DSYMUTIL} --flat --minimize ${strip_source_file}
                    COMMAND ${STRIP} -u -r ${strip_source_file}
                    COMMENT Stripping symbols from ${strip_source_file} into file ${strip_destination_file}
                )
            else(CMAKE_SYSTEM_NAME STREQUAL Darwin)
                set(strip_destination_file ${strip_source_file}.dbg)

                add_custom_command(
                    TARGET ${targetName}
                    POST_BUILD
                    VERBATIM
                    COMMAND ${OBJCOPY} --only-keep-debug ${strip_source_file} ${strip_destination_file}
                    COMMAND ${OBJCOPY} --strip-unneeded ${strip_source_file}
                    COMMAND ${OBJCOPY} --add-gnu-debuglink=${strip_destination_file} ${strip_source_file}
                    COMMENT Stripping symbols from ${strip_source_file} into file ${strip_destination_file}
                )
            endif(CMAKE_SYSTEM_NAME STREQUAL Darwin)

            set(${outputFilename} ${strip_destination_file} PARENT_SCOPE)
        endif(STRIP_SYMBOLS)
    endif(CLR_CMAKE_PLATFORM_UNIX)
endfunction()

function(install_library_and_symbols targetName)
    strip_symbols(${targetName} strip_destination_file)

    install (TARGETS ${targetName} DESTINATION .)

    if(WIN32)
        install(FILES ${CMAKE_CURRENT_BINARY_DIR}/${CMAKE_BUILD_TYPE}/${targetName}.pdb DESTINATION .)
    else()
        install(FILES ${strip_destination_file} DESTINATION .)
    endif()
endfunction()

add_subdirectory(LibTorchSharp)
