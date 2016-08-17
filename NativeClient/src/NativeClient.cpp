// NativeClient.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "quasar_client.h"
#include <cstdlib>
#include <iostream>
#include "serializer.h"
//#include "cryptopp/cryptlib.h"
//#include "cryptopp/modes.h"
//#include "cryptopp/aes.h"
#include <aes_crypt.h>

struct HelloWorld
{
	void operator()() const
	{
		std::cout << "Hello, World!" << std::endl;
	}
};

int main()
{
	system("pause");
#ifndef USE_BOOST
	quasar_client c;
	c.q_connect("localhost", "4782");
#else
	boost::asio::io_service io_srvc;

	HelloWorld h;

	quasar_client c(io_srvc);

	c.q_connect("localhost", "4782");
	io_srvc.run();
#endif


	for (;;) {}
	return 0;
}

