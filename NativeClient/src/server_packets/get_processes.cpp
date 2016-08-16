#include "stdafx.h"
#include "server_packets.h"

using namespace std;

get_processes_packet::get_processes_packet() :
	quasar_server_packet(PACKET_GET_PROCESSES) {
}

void get_processes_packet::deserialize_packet(memstream &stream) {

}