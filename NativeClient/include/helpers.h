#pragma once
#include <locale>
#include <codecvt>
#include <string>

/* http://stackoverflow.com/a/18374698 */

inline std::wstring s2ws(const std::string& str)
{
	using convert_typeX = std::codecvt_utf8<wchar_t>;
	std::wstring_convert<convert_typeX, wchar_t> converterX;

	return converterX.from_bytes(str);
}

inline std::string ws2s(const std::wstring& wstr)
{
	using convert_typeX = std::codecvt_utf8<wchar_t>;
	std::wstring_convert<convert_typeX, wchar_t> converterX;

	return converterX.to_bytes(wstr);
}

template<typename T>
void prefix_vector_length(std::vector<T> &vector) {
	throw std::runtime_error("Vector must be of char/unsigned char type");
}

template <>
inline void prefix_vector_length<char>(std::vector<char> &vector) {
	int32_t payloadSize = vector.size();
	char *chars = reinterpret_cast<char*>(&payloadSize);
	vector.insert(vector.begin(), chars, chars + sizeof(int32_t));
}

template <>
inline void prefix_vector_length<unsigned char>(std::vector<unsigned char> &vector) {
	int32_t payloadSize = vector.size();
	char *chars = reinterpret_cast<char*>(&payloadSize);
	vector.insert(vector.begin(), chars, chars + sizeof(int32_t));
}