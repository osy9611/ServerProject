using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;
using System.Collections.Generic;

[System.Serializable]
struct OtherHP
{
    public string nickname;
    public int otherHP;
    public int otherFullHP;
}

public class HPManager : MonoBehaviour
{
    public static HPManager instance;

    public Image image_myHP;
    public int myHP;
    public float myFullHP;

    public List<Image> image_otherHP;
    public List<string> nickname;
    public List<float> otherFullHP;

    private bool isSend;
    private int send_hp;
    private string send_nickname;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        switch(GameManager.instance.type) // 직업에 따른 내 HP 최대체력 지정
        {
            case 0: // 전사
                myHP = 500;
                break;
            case 1: // 마법사
                myHP = 300;
                break;
        }

        myFullHP = myHP;

        for (int i = 0; i < GameManager.instance.playerInfo.Count; i++) // 다른 플레이어의 최대체력 지정
        {
            switch(GameManager.instance.playerInfo[i].type) // 다른 플레이어들의 최대HP값을 저장
            {
                case 0: // 전사
                    otherFullHP.Add(500); 
                    break;
                case 1: // 마법사
                    otherFullHP.Add(300);
                    break;
            }
            nickname.Add(GameManager.instance.playerInfo[i].Name);
        }
    }

    private void Update()
    {
        if(isSend)
        {
            for (int i = 0; i < nickname.Count; i++)
            {
                if (nickname[i] == send_nickname)
                {
                    image_otherHP[i].fillAmount = send_hp / otherFullHP[i];
                    isSend = false;
                    break;
                }
            }
        }
    }
    public void ReceiveHp(JsonData Data)
    {
        send_hp = int.Parse(Data["HP"].ToString());
        send_nickname = Data["nickname"].ToString();
        
        isSend = true;
    }
    
    public void SetHP()
    {
        image_myHP.fillAmount = myHP / myFullHP;
    }
}
