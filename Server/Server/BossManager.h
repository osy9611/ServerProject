#pragma once
#pragma comment(lib,"winmm.lib")
#pragma comment(lib,"d3d11.lib")
#pragma comment(lib,"dxgi.lib")
#pragma comment(lib, "d3dcompiler.lib")

#include <math.h>
#include <d3d11.h>
#include <d3dcompiler.h>
#include <DirectXMath.h>

using namespace DirectX;

class BossManager
{
public:
	BossManager();
	~BossManager();

	void SetBossData(int Data);
	void ResetBossData();
	BossResult HitBoss(float Damage);
	BossPhaseResult CalcPhase(Json::Value _messsage);
	int PhaseCheck(int Hp);

private:
	//Hp
	float Hp;
	//보스 현재 페이즈
	int Phase = 0;
	int PhaseCount = 0;
	//보스 드랍 데이터
	BossData _BossData;
	//DB 매니저
	DBManager *dbManager;

	//드랍 아이템을 넣어주는 곳
	int Item[3];
	int ItemCount;
	//드랍 돈
	int Money;

	//DB에서 받은 데이터를 셋팅해놓는다
	void RandomSet(int ItemID,int ItemPer);

	XMFLOAT2 DirCalc(float px, float py, float bx, float by);
};

