using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip instance;
    public Text itemName;
    public Text itemDes;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void ShowItemInfo(string _itemName, string _itemDes)
    {
        gameObject.SetActive(true);
        itemName.text = _itemName;
        itemDes.text = _itemDes;
    }
}
