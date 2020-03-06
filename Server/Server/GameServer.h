#pragma once
#pragma comment(lib , "lib_json")

#include<iostream>
#include<vector>
#include<deque>
#include<string>
#include<tchar.h>
#include"ServerSession.h"
#include"RoomManager.h"
#include "Database.h"
#include "DBManager.h"
#include"Protocol.h"
#include<map>

class GameServer : public RoomManager
{
public:
	GameServer(boost::asio::io_context& io_context)
		:m_acceptor(io_context, boost::asio::ip::tcp::endpoint(boost::asio::ip::tcp::v4(), PORT_NUMBER))		
	{
		m_acceptor.set_option(boost::asio::ip::tcp::no_delay(true));
		m_bIsAccepting = false;
	}

	~GameServer()
	{
		for (size_t i = 0; i < m_SessionList.size(); ++i)
		{
			if (m_SessionList[i]->Socket().is_open())
			{
				m_SessionList[i]->Socket().close();
			}

			delete  m_SessionList[i];
		}
	}

	/*
	Session Ŭ������ �ִ� ���� ����ŭ �̸� �Ҵ��ؼ� m_SessionList�� ��� ���´�. ���ο� ������ ���� ������ ������� �ʴ� ServerSession�� �Ҵ��ϱ� ����
	���� ������� ���� ServerSession�� �ε��� ��ȣ�� m_SessionQueue�� ������ ���´�
	*/
	void Init(const int nMaxSesssionCount)
	{
		for (int i = 0; i < nMaxSesssionCount; ++i)
		{
			ServerSession *pSession = new ServerSession(i, (boost::asio::io_context&)m_acceptor.get_executor().context(), this);
			m_SessionList.push_back(pSession);
			m_SessionQueue.push_back(i);
		}
	}

	void Start()
	{
		dbManager.InitDB();
		std::cout << "���� ����....." << std::endl;
		PostAccept();
	}

	/*
	Ŭ���̾�Ʈ�� ������ ����� CloseSession() �Լ��� ����Ͽ� �ش� Session �ε��� ��ȣ�� �ٽ� m_SessionQueue�� �����ϰ�, ���� ���� �ޱ� ��û�� �ϰ� ���� �ʴٸ� ���� �ޱ� ��û�� �Ѵ�.
	*/
	void CloseSession(const int nSessionID)
	{
		std::cout << "Ŭ���̾�Ʈ ���� ����, ���� ID: " << nSessionID << std::endl;

		if (m_SessionList[nSessionID]->RoomName != "")
		{
			UserOutCheck(m_SessionList[nSessionID]->RoomName.c_str(), nSessionID, m_SessionList);		
		}
		m_SessionList[nSessionID]->Reset();
		m_SessionList[nSessionID]->Socket().close();

		m_SessionQueue.push_back(nSessionID);



		if (m_bIsAccepting == false)
		{
			PostAccept();
		}
	}

