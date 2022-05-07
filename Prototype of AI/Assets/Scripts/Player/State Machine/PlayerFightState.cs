
public class PlayerFightState : PlayerBaseState
{
    public PlayerFightState(PlayerStateManager currentContext, PlayerStateFactory playerStateFactory)
   : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (Context.IsMovementPressed && !Context.IsFighting)
        {
            SwitchState(Factory.Walk());
        }
        else if (!Context.IsMovementPressed && !Context.IsFighting)
        {
            SwitchState(Factory.Idle());
        }
    }

    public override void EnterState()
    {
        Context.MovementVelocity = PlayerStateManager.IdleSpeed;

        Context.CanIPunch = true;
        Context.LightsPunchsCount = 0;
        StartLightCombo();
        Context.IsFighting = true;
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
        Tick();
    }

    void Tick()
    {
        if (Context.LightPunch)
        {
            StartLightCombo();
        }

        if (Context.CanIMove)
        {
            Context.MoveTowardsTarget();
            Context.CanIMove = false;
        }
    }

    void StartLightCombo()
    {
        if (Context.CanIPunch && Context.LightsPunchsCount <= 3)
            ++Context.LightsPunchsCount;

        if (Context.LightsPunchsCount == 1)
        {
            Context.Animator.SetInteger(Context.LightAttackHash, 1);
            Context.CanIMove = true;
        }
    }
}
