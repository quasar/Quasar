#pragma once
#include "boost/shared_ptr.hpp"
#include <vector>
#include "serializer.h"
#include "deserializer.h"

enum QuasarPacketId {
	PACKET_UNKNOWN = 0,
	PACKET_GET_AUTHENTICATION = 106,
	PACKET_GET_AUTHENTICATION_RESPONSE = 56,
	PACKET_GET_PROCESSES = 97,
	PACKET_GET_PROCESSES_RESPONSE = 49,
	PACKET_DO_SHOW_MESSAGEBOX = 85,
	PACKET_SET_STATUS = 55
};

class quasar_packet {
public:
	virtual ~quasar_packet() {
	}

	quasar_packet();
	quasar_packet(QuasarPacketId id);

	QuasarPacketId get_id() const;

	//virtual std::vector<char> serialize_packet() = 0;
	//virtual void deserialize_packet(memstream &stream) = 0;

	static bool is_unknown(const quasar_packet &packet) {
		return packet.get_id() == PACKET_UNKNOWN;
	}

	static bool is_unknown(const boost::shared_ptr<quasar_packet> packet) {
		return packet == nullptr || packet->get_id() == PACKET_UNKNOWN;
	}

protected:
	quasar_serializer m_serializer;
	quasar_deserializer m_deserializer;

	// must be called at start of serialize functions
	void begin_serialization();
	// must be called at the end of serialize functions
	void finalize_serialization();

private:
	QuasarPacketId m_id;

	void write_header(std::vector<unsigned char> &payloadBuf) const;
};
