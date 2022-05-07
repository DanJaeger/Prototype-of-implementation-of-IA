
public class ChasingState : EnemyBaseState
{
    public ChasingState(EnemyStateManager currentContext, EnemyStateFactory enemyStateFactory)
     : base(currentContext, enemyStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (Context.CanAttack)
        {
            SwitchState(Factory.Attack());
        }else if (Context.IsPlayerOutOfView)
        {
            SwitchState(Factory.Patrol());
        }else if (Context.Health <= 0)
        {
            SwitchState(Factory.Die());
        }
    }
    public override void EnterState()
    {
        Context.StartCoroutine("UpdatePath");
    }
    public override void ExitState()
    {
        Context.StopCoroutine("UpdatePath");
    }
    public override void UpdateState()
    {
        Context.CheckIfCanChase();
        Chase();
        Context.CheckIfCanAttack();
        if(Context.PlayerGameObject == null)
        {
            Context.PlayerOutOfView();
        }
        CheckSwitchStates();
    }
    public void Chase()
    {
        if (!Context.CanChase())
        {
            Context.StopCoroutine("Move");
            Context.Animator.SetBool("IsWalking", false);
        }
    }
}
