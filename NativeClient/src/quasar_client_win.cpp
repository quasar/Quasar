#include "stdafx.h"
#if WIN32 && !USE_BOOST
#include "quasar_client_win.h"
#include <quicklz_helper.h>
#include <packet_factory.h>
#include <helpers.h>
//#include <cryptopp/hrtimer.h>

#define NON_BLOCKING 1
#define BLOCKING 0

using namespace std;

quasar_client_win::quasar_client_win() :
	m_connected(false),
	m_compress(true),
	m_encrypt(true),
	m_aes("1WvgEMPjdwfqIMeM9MclyQ==", "NcFtjbDOcsw7Evd3coMC0y4koy/SRZGydhNmno81ZOWOvdfg7sv0Cj5ad2ROUfX4QMscAIjYJdjrrs41+qcQwg=="),
	m_sock(0),
	m_initialized(false) {
}

quasar_client_win::~quasar_client_win() {
	cleanup();
}

void quasar_client_win::q_connect(string hostname, string port) {
	if (!m_initialized) {
		if (!init_socket()) {
			return;
		}
	}

	hostent *host = gethostbyname(hostname.c_str());
	SOCKADDR_IN SockAddr;
	SockAddr.sin_port = htons(stoi(port));
	SockAddr.sin_family = AF_INET;
	SockAddr.sin_addr.s_addr = *((unsigned long*)host->h_addr);

	if (connect(m_sock, reinterpret_cast<SOCKADDR*>(&SockAddr), sizeof SockAddr) != ERROR_SUCCESS) {
		cleanup();
		return;
	}

	u_long iMode = NON_BLOCKING;
	ioctlsocket(m_sock, FIONBIO, &iMode);
	do_receive();
}

void quasar_client_win::q_send(shared_ptr<quasar_client_packet> packet) {
	vector<unsigned char> payloadBuf = packet->serialize_packet();

	if (m_compress) {
		quicklz_helper::compress_data(payloadBuf);
	}

	if (m_encrypt) {
		m_aes.encrypt(payloadBuf);
	}

	prefix_vector_length(payloadBuf);
	do_send(payloadBuf);
}

bool quasar_client_win::is_connected() const {
	return m_connected;
}

bool quasar_client_win::get_compress() const {
	return m_compress;
}

void quasar_client_win::set_compress(const bool value) {
	m_compress = value;
}

void quasar_client_win::do_send(vector<unsigned char> &data) {
	int32_t sent = 0, wsaerr;
	int32_t payloadSize = data.size();
	while (sent < payloadSize) {
		sent += send(m_sock, reinterpret_cast<const char*>(&data[0]),
			data.size(), 0);
		
		wsaerr = WSAGetLastError();
		if(wsaerr != WSAEWOULDBLOCK && wsaerr != ERROR_SUCCESS) {
			cleanup();
			break;
		}
	}
}

void quasar_client_win::do_receive() {
	char hdrBuf[4];
	int32_t readLen, wsaerr, read;
	ZeroMemory(&hdrBuf, sizeof hdrBuf);

	while(true) {
		readLen = recv(m_sock, hdrBuf, sizeof hdrBuf, 0);
		wsaerr = WSAGetLastError();
		if (wsaerr != WSAEWOULDBLOCK && wsaerr != ERROR_SUCCESS) {
			cleanup();
			break;
		}
		if(readLen == -1) {
			continue;
		}
		int32_t payloadSize = *reinterpret_cast<int32_t*>(hdrBuf);
		if(payloadSize <= 0 || payloadSize > MAX_PACKET_SIZE) {
			continue;
		}

		m_payload_buf.clear();
		m_payload_buf.resize(payloadSize);
		read = 0;
		while(read < m_payload_buf.size()) {
			read += recv(m_sock, reinterpret_cast<char*>(&m_payload_buf[0]), payloadSize, 0);
			wsaerr = WSAGetLastError();
			if (wsaerr != WSAEWOULDBLOCK && wsaerr != ERROR_SUCCESS) {
				cleanup();
				break;
			}
			process_payload();
		}
	}
}

void quasar_client_win::process_payload() {
	if(m_encrypt) {
		m_aes.decrypt(m_payload_buf);
	}
	if(m_compress) {
		quicklz_helper::decompress_data(m_payload_buf);
	}
	shared_ptr<quasar_server_packet> parsedPacket;

	try {
		parsedPacket = packet_factory::create_packet(m_payload_buf);
	}
	catch (...) {
		parsedPacket = nullptr;
	}

	if (quasar_packet::is_unknown(parsedPacket) || parsedPacket == nullptr) {
		// skip this packet
		return;
	}
	// to allow compilation

	parsedPacket->execute(*this);

}

bool quasar_client_win::init_socket() {
	if (WSAStartup(MAKEWORD(2, 2), &m_wsadat) != ERROR_SUCCESS) {
		cleanup();
		return false;
	}

	m_sock = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if(m_sock == INVALID_SOCKET) {
		cleanup();
		return false;
	}

	m_initialized = true;
	return true;
}

void quasar_client_win::cleanup() {
	WSACleanup();
	if(m_sock != 0) {
		shutdown(m_sock, SD_SEND);
		closesocket(m_sock);
		m_sock = 0;
	}
	m_initialized = false;
}
#endif