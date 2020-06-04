using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.UI;
using Cinemachine;

public class MakeManager : MonoBehaviour
{
    public List<Transform> SpawnPoint;
    public Transform spawnPoint_boss;

    private GameObject obj_temp;
    
    public GameObject[] otherPlayerHPBar;

    public Text hpBar_mynickname;
    public Text[] hpBar_othernickname;

    public CinemachineVirtualCamera cine_Camera;

    // Start is called before the first frame update
    void Awake()
    {
        if(GameManager.instance.GameSpawnData != "")
        {
            JsonData Data = JsonMapper.ToObject(GameManager.instance.GameSpawnData);

            for (int i = 0; i < Data["SessionIDList"].Count; i++) // 플레이어 및 서버플레이어 소환
            {
                if (GameManager.instance.PlayerName == Data["SessionIDList"][i]["SessionID"].ToString())
                {
                    obj_temp = Instantiate(GameManager.instance.Heros[GameManager.instance.type]
                               , SpawnPoint[i].position, Quaternion.identity); // 플레이어 생성
                    obj_temp.name = GameManager.instance.PlayerName; // 플레이어 이름 변경
                    cine_Camera.Follow = obj_temp.transform; // 플레이어에게 시네머신 적용
                    GameManager.instance._player = obj_temp.GetComponent<Player>(); // 게임매니저에 플레이어 스크립트 적용
                    hpBar_mynickname.text = GameManager.instance.PlayerName; // 플레이어 HP바 닉네임 적용
                }
                else 
                {
                    for (int j = 0; j < GameManager.instance.playerInfo.Count; j++)
                    {
                        if (GameManager.instance.playerInfo[j].Name == Data["SessionIDList"][i]["SessionID"].ToString())
                        {
                            obj_temp = Instantiate(GameManager.instance.ServerHeros[GameManager.instance.playerInfo[j].type]
                                , SpawnPoint[i].position, Quaternion.identity);
                            obj_temp.name = GameManager.instance.playerInfo[j].Name;
                            OtherPlayerManager.instance.PlayerList.Add(GameManager.instance.playerInfo[j].Name, obj_temp.GetComponent<Player_Server>());

                            // HpBar 할당
                            otherPlayerHPBar[j].SetActive(true); // 서버 플레이어 HP바 활성화
                            hpBar_othernickname[j].text = GameManager.instance.playerInfo[j].Name; // HpBar에 닉네임 삽입
                        }
                    }
                }
            }

            // 보스 소환
            obj_temp = Instantiate(GameManager.instance.boss, spawnPoint_boss.position, Quaternion.identity);
            obj_temp.name = "Boss";

            // 브금 재생
            // SoundManager.instance.PlayBGM("TalesWeaver");
        }
    }
}
