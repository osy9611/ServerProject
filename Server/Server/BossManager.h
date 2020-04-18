#pragma once
#pragma comment(lib,"winmm.lib")
#pragma comment(lib,"d3d11.lib")
#pragma comment(lib,"dxgi.lib")
#pragma comment(lib, "d3dcompiler.lib")

#include <math.h>
#include <d3d11.h>
#include <d3dcompiler.h>
#include <DirectXMath.h>

#define MAX_SWAP_POS 4
using namespace DirectX;

class BossManager
{
public:
	BossManager();
	~BossManager();

	void SetBossData(int Data);
	void UserSet(int _userCount);
	void ResetBossData();
	BossResult HitBoss(float Damage);
	BossPhaseResult CalcPhase(Json::Value _messsage);

	//각 클라이언트의 페이즈가 끝났는지 확인하는 함수
	bool RestartCheck();
	int PhaseCheck(int Hp);

	//각각의 클라이언트 페이즈 완료여부
	int PhaseClearCount = 0;
private:
	//Hp
	float Hp;
	//보스 현재 페이즈
	int Phase = 0;
	int PhaseCount = 0;

	//접속된 유저수
	int UserCount;

	//보스 드랍 데이터
	BossData _BossData;
	//DB 매니저
	DBManager *dbManager;

	//드랍 아이템을 넣어주는 곳
	int Item[3];
	int ItemCount;
	//드랍 돈
	int Money;

	//레이저 방항
	int SetLaser();

	//DB에서 받은 데이터를 셋팅해놓는다
	void RandomSet(int ItemID,int ItemPer);

	//탄환 반향값 계산
	XMFLOAT2 DirCalc(float px, float py, float bx, float by);
};

