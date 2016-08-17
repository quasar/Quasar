#include "stdafx.h"
#include "aes_crypt.h"
#include <cryptopp/modes.h>
#include <cryptopp/aes.h>
#include <cryptopp/randpool.h>
#include <cryptopp\osrng.h>

using namespace std;
using namespace CryptoPP;

aes_crypt::aes_crypt() {
	
}

aes_crypt::aes_crypt(std::string b64key, std::string b64authKey) : 
	m_prng(),
	m_key(),
	m_auth_key(),
	m_b64_decoder() {
	unsigned char *tmp = decode_b64_data(b64key);
	if (tmp != nullptr) {
		memcpy(&m_key[0], tmp, AES::DEFAULT_KEYLENGTH);
	}
	delete[] tmp;
	tmp = decode_b64_data(b64authKey);
	if (tmp != nullptr) {
		memcpy(&m_auth_key[0], tmp, 64);
	}
	delete[] tmp;
}

void aes_crypt::set_key(string b64key) {
	unsigned char *tmp = decode_b64_data(b64key);
	if (tmp != nullptr) {
		memcpy(&m_key[0], tmp, AES::DEFAULT_KEYLENGTH);
	}
	delete[] tmp;
}

void aes_crypt::set_key(vector<unsigned char> key) {
	if(key.size() >= sizeof m_key) {
		memcpy(&m_key[0], &key[0], sizeof m_key);
	}
}

void aes_crypt::set_auth_key(string b64authKey) {
	unsigned char *tmp = decode_b64_data(b64authKey);
	if (tmp != nullptr) {
		memcpy(&m_auth_key[0], tmp, 64);
	}
	delete[] tmp;
}

void aes_crypt::set_auth_key(std::vector<unsigned char> authKey) {
	if (authKey.size() >= sizeof m_auth_key) {
		memcpy(&m_auth_key[0], &authKey[0], sizeof m_auth_key);
	}
}

/* FORMAT
* ----------------------------------------
* |     HMAC     |   IV   |  CIPHERTEXT  |
* ----------------------------------------
*     32 bytes    16 bytes
*/

void aes_crypt::encrypt(vector<unsigned char> &data) {
	char testBlock1[16], testBlock2[64];
#ifdef WIN32
	ZeroMemory(testBlock1, 16);
	ZeroMemory(testBlock2, 64);
#else
	memset(testBlock1, 0, 16);
	memset(testBlock2, 0, 64);
#endif
	if(!memcmp(testBlock1, m_key, sizeof m_key)
		|| !memcmp(testBlock2, m_auth_key, sizeof m_auth_key)) {
		//TODO: error handling?
		return;
	}

	pkcs7_pad(data);
	vector<unsigned char> finalBuff(data.size() + sizeof m_key + 32);

	m_prng.GenerateBlock(m_iv, sizeof m_iv);
	memcpy(&finalBuff[SHA256::DIGESTSIZE], m_iv, sizeof m_iv);

	auto pData = reinterpret_cast<unsigned char*>(&data[0]);

	CBC_Mode<AES>::Encryption enc(&m_key[0], sizeof m_key, m_iv);
	enc.ProcessData(pData, pData, data.size());
	memcpy(&finalBuff[SHA256::DIGESTSIZE+AES::BLOCKSIZE], pData, data.size());

	unsigned char digest[SHA256::DIGESTSIZE];
	HMAC<SHA256> hmac(&m_auth_key[0], sizeof m_auth_key);
	hmac.CalculateDigest(digest, reinterpret_cast<unsigned char*>(&finalBuff[SHA256::DIGESTSIZE]), finalBuff.size()- SHA256::DIGESTSIZE);
	memcpy(&finalBuff[0], digest, SHA256::DIGESTSIZE);

	data.clear();
	data.resize(finalBuff.size());
	memcpy(&data[0], &finalBuff[0], finalBuff.size());
}

void aes_crypt::decrypt(vector<unsigned char> &data) {
	if(data.size() <= SHA256::DIGESTSIZE+AES::BLOCKSIZE) {
		return;
	}
	
	unsigned char receivedHash[SHA256::DIGESTSIZE];
	unsigned char calculatedHash[SHA256::DIGESTSIZE];

	memcpy(receivedHash, &data[0], sizeof receivedHash);

	HMAC<SHA256> hmac(&m_auth_key[0], sizeof m_auth_key);
	hmac.CalculateDigest(calculatedHash, &data[SHA256::DIGESTSIZE], data.size() - SHA256::DIGESTSIZE);

	if(memcmp(receivedHash, calculatedHash, SHA256::DIGESTSIZE)) {
		// hash mismatch
		return;
	}

	auto pData = reinterpret_cast<unsigned char*>(&data[0]);
	// includes padding
	uint32_t payloadSize = data.size() - (SHA256::DIGESTSIZE + AES::BLOCKSIZE);
	vector<unsigned char> decrypted(payloadSize);

	memcpy(m_iv, &data[SHA256::DIGESTSIZE], AES::BLOCKSIZE);
	CBC_Mode<AES>::Decryption dec(&m_key[0], sizeof m_key, m_iv);
	dec.ProcessData(&decrypted[0], pData + SHA256::DIGESTSIZE + AES::BLOCKSIZE, payloadSize);

	pkcs7_depad(decrypted);
	data.swap(decrypted);
}

unsigned char* aes_crypt::decode_b64_data(const string data) {
	// TODO: make real instance reusable...
	Base64Decoder decoder;
	decoder.Put(reinterpret_cast<const unsigned char*>(data.c_str()), data.size(), true);
	decoder.MessageEnd();

	auto size = decoder.MaxRetrievable();
	unsigned char *decoded = nullptr;

	if (size && size <= SIZE_MAX) {
		decoded = new unsigned char[size];
		decoder.Get(decoded, size);
	}

	return decoded;
}

void aes_crypt::pkcs7_pad(vector<unsigned char> &data) {
	int32_t padReq = AES::BLOCKSIZE - (data.size() % AES::BLOCKSIZE);
	for (int i = 0; i < padReq; i++) {
		data.push_back(static_cast<unsigned char>(padReq));
	}
}

void aes_crypt::pkcs7_depad(std::vector<unsigned char> &data) {
	unsigned char padSize = data.back();
	if(data.size() < padSize) {
		// invalid padding
		return;
	}
	data.erase(data.end() - padSize, data.end());
}
