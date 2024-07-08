# Install script for directory: C:/Users/rafal/Desktop/PSU-Obfuscator/obf-dotnet/Lua/lua

# Set the install prefix
if(NOT DEFINED CMAKE_INSTALL_PREFIX)
  set(CMAKE_INSTALL_PREFIX "C:/Users/rafal/Desktop/PSU-Obfuscator/obf-dotnet/Lua/lua/out/install/x64-Debug")
endif()
string(REGEX REPLACE "/$" "" CMAKE_INSTALL_PREFIX "${CMAKE_INSTALL_PREFIX}")

# Set the install configuration name.
if(NOT DEFINED CMAKE_INSTALL_CONFIG_NAME)
  if(BUILD_TYPE)
    string(REGEX REPLACE "^[^A-Za-z0-9_]+" ""
           CMAKE_INSTALL_CONFIG_NAME "${BUILD_TYPE}")
  else()
    set(CMAKE_INSTALL_CONFIG_NAME "Debug")
  endif()
  message(STATUS "Install configuration: \"${CMAKE_INSTALL_CONFIG_NAME}\"")
endif()

# Set the component getting installed.
if(NOT CMAKE_INSTALL_COMPONENT)
  if(COMPONENT)
    message(STATUS "Install component: \"${COMPONENT}\"")
    set(CMAKE_INSTALL_COMPONENT "${COMPONENT}")
  else()
    set(CMAKE_INSTALL_COMPONENT)
  endif()
endif()

# Is this installation the result of a crosscompile?
if(NOT DEFINED CMAKE_CROSSCOMPILING)
  set(CMAKE_CROSSCOMPILING "FALSE")
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xRuntimex" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib/lua" TYPE FILE RENAME "llex.lua" FILES "C:/Users/rafal/Desktop/PSU-Obfuscator/obf-dotnet/Lua/lua/llex.lua")
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xRuntimex" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib/lua" TYPE FILE RENAME "lparser.lua" FILES "C:/Users/rafal/Desktop/PSU-Obfuscator/obf-dotnet/Lua/lua/lparser.lua")
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xRuntimex" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib/lua" TYPE FILE RENAME "optlex.lua" FILES "C:/Users/rafal/Desktop/PSU-Obfuscator/obf-dotnet/Lua/lua/optlex.lua")
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xRuntimex" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib/lua" TYPE FILE RENAME "optparser.lua" FILES "C:/Users/rafal/Desktop/PSU-Obfuscator/obf-dotnet/Lua/lua/optparser.lua")
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xRuntimex" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib/lua/plugin" TYPE FILE RENAME "example.lua" FILES "C:/Users/rafal/Desktop/PSU-Obfuscator/obf-dotnet/Lua/lua/plugin/example.lua")
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xRuntimex" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib/lua/plugin" TYPE FILE RENAME "sloc.lua" FILES "C:/Users/rafal/Desktop/PSU-Obfuscator/obf-dotnet/Lua/lua/plugin/sloc.lua")
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xRuntimex" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib/lua/plugin" TYPE FILE RENAME "html.lua" FILES "C:/Users/rafal/Desktop/PSU-Obfuscator/obf-dotnet/Lua/lua/plugin/html.lua")
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xRuntimex" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/bin" TYPE PROGRAM RENAME "LuaSrcDiet" FILES "C:/Users/rafal/Desktop/PSU-Obfuscator/obf-dotnet/Lua/lua/LuaSrcDiet.lua")
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xDatax" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/share/luasrcdiet" TYPE FILE FILES "C:/Users/rafal/Desktop/PSU-Obfuscator/obf-dotnet/Lua/lua/README")
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xDatax" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/share/luasrcdiet" TYPE FILE FILES "C:/Users/rafal/Desktop/PSU-Obfuscator/obf-dotnet/Lua/lua/technotes.txt")
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xDatax" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/share/luasrcdiet" TYPE FILE FILES "C:/Users/rafal/Desktop/PSU-Obfuscator/obf-dotnet/Lua/lua/COPYRIGHT")
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xDatax" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/share/luasrcdiet" TYPE FILE FILES "C:/Users/rafal/Desktop/PSU-Obfuscator/obf-dotnet/Lua/lua/Changelog")
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xExamplex" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/share/luasrcdiet/example/" TYPE DIRECTORY FILES "C:/Users/rafal/Desktop/PSU-Obfuscator/obf-dotnet/Lua/lua/sample/")
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xTestx" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/share/luasrcdiet/test/" TYPE DIRECTORY FILES "C:/Users/rafal/Desktop/PSU-Obfuscator/obf-dotnet/Lua/lua/test/")
endif()

if(CMAKE_INSTALL_COMPONENT)
  set(CMAKE_INSTALL_MANIFEST "install_manifest_${CMAKE_INSTALL_COMPONENT}.txt")
else()
  set(CMAKE_INSTALL_MANIFEST "install_manifest.txt")
endif()

string(REPLACE ";" "\n" CMAKE_INSTALL_MANIFEST_CONTENT
       "${CMAKE_INSTALL_MANIFEST_FILES}")
file(WRITE "C:/Users/rafal/Desktop/PSU-Obfuscator/obf-dotnet/Lua/lua/out/build/x64-Debug/${CMAKE_INSTALL_MANIFEST}"
     "${CMAKE_INSTALL_MANIFEST_CONTENT}")
