using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private DataBase theDataBase; // 아이템, 조합 관련 데이터베이스 받아오기
    private InventorySlot[] slots; // 인벤토리 슬롯들

    private List<ItemInfo> inventoryItemlist; // 플레이어가 소지한 아이템 리스트

    public Transform tf; // slot 부모객체

    private void Start()
    {
        instance = this;
        inventoryItemlist = new List<ItemInfo>(); 
        slots = tf.GetComponentsInChildren<InventorySlot>(); // 슬롯을 전부 가져옴
        theDataBase = FindObjectOfType<DataBase>();
    }

    public bool GetItem(int _itemID)
    {
        for (int i = 0; i < theDataBase.itemList.Count; i++) // 플레이어 보유 아이템 리스트 for문
        {
            if (_itemID == theDataBase.itemList[i].itemID) // 습득한 아이템 번호와 아이템 리스트의 번호가 맞으면
            {
                for (int j = 0; j < slots.Length; j++) // 인벤토리 슬롯 for문
                {
                    if (slots[j].item.itemID == 0) // 아이템이 비어있는 슬롯을 찾으면
                    {
                        inventoryItemlist.Add(theDataBase.itemList[i]); // 플레이어 보유 아이템 리스트에 아이템을 넣어줌
                        slots[j].AddItem(theDataBase.itemList[i]); // 비어있는 슬롯에 아이템을 넣어줌
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
