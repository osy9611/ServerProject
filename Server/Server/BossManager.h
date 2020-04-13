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
	//���� ��� ������
	BossData DropData;
	//DB �Ŵ���
	DBManager *dbManager;

	//��� �������� �־��ִ� ��
	int Item[3];
	int ItemCount;
	//��� ��
	int Money;

	//DB���� ���� �����͸� �����س��´�
	void RandomSet(int ItemID,int ItemPer);
};

