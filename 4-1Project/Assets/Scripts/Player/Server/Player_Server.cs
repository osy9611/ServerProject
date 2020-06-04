using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using LitJson;

public class Player_Server : MonoBehaviour
{
    Animator _animator;
    SubAnimator[] _subAnimators;

    private int _layerMask;

    private bool _setOn, _setSwitch;
    private bool _isGetSwitch; // 스위치를 획득했는지 여부를 결정.(true면 얻음)
    private bool _isSpawnSwitchState; // 스위치를 얻을 수 있는 상태인지 결정.

    public PlayerState PS;
    private bool _isPortal;
    private ItemDropObject itemDropObject = null;
    public ParticleSystem[] dashBlur;

    // 플레이어타입
    public int playerType;

    // 캐릭터 위치
    public Vector2 SyncPos;
    public Vector2 NowPos;

    // 방향(키보드 클릭)
    public Vector2 Pos;
    // 시점(마우스 커서)
    public Vector2 Rot;
    // 마법사 공격 방향
    public Vector2 _mouse_direction;
    // 대시 목적지 좌표
    public Vector2 dashtoPos;
    // 포탈 탑승 좌표
    private Vector2 portalPos;
    //플레이어 속도
    public float Speed;

    //시간
    public float GetMillTime;
    private float dashSpeed = 0.2f;

    // 전사 무적벽
    public GameObject invincibleWall;

    private void Awake()
    {
        _layerMask = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("RoomCollider");
        _layerMask = ~_layerMask;
        _animator = GetComponent<Animator>();
        _subAnimators = GetComponentsInChildren<SubAnimator>();

        if (_animator.runtimeAnimatorController.name == "WarriorController")
            playerType = 0; // 전사일 경우 playerType은 0
        else
            playerType = 1; // 마법사일 경우 playerType은 1
    }

    private void Update()
    {
        if (!_setOn && PS != PlayerState.Dash)
        {
            if (Pos != Vector2.zero)
            {
                Move(Speed, true);
                PS = PlayerState.Move;
            }
            else
            {
                Move(0, false);
                PS = PlayerState.Idle;
            }
        }
        else
        {
            dashSpeed = 0.2f;
            ShowDashBlur(false);
            transform.position = Vector2.MoveTowards(transform.position, SyncPos, Speed * Time.smoothDeltaTime);
            if (Vector2.Distance(transform.position, SyncPos) == 0)
                _setOn = false;
        }

        _animator.SetFloat("xPos", Rot.x);
        _animator.SetFloat("yPos", Rot.y);

        if (PS == PlayerState.Attack) // 기본공격
        {
            ChangeAnimationState_Attack(); // 애니메이션 상태 변경
            FindItemDropObject(); // 마우스 커서 방향에 채집물이 있는지 확인
            PS = PlayerState.Idle;
        }
        else if (PS == PlayerState.Skill) // 스킬공격
        {
            if (playerType == 1) // 마법사일 때만
            {
                Debug.Log("법사서버 스킬샷");
                ChangeAnimationState_Attack();
                ObjectPoolingManager.instance.GetQueue(_mouse_direction, transform.position, gameObject.name);
                PS = PlayerState.Idle;
            }
        }

        if (PS == PlayerState.Dash)
        {
            ShowDashBlur(true); // 대시 시작 잔상을 집어넣는다
            transform.position = Vector2.Lerp(transform.position, dashtoPos, dashSpeed); // 대시!
            dashSpeed += 0.03f;
        }

        if(PS == PlayerState.Invincible) //전사 스킬
        {
            PS = PlayerState.Idle;
            invincibleWall.SetActive(true);
            Debug.Log("(서버) 전사 무적이펙트 활성화");// 이펙트 활성화
            Invoke("OFFWarriorInvEffect", 1f); 
        }

        if(PS == PlayerState.Meteor) // 마법사 메테오 스킬
        {
            PS = PlayerState.Idle;
            ChangeAnimationState_Meteor();
            //Debug.Log("(서버) 법사 메테오 시전준비 애니메이션 진행");
            Invoke("OFFMagicianSkill", 3.0f);
        }

        if(_isPortal)
        {
            transform.position = portalPos;
            _isPortal = false;
        }

        if(_setSwitch)
        {
            itemDropObject.ChangeSpawnSwitchState(_isSpawnSwitchState); // 클라이언트에서 스위치 생성을 랜덤한 bool값을 받아오기 때문에 그 캐릭터와 똑같은 값을 가지고 있다.
            _setSwitch = false;
        }
    }

    public void Teleport(JsonData Data)
    {
        portalPos.x = float.Parse(Data["x"].ToString());
        portalPos.y = float.Parse(Data["y"].ToString());

        _isPortal = bool.Parse(Data["get"].ToString());
    }

