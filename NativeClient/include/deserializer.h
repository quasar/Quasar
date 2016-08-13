#pragma once
#include <vector>
#include "primitives.h"

// ReSharper disable CppExplicitSpecializationInNonNamespaceScope
class quasar_deserializer {
public:
	quasar_deserializer() {
	}

	template<typename T>
	T read_primitive(memstream &stream) {
		throw 0;
	};

	template <>
	std::string read_primitive<std::string>(memstream &stream) {
		uint32_t totalBytes = primitives::read_varint32(stream);

		if(totalBytes == 0 || totalBytes == 1) {
			return nullptr;
		}

		totalBytes--;
		// skip totalchars
		primitives::read_varint32(stream);
		int32_t streamBytesLeft = totalBytes;
		std::vector<char> buf(totalBytes);

		stream.read(&buf[0], totalBytes);
		return std::string(&buf[0], buf.size());
	}
};