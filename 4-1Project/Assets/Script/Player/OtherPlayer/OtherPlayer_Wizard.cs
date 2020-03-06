using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayer_Wizard : MonoBehaviour
{
    public OhterPlayer_Actor actor;
    OtherPlayer_Command Attack, Skill, Move, command;

    // Start is called before the first frame update
    void Start()
    {
        actor = GetComponent<OhterPlayer_Actor>();
        SetCommand();
    }

    void SetCommand()
    {
        Move = new CommandServerMove();
        Attack = new CommandServerFire();
        Skill = new CommandServerSkill();
    }


    //체력 체크
    public void CheckHp()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (actor.PS != PlayerState.Die)
        {
            if (actor.SyncOn)
            {
                actor.ChangeNow();
            }

            if (actor.SetOn)
            {
                actor.SetPos();
            }

            command = GetCommand();


            if (command == Move)
            {
                command.Execute(actor, actor.Pos, actor.Speed);
            }
            if (command == Attack)
            {
                command.Execute(actor);
            }
        }

    }

    OtherPlayer_Command GetCommand()
    {
        if (actor.PS == PlayerState.Attack)
        {
            actor.PS = PlayerState.Idle;
            return Attack;
        }
        else if (actor.Pos.x != 0 || actor.Pos.y != 0)
        {
            return Move;
        }

        return null;
    }
}
