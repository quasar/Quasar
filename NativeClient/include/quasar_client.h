#pragma once
#include <vector>
#include "boost/array.hpp"
#include "boost/asio.hpp"
#include "boost/signals2.hpp"
#include <string>
#include "packet.h"


#define MAX_PACKET_SIZE (1024 * 1024) * 5

class quasar_client {
public:
	quasar_client(boost::asio::io_service &io_srvc);
	~quasar_client();

	void connect(std::string hostname, std::string port);
	void send(boost::shared_ptr<quasar_packet> packet);

	bool is_connected() const;
	
	/* events */
	boost::signals2::signal<void()> msig_on_disconnected;
private:
	boost::asio::ip::tcp::socket m_sock;
	boost::asio::ip::tcp::resolver m_resolver;
	std::vector<char> m_payload_buf;
	/* use a statically sized buffer for header size since it's always sizeof(int)==4 */
	boost::array<char, 4> m_hdr_buf;
	bool m_connected;

	void connect_handler(const boost::system::error_code &ec);
	void read_header();
	void read_payload();

};
