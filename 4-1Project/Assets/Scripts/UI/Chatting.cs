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

    private bool _isActive; // ä��â�� Ȱ��ȭ ����

    private string _sendMessageBuffer;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // ä���� ���� ��
        {
            if (UI_typingfield.text.Length > 0) // ä��â�� ������ ������
            {
                Data.Init(GameManager.instance.PlayerName, UI_typingfield.text);
                JsonData SendData = JsonMapper.ToJson(Data);
                ServerClient.instance.Send(SendData.ToString());

                UI_chattingLog.text += "\n" + GameManager.instance.PlayerName + " : " + UI_typingfield.text;
                UI_typingfield.text = "";
                UI_typingfield.gameObject.SetActive(false);
            }
            else // ĳ�� ON / OFF
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

    public void PutSystemMessage(string message, string color = null) // �ܺο��� �޼����� �־��ִ� �Լ�.
    {
        if (color == null) // ������ ���� ���(�⺻ ���)
            UI_chattingLog.text += "\n" + message;
        else // ������ ���� ���
            UI_chattingLog.text += "\n" + "<color=" + color + ">" + message + "</color>";
    }

    public bool CheckActive() // ä��â�� ���ȴ��� �� ���ȴ����� ��ȯ�ϴ� �Լ�
    {
        return _isActive;
    }
}
