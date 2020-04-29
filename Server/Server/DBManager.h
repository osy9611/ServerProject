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
	BossData SearchBoss(int BossNum);

private:
	Database db;
	Table tbl;

	//ODBC�� �����ϱ� ���� ���ڿ�
	char CnnStr[200] = "DRIVER={MySQL ODBC 8.0 ANSI Driver};\
				   SERVER=localhost;\
				   DATABASE=gamedatas;\
				   USER=root;\
				   PASSWORD=@ppgk38629;";

	//DB�� ����� ������
	std::string Query;

	//������ ��� ���̺� ���� 
	std::string _item[3] = { "@Item1", "@Item2" ,"@Item3" };
	std::string _itemPer[3] = { "@ItemPer1","@ItemPer2","@ItemPer3" };

	//���� ������ ����
	std::string _phase[4] = { "@Phase1","@Phase2","@Phase3","@Phase4" };

	//���� ����
	char ErrStr[200];
};

