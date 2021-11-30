using System;
using System.Collections.Generic;

public delegate bool Condition();
public delegate void Action();

public class StateMachine<T> where T : Enum
{
    public T currentState;
    public T endState;

    public Dictionary<T, List<Transition<T>>> Transitions { get; private set;}
    public Dictionary<T, Action> Actions { get; private set; }

    public StateMachine(T initialState)
    {
        Transitions = new Dictionary<T, List<Transition<T>>>();
        Actions = new Dictionary<T, Action>();
        currentState = initialState;
    }

    public StateMachine(T _initialState, T _endState)
    {
        Transitions = new Dictionary<T, List<Transition<T>>>();
        Actions = new Dictionary<T, Action>();
        currentState = _initialState;
        endState = _endState;
    }

    public void AddTransition(T startState, T nextState, Condition condition, Action onTransitionChanged)
    {
        if (!Transitions.TryGetValue(startState, out List<Transition<T>> stateTransition))
        {
            stateTransition = new List<Transition<T>>();
            Transitions.Add(startState, stateTransition);
        }
        stateTransition.Add(new Transition<T>(startState, nextState, condition, onTransitionChanged));
    }

    public void RemoveTransition(Transition<T> transition)
    {
        if (Transitions.TryGetValue(transition.StartState, out List<Transition<T>> stateTransition))
            stateTransition.Remove(transition);
    }

    public void AddAction(T state, Action _action)
    {
        if (Actions.ContainsKey(state))
            Actions[state] += _action;
        else
            Actions.Add(state, _action);
    }

    public void Update()
    {
        InvokeAction();
        UpdateTransition();
    }

    public void InvokeAction()
    {
        Actions[currentState]?.Invoke();
    }

    public void UpdateTransition()
    {
        if (Transitions.TryGetValue(currentState, out List<Transition<T>> stateTransition))
        {
            foreach (var transition in stateTransition)
            {
                if (transition.TransitionCondition != null && transition.TransitionCondition.Invoke())
                {
                    currentState = transition.NextState;
                    transition.OnTransitionChanged?.Invoke();
                    return;
                }
            }
        }
    }
}

public class Transition<T> where T : Enum
{
    public T StartState;
    public T NextState;
    public Condition TransitionCondition;
    public Action OnTransitionChanged;

    public Transition(T startState, T nextState, Condition condition, Action onTransitionChanged)
    {
        StartState = startState;
        NextState = nextState;
        TransitionCondition = condition;
        OnTransitionChanged = onTransitionChanged;
    }
}