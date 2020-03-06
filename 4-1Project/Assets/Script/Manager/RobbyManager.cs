using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
[System.Serializable]
public struct Charactor
{
    public List<GameObject> charactor;
}

public class RobbyManager : MonoBehaviour
{
    public static RobbyManager instance;

    public Transform[] RobbySpawn;
    public GameObject[] Charactor;

    public int cnt;

    List<string> Name;
    List<int> type;

    //생성될 캐릭터들을 집넣음
    [SerializeField]
    public List<Charactor> charactors;
    Charactor dummy;    //캐릭터를 넣어줄 더미

    public Transform OtherCharacotors;

    //캐릭터 변경을 위한 더미 데이터
    int dummytype;
    string dummyName;

    //캐릭터 변경 관련
    bool changeNow;

    //래디 변경 관련
    bool ReadyNow;

    //내 캐릭터 텍스트
    public Text MyReady;

    //텍스트
    public Text[] ReadyText;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        Name = new List<string>();
        type = new List<int>();

        charactors = new List<Charactor>();
        dummy.charactor = new List<GameObject>();

        //내 캐릭터 닉네임 텍스트에 넣음
        MyReady.text = GameManager.instance.PlayerName;

        for (int i=0;i< RobbySpawn.Length; i++)
        {
            Charactor a;
            a = new Charactor();
            a.charactor = new List<GameObject>();
            for (int j=0;j<Charactor.Length;j++)
            {
                GameObject obj = Instantiate(Charactor[j], RobbySpawn[i].position, Quaternion.identity);
                obj.transform.parent = OtherCharacotors;
                obj.SetActive(false);
                a.charactor.Add(obj);
            }

            charactors.Add(a);
        }
    }

    public void MakeCharactor()
    {
        for(int i=0;i< GameManager.instance.playerInfo.Count;i++)
        {
            if(!Name.Contains(GameManager.instance.playerInfo[i].Name))
            {
                Name.Add(GameManager.instance.playerInfo[i].Name);
                charactors[cnt].charactor[GameManager.instance.playerInfo[i].type].SetActive(true);
                type.Add(GameManager.instance.playerInfo[i].type);
                ReadyText[i].text = Name[i];
                cnt++;
            }
        }
    }

    public void DestroyCharactor()
    {
        charactors[GameManager.instance.DeleteNum].charactor[type[GameManager.instance.DeleteNum]].SetActive(false);
        Name.RemoveAt(GameManager.instance.DeleteNum);
        type.RemoveAt(GameManager.instance.DeleteNum);
        cnt--;

        for(int i=0;i<charactors.Count;i++)
        {
            if(i<= type.Count-1)
            {
                charactors[i].charactor[type[i]].SetActive(true);
                ReadyText[i].text = Name[i];
            }
            else
            {
                for(int j=0;j< charactors[i].charactor.Count;j++)
                {
                    charactors[i].charactor[j].SetActive(false);
                    ReadyText[i].text = "";
                    ReadyText[i].color = new Color(1, 1, 1, 1);
                }
            }
        }
    }

    public void ChangeCharactor(JsonData Data)
    {
        dummyName = Data["Name"].ToString();
        dummytype = int.Parse(Data["CharactorNum"].ToString());
        changeNow = true;
    }

    public void ChangeTypes()
    {
        for (int i = 0; i < Name.Count; i++)
        {
            if (Name[i] == dummyName)
            {
                charactors[i].charactor[type[i]].SetActive(false);
                type[i] = dummytype;
                OtherPlayerInfo player;
                player.Name = GameManager.instance.playerInfo[i].Name;
                player.type = dummytype;
                GameManager.instance.playerInfo[i] = player;
                charactors[i].charactor[type[i]].SetActive(true);
                break;
            }
        }
    }

    public void ReadyOn(JsonData Data)
    {
        dummyName = Data["Name"].ToString();
        ReadyNow = true;
    }

    public void ReadyOn()
    {
        for (int i = 0; i < Name.Count; i++)
        {
            if (Name[i] == dummyName)
            {
                ReadyText[i].color = new Color(1, 0, 0, 1);
                break;
            }
        }
    }

    private void Update()
    {
        if (GameManager.instance.MakeNow)
        {
            GameManager.instance.MakeNow = false;
            MakeCharactor();

        }
        if(GameManager.instance.DeleteNow)
        {
            GameManager.instance.DeleteNow = false;
            DestroyCharactor();
        }

        if(changeNow)
        {
            changeNow = false;
            ChangeTypes();
        }

        if(ReadyNow)
        {
            ReadyNow = false;
            ReadyOn();
        }

        if(GameManager.instance.GameStart)
        {
            GameManager.instance.GameStart = false;
            GameManager.instance.SceneChange("InGame");
        }
    }
}
