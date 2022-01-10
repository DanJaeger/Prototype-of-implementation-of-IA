using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : EnemyBaseState
{
    readonly static AttackingState instance = new AttackingState();
    AttackingState() { }
    public static AttackingState Instance
    {
        get
        {
            return instance;
        }
    }
    public override void EnterState(EnemyStateManager enemy)
    {
        //Start Attack
    }

    public override void ExitState(EnemyStateManager enemy)
    {
        //Start chasing or die
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        //Keep attacking
    }
}
