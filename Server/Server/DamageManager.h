#pragma once
class DamageManager
{
public:
	DamageManager();
	~DamageManager();

	float PlayerDamageCalc(float Damage, float Armor);
	float BoosDamageCalc(float Damage, float Armmor);
};

