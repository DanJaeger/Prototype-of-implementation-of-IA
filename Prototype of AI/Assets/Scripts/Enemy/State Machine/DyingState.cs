
public class DyingState : EnemyBaseState
{
    public DyingState(EnemyStateManager currentContext, EnemyStateFactory enemyStateFactory)
        : base(currentContext, enemyStateFactory) { }
    public override void CheckSwitchStates()
    {
        
    }

    public override void EnterState()
    {
        Context.CanGetHit = false;
        Die();
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
        
    }
    void Die()
    {
        Context.Animator.Play("Die");
    }
}
