using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Wizard : MonoBehaviour
{
    PlayerState PS;

    public float Speed;

    public float timer;

    public Player_Actor actor;
    Player_Command Attack, Skill, Move, command;

    public PoolingManager poolingManager;

    void Awake()
    {
        actor = GetComponent<Player_Actor>();
        poolingManager = GetComponent<PoolingManager>();
        SetCommand();
    }

    //각각의 명령을 셋팅함
    void SetCommand()
    {
        Move = new CommandMove();
        Attack = new CommandMeleeAttack();
        Skill = new CommandSkill();
    }

    //현재 위치값과 방향값, 시간, 입력값을 실시간으로 체크한다
    public void MoveCheck()
    {
        //현재 내위치
        actor.Data.x = actor.Pos.x;
        actor.Data.y = actor.Pos.y;

        actor.Data.nx = transform.position.x;
        actor.Data.ny = transform.position.y;

        actor.Data.time = (System.DateTime.Now.Hour * 3600) + (System.DateTime.Now.Minute * 60) + (System.DateTime.Now.Second);
        actor.Data.time *= 0.001f;

        actor.Data.Speed = Speed;

        //입력하는 키를 체크한다
        actor.InputCheck();

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), collision.collider);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (PS != PlayerState.Die)
        {
            actor.Pos.x = Input.GetAxisRaw("Horizontal");
            actor.Pos.y = Input.GetAxisRaw("Vertical");

            //회전값을 체크함
            actor.RotationCheck();

            //움직임 체크
            MoveCheck();

            command = GetCommand();

            if (command == Move)
            {
                command.Execute(actor, actor.Pos, Speed);
            }
            if (command == Attack)
            {
                poolingManager.ObjOn("IceBall");
                command.Execute(actor);
            }

            actor.AniUpdate();
        }
    }

    Player_Command GetCommand()
    {
        if (Input.GetMouseButtonDown(0))
        {
            return Attack;
        }
        else if (actor.Pos.x != 0 || actor.Pos.y != 0)
        {
            return Move;
        }
        return null;
    }
}
