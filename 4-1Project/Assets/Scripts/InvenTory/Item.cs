using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public int itemID; // 아이템 번호
    public string itemName; // 아이템 이름
    public string itemDescription; // 아이템 설명
    public Sprite itemIcon; // 아이템 아이콘
    public int itemCount;

    public Item()
    {
        itemID = 0;
        itemName = "";
        itemDescription = "";
        itemIcon = null;
        itemCount = 0;
    }

    public Item(int _itemID, string _itemName, string _itemDes, string _itemFilename="") // Item 클래스에 대한 생성자
    {
        itemID = _itemID;
        itemName = _itemName;
        itemDescription = _itemDes;
        itemIcon = Resources.Load(_itemFilename, typeof(Sprite)) as Sprite;
        itemCount = 1;
    }

    public Item Init() // 깊은 복사
    {
        Item obj = new Item(itemID, itemName, itemDescription);
        obj.itemID = itemID;
        obj.itemName = itemName;
        obj.itemDescription = itemDescription;
        obj.itemIcon = itemIcon;
        obj.itemCount = itemCount;
        return obj;
    }
}
