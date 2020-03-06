using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DummyPlaeyer : MonoBehaviour
{
    public float Speed;

    Vector2 Pos;

    public Player_Actor actor;
    Player_Command Attack, Skill, Move,command;

    private void Start()
    {
        actor = GetComponent<Player_Actor>();
        SetCommand();
    }

    void SetCommand()
    {
        Move = new CommandMove();
        Attack = new CommandMeleeAttack();
        Skill = new CommandSkill();
    }

    // Update is called once per frame
    void Update()
    {
        Pos.x = Input.GetAxisRaw("Horizontal");
        Pos.y = Input.GetAxisRaw("Vertical");

        command = GetCommand();

        if(command == Move)
        {
            command.Execute(actor, Pos,Speed);
        }
    }

    Player_Command GetCommand()
    {
        if(Pos.x !=0 || Pos.y !=0)
        {
            return Move;
        }
        return null;
    }
}
