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
	//���� ���� ������
	int Phase = 0;
	int PhaseCount = 0;
	//���� ��� ������
	BossData _BossData;
	//DB �Ŵ���
	DBManager *dbManager;

	//��� �������� �־��ִ� ��
	int Item[3];
	int ItemCount;
	//��� ��
	int Money;

	//DB���� ���� �����͸� �����س��´�
	void RandomSet(int ItemID,int ItemPer);

	XMFLOAT2 DirCalc(float px, float py, float bx, float by);
};

