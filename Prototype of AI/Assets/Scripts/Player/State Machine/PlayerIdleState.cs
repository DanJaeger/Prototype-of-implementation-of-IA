
public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateManager currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    public override void CheckSwitchStates()
    {
        if (Context.LightPunch)
        {
            SwitchState(Factory.Fight());
        }
        else if (Context.IsMovementPressed)
        {
            SwitchState(Factory.Walk());
        }
    }

    public override void EnterState()
    {
        Context.MovementVelocity = PlayerStateManager.IdleSpeed;
    }

    public override void ExitState()
    {

    }

    public override void InitializeSubState()
    {

    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}
