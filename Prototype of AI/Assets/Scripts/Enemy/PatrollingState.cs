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
        
    }

    public override void ExitState(EnemyStateManager enemy)
    {
        //DO SOMETHING
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        enemy.Patrol();
        enemy.CheckIfCanChase();

        if (!enemy.PlayerIsOutOfView)
            enemy.ChangeState(ChasingState.Instance);
    }
}
