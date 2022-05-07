
public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateManager currentContext, PlayerStateFactory playerStateFactory)
   : base(currentContext, playerStateFactory) { }
    public override void CheckSwitchStates()
    {
        if (Context.LightPunch)
        {
            SwitchState(Factory.Fight());
        }
        else if (!Context.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        }
    }

    public override void EnterState()
    {
        Context.Animator.SetBool(Context.IsWalkingHash, true);
        Context.MovementVelocity = PlayerStateManager.WalkSpeed;
    }

    public override void ExitState()
    {
        Context.Animator.SetBool(Context.IsWalkingHash, false);
    }

    public override void InitializeSubState()
    {

    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}
