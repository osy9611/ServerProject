using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.UI;

public class MakeManager : MonoBehaviour
{
    public List<Transform> SpawnPoint;

    public CameraMove cameraMove;

    public List<Text> texts;

    // Start is called before the first frame update
    void Awake()
    {
        if(GameManager.instance.GameSpawnData != "")
        {
            JsonData Data = JsonMapper.ToObject(GameManager.instance.GameSpawnData);

            for (int i = 0; i < Data["SessionIDList"].Count; i++)
            {
                if (GameManager.instance.PlayerName == Data["SessionIDList"][i]["SessionID"].ToString())
                {
                    GameObject obj = Instantiate(GameManager.instance.Heros[GameManager.instance.type]
                               , SpawnPoint[i].position, Quaternion.identity);
                    obj.name = GameManager.instance.PlayerName;
                    obj.GetComponent<Player_Actor>().PosText = texts[i];
                    cameraMove.target = obj;
                }
                else 
                {
                    
                    for (int j = 0; j < GameManager.instance.playerInfo.Count; j++)
                    {
                        if (GameManager.instance.playerInfo[j].Name == Data["SessionIDList"][i]["SessionID"].ToString())
                        {
                            GameObject obj = Instantiate(GameManager.instance.ServerHeros[GameManager.instance.playerInfo[j].type]
                                , SpawnPoint[i].position, Quaternion.identity);
                            obj.name = GameManager.instance.playerInfo[j].Name;
                            obj.GetComponent<OhterPlayer_Actor>().PosText = texts[i];
                            OtherPlayerManager.instance.PlayerList.Add(GameManager.instance.playerInfo[j].Name, obj.GetComponent<OhterPlayer_Actor>());
                        }
                         
                    }
                }
            
            }
        }
      
    }
    
}
