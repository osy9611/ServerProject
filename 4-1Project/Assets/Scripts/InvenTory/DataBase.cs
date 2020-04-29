using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 0 : 아이템 없음
// 1 ~ 100 : 재료
// 101 ~ 200 : 소비
// 201 ~ 300 : 장비

public class DataBase : MonoBehaviour
{
    public List<Item> itemList = new List<Item>();

    // ItemID , ItemName, ItemDes, ItemSpriteFilename
    private void Start()
    {
        // Material Item
        itemList.Add(new Item(1, "주문서", "주문서.", "alchemy"));
        itemList.Add(new Item(2, "에메랄드", "에메랄드.", "ashvattha"));
        itemList.Add(new Item(3, "물방울", "물방울.", "Ereronium"));
        itemList.Add(new Item(4, "헝겊", "헝겊.", "fabric"));
        itemList.Add(new Item(5, "가넷", "가넷.", "hernite"));
        itemList.Add(new Item(6, "고철", "고철.", "iron"));
        itemList.Add(new Item(7, "사파이어", "사파이어", "gaiter"));

        // Potion Item
        itemList.Add(new Item(101, "파우더", "피부", "powder"));

        // Equipment Item
        itemList.Add(new Item(201, "펜던트", "펜던트...", "pendant"));
    }
}
