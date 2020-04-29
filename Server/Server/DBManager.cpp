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

BossData DBManager::SearchBoss(int BossNum)
{
	Query = "CALL BossDrop(" + std::to_string(BossNum) + "," +
		"@Item1,@ItemPer1,@Item2,@ItemPer2,@Item3,@ItemPer3,@Money)";

	BossData Result;
	
	if (db.Execute(Query.c_str(), tbl))
	{
		std::cout << "보스 드랍 테이블 저장 프로시저 성공" << std::endl;
		if (db.Execute("SELECT @Item1,@ItemPer1,@Item2,@ItemPer2,@Item3,@ItemPer3,@Money", tbl))
		{
			if (!tbl.ISEOF())
			{
				std::cout << "성공" << std::endl;		
				for (size_t i = 0; i < 3; ++i)
				{
					Result.Item[i] = tbl.Get((char*)_item[i].c_str());
					Result.ItemPer[i] = tbl.Get((char*)_itemPer[i].c_str());
				}
				
				Result.Money = tbl.Get((char*)"@Money");
			}
		}
	}

	Query = "CALL BossPhase(" + std::to_string(BossNum) + "," +
		"@Hp,@Phase1,@Phase2,@Phase3,@Phase4)";

	if (db.Execute(Query.c_str(), tbl))
	{
		std::cout << "보스 페이즈 저장 프로시저 성공" << std::endl;
		if (db.Execute("SELECT @Hp,@Phase1,@Phase2,@Phase3,@Phase4", tbl))
		{
			if (!tbl.ISEOF())
			{
				std::cout << "성공" << std::endl;
				for (size_t i = 0; i < 4; ++i)
				{
					Result.Phase[i] = tbl.Get((char*)_phase[i].c_str());
				}

				Result.Hp = tbl.Get("@Hp");
			}
		}

	}

	return Result;
}
