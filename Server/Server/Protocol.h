#pragma once

#define PORT_NUMBER 8081
#define MAX_RECEIVE_BUFFER_LEN 1024
#define MSGSIZE     (MAX_RECEIVE_BUFFER_LEN-sizeof(int))  // 채팅 메시지 최대 길이
#define MAX_NAME_LEN 17
#define MAX_MESSAGE_LEN 129

#define MAX_USER_COUNT 4

#define MAX_INVENTORY 9

#define MAX_BOSS 2
#define SOURCEITEMPER 50

//현재 유저가 로비에 있는지 아니면 로비에 있지않고 대기방이다 인게임에 있는지 확인하는 함수
enum SessionState
{
	ROBBY,
	NOT_ROBBY
};

struct Inventory
{
	int Inventory;
	int ItemCount;
};

//각각의 캐릭터 타입과 ID를 클라이언트에 보내주기 위한 함수들
struct MyData
{
	std::string ID;
	int type;

	void SetData(const char * _ID, int _type)
	{
		ID = _ID;
		type = _type;
	}
};

class BossManager;

//방 데이터
struct RoomData 
{
	int Count = 0;
	int ReadyCount = 0;
	int nSessionIDs[MAX_USER_COUNT];

	Inventory _Inventory[MAX_INVENTORY] = { 0, };
	bool Ready[MAX_USER_COUNT];

	bool GameStart = false;
	BossManager *bossManager;

	void SetSessionId(int nSessionID)
	{
		nSessionIDs[Count] = nSessionID;
		Count++;
	}
};


struct PacketMessage
{
	int size;
	char dummy[MAX_RECEIVE_BUFFER_LEN * 2];
};

struct JsonData
{
	Json::Value root;
	Json::StyledWriter writer;
	std::string str = "";

	PacketMessage packet;
	int size;

	void SetJsonData()
	{
		str = writer.write(root);
		packet.size = str.length() + 4;
		strcpy(packet.dummy, str.c_str());
	}
};

struct PacketHeader
{
	int size;
};



struct LoginCheck : public JsonData
{
	void Init(const char* pData)
	{
		root["type"] = "LoginCheck";
		root["Message"] = pData;
		SetJsonData();
	}
};

struct ResultFriend : public JsonData
{
	void Init(bool result, const char* pData)
	{
		if (result)
		{
			root["type"] = "UI";
			root["UItype"] = "InviteMessage";
			root["Message"] = pData;
		}
		else
		{
			root["type"] = "UI";
			root["UItype"] = "NoUserMessage";
		}
		SetJsonData();
	}
};

struct UserData : public JsonData
{
	Json::Value Data;
	void Init(MyData *SessionID, int len)
	{
		root["type"] = "UserInfo";
		root["Count"] = len;
		for (size_t i = 0; i < len; ++i)
		{
			Json::Value Charactor;
			Charactor["ID"] = SessionID[i].ID;
			Charactor["Type"] = SessionID[i].type;
			Data.append(Charactor);
		}

		root["UserList"] = Data;
		SetJsonData();
	}
};

struct UserOut : public JsonData
{
	void Init(const char* pData)
	{
		root["type"] = "UserOut";
		root["Name"] = pData;
		SetJsonData();
	}
};

struct ChangeType : public JsonData
{
	void Init(int type, const char *pData)
	{
		root["type"] = "ChangeType";
		root["Name"] = pData;
		root["CharactorNum"] = type;
		SetJsonData();
	}
};

struct Ready : public JsonData
{
	Json::Value Data;
	void Init(const char *pData, bool Start, MyData *mData, int mCnt)
	{
		root["type"] = "Ready";
		root["Name"] = pData;
		if (!Start)
		{
			root["Start"] = false;
		}
		else
		{
			root["Start"] = true;
			for (size_t i = 0; i < mCnt; ++i)
			{
				Json::Value Session;
				Session["SessionID"] = mData[i].ID;
				Data.append(Session);
			}
			root["SessionIDList"] = Data;
		}
		SetJsonData();
	}
};

