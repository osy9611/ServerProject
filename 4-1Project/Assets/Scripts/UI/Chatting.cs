using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class Chatting : MonoBehaviour
{
    public static Chatting instance;

    public InputField UI_typingfield;
    public Text UI_typingText;
    public Text UI_chattingLog;
    
    public SendMessage Data; // server Protocol Resolve

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

                UI_chattingLog.text += GameManager.instance.PlayerName + " : " + UI_typingfield.text + "\n";
                UI_typingfield.text = "";
                UI_typingfield.gameObject.SetActive(false);
            }
            else // 캐럿 ON / OFF
            {
                if (UI_typingfield.gameObject.activeSelf)
                {
                    UI_typingfield.gameObject.SetActive(false);
                    return;
                }
                else
                {
                    UI_typingfield.gameObject.SetActive(true);
                    UI_typingfield.ActivateInputField();
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
        _sendMessageBuffer += _data["username"].ToString() + " : " + _data["message"].ToString() + "\n";
    }
}
