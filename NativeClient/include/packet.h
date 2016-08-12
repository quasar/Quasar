#pragma once
#include "membuf.h"
#include "boost/shared_ptr.hpp"
#include <vector>

enum QuasarPacketId {
	PACKET_UNKNOWN = -1,
	PACKET_GET_AUTHENTICATION = 106,
	PACKET_GET_AUTHENTICATION_RESPONSE = 56
};

class quasar_packet {
public:
	virtual ~quasar_packet() {
	}

	quasar_packet();
	quasar_packet(QuasarPacketId id);

	QuasarPacketId get_id() const;

	// must be called at start of serialize functions
	virtual void serialize_packet(std::vector<char> &payloadBuf) = 0;
	virtual void deserialize_packet(memstream &stream) = 0;

	static bool is_unknown(const quasar_packet &packet) {
		return packet.get_id() == PACKET_UNKNOWN;
	}

	static bool is_unknown(const boost::shared_ptr<quasar_packet> packet) {
		return packet->get_id() == PACKET_UNKNOWN;
	}

	static void begin_serialization(std::vector<char> &payloadBuf, QuasarPacketId id);
	// must be called at the end of serialize functions
	static void finalize_serialization(std::vector<char> &payloadBuf);

private:
	QuasarPacketId m_id;

	void write_header(std::vector<char> &payloadBuf) const;
};

class get_authentication_packet : public quasar_packet {
public:
	get_authentication_packet();

	void serialize_packet(std::vector<char> &payloadBuf) override;
	void deserialize_packet(memstream &stream) override;
};

class get_authentication_response_packet : public quasar_packet {
public:
	get_authentication_response_packet();

	void serialize_packet(std::vector<char> &payloadBuf) override;
	void deserialize_packet(memstream &stream) override;

private:
	std::string m_account_type;
	std::string m_city;
	std::string m_country;
	std::string m_country_code;
	std::string m_id;
	std::string m_os;
	int32_t m_img_idx;
	std::string m_pcname;
	std::string m_region;
	std::string m_tag;
	std::string m_username;
	std::string m_ver;

	void initialize_values();
};