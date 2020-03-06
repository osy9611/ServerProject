#include"GameServer.h"
#include "DBManager.h"


DBManager::DBManager()
{
}


DBManager::~DBManager()
{
}

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

bool DBManager::SearchItem(const char * Source1, const char * Source2, const char * Source3,char * _result)
{
	std::string Query;
	std::string Source[3];
	Source[0] = Source1;
	Source[1] = Source2;
	Source[2] = Source3;
	Query = "CALL SearchItem('" + Source[0] + "','" + Source[1] + "','" + Source[2] + "'," + "@result)";

	const char *ch = Query.c_str();
	if (db.Execute(ch, tbl))
	{
		char item[50];
		if (db.Execute("SELECT @result", tbl))
		{
			if (!tbl.ISEOF())
			{
				std::cout << "¼º°ø" << std::endl;
				tbl.Get((char*)"@result", item);
				sprintf(_result, "%s", item);
			}
		}
	}

	return 1;
}
