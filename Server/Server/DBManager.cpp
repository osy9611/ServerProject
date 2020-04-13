#include"GameServer.h"
#include "DBManager.h"

DBManager* DBManager::instance = nullptr;

DBManager* DBManager::getInstance()
{
	if (instance == nullptr)
		instance = new DBManager();

	return instance;
}

void DBManager::FreeInstance()
{
	if (NULL != instance)
	{
		delete instance;
		instance = NULL;
	}
}

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

BossData DBManager::SearchBossDrop(int BossNum)
{
	std::string Query;

	Query = "CALL BossDrop(" + std::to_string(BossNum) + "," +
		"@Item1,@ItemPer1,@Item2,@ItemPer2,@Item3,@ItemPer3,@Money)";

	BossData Result;

	const char *ch = Query.c_str();

	std::string _item[3] = { "@Item1", "@Item2" ,"@Item3" };
	std::string _itemPer[3] = { "@ItemPer1","@ItemPer2","@ItemPer3" };

	char result[50];
	if (db.Execute(ch, tbl))
	{
		std::cout << "저장 프로시저 성공" << std::endl;
		if (db.Execute("SELECT @Item1,@ItemPer1,@Item2,@ItemPer2,@Item3,@ItemPer3,@Money", tbl))
		{
			if (!tbl.ISEOF())
			{
				std::cout << "성공" << std::endl;		
				for (int i = 0; i < 3; ++i)
				{
					tbl.Get((char*)_item[i].c_str(), result);
					Result.Item[i] = atoi(result);
					result[0] = '\0';

					tbl.Get((char*)_itemPer[i].c_str(), result);
					Result.ItemPer[i] = atoi(result);
					result[0] = '\0';
				}
				tbl.Get((char*)"@Money", result);
				Result.Money = atoi(result);
			}
		}
	}
	return Result;
}
