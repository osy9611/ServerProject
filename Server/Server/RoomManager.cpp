//#include "RoomManager.h"
#include "GameServer.h"
RoomManager::RoomManager()
{

}


RoomManager::~RoomManager()
{
}

int RoomManager::BossSet()
{
	//random 함수 셋팅
	std::random_device randDevice;	//렌덤 디바이스 생성
	std::mt19937 mt(randDevice());
	std::uniform_int_distribution<int> distribution(0, MAX_BOSS);
	
	return distribution(mt);
}

float RoomManager::CharactorHpCheck(float Damage)
{
	return Damage;
}

float RoomManager::BoostHpCheck(float Damage)
{
	return Damage;
}

//방을 만듬
void RoomManager::MakeRoom(const char* RoomName, int SessionID, int SessionID2)
{
	RoomData roomData;
	roomData.SetSessionId(SessionID);
	roomData.SetSessionId(SessionID2);
	RoomNames.push_back(RoomName);
	Room.insert(std::pair<std::string, RoomData>(RoomName, roomData));
	
}

//방이 들어옴
void RoomManager::EneterRoom(const char *RoomName, const int nSessionID)
{
	Room[RoomName].nSessionIDs[Room[RoomName].Count] = nSessionID;
	Room[RoomName].Count++;
}

//플레이어가 래디를 했을 경우에 호출됨
void RoomManager::RoomReady(GameServer* pServer,const char* RoomName, const int nSessionID, std::vector<ServerSession*> m_SessionList)
{
	for (size_t i = 0; i < Room[RoomName].Count; ++i)
	{
		if (Room[RoomName].nSessionIDs[i] == nSessionID)
		{
			if (Room[RoomName].Ready[i] == true)
			{
				Room[RoomName].Ready[i] = false;
				Room[RoomName].ReadyCount--;
			}
			else
			{
				Room[RoomName].Ready[i] = true;
				Room[RoomName].ReadyCount++;
			}

			Ready ready;

			if (Room[RoomName].Count == Room[RoomName].ReadyCount)
			{
				// 해당 클라이언트에 보내줄 Json 문자열을 만듬
				MyData ID[MAX_USER_COUNT];

				for (size_t i = 0; i < Room[RoomName].Count; ++i)
				{
					ID[i].ID = m_SessionList[Room[RoomName].nSessionIDs[i]]->GetName();
					ID[i].type = m_SessionList[Room[RoomName].nSessionIDs[i]]->CharactorType;
				}

				ready.Init(m_SessionList[nSessionID]->GetName(), true, ID, Room[RoomName].Count);

				Room[RoomName].bossManager = new BossManager(pServer, RoomName);
				Room[RoomName].bossManager->SetBossData(1);
				Room[RoomName].bossManager->UserSet(Room[RoomName].Count);

				Room[RoomName].GameStart = true;
				std::cout << ready.str << std::endl;
			}
			else
			{
				ready.Init(m_SessionList[nSessionID]->GetName(), false, NULL, 0);
			}

			for (size_t j = 0; j < Room[m_SessionList[nSessionID]->RoomName].Count; ++j)
			{
				m_SessionList[Room[RoomName].nSessionIDs[j]]->PostSend(false, ready.packet.size, (char *)&ready.packet);
			}
			break;
		}
	}
}



//유저가 나감을 체크
void RoomManager::UserOutCheck(const char* RoomName, const int nSessionID, std::vector<ServerSession*> m_SessionList)
{
	if (Room[RoomName].Count != 1)
	{
		for (size_t i = 0; i < Room[RoomName].Count; ++i)
		{
			if (m_SessionList[Room[RoomName].nSessionIDs[i]] == m_SessionList[nSessionID])
			{
				if (i != Room[m_SessionList[nSessionID]->RoomName].Count - 1)
				{
					Room[m_SessionList[nSessionID]->RoomName].nSessionIDs[i] = Room[m_SessionList[nSessionID]->RoomName].nSessionIDs[Room[m_SessionList[nSessionID]->RoomName].Count - 1];
					break;
				}
			}
		}

		Room[RoomName].Count--;
		if (Room[RoomName].GameStart)
		{
			Room[RoomName].bossManager->UserSet(Room[RoomName].Count);
		}
	
		UserOut userOut;
		userOut.Init(m_SessionList[nSessionID]->GetName());

		std::cout << "현재 남아있는 유저인원" << Room[m_SessionList[nSessionID]->RoomName].Count << std::endl;
		for (size_t i = 0; i < Room[m_SessionList[nSessionID]->RoomName].Count; ++i)
		{
			std::cout << "현재 남아있는 유저들" << m_SessionList[Room[m_SessionList[nSessionID]->RoomName].nSessionIDs[i]]->GetName() << std::endl;
			m_SessionList[Room[m_SessionList[nSessionID]->RoomName].nSessionIDs[i]]->PostSend(false, userOut.packet.size, (char *)&userOut.packet);
		}

	}
	else
	{
		//Room[m_SessionList[nSessionID]->RoomName].bossManager->ResetDB();
		//delete Room[m_SessionList[nSessionID]->RoomName].bossManager;
		Room.erase(m_SessionList[nSessionID]->RoomName);

		for (size_t i = 0; i < RoomNames.size(); ++i)
		{
			if (RoomNames[i] == m_SessionList[nSessionID]->RoomName)
			{
				RoomNames.erase(RoomNames.begin() + i);
				break;
			}
		}
		std::cout << "방에 유저가 없어서 삭제되었습니다" << std::endl;
	}
}

