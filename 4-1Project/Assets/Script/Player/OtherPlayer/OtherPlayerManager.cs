using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;


public class OtherPlayerManager : MonoBehaviour
{
    public static OtherPlayerManager instance;
    
    //public List<OtherPlayer> PlayerList;

    public Dictionary<string, OhterPlayer_Actor> PlayerList = new Dictionary<string, OhterPlayer_Actor>();

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
