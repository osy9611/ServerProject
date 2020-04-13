#pragma once
class BossManager
{
public:
	BossManager();
	~BossManager();

	void SetBossData(int Data);
	void ResetBossData();
	BossResult HitBoss(float Damage);
private:
	//Hp
	float Hp;
	//보스 드랍 데이터
	BossData DropData;
	//DB 매니저
	DBManager *dbManager;

	//드랍 아이템을 넣어주는 곳
	int Item[3];
	int ItemCount;
	//드랍 돈
	int Money;

	//DB에서 받은 데이터를 셋팅해놓는다
	void RandomSet(int ItemID,int ItemPer);
};

