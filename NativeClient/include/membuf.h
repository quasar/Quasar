#pragma once
#include <boost/iostreams/stream.hpp>
#include <boost/iostreams/device/array.hpp>
using namespace boost::iostreams;

typedef stream<basic_array_source<unsigned char> > memstream;

