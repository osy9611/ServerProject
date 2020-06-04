using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Warrior : MonoBehaviour
{
    private RaycastHit2D _hit2D;
    private Player _mainPlayer;

    private bool _isHit, _isSkill;
    private float _attackcooltime, _skillcooltime;
    public float attackspeed, skillcooltime;

    public GameObject invincibleWall;

    private int _layerMask;

    private void Awake()
    {
        _layerMask = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("RoomCollider");
        _layerMask = ~_layerMask;
        _mainPlayer = GetComponent<Player>();
    }

    private void Start()
    {
        CharacterInfoWindow.instance.UpdateASPD(attackspeed);
    }

    private void Update()
    {
        if (_mainPlayer.playerState == PlayerState.Restriction)
            return;

        #region Attack
        if (_isHit) // 공격쿨타임 중
        {
            if (Time.time - _attackcooltime > attackspeed)
                _isHit = false;
        }

        if (!_isHit) // 공격쿨타임 종료
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                _mainPlayer.AttackPlayer();
                _isHit = true;
                _hit2D = Physics2D.Raycast(transform.position, _mainPlayer._mousePos, 2f, _layerMask);
                _mainPlayer.ChangeAnimationState_Attack();
                _attackcooltime = Time.time;

                if (_hit2D.collider != null)
                {
                    if (_hit2D.collider.name == "Boss") // 보스에 맞으면
                    {
                        _mainPlayer.SendDamageInfo(Boss.instance.DEF);
                        Boss.instance.ActiveHPBar();
                    }

                    if (_hit2D.collider.gameObject.tag == "FireBall") // 보스가 소환한 불구슬에 맞으면
                        Boss.instance._fireBall.HitFireBall(_hit2D.collider.gameObject.name);

                        _mainPlayer.temp = _hit2D.collider.GetComponent<ItemDropObject>();

                    if (_mainPlayer.temp != null) // 채집물에 맞으면
                    {
                        _mainPlayer.temp.MinusCount(gameObject.name);
                        if (!_mainPlayer.isGetSwitch) // 스위치를 스폰하지 못했을경우
                            _mainPlayer.SendItemPercentPacket();
                    }
                }
            }
        }
        #endregion
        #region Skill
        if(_isSkill) // 스킬 쿨타임 중
        {
            if (Time.time - _skillcooltime > skillcooltime)
                _isSkill = false;
        }
        if(!_isSkill) // 스킬 쿨타임 종료(스킬발동)
        {
            if(Input.GetMouseButtonDown(1))
            {
                _skillcooltime = Time.time; // 스킬발동시간 기록
                _mainPlayer.DEF *= 2; // 방어력 X2
                CharacterInfoWindow.instance.UpdateDEF(_mainPlayer.DEF);
                _mainPlayer.AttackPlayer(PlayerState.Invincible);
                invincibleWall.SetActive(true);
                // 무적 이펙트 발동
                _isSkill = true;
                // 스킬발동 후 해제
                _mainPlayer.Invoke("Invoke_ChangePSIdle", 1f); // 무적 상태 해제
                Invoke("Invoke_OffEffect", 1f);
                _mainPlayer.Invoke("Invoke_DivideDEF", 15f); // 방어력 원상복구
            }
        }
        #endregion
    }

    #region Invoke
    private void Invoke_OffEffect()
    {
        Debug.Log("전사 무적이펙트 해제");
        invincibleWall.SetActive(false);
        // 이펙트 해제
    }
    #endregion
}
