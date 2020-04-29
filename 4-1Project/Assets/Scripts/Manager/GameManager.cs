using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LitJson;

[System.Serializable]
public struct OtherPlayerInfo
{
    public string Name;
    public int type;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //플레이어 이름
    public string PlayerName;
    //플레이어 타입(캐릭터)
    public int type;

    //게임스폰 포인트를 잡아주는 데이터
    public string GameSpawnData;

    //다른 플레이어의 정보를 저장
    public List<OtherPlayerInfo> playerInfo;

    //플레이어 영웅
    public List<GameObject> Heros;
    //영웅 정보들
    [Header("영웅들을 저장하는 곳입니다 영웅캐릭터 프리팹을 올려주세요(로비에서 사용하는 것은 로비 씬에 있음)")]
    public List<GameObject> ServerHeros;

    [Header("보스 프리팹 저장")]
    public GameObject boss;

    //생성을 위한 함수
    public bool MakeNow;

    //파괴를 위한 함수
    public bool DeleteNow;
    public int DeleteNum;

    //준비가 됨을 체크
    public bool PlayerReady;

    //게임을 시작할때 사용
    public bool GameStart;

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(gameObject);
        
    }

    //로비에서 캐릭터를 생성할때 사용 
    public void AddCharactor(JsonData Data)
    {
        for (int i = 0; i < int.Parse(Data["Count"].ToString()); i++)
        {
            if (Data["UserList"][i]["ID"].ToString() != PlayerName)
            {
                OtherPlayerInfo dummy;
                dummy.Name = Data["UserList"][i]["ID"].ToString();
                dummy.type = int.Parse(Data["UserList"][i]["Type"].ToString());
                if (!playerInfo.Contains(dummy))
                {
                    playerInfo.Add(dummy);                    
                }
            }
        }
        MakeNow = true;
    }
    
    public void DeleteCharactor(JsonData Data)
    {
        for(int i=0;i<playerInfo.Count;i++)
        {
            if(Data["Name"].ToString() == playerInfo[i].Name)
            {
                playerInfo.RemoveAt(i);
                DeleteNow = true;
                DeleteNum = i;
                break;
            }
        }
    }


    public void SceneChange(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
}
