using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

// 취급 주의
public class OtherPlayerManager : MonoBehaviour
{
    public static OtherPlayerManager instance;
    
    //public List<OtherPlayer> PlayerList;

    public Dictionary<string, Player_Server> PlayerList = new Dictionary<string, Player_Server>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