struct CoverMessage : public JsonData
{
	void Init(PacketMessage _packet)
	{
		packet.size = _packet.size;
		strcpy(packet.dummy, _packet.dummy);
	}
};

struct ItemMixResult : public JsonData
{
	Json::Value Data;

	void Init(const char* resultItem,int resultMoney)
	{
		root["type"] = "ItemMixResult";
		root["money"] = resultMoney;
		if (strcmp(resultItem,"(null)") == 1)
		{
			root["result"] = "true";
			root["Item"] = resultItem;
		}
		else
		{
			root["result"] = "false";
		}
		
		SetJsonData();
	}
};

struct TotalItem : public JsonData
{
	Json::Value Data;

	void Init(int source[], int size)
	{
		for (int i = 0; i < size; ++i)
		{
			root["source"].append(source[i]);
		}
		root["type"] = "TotalItems";

		SetJsonData();
	}
};

struct SharedInventory :public JsonData
{
	void Init(Inventory Inventory[])
	{
		for (size_t i = 0; i < MAX_INVENTORY; ++i)
		{
			root["Inventory"].append(Inventory[i].Inventory);
			root["ItemCount"].append(Inventory[i].ItemCount);
		}
		root["type"] = "SendShareInvInfo";

		SetJsonData();
	}	
};


/*보스 데이터 및 보스 패킷 관련된 구조체들*/
struct BossData
{
	int Item[3] = { 0, };
	int ItemPer[3] = { 0, };
	int Money = 0;
	int Phase[4] = { 0, };
	int Hp = 0;
};


struct BossResult :public JsonData
{
	void Init(int Item[], int ItemCount, int Money)
	{
		root["type"] = "BossHp";
		root["Hp"] = 0;
		for (size_t i = 0; i < ItemCount; ++i)
		{
			root["items"].append(Item[i]);
		}
		root["money"] = Money;

		SetJsonData();
	}

	void Init(int Hp)
	{
		root["type"] = "BossHp";
		root["Hp"] = Hp;
		SetJsonData();
	}

	void Init(int Hp,int Phase)
	{
		root["type"] = "BossHp";
		root["Hp"] = Hp;
		root["Phase"] = Phase;
		SetJsonData();
	}
};

struct BossPhaseResult : public JsonData
{
	bool PhaseCalc;

	void DirInit(float x,float y)
	{
		root["type"] = "BulletDir";
		root["x"] = (double)x;
		root["y"] = (double)y;
		SetJsonData();
	}

	void RandomLaser(int arr)
	{
		root["type"] = "RandomLaser";
		root["laserDir"] = arr;
		SetJsonData();
	}

	void RandomCircleBullet(int num)
	{
		root["type"] = "CircleBulletType";
		root["bulletType"] = num;
		SetJsonData();
	}

	void CircleFloor(const char *_name,int num)
	{
		root["type"] = "CircleFloor";
		root["targetName"] = _name;
		root["bulletType"] = num;
		SetJsonData();
	}

	void FirBall(int milltime, int num)
	{
		root["type"] = "FireBall";
		root["millTime"] = milltime;
		root["bulletType"] = num;
		SetJsonData();
	}

	void Restriction(const char * _name, int num)
	{
		root["type"] = "Restriction";
		root["targetName"] = _name;
		SetJsonData();
	}

	void TimerOn()
	{
		root["type"] = "TimerOn";
		SetJsonData();
	}
};

struct PhaseRestart : public JsonData
{
	void Init(int _phase)
	{
		root["type"] = "PhaseStart";
		root["Phase"] = _phase;
		SetJsonData();
	}

	void Init()
	{
		root["type"] = "PhaseRestart";
		SetJsonData();
	}
};

struct SyncPosition :public JsonData
{
	void Init()
	{
		root["type"] = "SyncPosition";
		SetJsonData();
	}
};

struct ItmePerResult : public JsonData
{
	void Init(bool result,const char * UserName)
	{
		root["type"] = "ItemPerResult";
		root["result"] = result;
		root["name"] = UserName;
		SetJsonData();
	}
};