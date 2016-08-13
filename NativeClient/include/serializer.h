#pragma once
#include <vector>
#include "primitives.h"

// ReSharper disable CppExplicitSpecializationInNonNamespaceScope
class quasar_serializer {
public:
	quasar_serializer() :
		m_stream(std::vector<char>()) {
	}

	std::vector<char>* get_serializer_instance() {
		return &m_stream;
	}

	std::vector<char> get_serializer_data() const {
		return m_stream;
	}

	template<typename T>
	void write_primitive_array(const std::vector<T> &val) {
		throw 0;
	}

	template <> 
	void write_primitive_array<std::string>(const std::vector<std::string> &val) {
		write_primitive(static_cast<uint32_t>(val.size() + 1));
		for (std::vector<std::string>::const_iterator it = val.begin(); it != val.end(); ++it) {
			write_primitive(*it);
		}
	}

	template <> 
	void write_primitive_array<int32_t>(const std::vector<int32_t> &val) {
		write_primitive(static_cast<uint32_t>(val.size() + 1));
		for (std::vector<int32_t>::const_iterator it = val.begin(); it != val.end(); ++it) {
			write_primitive(*it);
		}
	}

	template <> 
	void write_primitive_array<uint32_t>(const std::vector<uint32_t> &val) {
		write_primitive(static_cast<uint32_t>(val.size() + 1));
		for (std::vector<uint32_t>::const_iterator it = val.begin(); it != val.end(); ++it) {
			write_primitive(*it);
		}
	}

	template<typename T>
	void write_primitive(const T &val) {
		throw 0;
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
	std::vector<char> m_stream;
};