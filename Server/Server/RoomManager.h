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
	void RoomReady(const char* RoomName, const int nSessionID, std::vector<ServerSession*> m_SessionList);
	//�濡�ִ� �������� ���� ���̵� �޾ƿ�
	int GetRoomUserSessionID(const char * RoomName,int Num);
	//�濡 �ִ� �������� �ο����� ������
	int GetRoomUserCount(const char* RoomName);
protected:
	std::map<std::string, RoomData>Room;
};

