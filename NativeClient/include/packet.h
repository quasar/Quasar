#pragma once
#include "membuf.h"
#include "boost/shared_ptr.hpp"
#include <vector>

enum QuasarPacketId {
	PACKET_UNKNOWN = -1,
	PACKET_GET_AUTHENTICATION = 106,
	PACKET_GET_AUTHENTICATION_RESPONSE = 114
};

class quasar_packet {
public:
	virtual ~quasar_packet() {
	}

	quasar_packet();
	quasar_packet(QuasarPacketId id);

	QuasarPacketId get_id() const;

	virtual void serialize_packet(std::vector<char> &payloadBuf) = 0;
	virtual void deserialize_packet(memstream &stream) = 0;

	static bool is_unknown(const quasar_packet &packet) {
		return packet.get_id() == PACKET_UNKNOWN;
	}

	static bool is_unknown(const boost::shared_ptr<quasar_packet> packet) {
		return packet->get_id() == PACKET_UNKNOWN;
	}

private:
	QuasarPacketId m_id;
};

class get_authentication_response_packet : public quasar_packet {
public:
	get_authentication_response_packet();

	void serialize_packet(std::vector<char> &payloadBuf) override;
	void deserialize_packet(memstream &stream) override;

private:
	std::string m_account_type;
};