using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

/// <summary> 테트리스 게임 알고리즘과 여러 정의를 관리 </summary>
public class TetrisManager : SceneSingleton<TetrisManager>, IBoardObserver
{
    public enum TetrominoEnum
    {
        O,
        T,
        S,
        Z,
        J,
        L
    }

    public struct Tetromino
    {
        public List<GameObject> list;
        public int[,] Shape;
        public int X, Y;
        public TetrominoEnum type;
        public int rotateIdx;

        public Tetromino(TetrominoEnum typeEnum) {
            list = new();
            Shape = new int[4, 4];
            rotateIdx = 0;
            X = Define.TBoardWidth / 2 - 3;
            Y = Define.TBoardHeight - 1; // 3
            type = typeEnum;
        }
    }

    private Tetromino currentBlock = new();
    private Tetromino shadowBlock = new();

    public Vector3[,] gridPos;
    public GameObject[,] grid;
    public GameObject blockParent;
    public GameObject currentBlockParent;
    public GameObject shadowParent;

    private Coroutine tetrisGameCoroutine;
    public GameObject blockPrefab;

    #region Init Setting

    public void Init() {
        blockPrefab = blockPrefab ?? Managers.Resource.LoadPrefab("Prefabs/Block/Block");
        if (grid == null) CreateGrid();
        else ClearGrid();

        Managers.Board.AddObserver(this);
    }
    
    public void ClearBoard() {
        Managers.Resource.Destroy(blockParent);
        Managers.Resource.Destroy(shadowParent);
        Managers.Resource.Destroy(currentBlockParent);
        Managers.Resource.Destroy(Managers.Board.BoardPaent);
        
        ClearGrid();
        grid = null;
    }

    private void ClearGrid() {
        if (grid == null) return;
        
        for (int w = 0; w < Define.TBoardWidth; w++) {
            for (int h = 0; h < Define.TBoardHeight; h++) {
                    if (grid[w, h] == null) continue;

                Managers.Resource.Destroy(grid[w, h]);
                grid[w, h] = null;
            }
        }
    }

    public void CreateGrid() {
        grid = new GameObject[Define.TBoardWidth, Define.TBoardHeight];
        gridPos = Managers.Board.CreateBlankBoard(Define.TBoardWidth, Define.TBoardHeight, 
            Define.TBoardBlockSize, Define.TBoardBlockInterval);
    }

    #endregion

    public void GameLose() {
        Debug.Log("Tetris GameLose");
        MinigameManager.Instance.ChangeState(GamemodeState.TetrisStayState);
        StopCoroutine(tetrisGameCoroutine);
    }

    public void StartGame() {
        tetrisGameCoroutine = StartCoroutine(TetrisProgress());
    }

