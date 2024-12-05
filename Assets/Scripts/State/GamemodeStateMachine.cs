using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GamemodeState
{
    MainStayState,
    TetrisGameState,
    TetrisStayState,
    AniPangGameState,
    AniPangStayState
}

public class GamemodeStateMachine : StateMachine<GamemodeState>
{
    protected override void Awake() {
        foreach (GamemodeState state in Enum.GetValues(typeof(GamemodeState))) {
            string className = state.ToString();
            Type stateType = Type.GetType(className);
        
            if (stateType != null) {
                gameObject.AddComponent(stateType);
            }
            else {
                Debug.LogWarning($"{GetType().Name} | State class '{className}' not found.");
            }
        }
    
        base.Awake();
    }
}