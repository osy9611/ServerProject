#pragma once

#define PORT_NUMBER 8081
#define MAX_RECEIVE_BUFFER_LEN 1024
#define MSGSIZE     (MAX_RECEIVE_BUFFER_LEN-sizeof(int))  // 채팅 메시지 최대 길이
#define MAX_NAME_LEN 17
#define MAX_MESSAGE_LEN 129

#define MAX_USER_COUNT 4

//현재 유저가 로비에 있는지 아니면 로비에 있지않고 대기방이다 인게임에 있는지 확인하는 함수
enum SessionState
{
	ROBBY,
	NOT_ROBBY
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

struct RoomData
{
	int Count = 0;
	int ReadyCount = 0;
	int nSessionIDs[MAX_USER_COUNT];
	int Source[3] = { 0,0,0 };
	bool Ready[MAX_USER_COUNT];

	void SetSessionId(int nSessionID)
	{
		nSessionIDs[Count] = nSessionID;
		Count++;
	}
};

struct JsonData
{
	Json::Value root;
	Json::StyledWriter writer;
	std::string str = "";
	int size;
};

struct PacketHeader
{
	int size;
};

struct PacketMessage
{
	int size;
	char dummy[MAX_RECEIVE_BUFFER_LEN * 2];
};

struct LoginCheck : public JsonData
{
	PacketMessage packet;
	void Init(const char* pData)
	{
		root["type"] = "LoginCheck";
		root["Message"] = pData;
		str = writer.write(root);
		packet.size = str.length() + 4;
		strcpy(packet.dummy, str.c_str());
	}
};

struct ResultFriend : public JsonData
{
	PacketMessage packet;
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
		str = writer.write(root);
		packet.size = str.length() + 4;
		strcpy(packet.dummy, str.c_str());
	}
};

struct UserData : public JsonData
{
	PacketMessage packet;
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
		str = writer.write(root);
		packet.size = str.length() + 4;
		strcpy(packet.dummy, str.c_str());
	}
};

struct UserOut : public JsonData
{
	PacketMessage packet;
	void Init(const char* pData)
	{
		root["type"] = "UserOut";
		root["Name"] = pData;
		str = writer.write(root);
		packet.size = str.length() + 4;
		strcpy(packet.dummy, str.c_str());
	}
};

struct ChangeType : public JsonData
{
	PacketMessage packet;
	void Init(int type, const char *pData)
	{
		root["type"] = "ChangeType";
		root["Name"] = pData;
		root["CharactorNum"] = type;
		str = writer.write(root);
		packet.size = str.length() + 4;
		strcpy(packet.dummy, str.c_str());
	}
};

struct Ready : public JsonData
{
	PacketMessage packet;
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
			for (int i = 0; i < mCnt; ++i)
			{
				Json::Value Session;
				Session["SessionID"] = mData[i].ID;
				Data.append(Session);
			}
			root["SessionIDList"] = Data;
		}
		str = writer.write(root);
		packet.size = str.length() + 4;
		strcpy(packet.dummy, str.c_str());
	}
};

struct CoverMessage : public JsonData
{
	PacketMessage packet;

	void Init(PacketMessage _packet)
	{
		packet.size = _packet.size;
		strcpy(packet.dummy, _packet.dummy);
	}
};