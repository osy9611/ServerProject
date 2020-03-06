using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//커멘드 패턴을 사용했다 
public class OtherPlayer_Command 
{
   public virtual void Execute(OhterPlayer_Actor actor) { }
   public virtual void Execute(OhterPlayer_Actor actor,Vector2 Pos,float Speed) { }
}

public class CommandServerMove : OtherPlayer_Command
{
    public override void Execute(OhterPlayer_Actor actor, Vector2 Pos, float Speed)
    {
        actor.Move(Speed);
    }
}

public class CommandServerFire : OtherPlayer_Command
{
    public override void Execute(OhterPlayer_Actor actor)
    {
        actor.Fire();
    }
}

public class CommandServerMeleeAttack : OtherPlayer_Command
{
    public override void Execute(OhterPlayer_Actor actor)
    {
        actor.MeleeAttack();
    }
}

public class CommandServerSkill : OtherPlayer_Command
{
    public override void Execute(OhterPlayer_Actor actor)
    {
        actor.Skill();
    }
}
