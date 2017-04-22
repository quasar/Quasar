#pragma once
#include <vector>
#include "primitives.h"

// ReSharper disable CppExplicitSpecializationInNonNamespaceScope
class quasar_deserializer {
public:
	quasar_deserializer() {
	}

	template<typename T>
	T read_primitive(mem_istream &stream) {
		throw 0;
	};

	template <>
	std::string read_primitive<std::string>(mem_istream &stream) {
		uint32_t totalBytes = primitives::read_varint32(stream);

		if(totalBytes == 0 || totalBytes == 1) {
			return nullptr;
		}

		totalBytes--;
		// skip totalchars
		primitives::read_varint32(stream);
		int32_t streamBytesLeft = totalBytes;
		std::vector<unsigned char> buf(totalBytes);

		stream.read(&buf[0], totalBytes);
		return std::string(reinterpret_cast<char*>(&buf[0]), buf.size());
	}
};