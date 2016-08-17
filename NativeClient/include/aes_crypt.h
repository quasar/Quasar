#pragma once
#include <vector>
#include "cryptopp/base64.h"
#include <cryptopp/osrng.h>
#include <cryptopp/hmac.h>

class aes_crypt {
public:
	aes_crypt();
	aes_crypt(std::string b64key, std::string b64authKey);

	void set_key(std::string b64key);
	void set_key(std::vector<unsigned char> key);
	void set_auth_key(std::string b64authKey);
	void set_auth_key(std::vector<unsigned char> authKey);

	void encrypt(std::vector<unsigned char> &data);
	void decrypt(std::vector<unsigned char> &data);

static void pkcs7_pad(std::vector<unsigned char> &data);
	static void pkcs7_depad(std::vector<unsigned char> &data);

private:
	CryptoPP::AutoSeededRandomPool m_prng;
	unsigned char m_iv[CryptoPP::AES::BLOCKSIZE];
	unsigned char m_key[CryptoPP::AES::DEFAULT_KEYLENGTH];
	unsigned char m_auth_key[64];
	CryptoPP::Base64Decoder m_b64_decoder;

	unsigned char* decode_b64_data(const std::string data);
};
