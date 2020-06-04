using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 0 : 아이템 없음
// 1 ~ 100 : 재료 (1)
// 101 ~ 200 : 소비 (2)
// 201 ~ 300 : 장비 (0)

public class DataBase : MonoBehaviour
{
    public static DataBase instance;

    public List<Item> itemList = new List<Item>();
    public List<Recipe> armorRecipeList = new List<Recipe>();
    public List<Recipe> weaponRecipeList = new List<Recipe>();
    public List<Recipe> subweaponRecipeList = new List<Recipe>();
    public List<Recipe> accessoryRecipeList = new List<Recipe>();
    public List<Recipe> activeRecipeList = new List<Recipe>();

    private void Awake()
    {
        instance = this;
    }
    // ITEM : ItemID , ItemName, ItemDes, ItemSpriteFilename
    // RECIPE : (item1ID, item1Count, item1filename), (2), (3), money, resultitemfilepath, resultitemID

    private void Start()
    {
        // Material Item
        itemList.Add(new Item(1, "주문서", "주문서.", "alchemy"));
        itemList.Add(new Item(2, "에메랄드", "에메랄드.", "ashvattha"));
        itemList.Add(new Item(3, "물방울", "물방울.", "Ereronium"));
        itemList.Add(new Item(4, "헝겊", "헝겊.", "fabric"));
        itemList.Add(new Item(5, "가넷", "가넷.", "hernite"));
        itemList.Add(new Item(6, "고철", "고철.", "iron"));
        itemList.Add(new Item(7, "사파이어", "사파이어", "gaiter"));

        // Potion Item
        itemList.Add(new Item(101, "파우더", "피부", "powder"));

        // Equipment Item
        itemList.Add(new Item(201, "펜던트", "펜던트...", "pendant"));
        itemList.Add(new Item(202, "칼", "칼...", "knife"));
        itemList.Add(new Item(203, "보스방 열쇠", "이 열쇠가 있어야 보스방에 입장할 수 있습니다.", "bosskey"));

        // Armor Recipe

        // Weapon Recipe
        weaponRecipeList.Add(new Recipe(5, 1, "hernite", 6, 1, "iron", 7, 1, "gaiter", 5000, "knife", 202));
        // SubWeapon Recipe

        // Accessory Recipe
        accessoryRecipeList.Add(new Recipe(1, 1, "alchemy", 2, 1, "ashvattha", 3, 1, "Ereronium", 5000, "pendant", 201));
        // Active Recipe
        activeRecipeList.Add(new Recipe(1, 1, "alchemy", 4, 1, "fabric", 7, 1, "gaiter", 10000, "bosskey", 203));
    }
}
