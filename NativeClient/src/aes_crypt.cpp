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
	m_b64_decoder(),
	m_prng(),
	m_key(),
	m_auth_key() {
	byte *tmp = decode_b64_data(b64key);
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
	byte *tmp = decode_b64_data(b64key);
	if (tmp != nullptr) {
		memcpy(&m_key[0], tmp, AES::DEFAULT_KEYLENGTH);
	}
	delete[] tmp;
}

void aes_crypt::set_key(vector<byte> key) {
	if(key.size() >= sizeof m_key) {
		memcpy(&m_key[0], &key[0], sizeof m_key);
	}
}

void aes_crypt::set_auth_key(string b64authKey) {
	byte *tmp = decode_b64_data(b64authKey);
	if (tmp != nullptr) {
		memcpy(&m_auth_key[0], tmp, 64);
	}
	delete[] tmp;
}

void aes_crypt::set_auth_key(std::vector<byte> authKey) {
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

void aes_crypt::encrypt(vector<byte> &data) {
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
	vector<byte> finalBuff(data.size() + sizeof m_key + 32);

	m_prng.GenerateBlock(m_iv, sizeof m_iv);
	memcpy(&finalBuff[SHA256::DIGESTSIZE], m_iv, sizeof m_iv);

	auto pData = reinterpret_cast<byte*>(&data[0]);

	CBC_Mode<AES>::Encryption enc(&m_key[0], sizeof m_key, m_iv);
	enc.ProcessData(pData, pData, data.size());
	memcpy(&finalBuff[SHA256::DIGESTSIZE+AES::BLOCKSIZE], pData, data.size());

	byte digest[SHA256::DIGESTSIZE];
	HMAC<SHA256> hmac(&m_auth_key[0], sizeof m_auth_key);
	hmac.CalculateDigest(digest, reinterpret_cast<byte*>(&finalBuff[SHA256::DIGESTSIZE]), finalBuff.size()- SHA256::DIGESTSIZE);
	memcpy(&finalBuff[0], digest, SHA256::DIGESTSIZE);

	data.clear();
	data.resize(finalBuff.size());
	memcpy(&data[0], &finalBuff[0], finalBuff.size());
}

void aes_crypt::decrypt(vector<byte> &data) {
	if(data.size() <= SHA256::DIGESTSIZE+AES::BLOCKSIZE) {
		return;
	}
	
	byte receivedHash[SHA256::DIGESTSIZE];
	byte calculatedHash[SHA256::DIGESTSIZE];

	memcpy(receivedHash, &data[0], sizeof receivedHash);

	HMAC<SHA256> hmac(&m_auth_key[0], sizeof m_auth_key);
	hmac.CalculateDigest(calculatedHash, &data[SHA256::DIGESTSIZE], data.size() - SHA256::DIGESTSIZE);

	if(memcmp(receivedHash, calculatedHash, SHA256::DIGESTSIZE)) {
		// hash mismatch
		return;
	}

	auto pData = reinterpret_cast<byte*>(&data[0]);
	// includes padding
	uint32_t payloadSize = data.size() - (SHA256::DIGESTSIZE + AES::BLOCKSIZE);
	vector<byte> decrypted(payloadSize);

	memcpy(m_iv, &data[SHA256::DIGESTSIZE], AES::BLOCKSIZE);
	CBC_Mode<AES>::Decryption dec(&m_key[0], sizeof m_key, m_iv);
	dec.ProcessData(&decrypted[0], pData + SHA256::DIGESTSIZE + AES::BLOCKSIZE, payloadSize);
	data.swap(decrypted);
}

byte* aes_crypt::decode_b64_data(const string data) {
	// TODO: make real instance reusable...
	Base64Decoder decoder;
	decoder.Put(reinterpret_cast<const byte*>(data.c_str()), data.size(), true);
	decoder.MessageEnd();

	auto size = decoder.MaxRetrievable();
	byte *decoded = nullptr;

	if (size && size <= SIZE_MAX) {
		decoded = new byte[size];
		decoder.Get(decoded, size);
	}

	return decoded;
}

void aes_crypt::pkcs7_pad(vector<byte> &data) {
	int32_t padReq = AES::BLOCKSIZE - (data.size() % AES::BLOCKSIZE);
	while(data.size()%AES::BLOCKSIZE) {
		data.push_back(static_cast<byte>(padReq));
	}
	// If data is a multiple of blocksize we need to add an entire pad block
	if(padReq == AES::BLOCKSIZE) {
		for(int i = 0;i < AES::BLOCKSIZE;i++) {
			data.push_back(0x10 /* 16 pad bytes */);
		}
	}
}

void aes_crypt::pkcs7_depad(std::vector<byte> &data) {
	byte padSize = data.back();
	if(data.size() < padSize) {
		// invalid padding
		return;
	}
	for(int i = 0;i < padSize;i++) {
		data.pop_back();
	}
}
