using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainStayState : VMyState<GamemodeState>
{
    public override GamemodeState StateEnum => GamemodeState.MainStayState;
    
    protected override void EnterState() {
        Managers.Board.ClearBoard();
        Managers.Board.ClearObservers();

        // TODO Awake 순서
        if (((UI_GameScene)Managers.UI.SceneUI).isInitialized) {
            ((UI_GameScene)Managers.UI.SceneUI).DisplayScoreText(false);
            ((UI_GameScene)Managers.UI.SceneUI).DisplayAniPangTimerBar(false);
        } 
    }

    protected override void ExcuteState() {
    }

    protected override void ExitState() {
    }
}