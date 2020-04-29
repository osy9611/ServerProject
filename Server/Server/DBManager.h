#pragma once
#include"GameServer.h"

class DBManager
{
private:

	//데이터베이스는 어디서든 접근을 해야하기 때문에 싱글콘으로 만들었다
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

	//ODBC를 연결하기 위한 문자열
	char CnnStr[200] = "DRIVER={MySQL ODBC 8.0 ANSI Driver};\
				   SERVER=localhost;\
				   DATABASE=gamedatas;\
				   USER=root;\
				   PASSWORD=@ppgk38629;";

	//DB에 사용할 쿼리문
	std::string Query;

	//아이템 드랍 테이블 관련 
	std::string _item[3] = { "@Item1", "@Item2" ,"@Item3" };
	std::string _itemPer[3] = { "@ItemPer1","@ItemPer2","@ItemPer3" };

	//보스 페이즈 관련
	std::string _phase[4] = { "@Phase1","@Phase2","@Phase3","@Phase4" };

	//에러 검출
	char ErrStr[200];
};

