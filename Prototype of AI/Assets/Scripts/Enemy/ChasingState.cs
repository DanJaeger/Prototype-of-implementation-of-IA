using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingState : EnemyBaseState
{
    readonly static ChasingState instance = new ChasingState();
    ChasingState() { }
    public static ChasingState Instance
    {
        get
        {
            return instance;
        }
    }
    public override void EnterState(EnemyStateManager enemy)
    {
        enemy.StartCoroutine("UpdatePath");
    }

    public override void ExitState(EnemyStateManager enemy)
    {
        enemy.StopCoroutine("UpdatePath");
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        enemy.CheckIfCanChase();
        enemy.Chase();

        if (enemy.Player == null) { 
            enemy.PlayerOutOfView();
            if (enemy.PlayerIsOutOfView)
                enemy.ChangeState(PatrollingState.Instance);
        }
    }
}
