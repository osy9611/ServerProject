using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;
using System.Collections.Generic;

public class HPManager : MonoBehaviour
{
    public static HPManager instance;

    public Image image_myHP;
    public int myHP;
    public float myFullHP;

    public float invincibleTime;

    public List<Image> image_otherHP;
    public List<string> nickname;
    public List<float> otherFullHP;

    private bool isSend;
    private int send_hp;
    private float time;
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
        if(isSend) // 다른 플레이어에게서 체력 정보를 받으면
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

    public void SetHP() // 내 HP를 조정
    {
        image_myHP.fillAmount = myHP / myFullHP;
        CharacterInfoWindow.instance.UpdateHP(myHP);
    }
}
