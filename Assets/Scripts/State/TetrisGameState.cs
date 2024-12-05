using UnityEngine;

public class TetrisGameState : VMyState<GamemodeState>
{
    public override GamemodeState StateEnum => GamemodeState.TetrisGameState;
    
    protected override void EnterState() {
        Managers.Sound.PlayBGM(SoundManager.BGMEnum.Tetris);
        Managers.Score.Init();
        UI_GameScene gc = (UI_GameScene)Managers.UI.SceneUI;
        gc.DisplayScoreText(true);
        
        TetrisManager.Instance.Init();
        TetrisManager.Instance.StartGame();
    }

    protected override void ExcuteState() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            Move(-1, 0);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            Move(1, 0);
        }
        
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            Rotate();
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            Move(0, -1);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            MoveToFloor();
        }
    }

    protected override void ExitState() {
        Managers.Sound.StopBGM();
    }

    private void Move(int dx, int dy) {
        TetrisManager.Instance.MoveBlock(dx, dy);
    }

    private void MoveToFloor() {
        TetrisManager.Instance.MoveBlockToFloor();
    }

    private void Rotate() {
        TetrisManager.Instance.RotateBlock();
    }
    
}