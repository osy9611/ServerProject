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

void BossManager::UserSet(int _userCount)
{
	UserCount = _userCount;
}

int BossManager::GetPhase()
{
	return Phase;
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
	else if (_BossData.PhaseHp[3] >= Hp && 0 <= Hp)
	{
		return _BossData.Phase[3];
	}
}

bool BossManager::RestartCheck()
{
	//std::cout << "유저 페이즈 체크" << std::endl;

	PhaseClearCount++;

	//유저 숫자와 페이즈 클리어 숫자가 같으면 참을 반환
	if (UserCount <= PhaseClearCount)
	{
		//std::cout << "유저 페이즈 체크된 인원 : "<< PhaseClearCount << std::endl;
		PhaseClearCount = 0;
		return 1;
	}
	else
	{
		//std::cout << "유저 페이즈 체크된 인원 : " << PhaseClearCount << std::endl;
		
		return 0;
	}
}

BossPhaseResult BossManager::CalcPhase(Json::Value _message)
{
	BossPhaseResult result;
	
	switch (Phase)
	{
	case 3:	//페이즈 1
	{
		XMFLOAT2 dir;
		dir = DirCalc(_message["px"].asFloat(), _message["py"].asFloat(), _message["bx"].asFloat(), _message["by"].asFloat());
		result.DirInit(dir.x, dir.y);
		result.PhaseCalc = true;
		break;
	}	
	case 4:
	{
		PhaseCount++;

		if (PhaseCount == UserCount)
		{
			int index = SetLaser();
			result.RandomLaser(index);
			result.PhaseCalc = true;
			PhaseCount = 0;

		}
		else
		{
			result.PhaseCalc = false;
		}
		break;
	}

	default:
	{
		result.PhaseCalc = false;
		break;
	}
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

int BossManager::SetLaser()
{
	std::random_device randDevice;	//랜덤 디바이스 생성
	std::mt19937  mt(randDevice());
	std::uniform_int_distribution<int> distribution(0, 7);

	return  distribution(randDevice);
}