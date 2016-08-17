// The MIT License (MIT)
// Simplistic Binary Streams 0.9
// Copyright (C) 2014, by Wong Shao Voon (shaovoon@yahoo.com)
//
// http://opensource.org/licenses/MIT
//

#pragma once
#include <iostream>
#include <fstream>
#include <vector>

/* http://www.codeproject.com/Tips/808776/Cplusplus-Simplistic-Binary-Streams */

class mem_istream
{
public:
	mem_istream() : m_index(0) {}
	mem_istream(const char * mem, size_t size)
	{
		open(mem, size);
	}
	mem_istream(const unsigned char * mem, size_t size) 
	{
		open(mem, size);
	}
	mem_istream(const std::vector<char>& vec)
	{
		m_index = 0;
		m_vec.clear();
		m_vec.reserve(vec.size());
		m_vec.assign(vec.begin(), vec.end());
	}
	void open(const char * mem, size_t size)
	{
		m_index = 0;
		m_vec.clear();
		m_vec.reserve(size);
		m_vec.assign(mem, mem + size);
	}
	void open(const unsigned char * mem, size_t size)
	{
		m_index = 0;
		m_vec.clear();
		m_vec.reserve(size);
		m_vec.assign(mem, mem + size);
	}
	void close()
	{
		m_vec.clear();
	}
	bool eof() const
	{
		return m_index >= m_vec.size();
	}
	std::ifstream::pos_type tellg()
	{
		return m_index;
	}
	bool seekg(size_t pos)
	{
		if (pos<m_vec.size())
			m_index = pos;
		else
			return false;

		return true;
	}
	bool seekg(std::streamoff offset, std::ios_base::seekdir way)
	{
		if (way == std::ios_base::beg && offset < m_vec.size())
			m_index = offset;
		else if (way == std::ios_base::cur && (m_index + offset) < m_vec.size())
			m_index += offset;
		else if (way == std::ios_base::end && (m_vec.size() + offset) < m_vec.size())
			m_index = m_vec.size() + offset;
		else
			return false;

		return true;
	}

	const std::vector<unsigned char>& get_internal_vec()
	{
		return m_vec;
	}

	template<typename T>
	void read(T& t)
	{
		if (eof())
			throw std::runtime_error("Premature end of array!");

		if ((m_index + sizeof(T)) > m_vec.size())
			throw std::runtime_error("Premature end of array!");

		std::memcpy(reinterpret_cast<void*>(&t), &m_vec[m_index], sizeof(T));

		m_index += sizeof(T);
	}

#pragma warning(push)
#pragma warning(disable: 4244 /* possible loss of data*/ )
	void read(char* p, size_t size)
	{
		if (eof())
			throw std::runtime_error("Premature end of array!");

		if ((m_index + size) > m_vec.size())
			throw std::runtime_error("Premature end of array!");

		std::memcpy(reinterpret_cast<void*>(p), &m_vec[m_index], size);

		m_index += size;
	}

	void read(unsigned char* p, size_t size)
	{
		if (eof())
			throw std::runtime_error("Premature end of array!");

		if ((m_index + size) > m_vec.size())
			throw std::runtime_error("Premature end of array!");

		std::memcpy(reinterpret_cast<void*>(p), &m_vec[m_index], size);

		m_index += size;
	}
#pragma warning(pop)

private:
	std::vector<unsigned char> m_vec;
	std::streamoff m_index;
};
