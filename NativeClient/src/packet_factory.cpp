#include "stdafx.h"
#include "packet_factory.h"
#include <membuf.h>
#include <primitives.h>

using namespace std;

std::shared_ptr<quasar_server_packet> packet_factory::create_packet(vector<unsigned char> &payload) {
	//basic_array_source<unsigned char> input_source(&payload[0], payload.size());

	std::shared_ptr<quasar_server_packet> packet;
	mem_istream stream(&payload[0], payload.size());
	char packetId = primitives::read_varint32(stream);

	switch (static_cast<QuasarPacketId>(packetId)) {
	case PACKET_GET_AUTHENTICATION:
		packet = std::dynamic_pointer_cast<quasar_server_packet>(
			std::make_shared<get_authentication_packet>());
		break;
	case PACKET_UNKNOWN:
		break;
	default: break;
	case PACKET_GET_PROCESSES:
		packet = std::dynamic_pointer_cast<quasar_server_packet>(
			std::make_shared<get_processes_packet>());
		break;
	case PACKET_DO_SHOW_MESSAGEBOX:
		packet = std::dynamic_pointer_cast<quasar_server_packet>(
			std::make_shared<do_show_message_box_packet>());
		break;
	}

	// can be null because of dynamic ptr cast
	if (packet != nullptr) {
		packet->deserialize_packet(stream);
	}
	return packet;
}
