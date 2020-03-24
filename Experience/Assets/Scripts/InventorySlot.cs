using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    private Image item_Icon;

    public ItemInfo item;

    private Vector3 slotposition;

    private void Start()
    {
        item_Icon = GetComponent<Image>();
    }

    public void SetAlpha(float _alpha)
    {
        Color color = item_Icon.color;
        color.a = _alpha;
        item_Icon.color = color;
    }

    public void AddItem(ItemInfo itemInfo)
    {
        item_Icon.sprite = itemInfo.itemIcon;

        if (itemInfo.itemID != 0) // 아이템 위치 교환 할 때 빈 슬롯이면 -> OnDrop에서 해줘야 하는데 NULL이랑 꼬여서 AddItem에서 함.
            SetAlpha(1);
        else
            SetAlpha(0);

        item = itemInfo;
    }

    public void RemoveItem()
    {
        item_Icon.sprite = null;
        SetAlpha(0);
        item.itemID = 0; 
        item.itemName = ""; 
        item.itemIcon = null; 
        item.itemDescription = ""; 
    }

    void ShowTooltip()
    {
        if (item.itemID == 0)
            return;
        else
        {
            Debug.Log(item.itemID + "번 툴팁 표시");
            Debug.Log(gameObject.transform.position);
        }
    }

    void HideTooltip()
    {
        if (item.itemID == 0)
            return;
        else
            Debug.Log(item.itemID + "번 툴팁 숨김");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item.itemID != 0) // 아이템이 있어야 드래그 가능
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(item_Icon);
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragSlot.instance.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) // 그냥 드래그가 끝나고 드롭될 경우
    {
        DragSlot.instance.dragSlot = null;
        DragSlot.instance.SetColor(0);
    }

    public void OnDrop(PointerEventData eventData) // 다른 슬롯 위에서 드롭되었을경우
    {
        ItemInfo _temp = item;
        if (DragSlot.instance.dragSlot != null)
        {
            AddItem(DragSlot.instance.dragSlot.item); // 드롭한 공간에 아이템을 넣어줌.
            DragSlot.instance.dragSlot.AddItem(_temp); // 슬롯 교환
            MixSlot.instance.CheckMaterial();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //HideTooltip();
    }

}
