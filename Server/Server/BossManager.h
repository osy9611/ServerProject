#pragma once
class BossManager
{
public:
	BossManager();
	~BossManager();

	void SetBoosData(int Data);
	void RandomSet();
private:
	float Hp = 100;

	//보스 드랍 데이터
	BossData DropData;

	DBManager *dbManager;
};

