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

public struct SendMessage
{
    public string type;
    public string username;
    public string message;

    public void Init(string _username, string _message)
    {
        type = "SendMessage";
        username = _username;
        message = _message;
    }
}

public struct SendShareInvInfo
{
    public string type;
    public int arrayNum;
    public int itemNum;

    public void Init(int _arrayNum, int _itemNum)
    {
        type = "SendShareInvInfo";
        arrayNum = _arrayNum;
        itemNum = _itemNum;
    }
}

public struct SendShareSwapInfo
{
    public string type;
    public int arrayNum_1;
    public int arrayNum_2;

    public void Init(int _arrayNum_1, int _arrayNum_2)
    {
        type = "SendShareSwapInfo";
        arrayNum_1 = _arrayNum_1;
        arrayNum_2 = _arrayNum_2;
    }
}

public struct SendShareDeleteInfo
{
    public string type;
    public int arrayNum;

    public void Init(int _arrayNum)
    {
        type = "SendShareDeleteInfo";
        arrayNum = _arrayNum;
    }
}

public struct Phase
{
    public string type;
    public double px, py; // 플레이어 x, y
    public double bx, by; // 보스 x, y
    public bool isStart;

    public void Init(bool _isStart, double _px = 0, double _py = 0, double _bx = 0, double _by = 0)
    {
        type = "Phase";
        px = _px;
        py = _py;
        bx = _bx;
        by = _by;
        isStart = _isStart;
    }
}

public struct BossDamage // 플레이어가 보스를 때리면 서버에서 보스 체력을 관리하기 때문에 방어력까지 계산해서 보내준다.
{
    public string type;
    public int damage;

    public void Init()
    {
        type = "BossDamage";
    }
}

public struct PlayerDamage // 보스 탄환이 플레이어를 때리면 다른 플레이어들에게 내 HP를 알려줘야하므로 내 HP를 서버로 전송한다
{
    public string type;

    public int HP;
    public string nickname;

    public void Init()
    {
        type = "PlayerDamage";
    }
}

public struct PhaseRestart
{
    public string type;

    public void Init()
    {
        type = "PhaseRestart";
    }
}

public struct PhaseEnd
{
    public string type;

    public void Init()
    {
        type = "PhaseEnd";
    }
}

public struct PhaseTimeEnd
{
    public string type;

    public void Init()
    {
        type = "PhaseTimeEnd";
    }
}




