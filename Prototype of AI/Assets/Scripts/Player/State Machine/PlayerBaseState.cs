using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState
{
    private bool _isRootState = false;
    private PlayerStateManager _context;
    private PlayerStateFactory _factory;
    private PlayerBaseState _currentSubState;
    private PlayerBaseState _currentSuperState;

    public bool IsRootState { get => _isRootState; set => _isRootState = value; }
    public PlayerStateManager Context { get => _context; set => _context = value; }
    public PlayerStateFactory Factory { get => _factory; set => _factory = value; }

    public PlayerBaseState(PlayerStateManager currentContext, PlayerStateFactory playerStateFactory)
    {
        _context = currentContext;
        _factory = playerStateFactory;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    public void UpdateStates()
    {
        UpdateState();
        if (_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }
    protected void SwitchState(PlayerBaseState newState)
    {
        ExitState();
        newState.EnterState();
        if (_isRootState)
        {
            Context.CurrentState = newState;
        }
        else if (_currentSuperState != null)
        {
            _currentSuperState.SetSubState(newState);
        }
    }
    protected void SetSuperState(PlayerBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }
    protected void SetSubState(PlayerBaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