    //Json 데이터들을 파싱하여 데이터를 갱신한다
    public void SetInput(JsonData Data)
    {
        // 방향
        Pos.x = float.Parse(Data["x"].ToString());
        Pos.y = float.Parse(Data["y"].ToString());

        // 시점
        Rot.x = float.Parse(Data["rx"].ToString());
        Rot.y = float.Parse(Data["ry"].ToString());

        // 현재 위치
        SyncPos.x = float.Parse(Data["nx"].ToString());
        SyncPos.y = float.Parse(Data["ny"].ToString());

        // 마우스 포인터 방향벡터
        _mouse_direction.x = float.Parse(Data["ax"].ToString());
        _mouse_direction.y = float.Parse(Data["ay"].ToString());

        // 대시 목적지 좌표
        dashtoPos.x = float.Parse(Data["dx"].ToString());
        dashtoPos.y = float.Parse(Data["dy"].ToString());

        GetMillTime = float.Parse(Data["time"].ToString());
        
        Speed = float.Parse(Data["Speed"].ToString());
        PS = (PlayerState)int.Parse(Data["State"].ToString());

        _setOn = true;
    }

    public void Setpercent(JsonData Data)
    {
        _isSpawnSwitchState = bool.Parse(Data["result"].ToString());
        _setSwitch = true;
    }

    //만약에 위치를 동기화하는게 아니라면 서버에서 받은 방향값을 가지고 이동을 진행
    public void Move(float _speed, bool _state)
    {
        transform.Translate(Pos.normalized * Time.deltaTime * _speed);
        ChangeAnimationState_Move(_state);
    }

    //서버와 클라는 어느정도의 딜레이가 있다 때문에 클라에서 받은 시간을 계산해서 현재 위치를 예측해야한다
    //때문에 데드 레커닝을 사용해서 일부 값을 예측하여 딜레이가 없는 방법으로 만든다
    //지금은 유저수가 적어 딜레이는 크지 않지만 추후에 네트워크 딜레이가 크게 발생한다면 사용할 예정
    public Vector2 DeadReckonig(float NowSpeed, float OldSpeed, float time, Vector2 InputPos)
    {
        float a1 = NowSpeed * Time.deltaTime;
        float a2 = 0;
        if ((NowSpeed - OldSpeed) != 0)
        {
            a2 = 0.5f * ((NowSpeed - OldSpeed) / time) * Time.deltaTime * Time.deltaTime;
        }
        Vector2 result = new Vector2(
           NowPos.x = SyncPos.x - ((a1 + a2) * InputPos.x),
           NowPos.y = SyncPos.y - ((a1 + a2) * InputPos.y)
            );
        return result;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(gameObject.GetComponent<CapsuleCollider2D>(), collision.collider);
        }
    }

    void ChangeAnimationState_Move(bool _state) // 걷기
    {
        for (int i = 0; i < _subAnimators.Length; i++)
            _subAnimators[i].Move(_state);
    }

    void ChangeAnimationState_Attack() // 공격
    {
        for (int i = 0; i < _subAnimators.Length; i++)
            _subAnimators[i].Attack();
    }

    void ChangeAnimationState_Meteor() // 공격
    {
        for (int i = 0; i < _subAnimators.Length; i++)
            _subAnimators[i].Meteor();
    }

    void ShowDashBlur(bool _isStart) // 대시 표현
    {
        for (int i = 0; i < _subAnimators.Length; i++)
        {
            if (_subAnimators[i].active)
            {
                if (_isStart)
                    dashBlur[i].Play();
                else
                    dashBlur[i].Stop();
                break;
            }
        }
    }

    void FindItemDropObject()
    {
        RaycastHit2D _hit2D = Physics2D.Raycast(transform.position,_mouse_direction, 2f, _layerMask);
        itemDropObject = null;

        if (_hit2D.collider != null)
            itemDropObject = _hit2D.collider.GetComponent<ItemDropObject>();

        if (itemDropObject != null)
            itemDropObject.MinusCount(gameObject.name);
    }
    #region Invoke
    private void OFFWarriorInvEffect()
    {
        // 전사 무적 이펙트 해제
        invincibleWall.SetActive(false);
        Debug.Log("(서버)전사 무적 이펙트 해제");
    }
    private void OFFMagicianSkill()
    {
        PS = PlayerState.Idle;
        ObjectPoolingManager.instance.GetQueue_meteor(_mouse_direction, transform.position, gameObject.name);
        Debug.Log("(서버)메테오 소환");
        // 메테오 소환
    }
    #endregion
}

