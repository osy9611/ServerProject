using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.UI;

public class Player_Actor : MonoBehaviour
{
    public Animator Ani;

    public Vector2 Pos;
    public Vector2 MousePos;
    public Vector2 Rot;
    public PlayerData Data;

    Camera camera;

    public bool AttackOn;

    [HideInInspector]
    public Text PosText;

    public void Start()
    {
        Ani = GetComponent<Animator>();
        camera = FindObjectOfType<Camera>();
        Data.Init(GameManager.instance.PlayerName);
    }

    private void Update()
    {
        PosText.text = "MyPlayer X: " + this.transform.position.x + " Y: " + this.transform.position.y;
    }

    public void Move(Vector2 Pos,float Speed)
    {
        transform.Translate(Pos.normalized * Time.deltaTime * Speed);
    }

    public void Fire()
    {

    }

    public void MeleeAttack()
    {
        Ani.SetTrigger("Attack");    
    }

    public void Skill()
    {

    }
    
    public void InputCheck()
    {
        //키를 입력받은게 있으면 바로바로 데이터를 전송시킨다
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)
           || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.W)

           )
        {
            JsonData SendData = JsonMapper.ToJson(Data);
            ServerClient.instance.Send(SendData.ToString());
        }

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)
           || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.W))
        {
            JsonData SendData = JsonMapper.ToJson(Data);
            ServerClient.instance.Send(SendData.ToString());
        }

       
        if(Input.GetMouseButtonDown(0))
        {
            Data.State = (int)PlayerState.Attack;
            JsonData SendData = JsonMapper.ToJson(Data);
            ServerClient.instance.Send(SendData.ToString());
            Data.State = 0;
        }
    }

    //마우스 위치를 바라보기 위함
    public void RotationCheck()
    {
        MousePos = Input.mousePosition;
        MousePos = camera.ScreenToWorldPoint(MousePos);

        Data.ax = MousePos.x;
        Data.ay = MousePos.y;

        Vector2 vRot = MousePos - (Vector2)transform.position;
        int x = Mathf.RoundToInt(vRot.normalized.x);
        int y = Mathf.RoundToInt(vRot.normalized.y);

        Vector2 CheckRot = new Vector2(x, y);
        if (Rot != CheckRot)
        {
            Rot = CheckRot;
            Data.rx = CheckRot.x;
            Data.ry = CheckRot.y;

            JsonData SendData = JsonMapper.ToJson(Data);
            ServerClient.instance.Send(SendData.ToString());
        }
       
    }

    public void AniUpdate()
    {
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


        if (Ani.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            AttackOn = true;
        }
        else
        {
            AttackOn = false;
        }
    }

}
