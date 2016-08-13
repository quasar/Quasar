#include "stdafx.h"
#include "packet.h"
#include <primitives.h>
#include "win_helper.h"
#include <string>
#include "helpers.h"

using namespace std;

quasar_packet::quasar_packet() :
	m_id(PACKET_UNKNOWN),
	m_serializer(),
	m_deserializer(){
}

quasar_packet::quasar_packet(QuasarPacketId id) :
	m_id(id) {

}

QuasarPacketId quasar_packet::get_id() const {
	return m_id;
}

void quasar_packet::begin_serialization() {
	m_serializer.write_primitive(static_cast<uint32_t>(m_id));
}

void quasar_packet::finalize_serialization() {
	auto instance = m_serializer.get_serializer_instance();
	int32_t payloadSize = instance->size();
	char *chars = reinterpret_cast<char*>(&payloadSize);

	instance->insert(instance->begin(), chars, chars + sizeof(int32_t));
}

void quasar_packet::write_header(std::vector<char> &payloadBuf) const {
	primitives::write_varint32(payloadBuf, m_id);
}

/* ---------------------------------- */
/* GET_AUTHENTICATION_PACKET */
/* ---------------------------------- */

get_authentication_packet::get_authentication_packet() :
	quasar_packet(PACKET_GET_AUTHENTICATION) {

}

vector<char> get_authentication_packet::serialize_packet() {
	begin_serialization();
	finalize_serialization();
	return m_serializer.get_serializer_data();
}

void get_authentication_packet::deserialize_packet(memstream &stream) {

}

/* ---------------------------------- */
/* GET_AUTHENTICATION_RESPONSE_PACKET */
/* ---------------------------------- */

get_authentication_response_packet::get_authentication_response_packet() :
	quasar_packet(PACKET_GET_AUTHENTICATION_RESPONSE) {
	initialize_values();
}

vector<char> get_authentication_response_packet::serialize_packet() {
	begin_serialization();
	m_serializer.write_primitive(m_account_type);
	m_serializer.write_primitive(m_city);
	m_serializer.write_primitive(m_country);
	m_serializer.write_primitive(m_country_code);
	m_serializer.write_primitive(m_id);
	m_serializer.write_primitive(m_img_idx);
	m_serializer.write_primitive(m_os);
	m_serializer.write_primitive(m_pcname);
	m_serializer.write_primitive(m_region);
	m_serializer.write_primitive(m_tag);
	m_serializer.write_primitive(m_username);
	m_serializer.write_primitive(m_ver);
	finalize_serialization();

	return m_serializer.get_serializer_data();
}

void get_authentication_response_packet::deserialize_packet(memstream &stream) {

}

void get_authentication_response_packet::initialize_values() {
	if (is_elevated()) {
		m_account_type = "Admin";
	}
	else {
		m_account_type = "Guest";
	}

	wstring pcname, username;

	if (!get_pcname(pcname)) {
		pcname = L"";
	}
	if (!get_username(username)) {
		username = L"";
	}

	m_city = "secret";
	m_country = "Sweden";
	m_country_code = "SE";
	m_id = "AEB06EC90268D849910326C067314FD8EE2B93D4F6EC3752D4970A9EFC45AF21";
	m_os = "Windows 7 Professional 64 Bit";
	m_img_idx = 195;
	m_pcname = ws2s(pcname);
	m_region = "N";
	m_tag = "DEBUG-NATIVE";
	m_username = ws2s(username);
	m_ver = "1.2.0.0-N";
}

/* ---------------------------------- */
/* GET_PROCESSES_PACKET */
/* ---------------------------------- */

get_processes_packet::get_processes_packet() {
}

vector<char> get_processes_packet::serialize_packet() {
	begin_serialization();
	finalize_serialization();
	return m_serializer.get_serializer_data();
}

void get_processes_packet::deserialize_packet(memstream &stream) {
}

/* ---------------------------------- */
/* GET_PROCESSES_RESPONSE_PACKET */
/* ---------------------------------- */

get_processes_response_packet::get_processes_response_packet() {
}

vector<char> get_processes_response_packet::serialize_packet() {
	begin_serialization();
	finalize_serialization();
	return m_serializer.get_serializer_data();
}

void get_processes_response_packet::deserialize_packet(memstream& stream) {
}

/* ---------------------------------- */
/* DO_SHOW_MESSAGE_BOX_PACKET		 */
/* ---------------------------------- */

do_show_message_box_packet::do_show_message_box_packet() :
	quasar_packet(PACKET_DO_SHOW_MESSAGEBOX){
}

std::vector<char> do_show_message_box_packet::serialize_packet() {
	return m_serializer.get_serializer_data();
}

void do_show_message_box_packet::deserialize_packet(memstream &stream) {
	m_caption = m_deserializer.read_primitive<string>(stream);
	m_btn = m_deserializer.read_primitive<string>(stream);
	m_icon = m_deserializer.read_primitive<string>(stream);
	m_text = m_deserializer.read_primitive<string>(stream);
}

string do_show_message_box_packet::get_caption() const {
	return m_caption;
}

string do_show_message_box_packet::get_text() const {
	return m_text;
}

string do_show_message_box_packet::get_btn() const {
	return m_btn;
}

string do_show_message_box_packet::get_icon() const {
	return m_icon;
}

uint32_t do_show_message_box_packet::get_button_value(const std::string &val) {
	if (val == "OK") {
		return MB_OK;
	} if (val == "OKCancel") {
		return MB_OKCANCEL;
	} if (val == "AbortRetryIgnore") {
		return MB_ABORTRETRYIGNORE;
	} if (val == "YesNoCancel") {
		return MB_YESNOCANCEL;
	} if (val == "YesNo") {
		return MB_YESNO;
	} if (val == "RetryCancel") {
		return MB_RETRYCANCEL;
	}
	//default
	return 0;
}

uint32_t do_show_message_box_packet::get_icon_value(const std::string &val) {
	if (val == "None") {
		return 0;
	} if (val == "Hand") {
		return MB_ICONHAND;
	} if (val == "Stop") {
		return MB_ICONSTOP;
	} if (val == "Error") {
		return MB_ICONERROR;
	} if (val == "Question") {
		return MB_ICONQUESTION;
	} if (val == "Exclamation") {
		return MB_ICONEXCLAMATION;
	} if (val == "Warning") {
		return MB_ICONWARNING;
	} if (val == "Asterisk") {
		return MB_ICONASTERISK;
	} if (val == "Information") {
		return MB_ICONINFORMATION;
	}
	//default
	return 0;
}

/* ---------------------------------- */
/* SET_STATUS_PACKET				 */
/* ---------------------------------- */


set_status_packet::set_status_packet() :
	quasar_packet(PACKET_SET_STATUS) {	
}

set_status_packet::set_status_packet(string status) :
	quasar_packet(PACKET_SET_STATUS),
	m_status(status){
}

void set_status_packet::set_status(string value) {
	m_status = value;
}

vector<char> set_status_packet::serialize_packet() {
	begin_serialization();
	m_serializer.write_primitive(m_status);
	finalize_serialization();
	return m_serializer.get_serializer_data();
}

void set_status_packet::deserialize_packet(memstream &stream) {
}