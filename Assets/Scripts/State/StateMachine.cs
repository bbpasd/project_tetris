using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class NotifyBase
{
}

public interface IVMyState
{
    // 현재 상태에게 무언가를 알리려고 할때 쓰는 API
    void OnNotify<T, T2>(T eValue, T2 vValue) where T : Enum where T2 : NotifyBase;

    // 상태가 시작될때 불리는 API
    public void EnterStateWrapper();

    // Update와 같음
    public void ExcuteStateWrapper();

    // 상태가 끝날 때 불리는 API
    public void ExitStateWrapper();

    // FixedUpdate
    public void ExcuteState_FixedUpdateWrapper();

    // LateUpdate
    public void ExcuteState_LateUpdateWrapper();

    // 상태를 추가해주는 api
    public void AddState<T>(StateMachine<T> owner, ref object _states) where T : Enum;
}

public abstract class VMyState<T> : MonoBehaviour, IVMyState where T : Enum
{
    public abstract T StateEnum { get; }
    [NonSerialized] public StateMachine<T> OwnerStateMachine;

    protected virtual void Awake() {
    }

    protected virtual void Start() {
    }

    public void OnNotify<T1, T2>(T1 eValue, T2 vValue) where T1 : Enum where T2 : NotifyBase {
        throw new NotImplementedException();
    }

    public void EnterStateWrapper() {
        Debug.Log($"{StateEnum.ToString()} | EnterState");
        EnterState();
    }

    public void ExcuteStateWrapper() {
        ExcuteState();
    }

    public void ExitStateWrapper() {
        Debug.Log($"{StateEnum.ToString()} | ExitState");
        ExitState();
    }

    public void ExcuteState_FixedUpdateWrapper() {
    }

    public void ExcuteState_LateUpdateWrapper() {
    }

    public void AddState<T1>(StateMachine<T1> owner, ref object _states) where T1 : Enum {
        var cast = _states as Dictionary<T, IVMyState>;
        OwnerStateMachine = owner as StateMachine<T>;
        cast?.Add(StateEnum, this);
    }

    protected abstract void EnterState();

    protected abstract void ExcuteState();

    protected abstract void ExitState();
}

/// <summary> 상태관리머신 </summary>
public class StateMachine<T> : MonoBehaviour where T : Enum
{
    [SerializeField] private T defaultState;
    private IVMyState _currentMyState;

    // TODO 수정필요? 일단 임시로 넣음
    public T currentEnum;

    private Dictionary<T, IVMyState> _states = new();

    public void OnNotify<T1, T2>(T1 eValue, T2 vValue) where T1 : Enum where T2 : NotifyBase {
        _currentMyState.OnNotify(eValue, vValue);
    }

    private void ChangeState_Internal(IVMyState newMyState) {
        if (_currentMyState != null) {
            _currentMyState.ExitStateWrapper();
        }

        if (newMyState == null) {
            _currentMyState = null;
            return;
        }

        _currentMyState = newMyState;
        _currentMyState.EnterStateWrapper();
    }

    public void ChangeStateNull() {
        ChangeState_Internal(null);
    }

    public void ChangeState(T state) {
        currentEnum = state;
        if (_states.TryGetValue(state, out var newState)) {
            ChangeState_Internal(newState);
        }
    }

    protected virtual void Awake() {
        var stateList = GetComponents<IVMyState>().ToList();
        foreach (var state in stateList) {
            object states = _states;
            state.AddState(this, ref states);
        }
    }

    protected virtual void Start() {
        ChangeState(defaultState);
    }

    void Update() {
        if (_currentMyState != null) {
            _currentMyState.ExcuteStateWrapper();
        }
    }

    private void FixedUpdate() {
        if (_currentMyState != null) {
            _currentMyState.ExcuteState_FixedUpdateWrapper();
        }
    }
}