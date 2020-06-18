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

	//�� Ŭ���̾�Ʈ�� ����� �������� Ȯ���ϴ� �Լ�
	bool RestartCheck();

	//������ Ŭ���̾�Ʈ ������ �ϷῩ��
	int PhaseClearCount = 0;
	int GetPhase();
	int PhaseCount = 0;	
	void ShufflePhase();	

	//�ұ��� ���� üũ
	void FireBallCheck();
	void DestroyFireBall();

	//���� �ҷ� Ÿ���� ����
	void ShuffleMainBulletType();
	void ShuffleSubBulletType();

	//���� ȣ��
	int CheckHp();

	int PrevUserCount;
private:
	std::string RoomName;

	int BossNum;

	//Hp
	float FullHp;
	float Hp;

	//ó�� ���ۉ�����
	bool _firstStart = true;
	//���� ���� ������
	int Phase = 0;	
	int NowPhase = 0;

	//���� ������ ����
	int SubPhase = 2;

	//���ӵ� ������
	int UserCount;

	//���� ��� ������
	BossData _BossData;
	//DB �Ŵ���
	DBManager *dbManager;

	//�Ѿ� Ÿ��
	int mainBulletType[MAX_MAIN_BULLET] = { 0,1,2,3,4,5,6 };
	int mainBulletTypeCount = 0;
	int subBulletTYpe[MAX_SUB_BULLET] = { 0,1,2,3,4,5,6,7,8 };
	int subBulletTYpeCount = 0;

	//��� �������� �־��ִ� ��
	int Item[3];
	int ItemCount;
	//��� ��
	int Money;

	//8������ ȸ������ ���� ����
	int FireBallCount = 4;


	tm * curr_tm;

	//DB���� ���� �����͸� �����س��´�
	void RandomItemSet(int ItemID,int ItemPer);

	//���� ���� ����ϱ� ���� �Լ�
	int Random(int min, int max);
	//źȯ ���Ⱚ ���
	XMFLOAT2 DirCalc(float px, float py, float bx, float by);

	GameServer* m_pServer;
};



