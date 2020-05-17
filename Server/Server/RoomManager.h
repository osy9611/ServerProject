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

	//�游��
	void MakeRoom(const char* RoomName, int SessionID, int SessionID2);
	
	//�濡 ��
	void EneterRoom(const char *RoomName, const int nSessionID);

	//�� �غ� ����
	void RoomReady(GameServer* pServer,const char* RoomName, const int nSessionID, std::vector<ServerSession*> m_SessionList);
	//�濡�ִ� �������� ���� ���̵� �޾ƿ�
	int GetRoomUserSessionID(const char * RoomName,int Num);
	//�濡 �ִ� �������� �ο����� ������
	int GetRoomUserCount(const char* RoomName);

	void AddItemCount(const char* RoomName, int itemType);
	void SubItemCount(const char* RoomName, int source1, int source2, int source3);

	//�� �����͸� �޾ƿ��� �Լ�
	RoomData GetRoomData(const char*RoomName);

	//���� �κ��丮 ó���κ�
	SharedInventory SetInventory(int arrayNum, int itemNumber,const char* RoomName);
	SharedInventory SwapInventory(int arrayNum1, int arrayNum2, const char *RoomName);
	SharedInventory DeleteInventory(int arrayNum1, const char *RoomName);


	std::vector<std::string> RoomNames;

protected:
	std::map<std::string, RoomData>Room;
};

