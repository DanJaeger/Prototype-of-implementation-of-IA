using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingState : EnemyBaseState
{
    readonly static PatrollingState instance = new PatrollingState();
    PatrollingState() { }
    public static PatrollingState Instance
    {
        get
        {
            return instance;
        }
    }
    public override void EnterState(EnemyStateManager enemy)
    {
        //Change Objective
    }

    public override void ExitState(EnemyStateManager enemy)
    {
        //ChangeState to Chasing
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        enemy.Patrol();
        if (enemy.CanChase())
            enemy.ChangeState(ChasingState.Instance);
    }
}
