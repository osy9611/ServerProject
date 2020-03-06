#include <SDKDDKVer.h>


#include "GameServer.h"

#define MAX_SESSION_COUNT 100



int main()
{
	boost::asio::io_context io_context;

	GameServer server(io_context);
	server.Init(MAX_SESSION_COUNT);
	server.Start();

	io_context.run();

	std::cout << "��Ʈ�� ���� ����" << std::endl;

	getchar();

	return 0;
}
