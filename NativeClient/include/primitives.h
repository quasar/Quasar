#pragma once
#include <cstdint>
#include "membuf.h"
#include <vector>
#include <string>

class primitives {
public:
	static void write_varint32(std::vector<unsigned char> &payloadBuf, uint32_t value);
	static uint32_t read_varint32(mem_istream &stream);

	static void write_string(std::vector<unsigned char> &payloadBuf, std::string value);
	static void write_int32(std::vector<unsigned char> &payloadBuf, int32_t value);

private:
	static uint32_t encode_zigzag32(int32_t value);
};
