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

	//���� ��� ������
	BossData DropData;

	DBManager *dbManager;
};

