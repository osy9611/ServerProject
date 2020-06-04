using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class ControlUI : MonoBehaviour
{
    public GameObject[] openInventory;

    private void Update()
    {
        if (!Chatting.instance.CheckActive()) // 채팅창이 활성화되어있는동안 다른 UI는 열고 닫을 수 없다.
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (openInventory[0].activeSelf)
                {
                    for (int i = 0; i < openInventory.Length; i++)
                        openInventory[i].SetActive(false);
                }
                else
                {
                    for (int i = 0; i < openInventory.Length; i++)
                        openInventory[i].SetActive(true);
                }
                return;
            }
        }
    }
}
