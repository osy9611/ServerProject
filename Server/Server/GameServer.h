#pragma once
#pragma comment(lib , "lib_json")

#include<iostream>
#include<vector>
#include<deque>
#include<string>
#include<tchar.h>
#include<random>
#include"ServerSession.h"
#include"RoomManager.h"
#include "Database.h"
#include "DBManager.h"
#include"BossManager.h"
#include"Protocol.h"
#include<map>
#include <crtdbg.h>

#if _DEBUG 
#define new new(_NORMAL_BLOCK, __FILE__, __LINE__)
#define malloc(s) _malloc_dbg(s, _NORMAL_BLOCK, __FILE__, __LINE__) 
#endif // ���࿡�� �޸� ������ ������ �˷���.

/*
�ؽ��Լ��� ���� ���ڿ��� switch���� ����� �� �ֵ��� ����

constexpr C++11�� ���� �߰��� Ű����� ���� �Ǵ� �Լ��� ���� ������ ������ �����Ͽ� ���ȭ ���ѹ����� ����� �Ѵ�.
constexpr �Լ��� ���� ������ ���� �Ϲ� �Լ�ó�� ��������� ���� ���� ������ �� Ȱ���ϸ� ���� �͵��� ������ ������
����س� �� �̴� �ϼ� ����� �Ѵ�.
*/
constexpr unsigned int HashCode(const char* str)
{
	return str[0] ? static_cast<unsigned int>(str[0]) + 0xEDB8832Full * HashCode(str + 1) : 8603;
}

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
		dbManager = DBManager::getInstance();
		dbManager->InitDB();		
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
				switch (HashCode(Message["type"].asString().c_str()))
				{
				case HashCode("PlayerInfo"):
				{
					m_SessionList[nSessionID]->SetName(Message["ID"].asString().c_str());

					std::cout << "Ŭ���̾�Ʈ �α��� ���� Name : " << m_SessionList[nSessionID]->GetName() << std::endl;

					LoginCheck loginCheck;
					loginCheck.Init(Message["ID"].asString().c_str());
					m_SessionList[nSessionID]->PostSend(false, loginCheck.packet.size, (char *)&loginCheck.packet);
					break;
				}
				case HashCode("SendMessage"):
				{
					if (m_SessionList[nSessionID]->RoomName != "")
					{
						SendOtherPlayer(m_SessionList[nSessionID]->RoomName.c_str(), *packet, nSessionID);
					}
					break;
				}
				case HashCode("FindFriend"):
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
					break;
				}
				case HashCode("InviteResult"):
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
					break;
				}
				case HashCode("ChangeType"):
				{	m_SessionList[nSessionID]->CharactorType = Message["CharactorNum"].asInt();
				std::cout << m_SessionList[nSessionID]->GetName() << "�� ĳ���� Ÿ���� ����Ǿ����ϴ� : " << m_SessionList[nSessionID]->CharactorType << std::endl;

				ChangeType changeType;
				changeType.Init(m_SessionList[nSessionID]->CharactorType, m_SessionList[nSessionID]->GetName());

				if (m_SessionList[nSessionID]->RoomName != "")
				{
					SendAllPlayer(m_SessionList[nSessionID]->RoomName.c_str(), changeType.packet);
				}
				break;
				}
				case HashCode("Ready"):
				{
					if (m_SessionList[nSessionID]->RoomName != "")
					{
						RoomReady(this,m_SessionList[nSessionID]->RoomName.c_str(), nSessionID, m_SessionList);
					}
					else
					{
						//�ַ� �������� ����
					}
					break;
				}
				case HashCode("PlayerData"):
				{
					SendOtherPlayer(m_SessionList[nSessionID]->RoomName.c_str(), *packet, nSessionID);

					break;
				}
				case HashCode("ItemMix"):
				{	ItemMixResult mixResult = dbManager->SetResultItem(Message);
					SendOnePlayer(mixResult.packet, nSessionID);
					
					break;
				}
				case HashCode("SendShareInvInfo"):
				{
					SharedInventory sharedInventory = SetInventory(Message["arrayNum"].asInt(),
						Message["itemNum"].asInt(),
						m_SessionList[nSessionID]->RoomName.c_str());
					SendAllPlayer(m_SessionList[nSessionID]->RoomName.c_str(), sharedInventory.packet);
					break;
				}
				case HashCode("SendShareSwapInfo"):
				{
					SharedInventory sharedInventory = SwapInventory(Message["arrayNum_1"].asInt(),
						Message["arrayNum_2"].asInt(),
						m_SessionList[nSessionID]->RoomName.c_str());
					SendAllPlayer(m_SessionList[nSessionID]->RoomName.c_str(), sharedInventory.packet);
					break;
				} 
				case HashCode("SendShareDeleteInfo"):
				{
					SharedInventory sharedInventory = DeleteInventory(Message["arrayNum"].asInt(),
						m_SessionList[nSessionID]->RoomName.c_str());

					std::cout << sharedInventory.str << std::endl;
					SendAllPlayer(m_SessionList[nSessionID]->RoomName.c_str(), sharedInventory.packet);
					break;
				} 
				case HashCode("BossDamage"):
				{
					BossResult bossResult = Room[m_SessionList[nSessionID]->RoomName].bossManager->HitBoss(Message["damage"].asFloat());

					std::cout << bossResult.str << std::endl;
					SendAllPlayer(m_SessionList[nSessionID]->RoomName.c_str(), bossResult.packet);
					break;
				} 
				case HashCode("PlayerDamage"):
				{
					SendOtherPlayer(m_SessionList[nSessionID]->RoomName.c_str(), *packet, nSessionID);
					break;
				}
				case HashCode("Phase"):
				{
					BossPhaseResult bossPhaseResult = Room[m_SessionList[nSessionID]->RoomName].bossManager->CalcPhase(Message);
				
					if (bossPhaseResult.PhaseCalc == true)
					{
						SendAllPlayer(m_SessionList[nSessionID]->RoomName.c_str(), bossPhaseResult.packet);
					}									
					break;
				}				
				case HashCode("PhaseEnd"):
				{
					if (Room[m_SessionList[nSessionID]->RoomName].bossManager->RestartCheck())
					{
						PhaseRestart phaseRestart = Room[m_SessionList[nSessionID]->RoomName].bossManager->PhaseSet();
						SendAllPlayer(m_SessionList[nSessionID]->RoomName.c_str(), phaseRestart.packet);
					}
					break;
				}
				case HashCode("PhaseTimeEnd"):
				{
					if (Room[m_SessionList[nSessionID]->RoomName].bossManager->RestartCheck())
					{
						BossPhaseResult bossPhaseResult;
						bossPhaseResult.TimerOn();
						SendAllPlayer(m_SessionList[nSessionID]->RoomName.c_str(), bossPhaseResult.packet);
					}
					break;
				}

				case HashCode("PhaseRestart"):
				{
					if (Room[m_SessionList[nSessionID]->RoomName].bossManager->RestartCheck())
					{
						PhaseRestart phaseRestart;
						phaseRestart.Init();
						SendAllPlayer(m_SessionList[nSessionID]->RoomName.c_str(), phaseRestart.packet);
					}
					break;
				}
				default:
					std::cout << "�ش�Ǵ� ��Ŷ�� �����ϴ�" << std::endl;
					std::cout << packet->dummy << std::endl;
					break;
				}
		
			}
			catch (std::exception &ex)
			{
				std::cout << "���� �߻� : " << pData << std::endl;
			}
		}
		return;
	}

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

	void SendOnePlayer(PacketMessage packet, int nSessionID)
	{
		m_SessionList[nSessionID]->PostSend(false, packet.size, (char *)&packet);
	}

	const char* SearchUserName(int nSessionID,const char* RoomName)
	{
		std::cout << m_SessionList[GetRoomUserSessionID(RoomName, nSessionID)]->GetName() << std::endl;
		return m_SessionList[GetRoomUserSessionID(RoomName, nSessionID)]->GetName();
	}

private:
	/*
	���� �ޱ� ��û�� �Ҷ����� PostAccept �Լ��� �̿��Ͽ� m_SessionQueue���� ������� �ʴ� ���� ��ȣ�� ������ async_accept�� ����Ѵ�
	*/
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
	DBManager *dbManager;
};
