using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniPangStayState : VMyState<GamemodeState>
{
    public override GamemodeState StateEnum => GamemodeState.AniPangStayState;
    
    protected override void EnterState() {
        UI_GameScene gc = (UI_GameScene)Managers.UI.SceneUI;
        gc.DisplayGameOverButton(true);
    }

    protected override void ExcuteState() {
        
    }

    protected override void ExitState() {
        
    }
}
