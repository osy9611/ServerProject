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

	//�� Ŭ���̾�Ʈ�� ����� �������� Ȯ���ϴ� �Լ�
	bool RestartCheck();
	int PhaseCheck(int Hp);

	//������ Ŭ���̾�Ʈ ������ �ϷῩ��
	int PhaseClearCount = 0;
private:
	//Hp
	float Hp;
	//���� ���� ������
	int Phase = 0;
	int PhaseCount = 0;

	//���ӵ� ������
	int UserCount;

	//���� ��� ������
	BossData _BossData;
	//DB �Ŵ���
	DBManager *dbManager;

	//��� �������� �־��ִ� ��
	int Item[3];
	int ItemCount;
	//��� ��
	int Money;

	//������ ����
	int SetLaser();

	//DB���� ���� �����͸� �����س��´�
	void RandomSet(int ItemID,int ItemPer);

	//źȯ ���Ⱚ ���
	XMFLOAT2 DirCalc(float px, float py, float bx, float by);
};

