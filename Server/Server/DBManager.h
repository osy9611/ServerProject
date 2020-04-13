#pragma once
#include"GameServer.h"

class DBManager
{
private:

	//�����ͺ��̽��� ��𼭵� ������ �ؾ��ϱ� ������ �̱������� �������
	static DBManager* instance;

public:

	DBManager();
	~DBManager();

	static DBManager* getInstance();
	static void FreeInstance();

	void InitDB();

	ItemMixResult SetResultItem(Json::Value _message);
	BossData SearchBossDrop(int BossNum);

private:
	Database db;
	Table tbl;

	//ODBC�� �����ϱ� ���� ���ڿ�
	char CnnStr[200] = "DRIVER={MySQL ODBC 8.0 ANSI Driver};\
				   SERVER=localhost;\
				   DATABASE=gamedatas;\
				   USER=root;\
				   PASSWORD=@ppgk38629;";

	//���� ����
	char ErrStr[200];
};

