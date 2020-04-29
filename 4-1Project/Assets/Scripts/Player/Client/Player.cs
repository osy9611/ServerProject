using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.UI;

public enum PlayerState
{
    Idle,
    Move,
    Attack,
    Die,
};

public class Player : MonoBehaviour
{
    PlayerState playerState;

    Animator _animator;
    
    SubAnimator[] _subAnimator;
    SubAnimator _activesubAnimator;

    [HideInInspector]
    public Vector2 _mousePos;

    Vector2 _dirPos, _temp_dirPos;
    Vector2 _Pos, _movePos, _temp_movePos;

    public PlayerData Data;
    public BossDamage PtoB_damage_data; // 플레이어가 보스에게 데미지를 넣을 때
    public PlayerDamage BtoP_damage_data; // 보스가 쏜 탄환에 플레이어가 맞으면

    public float _movespeed = 5.0f, attackSpeed;
    public int STR, DEF, fullHP;

    void Start()
    {
        playerState = PlayerState.Idle;
        _animator = GetComponent<Animator>();
        _subAnimator = GetComponentsInChildren<SubAnimator>();
        _temp_dirPos = Vector2.zero;
        _temp_movePos = Vector2.zero;
        _activesubAnimator = _subAnimator[0];

        Data.Speed = _movespeed;
        Data.Init(GameManager.instance.PlayerName);
        PtoB_damage_data.Init();
        BtoP_damage_data.Init();
        BtoP_damage_data.nickname = GameManager.instance.PlayerName;
    }

    public void Update()
    {
        // 이동뿐만이 아니라 회전했을 때도 현재 위치를 패킷으로 보내주기 때문에
        // (패킷을 보낼 때 현재 위치도 계속 보내기 때문에 최신 위치 정보가 필요해서)
        if (playerState != PlayerState.Die)
        {
            // 현재 위치, 
            Data.nx = transform.position.x;
            Data.ny = transform.position.y;
            ChangeLookdirection();
            MoveCharacter();
        }
    }

    public void ChangeLookdirection()
    {
        _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _mousePos -= (Vector2)transform.position;

        // 마우스 커서의 좌표를 -1 ~ 1로 만들어줌
        _dirPos.x = Mathf.RoundToInt(_mousePos.normalized.x);
        _dirPos.y = Mathf.RoundToInt(_mousePos.normalized.y);

        // 마우스 좌표에 따른 캐릭터의 시점 변경
        _animator.SetFloat("xPos", _dirPos.x);
        _animator.SetFloat("yPos", _dirPos.y);

        Data.rx = _dirPos.x;
        Data.ry = _dirPos.y;

        if (_temp_dirPos != _dirPos)
        {
            _temp_dirPos = _dirPos;
            JsonData SendData = JsonMapper.ToJson(Data);
            ServerClient.instance.Send(SendData.ToString());
        }
    }

    public void MoveCharacter()
    {
        _movePos.x = Input.GetAxisRaw("Horizontal");
        _movePos.y = Input.GetAxisRaw("Vertical");

        // 방향(키보드)
        Data.x = _movePos.x;
        Data.y = _movePos.y;

        Data.State = (int)PlayerState.Move;

        if (_movePos != Vector2.zero)
        {
            ChangeAnimationState(true);
            transform.Translate(_movePos.normalized * Time.deltaTime * _movespeed);
        }
        else
        {
            Data.State = (int)PlayerState.Idle;
            ChangeAnimationState(false);
        }

        if(_temp_movePos != _movePos)
        {
            _temp_movePos = _movePos;
            JsonData SendData = JsonMapper.ToJson(Data);
            ServerClient.instance.Send(SendData.ToString());
        }
    }

    void ChangeAnimationState(bool _state)
    {
        for (int i = 0; i < _subAnimator.Length; i++)
        {
            if (_subAnimator[i].active)
            {
                _subAnimator[i].Move(_state);
                break;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(gameObject.GetComponent<CapsuleCollider2D>(), collision.collider);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Floor")
        {

        }
    }

    public void AttackPlayer()
    {
        playerState = PlayerState.Attack;
        Data.State = (int)PlayerState.Attack;

        for (int i = 0; i < _subAnimator.Length; i++)
        {
            if (_subAnimator[i].active)
            {
                _subAnimator[i].Attack();
                break;
            }
        }
        JsonData SendData = JsonMapper.ToJson(Data);
        ServerClient.instance.Send(SendData.ToString());
    }

    public void SendDamageInfo(int _def) // 내가 공격을 할 때
    {
        if (STR - _def <= 0)
            return;
        PtoB_damage_data.damage = STR - _def;
        JsonData SendData = JsonMapper.ToJson(PtoB_damage_data);
        ServerClient.instance.Send(SendData.ToString());
    }

    public void Attacked(int _damage) // 외부에서 공격이 들어올 때.
    {
        if (_damage - DEF <= 0)
            return;
        HPManager.instance.myHP -= (_damage - DEF);
        HPManager.instance.SetHP();
        BtoP_damage_data.HP = HPManager.instance.myHP;

        JsonData SendData = JsonMapper.ToJson(BtoP_damage_data);
        ServerClient.instance.Send(SendData.ToString());
    }
}
