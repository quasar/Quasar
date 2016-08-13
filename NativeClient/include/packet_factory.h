#pragma once
#include "packet.h"
#include <vector>
#include "boost\shared_ptr.hpp"
#include <server_packets.h>

class packet_factory {
public:
	static boost::shared_ptr<quasar_server_packet> create_packet(std::vector<char> &payload);
};
