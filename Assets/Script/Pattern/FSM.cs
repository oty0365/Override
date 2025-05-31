using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class State
{
    public Enemy enemy;
    public abstract void OnStateStart();

    public abstract void OnStateFixedUpdate();
    public abstract void OnStateUpdate();
    public abstract void OnStateEnd();

    public T GetEnemyAs<T>() where T : Enemy
    {
        return enemy as T;
    }
}

public class FSM
{
    public State currentState;
    public Dictionary<string, State> states = new();

    public void AddState(string name,State state,Enemy target)
    {
        state.enemy = target;
        states.Add(name, state);
    }
    public void InitState()
    {
        states.Clear();
    }
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
    public void FixedUpdateState()
    {
        if (currentState != null)
        {
            currentState.OnStateFixedUpdate();
        }
    }

}
