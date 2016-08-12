#include "stdafx.h"
#include "packet.h"
#include <primitives.h>

quasar_packet::quasar_packet() :
	m_id(PACKET_UNKNOWN){
}

quasar_packet::quasar_packet(QuasarPacketId id) :
	m_id(id) {

}

QuasarPacketId quasar_packet::get_id() const {
	return m_id;
}

get_authentication_response_packet::get_authentication_response_packet() :
	quasar_packet(PACKET_GET_AUTHENTICATION) {

}

void get_authentication_response_packet::serialize_packet(std::vector<char> &payloadBuf) {
	primitives::write_string(payloadBuf, m_account_type);
}

void get_authentication_response_packet::deserialize_packet(memstream &stream) {

}
