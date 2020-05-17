#include"GameServer.h"

//HANDLE SyncTimerTread;
//
//unsigned __stdcall SyncTimer(GameServer *p_Server)
//{
//	SyncPosition syncPosition;
//	syncPosition.Init();
//
//	while (true)
//	{
//		WaitForSingleObject(SyncTimerTread, 1000);
//
//		if (p_Server->RoomNames.size() != 0)
//		{
//			for (size_t i = 0; i < p_Server->RoomNames.size(); ++i)
//			{
//				if (p_Server->CheckStart(p_Server->RoomNames[i].c_str()) == true);
//				{
//					p_Server->SendAllPlayer(p_Server->RoomNames[i].c_str(), syncPosition.packet);
//					std::cout << "º¸³½´Ù" << std::endl;
//				}
//			}
//		}
//	}
//	
//	_endthread();
//	return 0;
//}