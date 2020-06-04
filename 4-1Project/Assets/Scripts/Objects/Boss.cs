using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class Boss : MonoBehaviour
{
    public static Boss instance;

    Animator _animator;
    Rigidbody2D _rigidbody2D;
    Transform _playerPos;
    ShakeCamera _shakeCamera;
    BossHPBar _BossHPBar;

    private float _Nowpercent;

    public int HP, STR, DEF;
    private int _fullHp;

    public int patternNum;

    public Phase Data;

    public int _circleBullet;

    public int _floorDeathOn;   //장판 작동

    public Fire_Ball _fireBall; //불 구슬 데미지 관련

    bool _firstStart = true, _attack;

    private void Awake()
    {
        instance = this;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerPos = FindObjectOfType<Player>().transform;
        _BossHPBar = FindObjectOfType<BossHPBar>();
        _animator = GetComponent<Animator>();
        _shakeCamera = FindObjectOfType<ShakeCamera>();
        _fullHp = HP;

        Data.Init(false);
    }

    public void SetHP(JsonData _data)
    {
        if (_fullHp == HP && _firstStart)
        {
            patternNum = int.Parse(_data["Phase"].ToString());
            _attack = true;
            PatternManager.instance._isStart = true;
            _firstStart = false;
        }

        HP = int.Parse(_data["Hp"].ToString());
    }
    private void Update()
    {
        if (_attack)
        {
            _animator.SetTrigger("Attack");
            _attack = false;
        }

        if (HP <= 0)
            _animator.SetTrigger("Dead");
    }

    public void Attack()
    {
        _attack = true;
    }

    public void ShakeCamera() // 애니메이션 이벤트 함수
    {
        _shakeCamera.PutShakeTime();
    }

    public void Dead()
    {
        gameObject.SetActive(false);
    }

    public void SetFullHP()
    {
        HP = _fullHp;
    }

    public void SetPhase(JsonData _data)
    {
        patternNum = int.Parse(_data["Phase"].ToString());
    }

    public void DelaySendPhaseData(float _delayTime)
    {
        Invoke("SendPhaseData", _delayTime);
    }

    private void SendPhaseData()
    {
        CancelInvoke("SendPhaseData");
        Data.bx = transform.position.x;
        Data.by = transform.position.y;

        Data.px = _playerPos.transform.position.x;
        Data.py = _playerPos.transform.position.y;

        JsonData SendData = JsonMapper.ToJson(Data);
        ServerClient.instance.Send(SendData.ToString());
    }

    public void ActiveHPBar()
    {
        if (!_BossHPBar.gameObject.activeSelf)
            _BossHPBar.gameObject.SetActive(true);
    }

    public void SearchFireBall(JsonData _data)
    {
        _fireBall.ServerHitFireBall(_data["Name"].ToString());
    }
}