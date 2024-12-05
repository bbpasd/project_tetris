using UnityEngine;

public class TetrisStayState : VMyState<GamemodeState>
{
    public override GamemodeState StateEnum => GamemodeState.TetrisStayState;
    
    protected override void EnterState() {
        UI_GameScene gc = (UI_GameScene)Managers.UI.SceneUI;
        gc.DisplayGameOverButton(true);
    }

    protected override void ExcuteState() {
    }

    protected override void ExitState() {
    }
}