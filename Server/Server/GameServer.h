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
#endif // 몇행에서 메모리 누수가 나는지 알려줌.

/*
해쉬함수를 만들어서 문자열도 switch문을 사용할 수 있도록 만듬

constexpr C++11에 새로 추가된 키워드로 변수 또는 함수의 값을 컴파일 시점에 도출하여 상수화 시켜버리는 기능을 한다.
constexpr 함수는 제약 사항이 많아 일반 함수처럼 자유자재로 만들 수는 없지만 잘 활용하면 많은 것들을 컴파일 시점에
계산해낼 수 이는 완소 기능을 한다.
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
	Session 클래스를 최대 접속 수만큼 미리 할당해서 m_SessionList에 담아 놓는다. 새로운 접속이 있을 떄마다 사용하지 않는 ServerSession을 할당하기 위해
	아직 사용하지 않은 ServerSession의 인덱스 번호를 m_SessionQueue에 저장해 놓는다
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
		std::cout << "서버 시작....." << std::endl;
		PostAccept();
	}

	/*
	클라이언트의 접속이 끊기면 CloseSession() 함수를 사용하여 해당 Session 인덱스 번호를 다시 m_SessionQueue에 저장하고, 현제 접속 받기 요청을 하고 있지 않다면 접속 받기 요청을 한다.
	*/
	void CloseSession(const int nSessionID)
	{
		std::cout << "클라이언트 접속 종료, 세션 ID: " << nSessionID << std::endl;

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

					std::cout << "클라이언트 로그인 성공 Name : " << m_SessionList[nSessionID]->GetName() << std::endl;

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

								//초대 전에는 데이터를 넣어서 누가 초대했고 누가 초대를 받았는지 기록해준다
								m_SessionList[i]->InviteNumber = nSessionID;
								m_SessionList[nSessionID]->InviteNumber = m_SessionList[i]->SessionID();

								std::cout << "친구 성공 " << m_SessionList[i]->SessionID() << std::endl;

							}
							else
							{
								ResultFriend result;
								result.Init(false, m_SessionList[i]->GetName());
								m_SessionList[nSessionID]->PostSend(false, result.packet.size, (char *)&result.packet);
								std::cout << "친구 로비에 없음 초대 실패" << std::endl;
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
								std::cout << "초대 실패" << std::endl;
							}
						}
					}
					break;
				}
				case HashCode("InviteResult"):
				{
					if (atoi(Message["Answer"].asString().c_str()) == 0)
					{
						std::cout << "초대를 수락하였습니다" << std::endl;
						m_SessionList[nSessionID]->State = NOT_ROBBY;

						//초대한 유저가 방에 들어가있는지 아닌지 확인한다
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
						std::cout << "초대를 거절하였습니다" << std::endl;
					}
					break;
				}
				case HashCode("ChangeType"):
				{	m_SessionList[nSessionID]->CharactorType = Message["CharactorNum"].asInt();
				std::cout << m_SessionList[nSessionID]->GetName() << "의 캐릭터 타입이 변경되었습니다 : " << m_SessionList[nSessionID]->CharactorType << std::endl;

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
						//솔로 게임으로 시작
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
					std::cout << "해당되는 패킷이 없습니다" << std::endl;
					std::cout << packet->dummy << std::endl;
					break;
				}
		
			}
			catch (std::exception &ex)
			{
				std::cout << "에러 발생 : " << pData << std::endl;
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
	접속 받기 요청을 할때마다 PostAccept 함수를 이용하여 m_SessionQueue에서 사용하지 않는 세션 번호를 가져와 async_accept에 사용한다
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
			std::cout << "클라이언트 접속 성공. SessionID" << pSession->SessionID() << std::endl;

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


	std::vector<ServerSession*> m_SessionList;	//유저 배열
	std::deque<int> m_SessionQueue;				//유저 배열 번호

	Json::Reader reader;
	Json::Value Message;

	int RoomNumber;

	//DB 매니저
	DBManager *dbManager;
};
