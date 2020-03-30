#pragma once
#include"GameServer.h"

class DBManager
{
public:
	DBManager();
	~DBManager();

	void InitDB();

	ItemMixResult SetResultItem(Json::Value _message);
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

