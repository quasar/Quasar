#pragma once
#ifdef USE_BOOST
#include <vector>
#include "boost/array.hpp"
#include "boost/asio.hpp"
#include <string>
#include "packet.h"
#include "aes_crypt.h"
#include "client_packets.h"

class quasar_client_multi {
public:
	quasar_client_multi(boost::asio::io_service &io_srvc);
	~quasar_client_multi();

	void q_connect(std::string hostname, std::string port);
	void q_send(std::shared_ptr<quasar_client_packet> packet);

	bool is_connected() const;
	bool get_compress() const;

	void set_compress(const bool value);
	
	/* events */
	//boost::signals2::signal<void()> msig_on_disconnected;
private:
	boost::asio::ip::tcp::socket m_sock;
	boost::asio::ip::tcp::resolver m_resolver;
	std::vector<unsigned char> m_payload_buf;
	/* use a statically sized buffer for header size since it's always sizeof(int)==4 */
	boost::array<unsigned char, 4> m_hdr_buf;
	bool m_connected;
	bool m_compress;
	bool m_encrypt;
	aes_crypt m_aes;

	void connect_handler(const boost::system::error_code &ec);
	void read_header();
	void read_payload();

};
#endif