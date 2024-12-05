using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


public class MinigameManager : SceneSingleton<MinigameManager>
{
    public GamemodeStateMachine stateMachine;

    // Start is called before the first frame update
    protected void Awake() {
        base.Awake();
        stateMachine = this.AddComponent<GamemodeStateMachine>();
    }

    public void ChangeState(GamemodeState state) {
        stateMachine.ChangeState(state);
    }


    public void ReStartGame() {
        GamemodeState nowMode = stateMachine.currentEnum;
        Debug.Log(nowMode);

        switch (nowMode) {
            case GamemodeState.TetrisStayState:
                ChangeState(GamemodeState.TetrisGameState);
                break;

            case GamemodeState.AniPangStayState:
                ChangeState(GamemodeState.AniPangGameState);
                break;
        }
    }

    public override void Init() {
        
    }
}