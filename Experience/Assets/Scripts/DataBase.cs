using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBase : MonoBehaviour
{
    public List<ItemInfo> itemList = new List<ItemInfo>();
    public List<MixInfo> mixList = new List<MixInfo>();

    // ItemID , ItemName, ItemDes, ItemSpriteFilename
    private void Start()
    {
        // Item Information
        itemList.Add(new ItemInfo(1, "50원", "50원입니다.", "50WON"));
        itemList.Add(new ItemInfo(2, "100원", "100원입니다.", "100WON"));
        itemList.Add(new ItemInfo(3, "500원", "500원입니다.", "500WON"));
        itemList.Add(new ItemInfo(4, "골드바", "매우 비싼 골드바", "goldbar"));

        // Mix Information
        mixList.Add(new MixInfo(4, 1, 2, 3));
    }
}
