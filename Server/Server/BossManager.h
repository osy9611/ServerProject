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

class GameServer;

class BossManager
{
public:
	BossManager(GameServer* pServer,const char* roomName);
	~BossManager();

	void SetBossData(int Data);
	void UserSet(int _userCount);
	void ResetBossData();
	BossResult HitBoss(float Damage);
	BossPhaseResult CalcPhase(Json::Value _messsage);
	PhaseRestart PhaseSet();

	//�� Ŭ���̾�Ʈ�� ����� �������� Ȯ���ϴ� �Լ�
	bool RestartCheck();

	//������ Ŭ���̾�Ʈ ������ �ϷῩ��
	int PhaseClearCount = 0;

	int GetPhase();

	int PhaseCount = 0;
	
	void ShufflePhase();	
private:
	const char* RoomName;
	//Hp
	float Hp;
	//���� ���� ������
	int Phase = 0;	
	int NowPhase = 0;

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

	//DB���� ���� �����͸� �����س��´�
	void RandomItemSet(int ItemID,int ItemPer);

	//���� ���� ����ϱ� ���� �Լ�
	int Random(int min, int max);
	//źȯ ���Ⱚ ���
	XMFLOAT2 DirCalc(float px, float py, float bx, float by);

	GameServer* m_pServer;
};

