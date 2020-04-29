using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private DataBase theDataBase; // 아이템, 조합 관련 데이터베이스 받아오기

    private Item[] equip_Inv;
    private Item[] mat_Inv;
    private Item[] potion_Inv;

    private InventorySlot[] inventorySlots; // 인벤토리 슬롯
    private MixMaterialSlot[] mixMaterialSlots; // 조합 슬롯
    private MixResultSlot mixResultSlot; // 조합결과 슬롯
    private ShareInventorySlot[] shareInventorySlot; // 공유 인벤토리 슬롯

    private int[] mat_itemID; // 조합 재료 아이템 번호를 정렬하기 위한 배열
    private int[] mat_itemCount; // 조합 재료 아이템 갯수를 정렬하기 위한 배열
    private bool isMixed; // 조합 성공 여부를 서버에서 받아옴
    private int isMixItemID; // 조합 성공 시 서버에서 받아오는 아이템 번호

    [HideInInspector]
    public int _tabIndex; // 장비 / 재료 / 소비탭 인덱스 번호

    public Transform inv_slot, mix_slot, share_slot;
    public InventoryTab[] inventoryTabs;
    public GameObject inventory;

    public ItemMix Data;

    private void Awake()
    {
        instance = this;

        // 슬롯 할당
        inventorySlots = inv_slot.GetComponentsInChildren<InventorySlot>();
        mixMaterialSlots = mix_slot.GetComponentsInChildren<MixMaterialSlot>();
        mixResultSlot = mix_slot.GetComponentInChildren<MixResultSlot>();
        shareInventorySlot = share_slot.GetComponentsInChildren<ShareInventorySlot>();

        // 장비, 재료, 소비 아이템을 저장하는 배열 할당
        equip_Inv = new Item[inventorySlots.Length];
        mat_Inv = new Item[inventorySlots.Length];
        potion_Inv = new Item[inventorySlots.Length];

        for(int i=0;i<equip_Inv.Length;i++)
        {
            equip_Inv[i] = new Item();
            mat_Inv[i] = new Item();
            potion_Inv[i] = new Item();
        }

        // 서버와의 연동을 위해 공유 인벤토리의 슬롯에 따로 슬롯 인덱스를 할당해줌.
        for (int i = 0; i < shareInventorySlot.Length; i++)
            shareInventorySlot[i].slotIndex = i;

        // 데이터베이스 검색
        theDataBase = FindObjectOfType<DataBase>();

        mat_itemID = new int[mixMaterialSlots.Length];
        mat_itemCount = new int[mixMaterialSlots.Length];

        isMixed = false;
        isMixItemID = 0;
    }

    private void Start()
    {
        InitEquipInv(0);
        inventory.SetActive(false);
    }

    private void Update()
    {
        if (isMixed) // 조합 성공 시
        {
            for (int i = 0; i < 3; i++)
                mixMaterialSlots[i].RemoveItem(); // 조합 슬롯의 아이템을 모두 없앤 다음

            for (int i = 0; i < theDataBase.itemList.Count; i++) // 아이템 데이터베이스에서 ID에 맞는 아이템을 찾은 뒤
            {
                if (isMixItemID == theDataBase.itemList[i].itemID)
                {
                    mixResultSlot.item.itemIcon = theDataBase.itemList[i].itemIcon; // 아이콘 삽입
                    mixResultSlot.item = theDataBase.itemList[i].Init(); // 아이템 정보를 조합결과 슬롯에 넣어준다.
                    mixResultSlot.item.itemCount = 1;
                    mixResultSlot.InitUI();
                }
            }
            isMixed = false;
            isMixItemID = 0;
        }

        for(int i=0;i<shareInventorySlot.Length;i++)
            shareInventorySlot[i].InitUI();
    }

    public bool GetItem(int _itemID) // 인게임 필드에서 새로운 아이템을 획득할 경우에 호출하는 함수.
    {
        for (int i = 0; i < theDataBase.itemList.Count; i++) // 아이템 데이터베이스
        {
            if(theDataBase.itemList[i].itemID == _itemID) // 데이터베이스 아이템 ID == 습득한 아이템 ID
            {
                if(_itemID > 200) // 장비템 습득
                {
                    for (int j = 0; j < equip_Inv.Length; j++) // 장비 배열 내 검색
                    {
                        if (equip_Inv[j].itemID == 0) // 빈 슬롯을 찾으면
                        {
                            equip_Inv[j] = theDataBase.itemList[i].Init();
                            if (_tabIndex == 0) // 현재 열린 창이 장비창일경우
                            {
                                inventorySlots[j].item = equip_Inv[j];
                                inventorySlots[j].InitUI();
                            }
                            return true;
                        }
                    }
                }
                else if(_itemID > 100) // 소비템 습득
                {
                    for (int j = 0; j < potion_Inv.Length; j++) // 같은 ID의 아이템이 있는지 체크
                    {
                        if (potion_Inv[j].itemID == _itemID) // 슬롯에 같은 아이템이 있으면
                        {
                            potion_Inv[j].itemCount++; // 아이템 갯수 1 증가
                            if (_tabIndex == 2) // 현재 열린 창이 소비창일경우
                            {
                                inventorySlots[j].item = potion_Inv[j];
                                inventorySlots[j].InitUI();
                            }
                            return true;
                        }
                    }

                    for (int j = 0; j < potion_Inv.Length; j++) // 아이템의 빈 공간 체크
                    {
                        if (potion_Inv[j].itemID == 0)
                        {
                            potion_Inv[j] = theDataBase.itemList[i].Init(); // 아이템 정보를 슬롯에 할당
                            if (_tabIndex == 2) // 현재 열린 창이 소비창일경우
                            {
                                inventorySlots[j].item = potion_Inv[j];
                                inventorySlots[j].InitUI();
                            }
                            return true;
                        }
                    }
                }
                else // 재료템 습득
                {
                    for (int j = 0; j < mat_Inv.Length; j++) // 같은 ID의 아이템이 있는지 체크
                    {
                        if (mat_Inv[j].itemID == _itemID) // 슬롯에 같은 아이템이 있으면
                        {
                            mat_Inv[j].itemCount++; // 아이템 갯수 1 증가
                            if (_tabIndex == 1) // 현재 열린 창이 재료창일경우
                            {
                                inventorySlots[j].item = mat_Inv[j];
                                inventorySlots[j].InitUI();
                            }
                            return true;
                        }
                    }

                    for (int j = 0; j < mat_Inv.Length; j++) // 아이템의 빈 공간 체크
                    {
                        if (mat_Inv[j].itemID == 0) // 슬롯에 같은 아이템이 없으면
                        {
                            mat_Inv[j] = theDataBase.itemList[i].Init(); // 아이템 정보를 슬롯에 할당
                            if (_tabIndex == 1) // 현재 열린 창이 재료창일경우
                            {
                                inventorySlots[j].item = mat_Inv[j];
                                inventorySlots[j].InitUI();
                            }
                            return true;
                        }
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

    // 타 슬롯에서 공유 슬롯으로 아이템이 넘어올 때, 슬롯에 같은 ID의 아이템 유무를 판별함.
    public ShareInventorySlot SearchShareInventorySlot(int _itemID)
    {
        for(int i=0;i<shareInventorySlot.Length;i++)
        {
            if (shareInventorySlot[i].item.itemID == _itemID)
                return shareInventorySlot[i];
        }
        return null;
    }

    public void CheckMaterial() // 조합 판단
    {
        for (int i = 0; i < 3; i++) // 조합 슬롯 3개의 itemID랑 itemCount를 받아온다.
        {
            Debug.Log("아이템 정보 대입중");
            mat_itemID[i] = mixMaterialSlots[i].item.itemID;
            mat_itemCount[i] = mixMaterialSlots[i].item.itemCount;
        }
        Debug.Log(mat_itemID[0] + "," + mat_itemID[1] + "," + mat_itemID[2]);
        Debug.Log(mat_itemCount[0] + "," + mat_itemCount[1] + "," + mat_itemCount[2]);

        for (int i = 0; i < 2; i++) // 조합 슬롯 3개의 ItemID를 오름차순으로 정렬한다.(itemCount도 같이 변경)
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
                Debug.Log("아이템 정렬 중");
            }
        }

        // 서버로 조합 슬롯의 데이터 전송
        Data.Init(mat_itemID, mat_itemCount, 10000); 
        JsonData SendData = JsonMapper.ToJson(Data);
        ServerClient.instance.Send(SendData.ToString()); // Send와 동시에 Resolve받아 조합 성공 여부를 알려줌.
    }

    public void ReceiveMixResult(JsonData _data) // 조합 결과를 서버에서 받아오는 함수
    {
        try
        {
            isMixed = bool.Parse(_data["result"].ToString());
            if(isMixed)
                isMixItemID = int.Parse(_data["Item"].ToString());
        }
        catch (Exception)
        {
            return;
        }
    }

    public void UpdateShareInfo(JsonData _data) // 공유 인벤토리 갱신
    {
        try
        {
            for (int i = 0; i < shareInventorySlot.Length; i++)
            {
                shareInventorySlot[i].item.itemID = int.Parse(_data["Inventory"][i].ToString());
                shareInventorySlot[i].item.itemCount = int.Parse(_data["ItemCount"][i].ToString());
                for (int j = 0; j < theDataBase.itemList.Count; j++)
                {
                    if (shareInventorySlot[i].item.itemID == theDataBase.itemList[j].itemID)
                    {
                        shareInventorySlot[i].item.itemIcon = theDataBase.itemList[j].itemIcon;
                        shareInventorySlot[i].item.itemDescription = theDataBase.itemList[j].itemDescription;
                        shareInventorySlot[i].item.itemName = theDataBase.itemList[j].itemName;
                    }
                }
            }
        }
        catch(Exception)
        {
            return;
        }
    }

    void UpdateItemInfo(int _tabIndex) // 탭을 바꿀 때마다 바꾸기 전 탭의 정보를 아이템 배열에 저장
    {
        switch(_tabIndex)
        {
            case 0: // 장비
                for (int i = 0; i < inventorySlots.Length; i++)
                    equip_Inv[i] = inventorySlots[i].item.Init();
                break;
            case 1: // 재료
                for (int i = 0; i < inventorySlots.Length; i++)
                    mat_Inv[i] = inventorySlots[i].item.Init();
                break;
            case 2: // 소비
                for (int i = 0; i < inventorySlots.Length; i++)
                    potion_Inv[i] = inventorySlots[i].item.Init();
                break;
        }
    }

    void ChangeTabColor(int _p_tabNum)
    {
        inventoryTabs[_tabIndex].DisableTab(); // 이전에 선택된 인덱스의 탭 색깔을 비선택 색깔로 바꿔준 뒤
        _tabIndex = _p_tabNum; // 탭 인덱스를 바꾸고
        inventoryTabs[_tabIndex].EnableTab(); // 새로 선택된 인덱스의 탭 색깔로 바꿔준다.
    }

    void UpdateInvSlot(Item[] _items)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].item = _items[i];
            inventorySlots[i].InitUI();
        }
    }

    public void InitEquipInv(int _tabNum) // 장비 탭 클릭 시
    {
        UpdateItemInfo(_tabIndex);
        UpdateInvSlot(equip_Inv);
        ChangeTabColor(_tabNum);
    }

    public void InitMatInv(int _tabNum) // 재료 탭 클릭 시
    {
        UpdateItemInfo(_tabIndex);
        UpdateInvSlot(mat_Inv);
        ChangeTabColor(_tabNum);
    }

    public void InitPotionInv(int _tabNum) // 소비 탭 클릭 시
    {
        UpdateItemInfo(_tabIndex);
        UpdateInvSlot(potion_Inv);
        ChangeTabColor(_tabNum);
    }
}
