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
Ŭ���̾�Ʈ���� ���� �����͸� �ޱ� ���� ServerSession Ŭ������ PostReceive() ��� �Լ��� ȣ���Ѵ�. PostReceive()�� Asio�� async_read_some �Լ��� ����Ͽ�
������ �ޱ� �Ϸ� �� ȣ��� ServerSession Ŭ������ handel_receive ��� �Լ��� ����Ѵ�. Asio�� handle_receive �Լ��� ȣ���ϸ� Ŭ���̾�Ʈ�� ���� �����͸� �޾Ƽ�
� �����͸� ���´��� �м��Ͽ� �츮�� ������ �������� ���� Ŭ���̾�Ʈ�� ��û�� ó���Ѵ�.
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
ServerSession Ŭ������ PostSend()�� handle_write�� ���� PostSend()������ ���� �����͸� �����ϰ� ������ ������ ��û�� �� ���� �Ϸ���� �ʾ��� ��� ���ٷ� ������
�ʰ� �����Ѵ�. handle_write������ ���� �����͸� �����ϰ� PostSend()���� async_write������ ���� �����͸� �����ϰ� PostSend()���� async_write�� ������ ���� ���� ������
���� ������. �񵿱� �����⸦ �� �� �Ǽ��ϱ� ���� �� �� �ϳ��� ������ ��û�� �� �� �����͸� �ٷ� �����Ͽ� '������ ����'�� �߻��ϴ� ���� �ݵ�� ������ �� ������ ��������
�����͸� �����ϰ� �� ���� ������ �����ؾ� �Ѵ�.
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
	�񵿱�� ��û�� ���� ������ ������� �ʴ´�. ��, �����Ͱ� ��� �� �����ڴٴ� ���� �񵿱� �Ϸ� �Լ��� ȣ��Ǿ����� �ǹ��Ѵ�.
	�׷��Ƿ� async_write ȣ�� �� handle_write �Լ��� ȣ��� ������ ���� �����͸� �������� ������, �����͸� �Ϻθ� ���� �� �ִ�.

	������ �����⸦ �� �Ŀ� ������ �����Ⱑ �Ϸ�Ǿ� Session Ŭ������ handle_write�� ȣ��� �������� ���� �������� pSendData��
	�� �����ϰ� �־�� �Ѵ�. handle_write�� ȣ��� �������� �����Ͱ� �� �������� �ƴϱ� �����̴�.
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
hancle_receive������ ���� ������ �߻��ߴ��� ������ ����, ������ �߻��� ��� GameServer Ŭ������ CloseSession �Լ��� ȣ���Ͽ� '���� ���� ó��'�� �Ѵ�.
������ ���� ����� ���� �����Ϳ��� ������ Ŭ���̾�Ʈ ���� ���� �������ݿ� ���� �����͸� ȣ���Ѵ�.
*/
void ServerSession::handle_receive(const boost::system::error_code& error, size_t bytes_transferred)
{
	if (error)
	{
		if (error == boost::asio::error::eof)
		{
			std::cout << "Ŭ���̾�Ʈ�� ������ ���������ϴ�" << std::endl;
		}
		else
		{
			std::cout << "error No :" << error.value() << "error Message: " << error.message() << std::endl;
		}

		//������ �߻��ϸ� ���� ����
		m_pServer->CloseSession(m_nSessionID);
	}
	else
	{
		/*
		��Ʈ��ũ�� Ư���� Ŭ���̾�Ʈ���� ������ �����͸� ������ ��û�� ���ÿ� ���� �� �ϸ�, ���������� Ŭ���̾�Ʈ���� ������ ������� ���� �ʴ´�. �Ѳ����� ��� ���� ����
		�ְ� ���� �� ������ ���� �� �ִ�.(��, Ŭ���̾�Ʈ�� write�� �� �� �ߴµ�, ���������� receive�� �� ���� �߻��Ѵ�.) �̷�, ��d�� ó���ϱ� ���ؼ� �մ� ���� ������ m_PacketBuffer��
		������ �� Ŭ���̾�Ʈ���� ���ÿ� ������ ��û�ϸ鼭 ������ �� ���� �� �����Ƿ�, �� ��û���� ����� ó���Ѵ�. �׸��� Ŭ���̾�Ʈ�� ���� �������� �Ϻ� ������ ���, �켱��
		ó������ �ʰ� ���ܳ��Ҵٰ� ������ ���� �����Ϳ� �����Ͽ� ó���Ѵ�.
		*/

		//���� �����͸� ��Ŷ ���ۿ� ����
		memcpy(&m_PacketBuffer[m_nPacketBufferMark], m_ReceiveBuffer.data(), bytes_transferred);

		int nPacketData = m_nPacketBufferMark + bytes_transferred;
		int nReadData = 0;

		while (nPacketData > 0)	//���� �����͸� ��� ó���� ������ �ݺ�
		{
			//���� �����Ͱ� ��Ŷ ������� ������ �ߴ� 
			if (nPacketData < sizeof(PacketHeader))
			{
				break;
			}

			PacketHeader* pHeader = (PacketHeader *)&m_PacketBuffer[nReadData];
			//std::cout << nReadData << std::endl;
			if (pHeader->size <= nPacketData)	//ó���� �� �ִ� ��ŭ �����Ͱ� �ִٸ� ��Ŷ�� ó��
			{
				m_pServer->ProcessPacket(m_nSessionID, &m_PacketBuffer[nReadData], pHeader->size);

				nPacketData -= pHeader->size;
				nReadData += pHeader->size;

			}
			else
			{
				break;	//��Ŷ���� ó���� �� ���� ��ŭ�� �ƴϸ� �ߴ�
			}
		}

		if (nPacketData > 0)
		{
			char TempBuffer[MAX_RECEIVE_BUFFER_LEN * 2] = { 0, };
			memcpy(&TempBuffer[0], &m_PacketBuffer[nReadData], nPacketData);
			memcpy(&m_PacketBuffer[0], &TempBuffer[0], nPacketData);
		}

		//���� ������ ���� �����ϰ� ������ �ޱ� ��û
		m_nPacketBufferMark = nPacketData;

		PostReceive();
	}
}