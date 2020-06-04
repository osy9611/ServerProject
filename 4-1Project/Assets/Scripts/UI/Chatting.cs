using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class Chatting : MonoBehaviour
{
    public static Chatting instance;

    public Image UI_chatWindowImage;
    public InputField UI_typingfield;
    public Text UI_typingText;
    public Text UI_chattingLog;
    
    public SendMessage Data; // server Protocol Resolve

    private bool _isActive; // 채팅창의 활성화 여부

    private string _sendMessageBuffer;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // 채팅을 보낼 때
        {
            if (UI_typingfield.text.Length > 0) // 채팅창에 뭔가를 쳤으면
            {
                Data.Init(GameManager.instance.PlayerName, UI_typingfield.text);
                JsonData SendData = JsonMapper.ToJson(Data);
                ServerClient.instance.Send(SendData.ToString());

                UI_chattingLog.text += "\n" + GameManager.instance.PlayerName + " : " + UI_typingfield.text;
                UI_typingfield.text = "";
                UI_typingfield.gameObject.SetActive(false);
            }
            else // 캐럿 ON / OFF
            {
                if (UI_typingfield.gameObject.activeSelf)
                {
                    UI_chatWindowImage.gameObject.SetActive(false);
                    UI_typingfield.gameObject.SetActive(false);
                    _isActive = false;
                    return;
                }
                else
                {
                    UI_chatWindowImage.gameObject.SetActive(true);
                    UI_typingfield.gameObject.SetActive(true);
                    UI_typingfield.ActivateInputField();
                    _isActive = true;
                }
            }
        }
        if (_sendMessageBuffer != null)
        {
            UI_chattingLog.text += _sendMessageBuffer;
            _sendMessageBuffer = null;
        }
    }

    public void ReceiveComment(JsonData _data)
    {
        _sendMessageBuffer += "\n" + _data["username"].ToString() + " : " + _data["message"].ToString();
    }

    public void PutSystemMessage(string message, string color = null) // 외부에서 메세지를 넣어주는 함수.
    {
        if (color == null) // 색깔이 없을 경우(기본 흰색)
            UI_chattingLog.text += "\n" + message;
        else // 색깔이 있을 경우
            UI_chattingLog.text += "\n" + "<color=" + color + ">" + message + "</color>";
    }

    public bool CheckActive() // 채팅창이 열렸는지 안 열렸는지를 반환하는 함수
    {
        return _isActive;
    }
}
