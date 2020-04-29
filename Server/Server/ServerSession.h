#pragma once
#pragma comment(lib , "lib_json.lib")
#pragma warning(disable: 4996) 
#include<sdkddkver.h>
#include<deque>

#include<boost/bind.hpp>
#include<boost/asio.hpp>
#include<boost/date_time/posix_time/posix_time.hpp>
#include"json/include/json.h"
#include "Protocol.h"


class GameServer;

class ServerSession
{
public:
	ServerSession(int SessionID, boost::asio::io_context& io_context, GameServer* pServer);
	~ServerSession();

	int SessionID() { return m_nSessionID; }

	boost::asio::ip::tcp::socket &Socket() { 		
		return m_Socket; }

	void Init();

	void PostReceive();

	void PostSend(const bool bImmediately, const int nSize, char* pData);

	void SetName(const char* pszName) { m_Name = pszName; }
	const char* GetName() { return m_Name.c_str(); }


	void Reset() { InviteNumber = 0, RoomName = "", State = ROBBY, m_Name = ""; }

	//�ʴ� ���� ���̵� �޾ƿ�
	int InviteNumber;
	//�� �̸�
	std::string RoomName;
	//���� ������ ����
	SessionState State;

	//ĳ���� Ÿ���� �����س��� ���� 
	int CharactorType;
private:

	void handle_write(const boost::system::error_code& error, size_t bytes_transferred);
	void handle_receive(const boost::system::error_code& error, size_t bytes_transferred);

	int m_nSessionID;
	boost::asio::ip::tcp::socket m_Socket;
	
	
	std::array<char, MAX_RECEIVE_BUFFER_LEN> m_ReceiveBuffer;

	int m_nPacketBufferMark;
	char m_PacketBuffer[MAX_RECEIVE_BUFFER_LEN * 2];
	std::string PacketDatas = "";

	std::deque<char*>m_SendDataQueue;
	std::deque<char*>m_RecvDataQueue;
	std::string m_Name;

	GameServer* m_pServer;
};

