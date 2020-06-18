//#include "BossManager.h"
#include "GameServer.h"


BossManager::BossManager(GameServer *pServer,const char* _roomName)
	:m_pServer(pServer),RoomName(_roomName)
{
	dbManager = DBManager::getInstance();
}


BossManager::~BossManager()
{
		
}

void BossManager::ResetDB()
{
	if (RoomName != "")
	{
		dbManager = NULL;
	}
}

void BossManager::UserSet(int _userCount)
{
	UserCount = _userCount;
}

int BossManager::GetPhase()
{
	return Phase;
}

int BossManager::CheckHp()
{
	return Hp;
}

void BossManager::SetBossData(int Data)
{
	std::cout << "보스 셋팅중입니다" << std::endl;

	BossNum = Data;

	ResetBossData();

	_BossData = dbManager->SearchBoss(Data);
	FullHp = _BossData.Hp;
	Hp = _BossData.Hp;
	_firstStart = true;
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
		std::uniform_int_distribution<int> distribution(0, 5);
		Item[ItemCount] = distribution(randDevice);
		ItemCount++;
	}
}

GetItemIDResult  BossManager::GetBossItemID()
{
	GetItemIDResult result;
	result.Init(Item,ItemCount, Money);
	return result;
}

void BossManager::ShufflePhase()
{
	int temp;
	int n;

	for (size_t i = 0; i < 4; ++i)
	{
		n = Random(0, 3);

		temp = _BossData.Phase[n];
		_BossData.Phase[n] = _BossData.Phase[3];
		_BossData.Phase[3] = temp;
	}
}

void BossManager::ShuffleMainBulletType()
{
	int temp;
	int n;

	for (size_t i = 0; i < MAX_MAIN_BULLET; ++i)
	{
		n = Random(0, MAX_MAIN_BULLET);

		temp = _BossData.Phase[n];
		_BossData.Phase[n] = _BossData.Phase[MAX_MAIN_BULLET- 1];
		_BossData.Phase[MAX_MAIN_BULLET - 1] = temp;
	}
}

void BossManager::ShuffleSubBulletType()
{
	int temp;
	int n;

	for (size_t i = 0; i < MAX_SUB_BULLET; ++i)
	{
		n = Random(0, MAX_SUB_BULLET);

		temp = _BossData.Phase[n];
		_BossData.Phase[n] = _BossData.Phase[MAX_SUB_BULLET - 1];
		_BossData.Phase[MAX_SUB_BULLET - 1] = temp;
	}
}

BossResult BossManager::HitBoss(float BossDamage)
{
	BossResult result;

	if (Hp == _BossData.Hp && _firstStart)
	{
		_firstStart = false;
		Hp -= BossDamage;
		ShufflePhase();
		Phase = _BossData.Phase[0];
		result.Init(Hp, _BossData.Phase[0]);
		NowPhase++;
	}
	else
	{
		Hp -= BossDamage;
		if (Hp <= 0)
		{
			result.Init(Item, ItemCount, Money);
			if (BossNum < BOSS_NUM_MAX)
			{
				BossNum++;
				
				SetBossData(BossNum);
			}
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
		Phase = _BossData.Phase[NowPhase];
		result.Init(_BossData.Phase[NowPhase]);
		NowPhase = 0;		
		ShufflePhase();
		return result;
	}
	else
	{
		Phase = _BossData.Phase[NowPhase];
		result.Init(_BossData.Phase[NowPhase]);
		NowPhase++;
		return result;
	}
	
}

BossPhaseResult BossManager::CalcPhase(Json::Value _message)
{
	BossPhaseResult result;
	PhaseCount++;
	if (PhaseCount == UserCount)
	{
		result.PhaseCalc = true;
		PhaseCount = 0;
		switch (Phase)
		{
		case 2:
		{
			result.RandomCircleBullet(mainBulletType[mainBulletTypeCount]);
			if (mainBulletTypeCount != MAX_MAIN_BULLET-1)
			{
				mainBulletTypeCount++;
			}
			else
			{
				mainBulletTypeCount = 0;
				ShuffleMainBulletType();
			}
			break;
		}			
		case 3:	//페이즈 1
		{
			XMFLOAT2 dir;
			dir = DirCalc(_message["px"].asFloat(), _message["py"].asFloat(), _message["bx"].asFloat(), _message["by"].asFloat());
			result.DirInit(dir.x, dir.y);
			break;
		}
		case 8:
		{
			FireBallCount = 4;
			struct timeb timer_msec;
			ftime(&timer_msec);

			curr_tm = localtime((time_t*)&timer_msec);
			int milltime = (curr_tm->tm_hour * pow(60, 3) +
				curr_tm->tm_min * pow(60, 2) +
				curr_tm->tm_sec * 60) * 1000 + timer_msec.millitm;

			result.FirBall(milltime, Random(0, MAX_SUB_BULLET));

			break;
		}
		case 12:
		{
			int index = Random(0, m_pServer->GetRoomData(RoomName.c_str()).Count - 1);
			result.CircleFloor(m_pServer->SearchUserName(index, RoomName.c_str()), Random(0, MAX_SUB_BULLET));
			break;
		}
		case 21:
		{
			int index = Random(0, m_pServer->GetRoomData(RoomName.c_str()).Count - 1);
			result.Restriction(m_pServer->SearchUserName(index, RoomName.c_str()), Random(0, MAX_SUB_BULLET));
			std::cout << result.str << std::endl;
			break;
		}
		default:
		{
			result.PhaseCalc = false;
			break;
		}
		}
	}
	else
	{
		result.PhaseCalc = false;
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

void BossManager::DestroyFireBall()
{
	FireBallCount -= 1;
	std::cout << "RoomName:"<<RoomName<< " " <<FireBallCount << std::endl;
}

void BossManager::FireBallCheck()
{
	if (Phase == 8)
	{
		if (FireBallCount > 0)
		{
			Hp = FullHp;
		}
		FireBallCount = MAX_FIRE_BALL;
	}

}
