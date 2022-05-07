
public class PlayerGroundedState : PlayerBaseState, IRootState
{
    public PlayerGroundedState(PlayerStateManager currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    public override void CheckSwitchStates()
    {
        if (Context.IsJumpPressed)
        {
            SwitchState(Factory.Jump());
        }
    }

    public override void EnterState()
    {
        HandleGravity();
        InitializeSubState();
    }

    public override void ExitState()
    {

    }

    public void HandleGravity()
    {
        Context.CurrentMovementY = Context.Gravity;
    }

    public override void InitializeSubState()
    {
        if (Context.LightPunch)
        {
            SetSubState(Factory.Fight());
        }
        else if (Context.IsMovementPressed)
        {
            SetSubState(Factory.Walk());
        }
        else
        {
            SetSubState(Factory.Idle());
        }
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}
