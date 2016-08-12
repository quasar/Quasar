#pragma once
#include "packet.h"
#include <vector>
#include "boost\shared_ptr.hpp"

class packet_factory {
public:
	static boost::shared_ptr<quasar_packet> create_packet(std::vector<char> &payload);
};
