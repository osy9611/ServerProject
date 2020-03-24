using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class GetItem : MonoBehaviour
{
    public int itemID;
    private bool isGetItem;

    public ItemGet Data;

    private void Start()
    {
        Data.Init(itemID);
    }

    //private void OnMouseDown()
    //{
    //    Debug.Log(gameObject.name);
    //    isGetItem = Inventory.instance.GetItem(itemID); // true를 반환하면 아이템을 획득한 것으로 판단.
    //    if (isGetItem)
    //    {
    //        JsonData SendData = JsonMapper.ToJson(Data);
    //        ServerClient.instance.Send(SendData.ToString());
    //        gameObject.SetActive(false);
    //    }
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player_Actor player_Actor = collision.gameObject.GetComponent<Player_Actor>();
        if(player_Actor != null)
        {
            isGetItem = Inventory.instance.GetItem(itemID); // true를 반환하면 아이템을 획득한 것으로 판단.
            if (isGetItem)
            {
                JsonData SendData = JsonMapper.ToJson(Data);
                ServerClient.instance.Send(SendData.ToString());
                gameObject.SetActive(false);
            }
        }
    }
}