    private IEnumerator TetrisProgress() {
        CreateMyBlockAndShadow();
        UpdateBlock(currentBlock);
        UpdateBlock(shadowBlock);

        while (true) {
            if (!MoveBlock(0, -1)) {
                PlaceMyBlock();
                CheckLine();
                CreateMyBlockAndShadow();
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    public bool IsCollision(Tetromino tet) {
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                int nowX = tet.X + j;
                int nowY = tet.Y + i;
                if (tet.Shape[i, j] == 0 ||
                    nowY >= Define.TBoardHeight) continue; // 보드칸 위를 뚫은 경우는 예외

                if (nowX < 0 || nowX >= Define.TBoardWidth || // 좌우 보드 끝일 경우
                    nowY < 0 || // 맨 바닥일 경우
                    grid[nowX, nowY] != null) { // 블록이 있는 경우
                    return true;
                }
            }
        }

        return false;
    }

    #region Block

    public bool CreateMyBlockAndShadow() {
        int randIdx = Random.Range(0, Define.BlockAmount);

        // myBlock 생성
        if (currentBlockParent == null) {
            currentBlockParent = Managers.Resource.CreateEmpty("@MyBlocks");
        }

        currentBlock = CreateNewBlock(currentBlockParent, randIdx);

        // 생성하자마자 충돌하면 게임 패배
        if (IsCollision(currentBlock)) {
            GameLose();
            return false;
        }

        if (shadowParent == null) {
            shadowParent = Managers.Resource.CreateEmpty("@Shadows");
        }

        // Shadow 블록 생성
        shadowBlock = CreateNewBlock(shadowParent, randIdx);
        for (int i = 0; i < 4; i++) {
            shadowBlock.list[i].GetComponent<TetrisBlock>().SetTransparency(0.4f);
        }

        return true;
    }

    public Tetromino CreateNewBlock(GameObject parent, int typeIdx) {
        Tetromino tet = new Tetromino((TetrominoEnum)typeIdx);
        int offset = typeIdx * 4 * 4 * 4 * sizeof(int);
        Buffer.BlockCopy(TetrisBlockTemplate.Block, offset,
            tet.Shape, 0, 4 * 4 * sizeof(int)); // 블록카피로 복사

        for (int i = 0; i < 4; i++) {
            GameObject go = Managers.Resource.Instantiate(blockPrefab, new Vector3(0, -10, 0));
            go.transform.parent = parent.transform;
            TetrisBlock tb = go.GetComponent<TetrisBlock>();
            tb.colorEnum = (Define.ColorEnum)typeIdx;
            tb.SetColor();
            tet.list.Add(go);
        }

        return tet;
    }

    private void UpdateBlock(Tetromino tet) {
        int count = 0;

        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                if (tet.Shape[i, j] == 0) continue; // 빈칸 통과
                int nowX = tet.X + j;
                int nowY = tet.Y + i;

                if (nowX < 0 || nowX >= Define.TBoardWidth ||
                    nowY < 0 || nowY >= Define.TBoardHeight) continue; // 보드칸 안에서만 적용 

                Vector3 pos = gridPos[nowX, nowY];
                tet.list[count++].transform.position = pos;
            }
        }
    }

    public bool MoveBlock(int dx, int dy) {
        currentBlock.X += dx;
        currentBlock.Y += dy;

        if (IsCollision(currentBlock)) {
            currentBlock.X -= dx;
            currentBlock.Y -= dy;
            return false;
        }

        UpdateBlock(currentBlock);
        MoveShadowToFloor();
        return true;
    }

    public bool MoveBlockToFloor() {
        while (MoveBlock(0, -1)) { }

        UpdateBlock(currentBlock);
        return true;
    }

    public bool RotateBlock() {
        int oldRotateIdx = currentBlock.rotateIdx;
        int[,] oldShape = new int[4, 4];

        Buffer.BlockCopy(currentBlock.Shape, 0, oldShape, 0, 4 * 4 * sizeof(int));

        currentBlock.rotateIdx = (currentBlock.rotateIdx + 1) % 4;

        int offset = ((int)currentBlock.type * 4 * 4 * 4 + currentBlock.rotateIdx * 4 * 4) * sizeof(int);

        Buffer.BlockCopy(TetrisBlockTemplate.Block, offset,
            currentBlock.Shape, 0, 4 * 4 * sizeof(int)); // 블록카피?


        if (IsCollision(currentBlock)) {
            currentBlock.Shape = oldShape;
            currentBlock.rotateIdx = oldRotateIdx;
            return false;
        }

        UpdateBlock(currentBlock);
        MoveShadowToFloor();
        return true;
    }

    #endregion

    #region Shadow

    public bool MoveShadow(int dy) {
        shadowBlock.Y += dy;

        if (IsCollision(shadowBlock)) {
            shadowBlock.Y -= dy;
            return false;
        }

        return true;
    }

    public bool MoveShadowToFloor() {
        Buffer.BlockCopy(currentBlock.Shape, 0,
            shadowBlock.Shape, 0, 4 * 4 * sizeof(int));
        shadowBlock.X = currentBlock.X;
        shadowBlock.Y = currentBlock.Y;

        int count = 0;
        while (true) {
            if (count++ > 30) break; // 무한루프 탈출용
            if (!MoveShadow(-1)) {
                break;
            }
        }

        UpdateBlock(shadowBlock);
        return true;
    }

    #endregion

    #region Line

    public void CheckLine() {
        // 라인 지우기 체크

        int count = 0;
        // height 위에서부터 체크
        for (int height = Define.TBoardHeight - 1; height >= 0; height--) {
            for (int width = 0; width < Define.TBoardWidth; width++) {
                if (grid[width, height] == null) break; // 하나라도 비어있으면 통과

                // 줄 하나 삭제
                if (width == Define.TBoardWidth - 1) {
                    ClearLine(height);
                    count++;
                }
            }
        }

        if (count != 0) Managers.Score.IncreaseScore(count);
    }

    public void ClearLine(int height) {
        // 삭제
        for (int i = 0; i < Define.TBoardWidth; i++) {
            Managers.Resource.Destroy(grid[i, height]);
            grid[i, height] = null;
        }

        // 위에 블록 당기기
        for (int nowHeight = height; nowHeight < Define.TBoardHeight - 1; nowHeight++) {
            for (int width = 0; width < Define.TBoardWidth; width++) {
                if (grid[width, nowHeight + 1] == null) continue; // 윗칸이 비었을 경우 당겨올 필요 없음 
                grid[width, nowHeight] = grid[width, nowHeight + 1];
                grid[width, nowHeight + 1] = null;
                grid[width, nowHeight].transform.position = gridPos[width, nowHeight];
            }
        }
    }

    public void PlaceMyBlock() {
        if (blockParent == null) {
            blockParent = Managers.Resource.CreateEmpty("@Blocks");
        }

        int count = 0;
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                if (currentBlock.Shape[i, j] == 0) continue;
                int nowX = currentBlock.X + j;
                int nowY = currentBlock.Y + i;
                
                if (nowY >= Define.TBoardHeight) continue;

                grid[nowX, nowY] = currentBlock.list[count++];
            }
        }

        // 부모 오브젝트 변경
        for (int i = 0; i < currentBlock.list.Count; i++) {
            currentBlock.list[i].transform.parent = blockParent.transform;
        }

        // Shadow 제거
        for (int i = 0; i < 4; i++) {
            Managers.Resource.Destroy(shadowBlock.list[i]);
        }
    }

    #endregion

    
}