using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItem : MonoBehaviour
{
    private void Start()
    {
    }
    public int itemID;
    private bool isGetItem;
    private void OnMouseDown()
    {
        Debug.Log(gameObject.name);
        isGetItem = Inventory.instance.GetItem(itemID); // true를 반환하면 아이템을 획득한 것으로 판단.
        if(isGetItem) 
            gameObject.SetActive(false);
    }
}