	void ProcessPacket(const int nSessionID, const char* pData, int nPacketData)
	{
		PacketMessage *packet = (PacketMessage*)pData;
		if (reader.parse(packet->dummy, Message))
		{
			try
			{
				//�÷��̾� ���� 
				if (Message["type"] == "PlayerInfo")
				{
					m_SessionList[nSessionID]->SetName(Message["ID"].asString().c_str());

					std::cout << "Ŭ���̾�Ʈ �α��� ���� Name : " << m_SessionList[nSessionID]->GetName() << std::endl;

					LoginCheck loginCheck;
					loginCheck.Init(Message["ID"].asString().c_str());
					m_SessionList[nSessionID]->PostSend(false, loginCheck.packet.size, (char *)&loginCheck.packet);
				}

				//ä��
				if (Message["type"] == "Chatting")
				{
					for (size_t i = 0; i < m_SessionList.size(); ++i)
					{
						if (m_SessionList[i]->Socket().is_open())
						{
							m_SessionList[i]->PostSend(false, packet->size, (char*)&packet);
						}
					}
				}

				//ģ��ã�� ����
				if (Message["type"] == "FindFriend")
				{
					for (size_t i = 0; i < m_SessionList.size(); ++i)
					{
						if (m_SessionList[i]->GetName() == Message["FriendName"].asString())
						{
							if (m_SessionList[i]->State == ROBBY)
							{
								ResultFriend result;
								result.Init(true, m_SessionList[nSessionID]->GetName());
								m_SessionList[i]->PostSend(false, result.packet.size, (char *)&result.packet);

								//�ʴ� ������ �����͸� �־ ���� �ʴ��߰� ���� �ʴ븦 �޾Ҵ��� ������ش�
								m_SessionList[i]->InviteNumber = nSessionID;
								m_SessionList[nSessionID]->InviteNumber = m_SessionList[i]->SessionID();

								std::cout << "ģ�� ���� " << m_SessionList[i]->SessionID() << std::endl;

							}
							else
							{
								ResultFriend result;
								result.Init(false, m_SessionList[i]->GetName());
								m_SessionList[nSessionID]->PostSend(false, result.packet.size, (char *)&result.packet);
								std::cout << "ģ�� �κ� ���� �ʴ� ����" << std::endl;
							}
							break;
						}
						else
						{
							if (i == m_SessionList.size() - 1)
							{
								ResultFriend result;
								result.Init(false, m_SessionList[i]->GetName());
								m_SessionList[nSessionID]->PostSend(false, result.packet.size, (char *)&result.packet);
								std::cout << "�ʴ� ����" << std::endl;
							}
						}
					}
				}

				//�ʴ����� ������
				if (Message["type"] == "InviteResult")
				{
					if (atoi(Message["Answer"].asString().c_str()) == 0)
					{
						std::cout << "�ʴ븦 �����Ͽ����ϴ�" << std::endl;
						m_SessionList[nSessionID]->State = NOT_ROBBY;

						//�ʴ��� ������ �濡 ���ִ��� �ƴ��� Ȯ���Ѵ�
						if (m_SessionList[m_SessionList[nSessionID]->InviteNumber]->RoomName == "")
						{
							m_SessionList[nSessionID]->RoomName = m_SessionList[m_SessionList[nSessionID]->InviteNumber]->GetName();
							m_SessionList[m_SessionList[nSessionID]->InviteNumber]->RoomName = m_SessionList[m_SessionList[nSessionID]->InviteNumber]->GetName();

							int InviteSessionID = m_SessionList[m_SessionList[nSessionID]->InviteNumber]->SessionID();
							MakeRoom(m_SessionList[nSessionID]->RoomName.c_str(), nSessionID, InviteSessionID);
						}
						else
						{
							m_SessionList[nSessionID]->RoomName = m_SessionList[m_SessionList[nSessionID]->InviteNumber]->RoomName;
							EneterRoom(m_SessionList[nSessionID]->RoomName.c_str(), nSessionID);
						}

						MyData ID[MAX_USER_COUNT];

						int RoomUserCount = GetRoomUserCount(m_SessionList[nSessionID]->RoomName.c_str());
						for (size_t i = 0; i < RoomUserCount; ++i)
						{
							int nSessionIDs = GetRoomUserSessionID(m_SessionList[nSessionID]->RoomName.c_str(), i);
							ID[i].SetData(m_SessionList[nSessionIDs]->GetName(), m_SessionList[nSessionIDs]->CharactorType);
						}

						UserData userdata;
						userdata.Init(ID, RoomUserCount);

						std::cout << userdata.str << std::endl;
						SendAllPlayer(m_SessionList[nSessionID]->RoomName.c_str(), userdata.packet);
					}
					else
					{
						m_SessionList[m_SessionList[nSessionID]->InviteNumber]->InviteNumber = 0;
						m_SessionList[nSessionID]->InviteNumber = 0;
						std::cout << "�ʴ븦 �����Ͽ����ϴ�" << std::endl;
					}
				}

				//ĳ���� Ÿ�� ����
				if (Message["type"] == "ChangeType")
				{
					m_SessionList[nSessionID]->CharactorType = Message["CharactorNum"].asInt();
					std::cout << m_SessionList[nSessionID]->GetName() << "�� ĳ���� Ÿ���� ����Ǿ����ϴ� : " << m_SessionList[nSessionID]->CharactorType << std::endl;

					ChangeType changeType;
					changeType.Init(m_SessionList[nSessionID]->CharactorType, m_SessionList[nSessionID]->GetName());
					
					if (m_SessionList[nSessionID]->RoomName != "")
					{
						SendAllPlayer(m_SessionList[nSessionID]->RoomName.c_str(), changeType.packet);
					}
				}

				//���� �޽���
				if (Message["type"] == "Ready")
				{
					if (m_SessionList[nSessionID]->RoomName != "")
					{
						RoomReady(m_SessionList[nSessionID]->RoomName.c_str(), nSessionID, m_SessionList);
					}
					else
					{
						//�ַ� �������� ����
					}
				}

				//Ű�Է��� �޾��� ��쿡 ����ϴ� �Լ�
				if (Message["type"] == "PlayerData")
				{
					CoverMessage coverMessage;
					coverMessage.Init(*packet);
					SendOtherPlayer(m_SessionList[nSessionID]->RoomName.c_str(), *packet, nSessionID);
				}

				//������ ���� ����
				if (Message["type"] == "ItemMix")
				{
					char result[50];
					dbManager.SearchItem("1", "1", "1", result);
					std::cout << "����� " << result << std::endl;
				}

			}
			catch (std::exception &ex)
			{
				std::cout << "���� �߻� : " << pData << std::endl;
			}
		}
		return;
	}
private:
	/*
	���� �ޱ� ��û�� �Ҷ����� PostAccept �Լ��� �̿��Ͽ� m_SessionQueue���� ������� �ʴ� ���� ��ȣ�� ������ async_accept�� ����Ѵ�
	*/

