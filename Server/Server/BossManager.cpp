//#include "BossManager.h"
#include "GameServer.h"


BossManager::BossManager()
{
	dbManager = DBManager::getInstance();
}


BossManager::~BossManager()
{
	dbManager = NULL;
}

void BossManager::SetBossData(int Data)
{
	std::cout << "보스 셋팅중입니다" << std::endl;

	DropData = dbManager->SearchBossDrop(Data);

	for (int i = 0; i < 3; ++i)
	{
		RandomSet(DropData.Item[i], DropData.ItemPer[i]);
		std::cout << DropData.Item[i] << " " << DropData.ItemPer[i] << std::endl;
	}
	Money = DropData.Money;
	std::cout << DropData.Money << std::endl;
}

void BossManager::ResetBossData()
{
	Hp = 100;
	ItemCount = 0;
	Money = 0;
}

void BossManager::RandomSet(int ItemID,int ItemPer)
{
	if (ItemID != 0)
	{
		//random 함수셋팅 
		std::random_device randDevice;	//랜덤 디바이스 생성
		std::mt19937  mt(randDevice());
		std::uniform_int_distribution<int> distribution(0, 100);

		int result = distribution(randDevice);

		if (result <= ItemPer)
		{
			Item[ItemCount] = ItemID;
			ItemCount++;
		}
	}
}

BossResult BossManager::HitBoss(float BossDamage)
{
	BossResult result;

	Hp -= BossDamage;
	if (Hp <= 0)
	{
		result.Init(Item, ItemCount, Money);
		ResetBossData();
	}
	else
	{
		result.Init(Hp);
	}

	return result;
}