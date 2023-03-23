using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMachine 
{
    public GameState currentState;


    public void OnStateUpdate() => currentState.OnStateUpdate?.Invoke();
  
    public void Init(GameState state)
    {
        currentState = state;
        currentState.OnStateEnter?.Invoke();
    }
    
    public void SetState(GameState newState)
    {
        currentState.OnStateExit?.Invoke();
        currentState = newState;
        currentState.OnStateEnter?.Invoke();
    }

}
