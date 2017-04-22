#pragma once
#include <vector>
#include "primitives.h"

// ReSharper disable CppExplicitSpecializationInNonNamespaceScope
class quasar_serializer {
public:
	quasar_serializer() :
		m_stream(std::vector<unsigned char>()) {
	}

	std::vector<unsigned char>* get_serializer_instance() {
		return &m_stream;
	}

	std::vector<unsigned char> get_serializer_data() const {
		return m_stream;
	}

	template<typename T>
	void write_primitive_array(const std::vector<T> &val) {
		throw std::runtime_error("Unsupported primitive type");
	}

	template <> 
	void write_primitive_array<std::string>(const std::vector<std::string> &val) {
		write_primitive(static_cast<uint32_t>(val.size() + 1));
		for (auto const str : val) {
			write_primitive(str);
		}
	}

	template <> 
	void write_primitive_array<int32_t>(const std::vector<int32_t> &val) {
		write_primitive(static_cast<uint32_t>(val.size() + 1));
		for (auto const curVal : val) {
			write_primitive(curVal);
		}
	}

	template <> 
	void write_primitive_array<uint32_t>(const std::vector<uint32_t> &val) {
		write_primitive(static_cast<uint32_t>(val.size() + 1));
		for (auto const curVal : val) {
			write_primitive(curVal);
		}
	}

	template<typename T>
	void write_primitive(const T &val) {
		throw std::runtime_error("Unsupported primitive type");
	}

	template <> 
	void write_primitive<std::string>(const std::string &val) {
		primitives::write_string(m_stream, val);
	}

	template <> 
	void write_primitive<int32_t>(const int32_t &val) {
		primitives::write_int32(m_stream, val);
	}

	template <> 
	void write_primitive<uint32_t>(const uint32_t &val) {
		primitives::write_varint32(m_stream, val);
	}

private:
	std::vector<unsigned char> m_stream;
};