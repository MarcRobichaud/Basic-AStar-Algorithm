using System;
using System.Collections.Generic;

public delegate bool Condition();
public delegate void Action();

public class StateMachine<T> where T : Enum
{
    public T initialState;
    public T currentState;
    public HashSet<T> endStates;

    public Dictionary<T, HashSet<Transition<T>>> Transitions { get; private set; }
    public Dictionary<T, Action> Actions { get; private set; }
    public HashSet<Transition<T>> AnyStateTransitions { get; private set; }

    public bool EndState => endStates.Contains(currentState);
    public bool looping = false;

    private bool TransitionCondition(Transition<T> transition) => transition.TransitionCondition != null && transition.TransitionCondition.Invoke();

    public StateMachine(T _initialState)
    {
        InitCollections();

        initialState = _initialState;

        Restart();
    }

    public StateMachine(T _initialState, HashSet<T> _endState)
    {
        InitCollections();

        initialState = _initialState;
        endStates = _endState;

        Restart();
    }

    private void InitCollections()
    {
        Transitions = new Dictionary<T, HashSet<Transition<T>>>();
        Actions = new Dictionary<T, Action>();
        AnyStateTransitions = new HashSet<Transition<T>>();
        endStates = new HashSet<T>();
    }

    public void Restart() => currentState = initialState;

    public void AddTransition(T startState, T nextState, Condition condition, Action onTransitionChanged)
    {
        Transition<T> transition = new Transition<T>(startState, nextState, condition, onTransitionChanged);
        AddTransition(transition);
    }

    public void AddTransition(Transition<T> transition)
    {
        if (!Transitions.TryGetValue(transition.StartState, out HashSet<Transition<T>> stateTransition))
        {
            stateTransition = new HashSet<Transition<T>>();
            Transitions.Add(transition.StartState, stateTransition);
        }
        stateTransition.Add(transition);
    }

    public void RemoveTransition(Transition<T> transition)
    {
        if (Transitions.TryGetValue(transition.StartState, out HashSet<Transition<T>> stateTransition))
            stateTransition.Remove(transition);
    }

    public void AddAnyStateTransition(T nextState, Condition condition, Action onTransitionChanged)
    {
        Transition<T> transition = new Transition<T>(default, nextState, condition, onTransitionChanged);
        AnyStateTransitions.Add(transition);
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
        if (!EndState)
        {
            InvokeAction();
            UpdateTransitions();
        }
        else if (looping)
        {
            Restart();
        }
    }

    public void InvokeAction() => Actions[currentState]?.Invoke();

    public void UpdateTransitions()
    {
        foreach (var transition in AnyStateTransitions)
        {
            if (CheckTransition(transition))
                return;
        }

        if (Transitions.TryGetValue(currentState, out HashSet<Transition<T>> stateTransition))
        {
            foreach (var transition in stateTransition)
            {
                if (CheckTransition(transition))
                    return;
            }
        }
    }

    private bool CheckTransition(Transition<T> transition)
    {
        bool transitionCondition = TransitionCondition(transition);
        if (transitionCondition)
        {
            currentState = transition.NextState;
            transition.OnTransitionChanged?.Invoke();
        }
        return transitionCondition;
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