
public abstract class EnemyBaseState
{
    EnemyStateManager _context;
    EnemyStateFactory _factory;
    public EnemyStateManager Context { get => _context; set => _context = value; }
    public EnemyStateFactory Factory { get => _factory; set => _factory = value; }

    public EnemyBaseState(EnemyStateManager currentContext, EnemyStateFactory enemyStatefactory)
    {
        _context = currentContext;
        _factory = enemyStatefactory;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    protected void SwitchState(EnemyBaseState newState)
    {
        ExitState();
        Context.CurrentState = newState;
        newState.EnterState();
    }
}
