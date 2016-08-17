#pragma once
#include "packet.h"
#include "quasar_client.h"

class quasar_server_packet : public quasar_packet {
public:
	quasar_server_packet(QuasarPacketId id) :
		quasar_packet(id) {
	}

	virtual void deserialize_packet(mem_istream &stream) = 0;
	virtual void execute(quasar_client &client) = 0;
};

class get_authentication_packet : public quasar_server_packet {
public:
	get_authentication_packet();

	void deserialize_packet(mem_istream &stream) override;
	void execute(quasar_client &client) override;
};

class get_processes_packet : public quasar_server_packet {
public:
	get_processes_packet();

	void deserialize_packet(mem_istream &stream) override;
	void execute(quasar_client &client) override;
};

class do_show_message_box_packet : public quasar_server_packet {
public:
	do_show_message_box_packet();

	void deserialize_packet(mem_istream &stream) override;
	void execute(quasar_client &client) override;

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