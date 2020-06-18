#pragma once
#pragma comment(lib,"winmm.lib")
#pragma comment(lib,"d3d11.lib")
#pragma comment(lib,"dxgi.lib")
#pragma comment(lib, "d3dcompiler.lib")

#include <math.h>
#include <d3d11.h>
#include <d3dcompiler.h>
#include <DirectXMath.h>
#include<ctime>
#include<cstdlib>
#include<sys/timeb.h>
#include<cstdio>
#include<cmath>
#define MAX_SWAP_POS 4
#define MAX_FIRE_BALL 4

#define MAX_MAIN_BULLET 7
#define MAX_SUB_BULLET 9

#define BOSS_NUM_MAX 2

using namespace DirectX;

class GameServer;

class BossManager
{
public:
	BossManager(GameServer* pServer,const char* roomName);
	~BossManager();
	void ResetDB();
	void SetBossData(int Data);
	void UserSet(int _userCount);
	void ResetBossData();
	BossResult HitBoss(float Damage);
	BossPhaseResult CalcPhase(Json::Value _messsage);
	PhaseRestart PhaseSet();
	GetItemIDResult GetBossItemID();

	//각 클라이언트의 페이즈가 끝났는지 확인하는 함수
	bool RestartCheck();

	//각각의 클라이언트 페이즈 완료여부
	int PhaseClearCount = 0;
	int GetPhase();
	int PhaseCount = 0;	
	void ShufflePhase();	

	//불구슬 관련 체크
	void FireBallCheck();
	void DestroyFireBall();

	//메인 불랫 타입을 섞음
	void ShuffleMainBulletType();
	void ShuffleSubBulletType();

	//피통 호출
	int CheckHp();

	int PrevUserCount;
private:
	std::string RoomName;

	int BossNum;

	//Hp
	float FullHp;
	float Hp;

	//처음 시작됬을때
	bool _firstStart = true;
	//보스 현재 페이즈
	int Phase = 0;	
	int NowPhase = 0;

	//서브 페이즈 관련
	int SubPhase = 2;

	//접속된 유저수
	int UserCount;

	//보스 드랍 데이터
	BossData _BossData;
	//DB 매니저
	DBManager *dbManager;

	//총알 타입
	int mainBulletType[MAX_MAIN_BULLET] = { 0,1,2,3,4,5,6 };
	int mainBulletTypeCount = 0;
	int subBulletTYpe[MAX_SUB_BULLET] = { 0,1,2,3,4,5,6,7,8 };
	int subBulletTYpeCount = 0;

	//드랍 아이템을 넣어주는 곳
	int Item[3];
	int ItemCount;
	//드랍 돈
	int Money;

	//8페이즈 회복구슬 관련 변수
	int FireBallCount = 4;


	tm * curr_tm;

	//DB에서 받은 데이터를 셋팅해놓는다
	void RandomItemSet(int ItemID,int ItemPer);

	//랜덤 값을 계산하기 위한 함수
	int Random(int min, int max);
	//탄환 반향값 계산
	XMFLOAT2 DirCalc(float px, float py, float bx, float by);

	GameServer* m_pServer;
};



