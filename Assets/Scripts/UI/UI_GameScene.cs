using System;
using System.Collections;
using System.Collections.Generic;
using Microlight.MicroBar;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary> 게임을 진행하는 씬의 UI 관리 </summary>
public class UI_GameScene : UI_Base, IMyObserver
{
    enum TextmeshPro
    {
        Text_Score
    }

    enum Buttons
    {
        Btn_Restart,
        Btn_BackToMenu,
        Btn_StartTetris,
        Btn_StartAniPang
    }

    enum MicroBars
    {
        MicroBar_AniPangBar
    }
    
    public bool isInitialized = false;
    
    // MicroBar
    private MicroBar mb;
    public float aniPangTimerMax = 50.0f;
    
    public override void Init() {
        Bind<TextMeshProUGUI>(typeof(TextmeshPro));
        Bind<Button>(typeof(Buttons));
        Bind<MicroBar>(typeof(MicroBars));
        
        BindEvent(GetButton((int)Buttons.Btn_Restart), data => ClickRestartButton());
        BindEvent(GetButton((int)Buttons.Btn_BackToMenu), data => ClickBackToMenuButton());
        BindEvent(GetButton((int)Buttons.Btn_StartTetris), data => ClickStartTetris());
        BindEvent(GetButton((int)Buttons.Btn_StartAniPang), data => ClickStartAniPang());
        
        
        
        SetScoreText(0);
        Managers.Score.AddObserver(this);
        
        HideAllButtons();
        DisplayScoreText(false);
        DisplayAniPangTimerBar(false);
        

        isInitialized = true;
    }

    #region Score Observer
    private void SetScoreText(int totalScore) {
        GetTMP((int)TextmeshPro.Text_Score).text = $"점수 : {totalScore}";
    }

    public void OnNotify(int amount, int totalScore) {
        SetScoreText(totalScore);
        UpdateAniPangTimerBar(amount);
    }

    #endregion

    #region For AniPang Timer Bar

    public void InitializeAniPangTimerBar() {
        GetSomething<MicroBar>((int)MicroBars.MicroBar_AniPangBar).Initialize(aniPangTimerMax);
    }
    
    public void UpdateAniPangTimerBar(float amount) {
        if (MinigameManager.Instance.stateMachine.currentEnum != GamemodeState.AniPangGameState) return;
        
        MicroBar mb = GetSomething<MicroBar>((int)MicroBars.MicroBar_AniPangBar);
        float newValue = mb.CurrentValue + amount;
        
        if (amount > 0) mb.UpdateBar(newValue, false, UpdateAnim.Heal);
        else mb.UpdateBar(newValue, true, UpdateAnim.Damage);
    }

    public float GetAniPangTimerValue() {
        return GetSomething<MicroBar>((int)MicroBars.MicroBar_AniPangBar).CurrentValue;
    }

    #endregion
    
    #region Button Method
    public void ClickRestartButton() {
        Debug.Log("ClickRestartButton");
        
        // TODO 재시작 수정 필요
        MinigameManager.Instance.ReStartGame();
        HideAllButtons();
    }

    public void ClickBackToMenuButton() {
        Debug.Log("ClickBackToMenuButton");
        HideAllButtons();
        DisplayMenuButton(true);
        MinigameManager.Instance.ChangeState(GamemodeState.MainStayState);
        // 보드 클리어
    }

    public void ClickStartTetris() {
        Debug.Log("ClickStartTetris");
        HideAllButtons();
        MinigameManager.Instance.ChangeState(GamemodeState.TetrisGameState);
    }

    public void ClickStartAniPang() {
        Debug.Log("ClickStartAniPang");
        HideAllButtons();
        
        MinigameManager.Instance.ChangeState(GamemodeState.AniPangGameState);
    }
    
    #endregion

    #region Display Elements

    public void DisplayScoreText(bool isVisible) {
        GetTMP((int)TextmeshPro.Text_Score).gameObject.SetActive(isVisible);
    }

    public void DisplayAniPangTimerBar(bool isVisible) {
        GetSomething<MicroBar>((int)MicroBars.MicroBar_AniPangBar).gameObject.SetActive(isVisible);
    }
    
    public void HideAllButtons() {
        foreach (Buttons bEnum in Enum.GetValues(typeof(Buttons))) {
            GetButton((int)bEnum).gameObject.SetActive(false);
        }
    }

    public void DisplayMenuButton(bool isVisible) {
        GetButton((int)Buttons.Btn_StartTetris).gameObject.SetActive(isVisible);
        GetButton((int)Buttons.Btn_StartAniPang).gameObject.SetActive(isVisible);
    }
    
    public void DisplayGameOverButton(bool isVisible) {
        GetButton((int)Buttons.Btn_Restart).gameObject.SetActive(isVisible);
        GetButton((int)Buttons.Btn_BackToMenu).gameObject.SetActive(isVisible);
    }
    
    #endregion

    
}