	void SendOtherPlayer(const char* RoomName, PacketMessage packet, int nSessionID)
	{
		for (size_t i = 0; i < Room[RoomName].Count; i++)
		{
			int nSessionIDs = GetRoomUserSessionID(RoomName, i);
			if (nSessionIDs != nSessionID)
			{
				m_SessionList[nSessionIDs]->PostSend(false, packet.size, (char *)&packet);
			}
		}
	}

	void SendAllPlayer(const char* RoomName, PacketMessage packet)
	{
		for (size_t i = 0; i < Room[RoomName].Count; i++)
		{
			int nSessionIDs = GetRoomUserSessionID(RoomName, i);
			m_SessionList[nSessionIDs]->PostSend(false, packet.size, (char *)&packet);
		}
	}

	bool PostAccept()
	{
		if (m_SessionQueue.empty())
		{
			m_bIsAccepting = false;
			return false;
		}

		m_bIsAccepting = true;
		int nSessionID = m_SessionQueue.front();

		m_SessionQueue.pop_front();

		m_acceptor.async_accept(m_SessionList[nSessionID]->Socket(),
			boost::bind(&GameServer::handle_accept,
				this,
				m_SessionList[nSessionID],
				boost::asio::placeholders::error)
		);

		return true;
	}

	void handle_accept(ServerSession* pSession, const boost::system::error_code& error)
	{
		if (!error)
		{
			std::cout << "Ŭ���̾�Ʈ ���� ����. SessionID" << pSession->SessionID() << std::endl;

			pSession->Init();
			pSession->PostReceive();

			PostAccept();
		}
		else
		{
			std::cout << "error No: " << error.value() << "error Message: " << error.message() << std::endl;
		}
	}


	int m_nSepNumber;

	bool m_bIsAccepting;

	boost::asio::ip::tcp::acceptor m_acceptor;

	std::vector<ServerSession*> m_SessionList;	//���� �迭
	std::deque<int> m_SessionQueue;				//���� �迭 ��ȣ

	Json::Reader reader;
	Json::Value Message;

	int RoomNumber;

	//DB �Ŵ���
	DBManager dbManager;
};
