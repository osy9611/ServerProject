//#include "BossManager.h"
#include "GameServer.h"


BossManager::BossManager()
{
	dbManager = DBManager::getInstance();
}


BossManager::~BossManager()
{
}

void BossManager::SetBoosData(int Data)
{
	std::cout << "보스 셋팅중입니다" << std::endl;
	BossData data = dbManager->SearchBossDrop(1);

	for (int i = 0; i < 3; ++i)
	{
		std::cout << data.Item[i] << " " << data.ItemPer[i] << std::endl;
	}
	std::cout << data.Money << std::endl;
}

void BossManager::RandomSet()
{

}