int RoomManager::GetRoomUserSessionID(const char * RoomName,int Num)
{
	return Room[RoomName].nSessionIDs[Num];
}

int RoomManager::GetRoomUserCount(const char* RoomName)
{
	return Room[RoomName].Count;
}

RoomData RoomManager::GetRoomData(const char* RoomName)
{
	return Room[RoomName];
}

SharedInventory RoomManager::SetInventory(int arrayNum, int itemNumber,const char* RoomName)
{
	for (int i = 0; i < MAX_INVENTORY; ++i)
	{
		if (Room[RoomName]._Inventory[i].Inventory == itemNumber )
		{
			if (itemNumber >= 201)
			{
				Room[RoomName]._Inventory[arrayNum].Inventory = itemNumber;
				Room[RoomName]._Inventory[arrayNum].ItemCount = 0;
				Room[RoomName]._Inventory[arrayNum].ItemCount++;				
				break;
			}
			else
			{
				Room[RoomName]._Inventory[i].ItemCount++;
				break;
			}
			
		}
		if (Room[RoomName]._Inventory[i].Inventory != itemNumber && i == MAX_INVENTORY-1)
		{
			Room[RoomName]._Inventory[arrayNum].Inventory = itemNumber;
			Room[RoomName]._Inventory[arrayNum].ItemCount = 0;
			Room[RoomName]._Inventory[arrayNum].ItemCount++;
		}
	}
	SharedInventory Data;
	Data.Init(Room[RoomName]._Inventory);

	std::cout << Room[RoomName]._Inventory[arrayNum].Inventory << std::endl;

	return Data;
}

SharedInventory RoomManager::SwapInventory(int arrayNum1, int arrayNum2, const char *RoomName)
{
	int SwapID,SwapCount;

	SwapID = Room[RoomName]._Inventory[arrayNum1].Inventory;
	Room[RoomName]._Inventory[arrayNum1].Inventory = Room[RoomName]._Inventory[arrayNum2].Inventory;
	Room[RoomName]._Inventory[arrayNum2].Inventory = SwapID;

	SwapCount = Room[RoomName]._Inventory[arrayNum1].ItemCount;
	Room[RoomName]._Inventory[arrayNum1].ItemCount = Room[RoomName]._Inventory[arrayNum2].ItemCount;
	Room[RoomName]._Inventory[arrayNum2].ItemCount = SwapCount;

	SharedInventory Data;
	Data.Init(Room[RoomName]._Inventory);
	return Data;
}

SharedInventory RoomManager::DeleteInventory(int arrayNum1, const char *RoomName)
{
	if (Room[RoomName]._Inventory[arrayNum1].ItemCount == 1)
	{
		Room[RoomName]._Inventory[arrayNum1].Inventory = 0;
		Room[RoomName]._Inventory[arrayNum1].ItemCount = 0;
	}
	else if (Room[RoomName]._Inventory[arrayNum1].ItemCount > 1)
	{
		Room[RoomName]._Inventory[arrayNum1].ItemCount--;
	}
	

	SharedInventory Data;
	Data.Init(Room[RoomName]._Inventory);
	return Data;
}

//아이템 확률 관련
ItmePerResult RoomManager::PercentageCheck(int Per,const char* UserName)
{
	ItmePerResult result;

	std::random_device randDevice;	//랜덤 디바이스 생성
	std::mt19937  mt(randDevice());
	std::uniform_int_distribution<int> distribution(0, 100);
	int perResult = distribution(randDevice);
	if (perResult <= SOURCEITEMPER)
	{
		result.Init(true,UserName);
	}
	else
	{
		result.Init(false,UserName);
	}

	return result;
}