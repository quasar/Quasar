// trap.h - written and placed in public domain by Jeffrey Walton.
//          Copyright assigned to Crypto++ project

#ifndef CRYPTOPP_TRAP_H
#define CRYPTOPP_TRAP_H

#include "config.h"

// CRYPTOPP_POSIX_ASSERT unconditionally disables the library assert and yields
//   to Posix assert. CRYPTOPP_POSIX_ASSERT can be set in config.h. if you want
//   to disable asserts, then define NDEBUG or _NDEBUG when building the library.

// Needed for NDEBUG and CRYPTOPP_POSIX_ASSERT
#include <cassert>

#if defined(CRYPTOPP_DEBUG)
#  include <iostream>
#  include <sstream>
#  if defined(CRYPTOPP_WIN32_AVAILABLE)
#    pragma push_macro("WIN32_LEAN_AND_MEAN")
#    pragma push_macro("_WIN32_WINNT")
#    pragma push_macro("NOMINMAX")
#    define WIN32_LEAN_AND_MEAN
#    define _WIN32_WINNT 0x0400
#    define NOMINMAX
#    include <Windows.h>
#  elif defined(CRYPTOPP_BSD_AVAILABLE) || defined(CRYPTOPP_UNIX_AVAILABLE)
#    include <signal.h>
#  endif
#endif // CRYPTOPP_DEBUG

// ************** run-time assertion ***************

// See test.cpp and DebugTrapHandler for code to install a null signal handler
// for SIGTRAP on BSD, Linux and Unix. The handler installs itself during
// initialization of the test program.

#if defined(CRYPTOPP_DEBUG) && (defined(CRYPTOPP_BSD_AVAILABLE) || defined(CRYPTOPP_UNIX_AVAILABLE))
#  define CRYPTOPP_ASSERT(exp) {                                  \
    if (!(exp)) {                                                 \
      std::ostringstream oss;                                     \
      oss << "Assertion failed: " << (char*)(__FILE__) << "("     \
          << (int)(__LINE__) << "): " << (char*)(__func__)        \
          << std::endl;                                           \
      std::cerr << oss.str();                                     \
      raise(SIGTRAP);                                             \
    }                                                             \
  }
#elif defined(CRYPTOPP_DEBUG) && defined(CRYPTOPP_WIN32_AVAILABLE)
#  define CRYPTOPP_ASSERT(exp) {                                  \
    if (!(exp)) {                                                 \
      std::ostringstream oss;                                     \
      oss << "Assertion failed: " << (char*)(__FILE__) << "("     \
          << (int)(__LINE__) << "): " << (char*)(__FUNCTION__)    \
          << std::endl;                                           \
      DebugBreak();                                               \
      std::cerr << oss.str();                                     \
    }                                                             \
  }
// Fallback to original behavior for NDEBUG (and non-Windows/non-Unix builds)
#else
#  define CRYPTOPP_ASSERT(exp) assert(exp)
#endif // DEBUG and Unix or Windows

#if defined(CRYPTOPP_DEBUG) && defined(CRYPTOPP_WIN32_AVAILABLE)
#  pragma pop_macro("WIN32_LEAN_AND_MEAN")
#  pragma pop_macro("_WIN32_WINNT")
#  pragma pop_macro("NOMINMAX")
#endif

#endif // CRYPTOPP_TRAP_H
