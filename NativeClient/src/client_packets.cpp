#include "stdafx.h"
#include "client_packets.h"
#include <vector>
#include <helpers.h>
#include <win_helper.h>
#include "cryptopp/cryptlib.h"
#include "cryptopp/pwdbased.h"
#include "cryptopp/sha.h"

using namespace std;

set_status_packet::set_status_packet() :
	quasar_client_packet(PACKET_SET_STATUS) {
}

set_status_packet::set_status_packet(string status) :
	quasar_client_packet(PACKET_SET_STATUS),
	m_status(status) {
}

void set_status_packet::set_status(string value) {
	m_status = value;
}

vector<unsigned char> set_status_packet::serialize_packet() {
	begin_serialization();
	m_serializer.write_primitive(m_status);
	finalize_serialization();
	return m_serializer.get_serializer_data();
}

get_processes_response_packet::get_processes_response_packet() :
	quasar_client_packet(PACKET_GET_AUTHENTICATION_RESPONSE) {
}

vector<unsigned char> get_processes_response_packet::serialize_packet() {
	begin_serialization();
	finalize_serialization();
	return m_serializer.get_serializer_data();
}

get_authentication_response_packet::get_authentication_response_packet() :
	quasar_client_packet(PACKET_GET_AUTHENTICATION_RESPONSE) {
	initialize_values();
}

vector<unsigned char> get_authentication_response_packet::serialize_packet() {
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

	m_city = "secreat";
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