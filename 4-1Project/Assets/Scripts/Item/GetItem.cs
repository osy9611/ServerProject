using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetItem : MonoBehaviour
{
    private BoxCollider2D boxCollider2D;
    private Collider2D[] playerColliders;
    private Animator animator;

    private bool isGetItem;
    private int _layerMask;

    private float _itemtoMyDistance;
    private float[] _itemtoOtherDistance;

    public int itemID;
    
    private void Awake()
    {
        _layerMask = 1 << LayerMask.NameToLayer("Player");
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        _itemtoOtherDistance = new float[GameManager.instance.playerInfo.Count];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int temp_otherposIndex = 0;
        if (collision.gameObject.name == GameManager.instance.PlayerName)
        {
            // 아이템에 충돌한 플레이어를 파악한다.
            playerColliders = Physics2D.OverlapBoxAll(transform.position, boxCollider2D.size, 0, _layerMask);

            // 플레이어와 아이템의 거리, 다른 플레이어와 아이템의 거리를 구한다.
            for (int i = 0; i < playerColliders.Length; i++)
            {
                if (playerColliders[i].name == GameManager.instance.PlayerName)
                    _itemtoMyDistance = Vector2.Distance(playerColliders[i].transform.position, transform.position);
                else
                    _itemtoOtherDistance[temp_otherposIndex++] = Vector2.Distance(playerColliders[i].transform.position, transform.position);
            }

            // 플레이어가 아이템과 가장 가까우면, 아이템은 플레이어가 획득한다.
            for (int i = 0; i < _itemtoOtherDistance.Length; i++)
            {
                if (_itemtoOtherDistance[i] > _itemtoMyDistance) // 만약 다른 플레이어가 나보다 가까이 있으면
                {
                    gameObject.SetActive(false); // 아이템을 없앤 후
                    return; // 그대로 함수를 종료시킨다.
                }
                // 이 for문을 빠져나가면 내가 가장 아이템에 가까우므로 아이템을 획득한다.
            }
            isGetItem = Inventory.instance.GetItem(itemID); // true를 반환하면 아이템을 획득한 것으로 판단.
            if (isGetItem)
                gameObject.SetActive(false);
        }
        else
            gameObject.SetActive(false);
    }

    public void DisableObject()
    {
        gameObject.SetActive(false);
    }

    public void EnableCollider()
    {
        boxCollider2D.enabled = true;
    }
    public void DisableCollider()
    {
        boxCollider2D.enabled = false;
    }
}
