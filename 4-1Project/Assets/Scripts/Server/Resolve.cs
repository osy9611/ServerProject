using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

/*
 * 네트워크의 특성상 서베어서 보내는 데이터를 클라는 패킷 단위로 받지 않는다. 한 번에 모두 받을 수 있고
 * 여러 번 나눠서 받을 수 있다.(즉, 서버는 send를 두 번 했는데, 클라에서는 receive 한 번만 발생하는 경우가 생김)
 * 이런 경우를 처리하기 위해서 먼저 받은 데이터를 PacketBuffer에 저장한 후 클라이언트에서 동시에 여러번 요청하면서 서버는 
 * 한 번에 다 받으므로, 각 요청별로 나누어 처리한다. 그리고 서버가 보낸 데이터 중 일부가 도착한 경우,
 * 우선은 처리하지 않고 남겨놓았다가 다음에 받은 데이터와 연결하여 처리한다.
 * 관련 링크는 
 * http://lab.gamecodi.com/board/zboard.php?id=GAMECODILAB_Lecture_series&no=63
 */
class Defines
{
    public static readonly int HEADERSIZE = 4;  //int 형은 크기가 4바이트이니까
}

public class Resolve
{
    byte[] PacketBuffer = new byte[2048];

    int PacketBufferMarker;
    // int MessageSize;
    int remain_byte;

    byte[] Header = new byte[Defines.HEADERSIZE];

    public Resolve()
    {
        this.PacketBufferMarker = 0;
        // this.MessageSize = 0;
        this.remain_byte = 0;
    }

    public void ReadMessage(byte[] msgByte, int recvBytes)
    {
        this.remain_byte = recvBytes;

        //받은 데이터를 패킷 버퍼에 저장한다
        Array.Copy(msgByte, 0, PacketBuffer, PacketBufferMarker, recvBytes);

        int nPacketData = this.PacketBufferMarker + msgByte.Length;
        int nReadData = 0;

        while (nPacketData > 0)    //받은 데이터를 모두 처리할 때까지 반복한다
        {
            //남은 데이터가 패킷 헤더보다 작으면 중단한다
            if (nPacketData < Defines.HEADERSIZE)
            {
                break;
            }

            Array.Copy(PacketBuffer, nReadData, Header, 0, Defines.HEADERSIZE);
            int size = BitConverter.ToInt32(Header, 0);

            if (size <= nPacketData)   //일단 처리할 수 있는 만큼의 데이터가 있다면 패킷을 처리한다
            {
                string message = System.Text.Encoding.UTF8.GetString(PacketBuffer, nReadData + Defines.HEADERSIZE, size - 4);
                CheckData(message);
                nPacketData -= size;
                nReadData += size;
            }
            else
            {
                break;  //패킷으로 처리할 수 없는 만큼이 아니면 중단한다
            }
        }

        if (nPacketData > 0)   //만약 패킷 데이터 크기가 남아있다면 다시 패킷 데이터에 다시 넣어줌
        {
            byte[] TempBuffer = new byte[2048];
            Array.Copy(PacketBuffer, nReadData, TempBuffer, 0, nPacketData);
            Array.Copy(TempBuffer, 0, PacketBuffer, 0, nPacketData);
        }

        //남은 데이터 양을 저장하고 데이터 받기를 요청한다
        PacketBufferMarker = nPacketData;
    }


    //Json으로 받은 데이터들을 파싱해서 타입별로 처리함
    public void CheckData(string data)
    {
        try
        {
            //Debug.Log(data);
            JsonData Data = JsonMapper.ToObject(data);
            switch (Data["type"].ToString())
            {
                case "UI":
                    RobbyUI.instance.Check(Data);
                    break;
                case "PlayerData":
                    if (OtherPlayerManager.instance.PlayerList.ContainsKey(Data["name"].ToString()))
                        OtherPlayerManager.instance.PlayerList[Data["name"].ToString()].SetInput(Data);
                    break;
                case "UserInfo":
                    GameManager.instance.AddCharactor(Data);
                    break;
                case "UserOut":
                    GameManager.instance.DeleteCharactor(Data);
                    break;
                case "ChangeType":
                    RobbyManager.instance.ChangeCharactor(Data);
                    break;
                case "Ready":
                    RobbyManager.instance.ReadyOn(Data);
                    if (bool.Parse(Data["Start"].ToString()))
                    {
                        GameManager.instance.GameSpawnData = data;
                        GameManager.instance.GameStart = true;
                    }
                    break;
                case "SendMessage":
                    Chatting.instance.ReceiveComment(Data);
                    break;
                case "ItemMixResult":
                    Inventory.instance.ReceiveMixResult(Data);
                    break;
                case "SendShareInvInfo":
                    Inventory.instance.UpdateShareInfo(Data);
                    break;
                case "RandomLaser": // 랜덤레이저
                    PatternManager.instance.LoadRandomLaser(Data);
                    break;
                case "CircleBulletType":  //원형 탄환
                    PatternManager.instance.LoadInduceCircleBullet(Data);
                    break;
                case "CircleFloor":
                    
                    PatternManager.instance.LoadInduceCircleFloor(Data);
                    break;
                case "TimerOn":
                    Debug.Log("들어옴!");
                    PatternManager.instance.LimitTimeOn();
                    break;
                case "PhaseRestart":
                    PatternManager.instance.PatternRestart();
                    break;
                case "PhaseStart":
                    PatternManager.instance.PatternStart(Data);
                    break;
                case "BossHp":
                    Debug.Log("패턴 재시작");
                    Boss.instance.SetHP(Data);
                    break;
                case "PlayerDamage":
                    Debug.Log("공격받음");
                    HPManager.instance.ReceiveHp(Data);
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            Debug.Log("데이터를 받는 도중에 오류 " + data);
        }
    }
}
