using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private DataBase theDataBase; // 아이템, 조합 관련 데이터베이스 받아오기

    private InventorySlot[] inventorySlots; // 인벤토리 슬롯
    private MixMaterialSlot[] mixMaterialSlots; // 조합 슬롯
    private MixResultSlot mixResultSlot; // 조합결과 슬롯

    private int[] mat_itemID; // 조합 재료 아이템 번호를 정렬하기 위한 배열
    private int[] mat_itemCount; // 조합 재료 아이템 갯수를 정렬하기 위한 배열
    public Transform inv_slot, mix_slot;

    public ItemMix Data;

    private void Awake()
    {
        instance = this;
        inventorySlots = inv_slot.GetComponentsInChildren<InventorySlot>();
        mixMaterialSlots = mix_slot.GetComponentsInChildren<MixMaterialSlot>();
        mixResultSlot = mix_slot.GetComponentInChildren<MixResultSlot>();
        theDataBase = FindObjectOfType<DataBase>();

        mat_itemID = new int[mixMaterialSlots.Length];
        mat_itemCount = new int[mixMaterialSlots.Length];
    }

    public bool GetItem(int _itemID) // 인게임 필드에서 새로운 아이템을 획득할 경우에 호출하는 함수.
    {
        for (int i = 0; i < theDataBase.itemList.Count; i++) // 아이템 데이터베이스
        {
            if(theDataBase.itemList[i].itemID == _itemID) // 데이터베이스 아이템 ID == 습득한 아이템 ID
            {
                for (int j = 0; j < inventorySlots.Length; j++) // 인벤토리에 같은 ID의 아이템을 보유하고 있는지를 판단.
                {
                    if (inventorySlots[j].item.itemID == _itemID) // 같은 ID의 아이템을 보유하고 있으면
                    {
                        inventorySlots[j].PlusItemCount();
                        return true;
                    }
                }

                for (int j = 0; j < inventorySlots.Length; j++) // 인벤토리 슬롯 for문
                {
                    if (inventorySlots[j].item.itemID == 0) // 아이템이 비어있는 슬롯을 찾으면
                    {
                        inventorySlots[j].AddItem(theDataBase.itemList[i]); // 비어있는 슬롯에 아이템을 넣어줌
                        return true;
                    }
                }
            }
        }
        return false;
    }
    
    // 타 슬롯에서 인벤토리 슬롯으로 아이템이 넘어올 때, 슬롯에 같은 ID의 아이템 유무를 판별함.
    public InventorySlot SearchInventorySlot(int _itemID) 
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].item.itemID == _itemID)
                return inventorySlots[i];
        }
        return null;
    }

    // 타 슬롯에서 조합 슬롯으로 아이템이 넘어올 때, 슬롯에 같은 ID의 아이템 유무를 판별함.
    public MixMaterialSlot SearchMixMaterialSlot(int _itemID)
    {
        for(int i=0;i<mixMaterialSlots.Length;i++)
        {
            if (mixMaterialSlots[i].item.itemID == _itemID)
                return mixMaterialSlots[i];
        }
        return null;
    }

    public void CheckMaterial() // 조합 판단
    {
        for (int i = 0; i < 3; i++)
        {
            mat_itemID[i] = mixMaterialSlots[i].item.itemID;
            mat_itemCount[i] = mixMaterialSlots[i].item.itemCount;
        }

        for (int i = 0; i < 2; i++)
        {
            for (int j = i + 1; j < 3; j++)
            {
                if (mat_itemID[i] > mat_itemID[j])
                {
                    int temp = mat_itemID[i];
                    mat_itemID[i] = mat_itemID[j];
                    mat_itemID[j] = temp;

                    temp = mat_itemCount[i];
                    mat_itemCount[i] = mat_itemCount[j];
                    mat_itemCount[j] = temp;
                }
            }
        }

        // 지금은 ABC템 각 1개씩으로만 조합을 만들 수 있음.
        // 추가적으로 아이템별 갯수도 조합에 영향을 주면 임시 배열 2차원 3*2로 만들고 중복계산 한번만 더 해주면 됨.
        for (int i = 0; i < theDataBase.mixList.Count; i++)
        {
            if (theDataBase.mixList[i].itemID_1 == mat_itemID[0] &&
                theDataBase.mixList[i].itemID_2 == mat_itemID[1] &&
                theDataBase.mixList[i].itemID_3 == mat_itemID[2]) // 데이터베이스 조합 정보와 조합슬롯의 아이템 ID와 일치하면
            {
                for (int j = 0; j < 3; j++)
                    mixMaterialSlots[j].RemoveItem(); // 조합 슬롯의 아이템을 모두 없앤 다음

                for (int j = 0; j < theDataBase.itemList.Count; j++) // 아이템 데이터베이스에서 ID에 맞는 아이템을 찾은 뒤
                {
                    if (theDataBase.mixList[i].itemID == theDataBase.itemList[j].itemID)
                    {
                        mixResultSlot.item.itemIcon = theDataBase.itemList[j].itemIcon; // 아이콘 삽입
                        mixResultSlot.item = theDataBase.itemList[j].Init(); // 아이템 정보를 조합결과 슬롯에 넣어준다.
                        // mixResultSlot.item.itemCount = 1;
                        mixResultSlot.InitUI();

                        Data.Init(mat_itemID, mat_itemCount, 10000);
                        JsonData SendData = JsonMapper.ToJson(Data);
                        ServerClient.instance.Send(SendData.ToString());
                    }
                }
                break;
            }
        }
    }
}
