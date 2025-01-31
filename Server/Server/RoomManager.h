#pragma once
#pragma warning(disable: 4996) 

#include <random> 
#include "GameServer.h"

class RoomManager
{
public:
	RoomManager();
	~RoomManager();

	int BossSet();

	float CharactorHpCheck(float Damage);
	float BoostHpCheck(float Damage);

	void UserOutCheck(const char* RoomName, const int nSessionID, std::vector<ServerSession*> m_SessionList);

	//방만듬
	void MakeRoom(const char* RoomName, int SessionID, int SessionID2);
	
	//방에 들어감
	void EneterRoom(const char *RoomName, const int nSessionID);

	//방 준비를 위함
	void RoomReady(GameServer* pServer,const char* RoomName, const int nSessionID, std::vector<ServerSession*> m_SessionList);
	//방에있는 유저들의 세션 아이디를 받아옴
	int GetRoomUserSessionID(const char * RoomName,int Num);
	//방에 있는 유저들의 인원수를 가져옴
	int GetRoomUserCount(const char* RoomName);

	//아이템 확률 관련
	ItmePerResult PercentageCheck(int Per,const char* UserName);

	void AddItemCount(const char* RoomName, int itemType);
	void SubItemCount(const char* RoomName, int source1, int source2, int source3);

	//룸 데이터를 받아오는 함수
	RoomData GetRoomData(const char*RoomName);

	//공유 인벤토리 처리부분
	SharedInventory SetInventory(int arrayNum, int itemNumber,const char* RoomName);
	SharedInventory SwapInventory(int arrayNum1, int arrayNum2, const char *RoomName);
	SharedInventory DeleteInventory(int arrayNum1, const char *RoomName);


	std::vector<std::string> RoomNames;

protected:
	std::map<std::string, RoomData>Room;
};

