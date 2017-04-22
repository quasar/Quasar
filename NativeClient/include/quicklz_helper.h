#pragma once
#include <vector>

class quicklz_helper {
public:
	static void compress_data(std::vector<char> &data);
	static void compress_data(std::vector<unsigned char> &data);

	static void decompress_data(std::vector<char> &data);
	static void decompress_data(std::vector<unsigned char> &data);
};
