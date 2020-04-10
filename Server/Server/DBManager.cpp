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



ItemMixResult DBManager::SetResultItem(Json::Value _message)
{

	Json::Value sourceID = _message["itemID"];
	Json::Value source = _message["itemCount"];
	
	std::string Query;
	for (int i = 0; i < 3; ++i)
	{
		std::cout << sourceID[i] << std::endl;
	}
	printf("\n");
	Query = "CALL SearchItem('" 
		+ sourceID[0].asString() + "','" + source[0].asString() + "','"
		+ sourceID[1].asString() + "','" + source[0].asString() + "','"
		+ sourceID[2].asString() + "','" + source[0].asString() + "','"
		+ _message["money"].asString() 
		+ "'," + "@result)";

	char result[50];

	const char *ch = Query.c_str();
	if (db.Execute(ch, tbl))
	{
		if (db.Execute("SELECT @result", tbl))
		{
			if (!tbl.ISEOF())
			{
				std::cout << "성공" << std::endl;
				tbl.Get((char*)"@result", result);
			}
		}
	}

	ItemMixResult mixResult;
	mixResult.Init(result);
	std::cout << mixResult.str << std::endl;
	return mixResult;
}
