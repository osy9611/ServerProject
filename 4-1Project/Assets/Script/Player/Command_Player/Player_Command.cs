using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Command
{
    public virtual void Execute(Player_Actor actor) { }
    public virtual void Execute(Player_Actor actor, Vector2 Pos,float Speed) { }
}

public class CommandMove : Player_Command
{
    public override void Execute(Player_Actor actor,Vector2 Pos,float Speed)
    {
        actor.Move(Pos,Speed);
    }
}



public class CommandFire :Player_Command
{
    public override void Execute(Player_Actor actor)
    {
        actor.Fire();
    }
}

public class CommandMeleeAttack : Player_Command
{
    public override void Execute(Player_Actor actor)
    {
        actor.MeleeAttack();
    }
}

public class CommandSkill : Player_Command
{
    public override void Execute(Player_Actor actor)
    {
        actor.Skill();
    }
}


