using DG.Tweening;
using UnityEngine;

public class AttackingState : EnemyBaseState
{
    public AttackingState(EnemyStateManager currentContext, EnemyStateFactory enemyStateFactory)
    : base(currentContext, enemyStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (!Context.CanAttack)
        {
            SwitchState(Factory.Chase());
        }else if (Context.Health <= 0)
        {
            SwitchState(Factory.Die());
        }
    }

    public override void EnterState()
    {
        
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
        Context.CheckIfCanAttack();
        if (Context.CanAttack)
        {
            Attack();
        }
        CheckSwitchStates();
    }
    
    void Attack()
    {
        if (Context.CanPunch)
        {
            Context.Animator.Play("RightPunch");
            Context.CanPunch = false;
            Context.transform.DOLookAt(Context.PlayerTransform.position, 0.15f);
            Context.transform.DOMove(TargetOffset(Context.PlayerTransform), 0.4f);
        }
    }
    Vector3 TargetOffset(Transform target)
    {
        Vector3 position;
        position = target.position;
        return Vector3.MoveTowards(position, Context.transform.position, .85f);
    }
}
