using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using LitJson;

public class OhterPlayer_Actor : MonoBehaviour
{
    public PlayerState PS;

    //동기화 위치
    public Vector2 SyncPos;
    public Vector2 NowPos;

    //방향
    public Vector2 Pos;
    //회전값
    public Vector2 Rot;

    //공격 방향
    public Vector2 AttackDir;
    
    //동기화 체크
    public bool SyncOn;
    
    //위치값 체크
    public bool SetOn;

    Animator Ani;

    //플레이어 속도
    public float Speed;
    public float OldSpeed;

    //시간
    public float GetMillTime;
    public float time;

    //공격 애니메이션을 제어
    public bool AttackOn;
    public Text PosText;

    //오브젝트 풀링 매니져를 가져온다
    public PoolingManager poolingManager;

    //현재 위치 좌표를 텍스트 창에 보여주기는 하지만
    //클라간의 위치를 체크하기 위한 거라서 추후에 제외할 예정이다
    public void TextCount()
    {
        PosText.text = this.gameObject.name + " X: " + this.transform.position.x + " Y: " + this.transform.position.y;
    }

    private void Awake()
    {
        Ani = GetComponent<Animator>();
        poolingManager = GetComponent<PoolingManager>();
    }

    private void Update()
    {
        TextCount();

        //서버에서 받은 시간과 현재 시간을 비교하기 위해서 만듬
        time = (System.DateTime.Now.Hour * 3600) + (System.DateTime.Now.Minute * 60) + (System.DateTime.Now.Second);
        time *= 0.001f;
        
        if (Pos.x != 0 || Pos.y != 0)
        {
            Ani.SetBool("Run", true);
            Ani.SetBool("Idle", false);
        }
        else if (Pos == Vector2.zero)
        {
            Ani.SetBool("Idle", true);
            Ani.SetBool("Run", false);
        }

        Ani.SetFloat("DirX", Rot.x);
        Ani.SetFloat("DirY", Rot.y);
    }

    //Json 데이터들을 파싱하여 데이터를 갱신한다
    public void SetInput(JsonData Data)
    {
        SyncPos.x = float.Parse(Data["nx"].ToString());
        SyncPos.y = float.Parse(Data["ny"].ToString());

        OldSpeed = Speed;
        Speed = float.Parse(Data["Speed"].ToString());

        GetMillTime = float.Parse(Data["time"].ToString());

        Pos.x = float.Parse(Data["x"].ToString());
        Pos.y = float.Parse(Data["y"].ToString());

        Rot.x = float.Parse(Data["rx"].ToString());
        Rot.y = float.Parse(Data["ry"].ToString());

        AttackDir.x = float.Parse(Data["ax"].ToString());
        AttackDir.y = float.Parse(Data["ay"].ToString());

        PS = (PlayerState)int.Parse(Data["State"].ToString());

        SyncOn = true;
    }

    //만약에 위치를 동기화하는게 아니라면 서버에서 받은 방향값을 가지고 이동을 진행
    public void Move(float Speed)
    {
        if (!SetOn)
        {
            transform.Translate(Pos.normalized * Time.deltaTime * Speed);
        }     
    }

    
    //동기화 On
    public void ChangeNow()
    {
        SetOn = true;
        SyncOn = false;
    }

    public void SetPos()
    {
        transform.position = Vector2.MoveTowards(transform.position, SyncPos, Speed * Time.smoothDeltaTime);
        if (Vector2.Distance(transform.position, SyncPos) == 0)
        {
            SetOn = false;
        }
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
            Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), collision.collider);
        }
    }

    public void Fire()
    {
        Ani.SetTrigger("Attack");
        poolingManager.ObjOn("IceBall");
    }

    public void MeleeAttack()
    {
        Ani.SetTrigger("Attack");
    }

    public void Skill()
    {

    }

}

