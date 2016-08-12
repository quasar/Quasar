#include "stdafx.h"
#include "command_handler.h"
#include <boost/smart_ptr/make_shared.hpp>

void command_handler::handle_packet(quasar_client *client, boost::shared_ptr<quasar_packet> packet) {
	switch(packet->get_id()) {

	case PACKET_UNKNOWN: break;
	case PACKET_GET_AUTHENTICATION: 
		handle_get_authentication(client, boost::dynamic_pointer_cast<get_authentication_packet>(packet));
		break;
	case PACKET_GET_AUTHENTICATION_RESPONSE: break;
	default: break;
	}
}

void command_handler::handle_get_authentication(quasar_client *client, 
	boost::shared_ptr<get_authentication_packet> packet) {
	auto response = boost::make_shared<get_authentication_response_packet>();

	client->send(response);
}
