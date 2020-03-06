using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance;
   
    //서버에 보낼 구조체들
    FindFriend findFriend;
    InviteResult inviteResult;
    ChanrageCharactor changeCharactor;
    Ready ready;

    public InputField inputField;

    //시스템 메시지
    public Text SystemMessage;
    public string Message;

    //초대 UI
    public GameObject InviteWindow;
    public Text InviteMessage;
    bool InviteWindowOn;
    public string OtherUserName;
    
    //캐릭터 변경할 리스트
    public List<GameObject> charactors;

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
        
        findFriend.Init();
        inviteResult.Init();
        changeCharactor.Init();
        ready.Init();
    }
    
    //서버에서 받은 데이터중 UI에 관련된 메시지를 처리해준다.
    public void Check(JsonData Data)
    {
        switch (Data["UItype"].ToString())
        {
            case "Chatting":

                break;
            case "InviteMessage":
                InviteWindowOn = true;
                OtherUserName = Data["Message"].ToString() + "님이 초대를 했습니다!";
                break;
            case "NoUserMessage":
                Message = "유저가 없습니다";
                break;
        }
    }

    //아이디를 입력해서 게임 메니져에 있는 PlayerName이라는 함수에 넣어준다
    public void WriteID(Text text)
    {
        GameManager.instance.PlayerName = text.text;
    }


    //엔터를 누르면 닉네임을 비교한다 같은 닉네임이면 경고 메시지를 뿌리고 그게 아니면 서버에 접속해있는 데이터를
    //검색한다
    public void InputFriendName(Text text)
    {
        findFriend.FriendName = text.text;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(findFriend.FriendName != GameManager.instance.PlayerName)
            {
                JsonData Data = JsonMapper.ToJson(findFriend);
                ServerClient.instance.Send(Data.ToString());
                //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Data.ToString());
                //TransportTCP.instance.Send(buffer, buffer.Length);
                inputField.text = null;
            }
            else
            {
                SystemMessage.text = "자신의 닉네임을 입력하셨습니다 !";
                Invoke("ResetSystemMessage", 1.0f);
            }
          
        }
    }
    

    //시스템 메시지를 초기화해줌
    public void ResetSystemMessage ()
    {
        SystemMessage.text = null;
    }

    //친구 찾기 기능을 키고 끄는 함수 
    public void FindFriendOn(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void FindFriendOff(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void InviteYes(GameObject obj)
    {
        inviteResult.Answer = 0;
        JsonData Data = JsonMapper.ToJson(inviteResult);
        ServerClient.instance.Send(Data.ToString());
        //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Data.ToString());
        //TransportTCP.instance.Send(buffer, buffer.Length);
        InviteWindowOn = false;
        obj.SetActive(false);

    }

    public void InviteNo(GameObject obj)
    {
        inviteResult.Answer = 1;
        JsonData Data = JsonMapper.ToJson(inviteResult);
        ServerClient.instance.Send(Data.ToString());
        //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Data.ToString());
        //TransportTCP.instance.Send(buffer, buffer.Length);
        InviteWindowOn = false;
        obj.SetActive(false);
    }

    public void ChangeType(int type)
    {
        if (!GameManager.instance.PlayerReady)
        {
            charactors[GameManager.instance.type].SetActive(false);
            charactors[type].SetActive(true);
            GameManager.instance.type = type;
            changeCharactor.Name = GameManager.instance.PlayerName;
            changeCharactor.CharactorNum = type;
            JsonData Data = JsonMapper.ToJson(changeCharactor);
            ServerClient.instance.Send(Data.ToString());
            //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Data.ToString());
            //TransportTCP.instance.Send(buffer, buffer.Length);
        }
    }

    public void Ready()
    {
        if (GameManager.instance.PlayerReady)
        {
            GameManager.instance.PlayerReady = false;
            RobbyManager.instance.MyReady.color = new Color(1, 1, 1, 1);
        }
        else
        {
            GameManager.instance.PlayerReady = true;
            RobbyManager.instance.MyReady.color = new Color(1, 0, 0, 1);
        }

       
        JsonData Data = JsonMapper.ToJson(ready);
        ServerClient.instance.Send(Data.ToString());
        //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Data.ToString());
        //TransportTCP.instance.Send(buffer, buffer.Length);
    }

    private void Update()
    {
        if(InviteWindowOn)
        {
            InviteWindow.SetActive(true);
            InviteMessage.text = OtherUserName;
        }

        if(Message != "")
        {
            SystemMessage.text = Message;
            Invoke("ResetSystemMessage", 1.0f);
            Message = "";
        }
    }
}
