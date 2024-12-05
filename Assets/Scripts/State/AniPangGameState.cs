using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniPangGameState : VMyState<GamemodeState>
{
    public override GamemodeState StateEnum => GamemodeState.AniPangGameState;

    protected override void EnterState() {
        Managers.Sound.PlayBGM(SoundManager.BGMEnum.AniPang);
        Managers.Score.Init();
        UI_GameScene gc = (UI_GameScene)Managers.UI.SceneUI; 
        gc.DisplayScoreText(true);
        gc.InitializeAniPangTimerBar();
        gc.DisplayAniPangTimerBar(true);
        
        AniPangManager.Instance.Init();
        AniPangManager.Instance.StartGame();
        
        Time.timeScale = 5.0f;
    }

    protected override void ExcuteState() {
    }

    protected override void ExitState() {
        Time.timeScale = 1.0f;
        Managers.Sound.StopBGM();
    }
}
