using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MixResultSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Image UI_item_image;
    //private Text UI_item_count;

    public Item item;

    private void Awake()
    {
        UI_item_image = GetComponent<Image>();
        // UI_item_count = GetComponentInChildren<Text>();
    }

    private void SetAlpha(float _alpha)
    {
        Color color = UI_item_image.color;
        color.a = _alpha;
        UI_item_image.color = color;
    }

    public void RemoveItem()
    {
        UI_item_image.sprite = null;
        // UI_item_count.text = "";
        SetAlpha(0);
        item.itemID = 0;
        item.itemName = "";
        item.itemIcon = null;
        item.itemDescription = "";
        item.itemCount = 0;
    }

    public void InitUI()
    {
        UI_item_image.sprite = item.itemIcon;
        SetAlpha(1);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item.itemID != 0) // 아이템이 있어야 드래그 가능
        {
            DragSlot.instance.DragSetImage(UI_item_image);
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragSlot.instance.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventorySlot inventorySlot = eventData.pointerEnter.gameObject.GetComponent<InventorySlot>();
        if(inventorySlot != null)
        {
            InventorySlot temp = Inventory.instance.SearchInventorySlot(item.itemID); // 인벤토리 슬롯에 같은 ID의 아이템이 있는지 검색
            if (temp != null) // 인벤토리 슬롯에 같은 아이템이 있으면
            {
                temp.PlusItemCount(); // 인벤토리 슬롯의 갯수 1개 증가
                RemoveItem(); // 조합결과 슬롯 삭제
                temp.InitUI(); // 인벤토리 슬롯 UI 업데이트
                SetAlpha(0);
            }
            else // 인벤토리 슬롯에 같은 ID의 아이템이 없을 경우
            {
                if (inventorySlot.item.itemID == 0)
                {
                    inventorySlot.item = item.Init(); // 인벤토리 슬롯에 아이템 정보 할당
                    inventorySlot.item.itemCount = 1; // 아이템의 갯수는 1로 초기화
                    inventorySlot.InitUI();
                    RemoveItem(); // 조합 슬롯 갯수 1개 감소
                    SetAlpha(0);
                }
            }
        }
        DragSlot.instance.SetColor(0);
    }

}
