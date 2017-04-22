#include "stdafx.h"
#include "client_packets.h"

using namespace std;

get_processes_response_packet::get_processes_response_packet() :
	quasar_client_packet(PACKET_GET_AUTHENTICATION_RESPONSE) {
}

vector<unsigned char> get_processes_response_packet::serialize_packet() {
	begin_serialization();
	finalize_serialization();
	return m_serializer.get_serializer_data();
}