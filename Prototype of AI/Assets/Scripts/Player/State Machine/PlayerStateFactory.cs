
using System.Collections.Generic;
enum PlayerStates
{
    idle,
    walk,
    fight,
    grounded,
    jump
}
public class PlayerStateFactory
{
    PlayerStateManager _context;
    Dictionary<PlayerStates, PlayerBaseState> _states = new Dictionary<PlayerStates, PlayerBaseState>();
    public PlayerStateFactory(PlayerStateManager currentContext)
    {
        _context = currentContext;
        _states[PlayerStates.idle] = new PlayerIdleState(_context, this);
        _states[PlayerStates.walk] = new PlayerWalkState(_context, this);
        _states[PlayerStates.fight] = new PlayerFightState(_context, this);
        _states[PlayerStates.jump] = new PlayerJumpState(_context, this);
        _states[PlayerStates.grounded] = new PlayerGroundedState(_context, this);
    }
    public PlayerBaseState Idle()
    {
        return _states[PlayerStates.idle];
    }
    public PlayerBaseState Walk()
    {
        return _states[PlayerStates.walk];
    }
    public PlayerBaseState Fight()
    {
        return _states[PlayerStates.fight];
    }
    public PlayerBaseState Jump()
    {
        return _states[PlayerStates.jump];
    }
    public PlayerBaseState Grounded()
    {
        return _states[PlayerStates.grounded];
    }
}
