using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MixSlot : MonoBehaviour
{
    public static MixSlot instance;

    public InventorySlot[] itemMaterialSlot;

    private DataBase theDataBase; // 아이템, 조합 데이터베이스

    private int[] mat_itemID;
    private Image item_Icon;
    private InventorySlot mixResultItem;

    private void Start()
    {
        instance = this;
        item_Icon = GetComponent<Image>();
        mixResultItem = GetComponent<InventorySlot>();
        theDataBase = FindObjectOfType<DataBase>();
        mat_itemID = new int[3];
    }

    public void CheckMaterial()
    {
        for (int i = 0; i < 3; i++)
            mat_itemID[i] = itemMaterialSlot[i].item.itemID;

        for (int i = 0; i < 2; i++)
        {
            for (int j = i + 1; j < 3; j++)
            {
                if(mat_itemID[i]>mat_itemID[j])
                {
                    int temp = mat_itemID[i];
                    mat_itemID[i] = mat_itemID[j];
                    mat_itemID[j] = temp;
                }
            }
        }

        for (int i = 0; i < theDataBase.mixList.Count; i++)
        {
            if (theDataBase.mixList[i].itemID_1 == mat_itemID[0] &&
                theDataBase.mixList[i].itemID_2 == mat_itemID[1] &&
                theDataBase.mixList[i].itemID_3 == mat_itemID[2]) // 데이터베이스 조합 정보와 조합슬롯의 아이템 ID와 일치하면
            {
                //for (int j = 0; j < 3; j++)
                itemMaterialSlot[0].RemoveItem(); // 조합 슬롯의 아이템을 모두 없앤 다음

                for(int j=0;j<theDataBase.itemList.Count;j++) // 아이템 데이터베이스에서 ID에 맞는 아이템을 찾은 뒤
                {
                    if (theDataBase.mixList[i].itemID == theDataBase.itemList[j].itemID)
                    {
                        item_Icon.sprite = theDataBase.itemList[j].itemIcon; // 아이콘 삽입
                        mixResultItem.item = theDataBase.itemList[j]; // 아이템 정보를 조합결과 슬롯에 넣어준다.
                    }
                }
                SetColor(1);
                break;
            }
        }
    }

    public void SetColor(float _alpha)
    {
        Color color = item_Icon.color;
        color.a = _alpha;
        item_Icon.color = color;
    }
}
