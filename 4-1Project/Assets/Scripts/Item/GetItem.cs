using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.UI;

public class GetItem : MonoBehaviour
{
    public int itemID;
    private bool isGetItem;

    public ItemGet Data;

    private void Start()
    {
        Data.Init(itemID);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == GameManager.instance.PlayerName)
        {
            isGetItem = Inventory.instance.GetItem(itemID); // true를 반환하면 아이템을 획득한 것으로 판단.
            if (isGetItem)
                gameObject.SetActive(false);
        }
        else
            gameObject.SetActive(false);
    }
}
