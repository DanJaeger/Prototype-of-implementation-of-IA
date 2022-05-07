using System.Collections.Generic;
    enum EnemyStates
    {
        patrol,
        chase,
        attack,
        die
    }
public class EnemyStateFactory
{
    EnemyStateManager _context;
    Dictionary<EnemyStates, EnemyBaseState> _states = new Dictionary<EnemyStates, EnemyBaseState>();

    internal Dictionary<EnemyStates, EnemyBaseState> States { get => _states; set => _states = value; }

    public EnemyStateFactory(EnemyStateManager currentContext)
    {
        _context = currentContext;
        _states[EnemyStates.patrol] = new PatrollingState(_context, this);
        _states[EnemyStates.chase] = new ChasingState(_context, this);
        _states[EnemyStates.attack] = new AttackingState(_context, this);
        _states[EnemyStates.die] = new DyingState(_context, this);
    }
    public EnemyBaseState Patrol()
    {
        return _states[EnemyStates.patrol];
    }
    public EnemyBaseState Chase()
    {
        return _states[EnemyStates.chase];
    }
    public EnemyBaseState Attack()
    {
        return _states[EnemyStates.attack];
    }
    public EnemyBaseState Die()
    {
        return _states[EnemyStates.die];
    }
}
