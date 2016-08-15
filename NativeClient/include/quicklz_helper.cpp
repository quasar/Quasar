#include "stdafx.h"
#include "quicklz_helper.h"
#include "quicklz.h"

using namespace std;

void quicklz_helper::compress_data(vector<char> &data) {
	int32_t finalSize = qlz_size_compressed(&data[0]);

}

void quicklz_helper::compress_data(vector<unsigned char> &data) {
	qlz_state_compress *state_compress = static_cast<qlz_state_compress *>(malloc(sizeof(qlz_state_compress)));
	char *dest = new char[data.size() + 400];
	auto len = qlz_compress(reinterpret_cast<char*>(&data[0]), dest, data.size(), state_compress);
	data.resize(len);
	memcpy(&data[0], dest, len);
}

void quicklz_helper::decompress_data(vector<char> &data) {
	int32_t finalSize = qlz_size_decompressed(&data[0]);
	vector<char> tmpBuf(finalSize);
	auto state_decompress = static_cast<qlz_state_decompress*>(malloc(sizeof(qlz_state_decompress)));

	qlz_decompress(&data[0], &tmpBuf[0], state_decompress);
	data.swap(tmpBuf);
}

void quicklz_helper::decompress_data(vector<unsigned char> &data) {
	int32_t finalSize = qlz_size_decompressed(reinterpret_cast<char*>(&data[0]));
	vector<unsigned char> tmpBuf(finalSize);
	auto state_decompress = static_cast<qlz_state_decompress*>(malloc(sizeof(qlz_state_decompress)));

	qlz_decompress(reinterpret_cast<char*>(&data[0]), &tmpBuf[0], state_decompress);
	data.swap(tmpBuf);
}
