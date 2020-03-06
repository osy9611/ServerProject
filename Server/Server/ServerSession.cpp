#include"ServerSession.h"
#include"GameServer.h"



ServerSession::ServerSession(int nSessionID, boost::asio::io_context& io_context, GameServer* pServer)
	:m_Socket(io_context), m_nSessionID(nSessionID), m_pServer(pServer)
{
	
}


ServerSession::~ServerSession()
{
	while (m_SendDataQueue.empty() == false)
	{
		delete[] m_SendDataQueue.front();
		m_SendDataQueue.pop_front();
	}
}

void ServerSession::Init()
{
	m_nPacketBufferMark = 0;
	State = ROBBY;
	CharactorType = 0;
}

/*
클라이언트에서 보낸 데이터를 받기 위해 ServerSession 클래스의 PostReceive() 멤버 함수를 호출한다. PostReceive()는 Asio의 async_read_some 함수를 사용하여
데이터 받기 완료 시 호출될 ServerSession 클래스의 handel_receive 멤버 함수를 등록한다. Asio가 handle_receive 함수를 호출하면 클라이언트가 보낸 데이터를 받아서
어떤 데이터를 보냈는지 분석하여 우리가 정의한 프로토콜 별로 클라이언트의 요청을 처리한다.
*/

void ServerSession::PostReceive()
{
	m_Socket.async_read_some
	(
		boost::asio::buffer(m_ReceiveBuffer),
		boost::bind(&ServerSession::handle_receive, this,
			boost::asio::placeholders::error,
			boost::asio::placeholders::bytes_transferred
		)
	);
}

/*
ServerSession 클래스의 PostSend()와 handle_write를 보면 PostSend()에서는 보낼 데이터를 저장하고 이전에 보내기 요청을 한 것이 완료되지 않았을 경우 곳바로 보내지
않고 보관한다. handle_write에서는 보낸 데이터를 제거하고 PostSend()에서 async_write에서는 보낸 데이터를 제거하고 PostSend()에서 async_write로 보내지 못한 것이 있으면
마저 보낸다. 비동기 보내기를 할 때 실수하기 쉬운 것 중 하나가 보내기 요청을 한 후 데이터를 바로 삭제하여 '보내기 실패'가 발생하는 경우다 반드시 완전히 다 보내기 전까지는
데이터를 보관하고 다 보낸 다음에 삭제해야 한다.
*/

void ServerSession::PostSend(const bool bImmediately, const int nSize, char* pData)
{
	char* pSendData = nullptr;

	if (bImmediately == false)
	{
		pSendData = new char[nSize];
		memcpy(pSendData, pData, nSize);
		m_SendDataQueue.push_back(pSendData);
	}
	else
	{
		pSendData = pData;
	}

	if (bImmediately == false && m_SendDataQueue.size() > 1)
	{
		return;
	}
	/*
	비동기는 요청이 끝날 때까지 대기하지 않는다. 즉, 데이터가 모두 다 보내겠다는 것은 비동기 완료 함수가 호출되었음을 의미한다.
	그러므로 async_write 호출 후 handle_write 함수가 호출될 때까지 보낸 데이터를 보관하지 않으면, 데이터를 일부만 보낼 수 있다.

	데이터 보내기를 한 후에 데이터 보내기가 완료되어 Session 클래스의 handle_write가 호출될 때까지는 보낸 데이터인 pSendData를
	잘 보존하고 있어야 한다. handle_write가 호출될 때까지는 데이터가 다 보내진게 아니기 때문이다.
	*/
	PacketMessage *packet = (PacketMessage*)pSendData;

	boost::asio::async_write(m_Socket, boost::asio::buffer(packet, packet->size),
		boost::bind(&ServerSession::handle_write, this,
			boost::asio::placeholders::error,
			boost::asio::placeholders::bytes_transferred)
	);

}

void ServerSession::handle_write(const boost::system::error_code &error, size_t byte_transferred)
{
	delete[] m_SendDataQueue.front();
	m_SendDataQueue.pop_front();

	if (m_SendDataQueue.empty() == false)
	{
		char* pData = m_SendDataQueue.front();

		PostSend(true, strlen(pData), pData);
	}
}

/*
hancle_receive에서는 먼저 에러가 발생했는지 조사한 다음, 에러가 발생한 경우 GameServer 클래스의 CloseSession 함수를 호출하여 '접속 종료 처리'를 한다.
에러가 없는 경우라면 받은 데이터에서 서버와 클라이언트 간에 정한 프로토콜에 맞춰 데이터를 호출한다.
*/
void ServerSession::handle_receive(const boost::system::error_code& error, size_t bytes_transferred)
{
	if (error)
	{
		if (error == boost::asio::error::eof)
		{
			std::cout << "클라이언트와 연결이 끊어졌습니다" << std::endl;
		}
		else
		{
			std::cout << "error No :" << error.value() << "error Message: " << error.message() << std::endl;
		}

		//에러가 발생하면 연결 종료
		m_pServer->CloseSession(m_nSessionID);
	}
	else
	{
		/*
		네트워크의 특성상 클라이언트에서 서버로 데이터를 보내기 요청을 동시에 여러 번 하면, 서버에서는 클라이언트에서 보내는 단위대로 받지 않는다. 한꺼번에 모두 받을 수도
		있고 여러 번 나눠서 받을 수 있다.(즉, 클라이언트는 write를 두 번 했는데, 서버에서는 receive가 한 번만 발생한다.) 이런, 경d를 처리하기 위해서 먼더 받은 데이터 m_PacketBuffer에
		저장한 후 클라이언트에서 동시에 여러번 요청하면서 서버는 한 번에 다 받으므로, 각 요청별로 나누어서 처리한다. 그리고 클라이언트가 보낸 데이터중 일부 도착한 경우, 우선은
		처리하지 않고 남겨놓았다가 다음에 받은 데이터와 연결하여 처리한다.
		*/

		//받은 데이터를 패킷 버퍼에 저장
		memcpy(&m_PacketBuffer[m_nPacketBufferMark], m_ReceiveBuffer.data(), bytes_transferred);

		int nPacketData = m_nPacketBufferMark + bytes_transferred;
		int nReadData = 0;

		while (nPacketData > 0)	//받은 데이터를 모두 처리할 때까지 반복
		{
			//남은 데이터가 패킷 헤더보다 작으면 중단 
			if (nPacketData < sizeof(PacketHeader))
			{
				break;
			}

			PacketHeader* pHeader = (PacketHeader *)&m_PacketBuffer[nReadData];
			//std::cout << nReadData << std::endl;
			if (pHeader->size <= nPacketData)	//처리할 수 있는 만큼 데이터가 있다면 패킷을 처리
			{
				m_pServer->ProcessPacket(m_nSessionID, &m_PacketBuffer[nReadData], pHeader->size);

				nPacketData -= pHeader->size;
				nReadData += pHeader->size;

			}
			else
			{
				break;	//패킷으로 처리할 수 없는 만큼이 아니면 중단
			}
		}

		if (nPacketData > 0)
		{
			char TempBuffer[MAX_RECEIVE_BUFFER_LEN * 2] = { 0, };
			memcpy(&TempBuffer[0], &m_PacketBuffer[nReadData], nPacketData);
			memcpy(&m_PacketBuffer[0], &TempBuffer[0], nPacketData);
		}

		//남은 데이터 양을 저장하고 데이터 받기 요청
		m_nPacketBufferMark = nPacketData;

		PostReceive();
	}
}