using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInfo
{
    public string type;
    public int size;
    public string ID;
    public string PassWord;

    public void Init(string UserID, string UserPassWord)
    {
        type = "PlayerInfo";
        ID = UserID;
        PassWord = UserPassWord;
    }
}

public struct Ready
{
    public string type;
    public string Name;
    public void Init()
    {
        type = "Ready";
        if(GameManager.instance != null)
        {
            Name = GameManager.instance.PlayerName;
        }
    }
}

public struct PlayerData
{
    public string type;
    public string name;

    //방향값
    public double x, y;
    //회전값
    public double rx, ry;
    //공격값
    public double ax, ay;
    //현재 위치
    public double nx, ny;
    //시간
    public double time;
    //속도
    public double Speed;
    //현재 상태
    public int State;

    public void Init(string Name)
    {
        type = "PlayerData";
        name = Name;
    }
}

public struct NowState
{
    public string type;
    public string name;
    public double x, y;
    public double nx, ny;
    public double time;

    public void Init(string Name)
    {
        type = "NowState";
        name = Name;
    }
}

public struct OtherPlayerData
{
    public string name;
    public double playerinfo;
}

public struct FindFriend
{
    public string type;
    public string FriendName;

    public void Init()
    {
        type = "FindFriend";
    }
}

public struct InviteResult
{
    public string type;
    public int Answer;
    public string FriendName;

    public void Init()
    {
        type = "InviteResult";
    }
}

public struct ChanrageCharactor
{
    public string type;
    public int CharactorNum;
    public string Name;
    public void Init()
    {
        type = "ChangeType";
    }
}

[System.Serializable]
public struct MonsterData
{
    public string type;
    //플레이어의 현재 위치
    public double x;
    public double y;

    public double mx;
    public double my;

    public string MonsterName;
    public string UserName;
    public void Init(string User,string Monster)
    {
        type = "MonsterCheck";
        this.UserName = User;
        this.MonsterName = Monster;
    }
}

public struct MosterMoving
{
    public string type;
    //public double x;
    //public double y;

    public List<double> x;
    public List<double> y;
    public double dx;
    public double dy;

    public string MonsterName;

    public void Init(string Monster)
    {
        type = "MonsterMove";
        this.MonsterName = Monster;

        x = new List<double>();
        y = new List<double>();
    }
}

public struct ItemGet
{
    public string type;

    public int itemID;
    // public string itemName;

    public void Init(int itemID)
    {
        type = "ItemGet";
        this.itemID = itemID;
    }
}

public struct ItemMix
{
    public string type;
    public int[] itemID;
    public int[] itemCount;
    public int money;

    public void Init(int[] _itemID, int[] _itemCount, int _money)
    {
        type = "ItemMix";
        itemID = _itemID;
        itemCount = _itemCount;
        money = _money;
    }
}
