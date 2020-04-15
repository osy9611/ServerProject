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

	ResetBossData();

	_BossData = dbManager->SearchBoss(Data);

	for (size_t i = 0; i < 3; ++i)
	{
		RandomSet(_BossData.Item[i], _BossData.ItemPer[i]);
	}
	Money = _BossData.Money;
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
		//ResetBossData();
	}
	else
	{
		Phase = PhaseCheck(Hp);
		result.Init(Hp,Phase);
	}

	return result;
}

int BossManager::PhaseCheck(int Hp)
{
	
	if (_BossData.PhaseHp[0] >= Hp && _BossData.PhaseHp[1] <= Hp)
	{
		return _BossData.Phase[0];
	}
	else if (_BossData.PhaseHp[1] >= Hp && _BossData.PhaseHp[2] <= Hp)
	{
		return _BossData.Phase[1];
	}
	else if (_BossData.PhaseHp[2] >= Hp && _BossData.PhaseHp[3] <= Hp)
	{
		return _BossData.Phase[2];
	}
	else if (_BossData.PhaseHp[3] >= Hp && _BossData.PhaseHp[4] <= Hp)
	{
		return _BossData.Phase[3];
	}
	else
	{
		return _BossData.Phase[4];
	}
}

BossPhaseResult BossManager::CalcPhase(Json::Value _message)
{
	BossPhaseResult result;
	XMFLOAT2 dir;
	switch (Phase)
	{
	case 0:	//페이즈 1
		dir =DirCalc(_message["px"].asInt(), _message["py"].asInt(),_message["bx"].asInt(),_message["by"].asInt());
		result.DirInit(dir.x,dir.y);
		break;
	
	default:
		dir = DirCalc(_message["px"].asInt(), _message["py"].asInt(), _message["bx"].asInt(), _message["by"].asInt());
		result.DirInit(dir.x, dir.y);
		break;
	}

	return result;

}

XMFLOAT2 BossManager::DirCalc(float px, float py,float bx,float by)
{
	XMFLOAT2 dir = XMFLOAT2(px-bx, py - by);
	XMVECTOR vecdir = XMLoadFloat2(&dir);
	XMVECTOR normaldir = XMVector2Normalize(vecdir);
	XMFLOAT2 result;
	XMStoreFloat2(&result, normaldir);

	return result;
}