using UnityEngine;

public class PatrollingState : EnemyBaseState
{
    public PatrollingState(EnemyStateManager currentContext, EnemyStateFactory enemyStateFactory)
     : base(currentContext, enemyStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (!Context.IsPlayerOutOfView)
        {
            SwitchState(Factory.Chase());
        }else if(Context.Health <= 0)
        {
            SwitchState(Factory.Die());
        }
    }

    public override void EnterState()
    {
        Context.CurrentTarget = Context.GetPatrolPoint();
        Context.GetPath(Context.CurrentTarget);
    }

    public override void ExitState()
    {
        Context.StopCoroutine("Move");
    }

    public override void UpdateState()
    {
        Patrol();
        Context.CheckIfCanChase();
        CheckSwitchStates();
    }
    public void Patrol()
    {
        Transform nextPatrolPoint = Context.PatrolPoints[Context.CurrentPatrolPointsIndex];
        float distanceToNextPoint = (nextPatrolPoint.position - Context.transform.position).sqrMagnitude;
        if (distanceToNextPoint < 0.1f)
        {
            Context.StopCoroutine("Move");
            Context.Animator.SetBool("IsWalking", false);

            Context.CurrentPatrolPointsIndex = (Context.CurrentPatrolPointsIndex + 1) % Context.PatrolPoints.Length;
            Context.CurrentTarget = Context.PatrolPoints[Context.CurrentPatrolPointsIndex];
            Context.GetPath(Context.CurrentTarget);
        }
    }
}
