#pragma once
#include "packet.h"
#include <vector>
#include <server_packets.h>

class packet_factory {
public:
	static std::shared_ptr<quasar_server_packet> create_packet(std::vector<unsigned char> &payload);
};
