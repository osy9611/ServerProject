using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInfo
{
    public int itemID; // 아이템 번호
    public string itemName; // 아이템 이름
    public string itemDescription; // 아이템 설명
    public Sprite itemIcon; // 아이템 아이콘

    public ItemInfo(int _itemID, string _itemName, string _itemDes, string _itemFilename) // ItemInfo 클래스에 대한 생성자
    {
        itemID = _itemID;
        itemName = _itemName;
        itemDescription = _itemDes;
        itemIcon = Resources.Load(_itemFilename, typeof(Sprite)) as Sprite;
    }
}
