using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubAnimator : MonoBehaviour
{
    Animator _animator;
    SpriteRenderer[] _characterSprite;

    public Player_Warrior player_Warrior;
    public Player_Magician player_Magician;

    private bool _walk;
    private bool _attack;

    public bool active;  // animator active option

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterSprite = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        IsActive(active); // Blend Tree에 따라서 애니메이션의 방향을 토대로 오브젝트를 활성화한다.
    }

    public void IsActive(bool _state)
    {
        if (_characterSprite[0].gameObject.activeSelf == _state) // 현재 활성화되어있으면
            return; // 아무것도 안 하고 반환
        for (int i = 0; i < _characterSprite.Length; i++) // 비활성화되어있으면
            _characterSprite[i].gameObject.SetActive(_state); // 스프라이트들을 활성화시킨다.
    }

    public void Move(bool _state)
    {
        _animator.SetBool("Walk", _state);
    }

    public void Attack()
    {
        _animator.SetTrigger("Attack");
    }

    public void Attacked(bool _state)
    {
        _animator.SetBool("Attacked", _state);
    }

    public void Meteor()
    {
        _animator.SetTrigger("Meteor");
    }
}
