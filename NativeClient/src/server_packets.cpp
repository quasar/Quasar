#include "stdafx.h"
#include "server_packets.h"

using namespace std;

get_authentication_packet::get_authentication_packet() :
	quasar_server_packet(PACKET_GET_AUTHENTICATION) {

}

void get_authentication_packet::deserialize_packet(memstream &stream) {

}

get_processes_packet::get_processes_packet() :
	quasar_server_packet(PACKET_GET_PROCESSES) {
}

void get_processes_packet::deserialize_packet(memstream &stream) {

}

do_show_message_box_packet::do_show_message_box_packet() :
	quasar_server_packet(PACKET_DO_SHOW_MESSAGEBOX) {
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
