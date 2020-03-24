#include"GameServer.h"
#include "DBManager.h"


DBManager::DBManager()
{
}


DBManager::~DBManager()
{
}

//ADO를 등록함
void DBManager:: InitDB()
{
	::CoInitialize(NULL);

	if (!db.Open("", "", CnnStr))
	{
		db.GetErrorErrStr(ErrStr);
		std::cout << ErrStr << "\n";
	}
	else
	{
		std::cout << "Database Connect Success!!" << std::endl;
	}
	::CoUninitialize();
}

bool DBManager::SearchItem(const char * Source1, const char * Source2, const char * Source3, const char* _Money, char * _result)
{
	//날 쿼리가 아닌 스토어드 프로시저를 사용중 추후에 개선 예정
	std::string Query;
	std::string Source[3];
	std::string Money;

	Source[0] = Source1;
	Source[1] = Source2;
	Source[2] = Source3;

	Money = _Money;

	Query = "CALL SearchItem('" + Source[0] + "','" + Source[1] + "','" + Source[2] + "','" + Money +"'," + "@result)";

	const char *ch = Query.c_str();
	if (db.Execute(ch, tbl))
	{
		char item[50];
		if (db.Execute("SELECT @result", tbl))
		{
			if (!tbl.ISEOF())
			{
				std::cout << "성공" << std::endl;
				tbl.Get((char*)"@result", item);
				sprintf(_result, "%s", item);
			}
		}
	}

	return 1;
}
