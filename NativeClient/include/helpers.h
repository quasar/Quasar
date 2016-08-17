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

//inline std::string download_webpage(std::string server, std::string cmd) {
//	boost::asio::io_service io_service;
//
//	// Get a list of endpoints corresponding to the server name.
//	boost::asio::ip::tcp::resolver resolver(io_service);
//	boost::asio::ip::tcp::resolver::query query(server, "http");
//	boost::asio::ip::tcp::resolver::iterator endpoint_iterator = resolver.resolve(query);
//	boost::asio::ip::tcp::resolver::iterator end;
//
//	// Try each endpoint until we successfully establish a connection.
//	boost::asio::ip::tcp::socket socket(io_service);
//	boost::system::error_code error = boost::asio::error::host_not_found;
//	while (error && endpoint_iterator != end)
//	{
//		socket.close();
//		socket.connect(*endpoint_iterator++, error);
//	}
//
//	boost::asio::streambuf request;
//	std::ostream request_stream(&request);
//
//	request_stream << "GET " << cmd << " HTTP/1.0\r\n";
//	request_stream << "Host: " << server << "\r\n";
//	request_stream << "Accept: */*\r\n";
//	request_stream << "User-Agent: Mozilla/5.0 (Windows NT 6.3; rv:48.0) Gecko/20100101 Firefox/48.0\r\n";
//	request_stream << "Connection: close\r\n\r\n";
//
//	// Send the request.
//	boost::asio::write(socket, request);
//
//	// Read the response status line.
//	boost::asio::streambuf response;
//	boost::asio::read_until(socket, response, "\r\n");
//
//	// Check that response is OK.
//	std::istream response_stream(&response);
//	std::string http_version;
//	response_stream >> http_version;
//	unsigned int status_code;
//	response_stream >> status_code;
//	std::string status_message;
//	std::getline(response_stream, status_message);
//
//
//	// Read the response headers, which are terminated by a blank line.
//	boost::asio::read_until(socket, response, "\r\n\r\n");
//
//	std::stringstream s;
//	// Write whatever content we already have to output.
//	if (response.size() > 0)
//	{
//		//s << &response;
//	}
//	// Read until EOF, writing data to output as we go.
//	while (boost::asio::read(socket, response, boost::asio::transfer_at_least(1), error))
//	{
//		s << &response;
//	}
//
//	return "";
//}