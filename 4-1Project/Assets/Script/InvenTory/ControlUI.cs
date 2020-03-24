using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlUI : MonoBehaviour
{
    public GameObject inventory = default;

    private void Awake()
    {
        inventory.SetActive(true);
        inventory.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
            inventory.active = !inventory.active;
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.
        }
    }
}
