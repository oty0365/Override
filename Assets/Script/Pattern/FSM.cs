using System;
using UnityEngine;


public class StateEnum<T> where T:Enum
{
    public T stateEnum;
}

public abstract class State
{
    protected Enemy _enemy;
    public abstract void OnStateStart();
    public abstract void OnStateUpdate();
    public abstract void OnStateEnd();
}

public class FSM
{
    public State currentState;
    public void ChangeState(State next)
    {
        if (next == currentState)
        {
            return;
        }
        if(currentState != null)
        {
            currentState.OnStateEnd();
        }
        currentState = next;
        currentState.OnStateStart();
    }
    public void UpdateState()
    {
        if (currentState != null)
        {
            currentState.OnStateUpdate();
        }
    }

}
