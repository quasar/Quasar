// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#include "targetver.h"

#include <stdio.h>
#include <tchar.h>
#include <memory>

//
//#define QLZ_COMPRESSION_LEVEL 3
//#define QLZ_STREAMING_BUFFER 0
//#define QLZ_MEMORY_SAFE 0

// TODO: reference additional headers your program requires here

#define MAX_PACKET_SIZE (1024 * 1024) * 5

#ifdef WIN32
#ifdef USE_BOOST
#include "quasar_client.h"
using quasar_client = quasar_client_multi;
#else
#include "quasar_client_win.h"
using quasar_client = quasar_client_win;
#endif

#define WIN32_LEAN_AND_MEAN
#include <Windows.h>
#else
#include "quasar_client.h"
using quasar_client = quasar_client_multi;
#endif
