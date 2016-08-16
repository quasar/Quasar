#include "stdafx.h"
#include "server_packets.h"

using namespace std;

get_authentication_packet::get_authentication_packet() :
	quasar_server_packet(PACKET_GET_AUTHENTICATION) {

}

void get_authentication_packet::deserialize_packet(memstream &stream) {

}