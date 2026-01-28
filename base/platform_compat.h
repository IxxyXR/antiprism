/*
   Copyright (c) 2025, Antiprism

   Antiprism - http://www.antiprism.com

   Platform compatibility definitions for MSVC and other compilers
*/

#ifndef PLATFORM_COMPAT_H
#define PLATFORM_COMPAT_H

/* MSVC compatibility */
#ifdef _MSC_VER

/* String comparison functions - MSVC uses _ prefix */
#define strcasecmp _stricmp
#define strncasecmp _strnicmp

/* File stat macros */
#include <sys/stat.h>
#ifndef S_ISDIR
#define S_ISDIR(m) (((m) & _S_IFMT) == _S_IFDIR)
#endif

/* unistd.h doesn't exist on Windows */
#define PLATFORM_NO_UNISTD

/* Disable specific MSVC warnings */
#pragma warning(disable: 4244)  /* conversion warnings */
#pragma warning(disable: 4267)  /* size_t conversion warnings */
#pragma warning(disable: 4305)  /* truncation warnings */

#endif /* _MSC_VER */

#endif /* PLATFORM_COMPAT_H */
