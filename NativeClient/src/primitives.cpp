#include "stdafx.h"
#include "primitives.h"

using namespace std;

void primitives::write_varint32(vector<unsigned char>& payloadBuf, uint32_t value) {
	for (; value >= 0x80u; value >>= 7) {
		payloadBuf.push_back(static_cast<char>(value | 0x80u));
	}
	payloadBuf.push_back(static_cast<char>(value));
}

uint32_t primitives::read_varint32(memstream& stream) {
	int32_t result = 0;
	int32_t offset = 0;

	for (; offset < 32; offset += 7)
	{
		unsigned char b;
		stream.read(&b, sizeof(char));

		result |= (b & 0x7f) << offset;

		if ((b & 0x80) == 0)
			return static_cast<uint32_t>(result);
	}
	return 0;
}

void primitives::write_string(vector<unsigned char> &payloadBuf, string value) {
	if(value.empty()) {
		write_varint32(payloadBuf, 1);
	}

	int32_t totalBytes = value.size();

	write_varint32(payloadBuf, totalBytes + 1);
	write_varint32(payloadBuf, totalBytes);

	for (auto const chr : value) {
		payloadBuf.push_back(chr);
	}
}

void primitives::write_int32(vector<unsigned char> &payloadBuf, int32_t value) {
	write_varint32(payloadBuf, encode_zigzag32(value));
}

uint32_t primitives::encode_zigzag32(int32_t value) {
	return static_cast<uint32_t>((value << 1) ^ (value >> 31));
}
