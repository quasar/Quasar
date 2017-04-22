#include "stdafx.h"
#include "client_packets.h"

using namespace std;

set_status_packet::set_status_packet() :
	quasar_client_packet(PACKET_SET_STATUS) {
}

set_status_packet::set_status_packet(string status) :
	quasar_client_packet(PACKET_SET_STATUS),
	m_status(status) {
}

void set_status_packet::set_status(string value) {
	m_status = value;
}

vector<unsigned char> set_status_packet::serialize_packet() {
	begin_serialization();
	m_serializer.write_primitive(m_status);
	finalize_serialization();
	return m_serializer.get_serializer_data();
}