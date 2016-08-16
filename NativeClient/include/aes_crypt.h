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
	void set_key(std::vector<byte> key);
	void set_auth_key(std::string b64authKey);
	void set_auth_key(std::vector<byte> authKey);

	void encrypt(std::vector<byte> &data);
	void decrypt(std::vector<byte> &data);

	static void pkcs7_pad(std::vector<byte> &data);
	static void pkcs7_depad(std::vector<byte> &data);

private:
	CryptoPP::AutoSeededRandomPool m_prng;
	byte m_iv[CryptoPP::AES::BLOCKSIZE];
	byte m_key[CryptoPP::AES::DEFAULT_KEYLENGTH];
	byte m_auth_key[64];
	CryptoPP::Base64Decoder m_b64_decoder;

	byte* decode_b64_data(const std::string data);
};
