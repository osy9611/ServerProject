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
		dbManager.InitDB();
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
				//플레이어 정보 
				if (Message["type"] == "PlayerInfo")
				{
					m_SessionList[nSessionID]->SetName(Message["ID"].asString().c_str());

					std::cout << "클라이언트 로그인 성공 Name : " << m_SessionList[nSessionID]->GetName() << std::endl;

					LoginCheck loginCheck;
					loginCheck.Init(Message["ID"].asString().c_str());
					m_SessionList[nSessionID]->PostSend(false, loginCheck.packet.size, (char *)&loginCheck.packet);
				}

				//채팅
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

				//친구찾기 관련
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
				}

				//초대결과를 보내줌
				if (Message["type"] == "InviteResult")
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
				}

				//캐릭터 타입 변경
				if (Message["type"] == "ChangeType")
				{
					m_SessionList[nSessionID]->CharactorType = Message["CharactorNum"].asInt();
					std::cout << m_SessionList[nSessionID]->GetName() << "의 캐릭터 타입이 변경되었습니다 : " << m_SessionList[nSessionID]->CharactorType << std::endl;

					ChangeType changeType;
					changeType.Init(m_SessionList[nSessionID]->CharactorType, m_SessionList[nSessionID]->GetName());
					
					if (m_SessionList[nSessionID]->RoomName != "")
					{
						SendAllPlayer(m_SessionList[nSessionID]->RoomName.c_str(), changeType.packet);
					}
				}

				//래디 메시지
				if (Message["type"] == "Ready")
				{
					if (m_SessionList[nSessionID]->RoomName != "")
					{
						RoomReady(m_SessionList[nSessionID]->RoomName.c_str(), nSessionID, m_SessionList);
					}
					else
					{
						//솔로 게임으로 시작
					}
				}

				//키입력을 받았을 경우에 사용하는 함수
				if (Message["type"] == "PlayerData")
				{
					CoverMessage coverMessage;
					coverMessage.Init(*packet);
					SendOtherPlayer(m_SessionList[nSessionID]->RoomName.c_str(), *packet, nSessionID);
				}

				//아이템 조합 관련
				if (Message["type"] == "ItemMix")
				{
					char result[50];
					dbManager.SearchItem("1", "1", "1", result);
					std::cout << "결과는 " << result << std::endl;
				}

			}
			catch (std::exception &ex)
			{
				std::cout << "에러 발생 : " << pData << std::endl;
			}
		}
		return;
	}
private:
	/*
	접속 받기 요청을 할때마다 PostAccept 함수를 이용하여 m_SessionQueue에서 사용하지 않는 세션 번호를 가져와 async_accept에 사용한다
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
	DBManager dbManager;
};
