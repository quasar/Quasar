#pragma once
#include "packet.h"
#include "boost/shared_ptr.hpp"
#include <quasar_client.h>

class command_handler {
public:
	static void handle_packet(quasar_client *client, boost::shared_ptr<quasar_packet> packet);

private:
	static void handle_get_authentication(quasar_client *client, boost::shared_ptr<get_authentication_packet> packet);
};
