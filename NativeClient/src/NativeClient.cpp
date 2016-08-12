// NativeClient.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "quasar_client.h"
#include <cstdlib>
#include <iostream>

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
	boost::asio::io_service io_srvc;

	HelloWorld h;

	quasar_client c(io_srvc);
	c.msig_on_disconnected.connect(h);

	c.connect("127.0.0.1", "4782");
	io_srvc.run();

	for(;;){}
    return 0;
}

