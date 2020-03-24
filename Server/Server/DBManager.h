#pragma once
#include"GameServer.h"

class DBManager
{
public:
	DBManager();
	~DBManager();

	void InitDB();
	bool SearchItem(const char * Source1, const char * Source2, const char * Source3,const char* _Money, char * _result);

private:
	Database db;
	Table tbl;

	//ODBC를 연결하기 위한 문자열
	char CnnStr[200] = "DRIVER={MySQL ODBC 8.0 ANSI Driver};\
				   SERVER=localhost;\
				   DATABASE=gamedatas;\
				   USER=root;\
				   PASSWORD=@ppgk38629;";

	//에러 검출
	char ErrStr[200];
};

