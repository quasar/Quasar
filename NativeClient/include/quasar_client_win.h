#pragma once

#include <vector>
#include <string>
#include "packet.h"
#include "aes_crypt.h"
#include "client_packets.h"
#include <winsock2.h>
#pragma comment(lib,"ws2_32.lib")

class quasar_client_win {
public:
	quasar_client_win();
	~quasar_client_win();

	void q_connect(std::string hostname, std::string port);
	void q_send(std::shared_ptr<quasar_client_packet> packet);

	bool is_connected() const;
	bool get_compress() const;

	void set_compress(const bool value);

	/* events */
	//boost::signals2::signal<void()> msig_on_disconnected;
private:
	bool m_initialized;
	bool m_connected;
	bool m_compress;
	bool m_encrypt;
	aes_crypt m_aes;
	WSADATA m_wsadat;
	SOCKET m_sock;
	std::vector<unsigned char> m_payload_buf;

	void do_send(std::vector<unsigned char> &data);
	void do_receive();
	void process_payload();
	bool init_socket();
	void cleanup();
};