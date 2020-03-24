using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    private Image UI_item_image;
    private Text UI_item_count;

    public Item item;

    void Awake()
    {
        UI_item_image = GetComponent<Image>();
        UI_item_count = GetComponentInChildren<Text>();
    }

    private void SetAlpha(float _alpha)
    {
        Color color = UI_item_image.color;
        color.a = _alpha;
        UI_item_image.color = color;
    }

    public void AddItem(Item item) // 빈 슬롯에 아이템의 정보를 삽입하는 과정
    {
        this.item = item.Init();
        InitUI();
    }

    public void RemoveItem()
    {
        UI_item_image.sprite = null;
        UI_item_count.text = "";
        SetAlpha(0);
        item.itemID = 0;
        item.itemName = "";
        item.itemIcon = null;
        item.itemDescription = "";
    }

    public void PlusItemCount()
    {
        item.itemCount++;
        UI_item_count.text = item.itemCount.ToString();
    }

    public void MinusItemCount()
    {
        item.itemCount--;
        UI_item_count.text = item.itemCount.ToString();
        if (item.itemCount == 0)
            RemoveItem();
    }

    public void InitUI()
    {
        UI_item_image.sprite = item.itemIcon;
        SetAlpha(1);
        UI_item_count.text = item.itemCount.ToString();
        if (item.itemCount == 0)
            RemoveItem();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(item.itemID != 0) // 아이템이 있어야 드래그 가능
        {
            DragSlot.instance.DragSetImage(UI_item_image);
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragSlot.instance.transform.position = eventData.position;
    }

    public void OnDrop(PointerEventData eventData) // 그냥 드래그가 끝나고 드롭될 경우
    {
        Debug.Log("Ondrop");
    }

    public void OnEndDrag(PointerEventData eventData) // 다른 슬롯 위에서 드롭되었을 경우
    {
        Debug.Log("OnEndDrag");
        InventorySlot inventorySlot = eventData.pointerEnter.gameObject.GetComponent<InventorySlot>();
        if(inventorySlot != null) // 드롭한 슬롯이 인벤토리 슬롯일 경우 슬롯의 정보를 맞교환해준다.
        {
            // 슬롯 간 정보를 교환해준 뒤
            Item temp = inventorySlot.item.Init();
            inventorySlot.item = item.Init(); 
            item = temp.Init();

            // UI 업데이트
            InitUI();
            inventorySlot.InitUI();
        }

        MixMaterialSlot mixMaterialSlot = eventData.pointerEnter.gameObject.GetComponent<MixMaterialSlot>();
        if(mixMaterialSlot != null) // 드롭한 슬롯이 조합재료 슬롯일 경우
        {
            Debug.Log("조합 슬롯 작동");
            MixMaterialSlot temp = Inventory.instance.SearchMixMaterialSlot(item.itemID); // 조합 슬롯 3개에 같은 ID의 아이템이 있는지 검색
            if (temp != null) // 조합 슬롯에 같은 ID의 아이템이 있으면
            {
                temp.PlusItemCount(); // 조합 슬롯 갯수 1개 증가
                MinusItemCount(); // 인벤토리 슬롯 갯수 1개 감소
                temp.InitUI(); // 조합 슬롯 UI 업데이트
            }
            else // 조합 슬롯에 같은 ID의 아이템이 없을 경우
            {
                if (mixMaterialSlot.item.itemID == 0) // 빈 슬롯에 드롭할 경우
                {
                    mixMaterialSlot.item = item.Init(); // 조합 슬롯에 아이템 정보 할당
                    mixMaterialSlot.item.itemCount = 1; // 아이템의 갯수는 1로 초기화
                    MinusItemCount(); // 인벤토리 아이템 갯수 1개 감소
                }
                else // 빈 슬롯이 아닐 경우(아이템을 스왑해 주어야 함)
                {
                    Item temp2 = mixMaterialSlot.item.Init();
                    mixMaterialSlot.item = item.Init();
                    item = temp2.Init();
                }
            }
            
            InitUI();
            mixMaterialSlot.InitUI(); // 조합 슬롯 UI 업데이트

            if (item.itemCount == 0)
                RemoveItem();
        }
        
        DragSlot.instance.SetColor(0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }
}
