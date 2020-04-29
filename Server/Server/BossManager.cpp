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
	Hp = _BossData.Hp;
	for (size_t i = 0; i < 3; ++i)
	{
		RandomItemSet(_BossData.Item[i], _BossData.ItemPer[i]);
	}
	Money = _BossData.Money;
}

void BossManager::ResetBossData()
{
	Hp = 0;
	ItemCount = 0;
	Money = 0;
}

void BossManager::RandomItemSet(int ItemID, int ItemPer)
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

void BossManager::ShufflePhase()
{
	int temp;
	int n;
	std::random_device randDevice;	//랜덤 디바이스 생성
	std::mt19937  mt(randDevice());
	std::uniform_int_distribution<int> distribution(0, 3);

	for (size_t i = 0; i < 4; ++i)
	{
		n = distribution(randDevice);

		temp = _BossData.Phase[n];
		_BossData.Phase[n] = _BossData.Phase[3];
		_BossData.Phase[3] = temp;
	}
}

BossResult BossManager::HitBoss(float BossDamage)
{
	BossResult result;

	if (Hp == _BossData.Hp)
	{
		Hp -= BossDamage;
		ShufflePhase();
		result.Init(Hp, _BossData.Phase[0]);
		NowPhase++;
	}
	else
	{
		Hp -= BossDamage;
		if (Hp <= 0)
		{
			result.Init(Item, ItemCount, Money);
		}
		else
		{
			result.Init(Hp);
		}
	}
	

	return result;
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

PhaseRestart BossManager::PhaseSet()
{
	PhaseRestart result;

	if (NowPhase == 3)
	{
		result.Init(_BossData.Phase[NowPhase]);
		NowPhase = 0;
		ShufflePhase();
		return result;
	}
	else
	{
		result.Init(_BossData.Phase[NowPhase]);
		NowPhase++;
		return result;
	}
	
}

void BossManager::print(const boost::system::error_code & error)
{
	std::cout << "타이머" << std::endl;
}

BossPhaseResult BossManager::CalcPhase(Json::Value _message, boost::asio::deadline_timer* t)
{
	BossPhaseResult result;
	
	switch (Phase)
	{
	case 2:
		PhaseCount++;

		if (PhaseCount == UserCount)
		{
			int index = Random(0, 3);
			result.RandomCircleBullet(index);
			result.PhaseCalc = true;
			PhaseCount = 0;

		}
		else
		{
			result.PhaseCalc = false;
			int count = 0;
			t->expires_at(t->expires_at() + boost::posix_time::seconds(1));
			t->async_wait(boost::bind(&BossManager::print,this ,boost::asio::placeholders::error));
		}
		break;
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
			int index = Random(0, 7);
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
	case 5:

		break;

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

int BossManager::Random(int min,int max)
{
	std::random_device randDevice;	//랜덤 디바이스 생성
	std::mt19937  mt(randDevice());
	std::uniform_int_distribution<int> distribution(min, max);

	return  distribution(randDevice);
}