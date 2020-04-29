using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class ControlUI : MonoBehaviour
{
    public GameObject inventory;
    public InputField UI_typingfield;

    private void Update()
    {
        if (!UI_typingfield.isFocused) // 채팅창이 활성화되어있는동안 다른 UI는 열고 닫을 수 없다.
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (inventory.activeSelf)
                    inventory.SetActive(false);
                else
                    inventory.SetActive(true);
                return;
            }
        }
    }
}
