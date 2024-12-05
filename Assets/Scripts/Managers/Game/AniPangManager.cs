using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AniPangManager : SceneSingleton<AniPangManager>, IBoardObserver
{
    public struct AniBlock
    {
        public int X, Y;
        public Define.ColorEnum ColorEnum;
        public AniPangBlock AniPangBlock;
        public Rigidbody rb;
        public int ID;

        public AniBlock(int id, int x, int y, AniPangBlock aniPangBlock, Rigidbody _rb) {
            ID = id;
            this.X = x;
            this.Y = y;
            ColorEnum = (Define.ColorEnum) Random.Range(0, Define.BlockAmount - 1);
            AniPangBlock = aniPangBlock;
            rb = _rb;
        }
    }
    
    
    public Vector3[,] gridPos;
    public AniBlock?[,] grid;
    public GameObject blockPrefab;
    public GameObject blockParent;
    
    public Coroutine _aniPangGameCoroutine; 
    public Coroutine _aniPangTimeProgressCoroutine;
    private Dictionary<int?, AniBlock?> movingBlockDict;
    private List<AniBlock?> checkingBlockList;

    private Dictionary<int?, AniBlock?> breakBlockDict;

    private GameObject floorBlock;

    private int BlockCount = 0;
    
    // Check
    private int[,,] _centerCheckDir;
    
    // Change Block
    public AniBlock? selectedBlock = null;
    

    public void Init() {
        blockPrefab = blockPrefab ?? Managers.Resource.LoadPrefab("Prefabs/Block/AniPangBlock");
        if (grid == null) CreateGrid();
        else ClearGrid();

        movingBlockDict = new();
        checkingBlockList = new();
        breakBlockDict = new();

        Managers.Board.AddObserver(this);
        
        _centerCheckDir = new int[2, 3, 2]{
            {
                { 0, -1 },
                { 0,  0 },
                { 0,  1 },
            }, {
                { -1, 0 },
                { 0,  0 },
                { 1,  0 },
            }
        };
    }

    private void CreateGrid() {
        grid = new AniBlock?[Define.ABoardWidth, Define.ABoardHeight];
        gridPos = Managers.Board.CreateBlankBoard(Define.ABoardWidth, Define.ABoardHeight, 
            Define.ABoardBlockSize, Define.ABoardBlockInterval);
        
        // TODO
        GameObject blank = GameObject.Find("@Board");
        blank.SetActive(false);
    }

    public void ClearBoard() {
        Managers.Resource.Destroy(floorBlock);
        Managers.Resource.Destroy(blockParent);
        ClearGrid();
        grid = null;

        GameObject blank = Managers.Board.BoardPaent;
        Managers.Resource.Destroy(blank);
    }

    private void ClearGrid() {
        for (int w = 0; w < Define.ABoardWidth; w++) {
            for (int h = 0; h < Define.ABoardHeight; h++) {
                if (grid[w, h] == null) continue;

                Managers.Resource.Destroy(grid[w, h].Value.AniPangBlock.gameObject);
                grid[w, h] = null;
            }
        }
    }

    public void StartGame() {
        CreateInitialBlocks();
        floorBlock = Managers.Board.CreateAniPangFloorBlock();
        
        _aniPangGameCoroutine = StartCoroutine(AniPangProgress());
        _aniPangTimeProgressCoroutine = StartCoroutine(AniPangTimeProgress());
    }
    
    public void GameLose() {
        Debug.Log("AniPang GameLose");
        StopCoroutine(_aniPangTimeProgressCoroutine);
        StopCoroutine(_aniPangGameCoroutine);
        MinigameManager.Instance.ChangeState(GamemodeState.AniPangStayState);
    }
    
    public void CreateInitialBlocks() {
        for (int wIdx = 0; wIdx < Define.ABoardWidth; wIdx++) {
            for (int hIdx = 0; hIdx < Define.ABoardHeight; hIdx++) {
                grid[wIdx, hIdx] = CreateNewBlock(wIdx, hIdx);
            }
        }
    }

    private AniBlock CreateNewBlock(int widthIdx, int heightIdx, int countValue = -1) {
        if (blockParent == null) blockParent = Managers.Resource.CreateEmpty("@Blocks");

        float maxHeight = gridPos[0, Define.ABoardHeight-1].y + Define.ABoardBlockSize + Define.ABoardBlockInterval;
        Vector3 spawnPos = gridPos[widthIdx, heightIdx] + new Vector3(0, maxHeight, 0);
        if (countValue >= 0) {
            spawnPos = gridPos[widthIdx, countValue] + new Vector3(0, maxHeight, 0);
        }
        GameObject go = Managers.Resource.Instantiate(blockPrefab, spawnPos);
        go.transform.parent = blockParent.transform;
        go.name = BlockCount.ToString();

        AniBlock block = new AniBlock(BlockCount, widthIdx, heightIdx, 
            go.GetComponent<AniPangBlock>(), go.GetComponent<Rigidbody>());
        block.AniPangBlock = go.GetComponent<AniPangBlock>();
        block.AniPangBlock.colorEnum = block.ColorEnum;
        block.AniPangBlock.SetColor();
        
        movingBlockDict.Add(BlockCount, block);

        BlockCount++;
        return block;
    }

    private IEnumerator AniPangTimeProgress() {
        yield return new WaitForSeconds(10.0f);  // 최초에 내려오는 시간 감안해서 쉼
        
        while (true) {
            yield return new WaitForSeconds(0.1f);
            ((UI_GameScene)Managers.UI.SceneUI).UpdateAniPangTimerBar(-0.1f);

            if (((UI_GameScene)Managers.UI.SceneUI).GetAniPangTimerValue() <= 0.0f) {
                GameLose();
            }
        }
    }
    
    private IEnumerator AniPangProgress() {
        yield return new WaitForSeconds(1.0f);
        while (true) {
            yield return WaitUntilAllBlocksStopped();

            ReadyToCheck();
            if (!IsMatched()) {
                
            }
            else {
                BreakBlocks();
            }
            
            yield return new WaitForSeconds(1.0f);
        }
    }
    
    private IEnumerator WaitUntilAllBlocksStopped() {
        while (!IsAllBlockStopped()) {
            yield return new WaitForSeconds(0.1f); // 0.1초마다 확인
        }
    }

    /// <summary>
    /// _movingBlockList 리스트 내의 모든 블록의 속도가 1.0f 이하일 경우 true 반환 
    /// </summary>
    private bool IsAllBlockStopped() {
        foreach (AniBlock block in movingBlockDict.Values) {
            if (block.rb != null && block.rb.velocity.magnitude > 1.0f) {
                return false;
            }
        }
        return true;
    }

    private void ReadyToCheck() {
        if (movingBlockDict.Count == 0) return;

        checkingBlockList = movingBlockDict.Values.ToList();
        movingBlockDict.Clear();
    }
    
    private AniBlock? GetBlockAtPosition(int x, int y) {
        if (x < 0 || x >= Define.ABoardWidth || y < 0 || y >= Define.ABoardHeight) return null;
        
        return grid[x, y];
    }

    private bool IsMatched() {
        breakBlockDict.Clear(); // 파괴될 블록 딕셔너리 매번 초기화
        
        foreach (AniBlock nowBlock in checkingBlockList) {
            int nowX = nowBlock.X;
            int nowY = nowBlock.Y;
            
            // 상하좌우 체크
            int[,] dir = {
                { 0, 1 },
                { 0, -1},
                { -1, 0},
                { 1, 0}
            };
            for (int i = 0; i < 4; i++) {
                int length = GetMatchedBlockLength(nowX, nowY, dir[i, 0], dir[i, 1]);
                AddBlocksToBreakDict(nowX, nowY, dir[i, 0], dir[i, 1], length);
            }
            
            // 기준점이 가운데 일 경우
            for (int i = 0; i < 2; i++) {
                if (IsCenterMatched(nowX, nowY, i)) {
                    AddCenterBlockToBreakDict(nowX, nowY, i);
                }
            }

        }

        checkingBlockList.Clear();
        return breakBlockDict.Count > 0;
    }

    private int GetMatchedBlockLength(int x, int y, int dx, int dy) {
        int count = 1;

        Define.ColorEnum? nowColor = GetBlockAtPosition(x, y)?.ColorEnum;
        
        // 일단 최대 7칸 체크하도록 i가 0이면 xy를 체크하므로 1부터 시작
        for (int i = 1; i < 6; i++) {
            int checkX = x + dx * i;
            int checkY = y + dy * i;

            if (checkX < 0 || checkX >= Define.ABoardWidth ||
                checkY < 0 || checkY >= Define.ABoardHeight) break; // 보드를 벗어났을 경우

            if (GetBlockAtPosition(checkX, checkY)?.ColorEnum != nowColor) break;   // 색깔이 다를 경우

            count++;

        }
        return count;
    }

    private void AddBlocksToBreakDict(int x, int y, int dx, int dy, int length) {
        if (length < 3) return; // 3보다 작으면 취소
        
        for (int i = 0; i < length; i++) {
            int checkX = x + dx * i;
            int checkY = y + dy * i;

            AniBlock? nowBlock = GetBlockAtPosition(checkX, checkY);
            breakBlockDict.TryAdd(nowBlock?.ID, nowBlock);
        }
    }

    
    /// <param name="dirMode">0:Vertical, 1:Horizontal</param>
    private bool IsCenterMatched(int x, int y, int dirMode) {
        Define.ColorEnum? nowColor = GetBlockAtPosition(x, y)?.ColorEnum;

        for (int i = 0; i < 3; i++) {
            int checkX = x + _centerCheckDir[dirMode, i, 0];
            int checkY = y + _centerCheckDir[dirMode, i, 1];
            
            if (checkX < 0 || checkX >= Define.ABoardWidth ||
                checkY < 0 || checkY >= Define.ABoardHeight) return false; // 보드를 벗어났을 경우
            
            if (GetBlockAtPosition(checkX, checkY)?.ColorEnum != nowColor) return false;   // 색깔이 다를 경우
        }
        

        return true;
    }

    /// <param name="dirMode">0:Vertical, 1:Horizontal</param>
    private void AddCenterBlockToBreakDict(int x, int y, int dirMode) {
        for (int i = 0; i < 3; i++) {
            int checkX = x + _centerCheckDir[dirMode, i, 0];
            int checkY = y + _centerCheckDir[dirMode, i, 1];
            
            AniBlock? nowBlock = GetBlockAtPosition(checkX, checkY);
            breakBlockDict.TryAdd(nowBlock?.ID, nowBlock);
        }
    }
    
    private void BreakBlocks() {
        if (breakBlockDict.Count == 0) return;
    
        int[] columnCheck = new int[Define.ABoardWidth];
    
        // 블록 제거
        foreach (AniBlock nowBlock in breakBlockDict.Values) {
            grid[nowBlock.X, nowBlock.Y] = null;
            int nowX = nowBlock.X;
            int nowY = nowBlock.Y;

            Managers.Resource.Destroy(nowBlock.AniPangBlock.gameObject);
            columnCheck[nowBlock.X]++;
        }

        for (int x = 0; x < Define.ABoardWidth; x++) {
            int count = columnCheck[x];
            if (count == 0) continue; // 해당 열에 수정 사항이 없으면 통과

            // 그리드 블록 내리기
            for (int y = 0; y < Define.ABoardHeight; y++) {
                if (grid[x, y] != null) continue; // x, y가 null이 아니면 당길 필요 없으니 통과

                for (int aboveY = y + 1; aboveY < Define.ABoardHeight; aboveY++) {
                    if (grid[x, aboveY] == null) continue;
                    grid[x, y] = grid[x, aboveY];
                    grid[x, aboveY] = null;

                    AniBlock temp = grid[x, y].Value;
                    temp.Y = y;
                    grid[x, y] = temp;
                    
                    movingBlockDict.TryAdd(temp.ID, temp);
                    
                    break;
                }
            }

            // 생성
            for (int y = 0; y < count; y++) {
                grid[x, Define.ABoardHeight-count+y] = CreateNewBlock(x, Define.ABoardHeight-count+y, countValue: y);
            }
        }
    
        IncreaseScore(breakBlockDict.Count);
        breakBlockDict.Clear();
    }

    public void OnBlockClicked(AniPangBlock clickedBlock) {
        // 게임 오버 상태일 때는 리턴
        if (MinigameManager.Instance.stateMachine.currentEnum != GamemodeState.AniPangGameState) return;
        
        for (int x = 0; x < Define.ABoardWidth; x++) {
            for (int y = 0; y < Define.ABoardHeight; y++) {
                if (grid[x, y]?.AniPangBlock == clickedBlock) {
                    if (selectedBlock == null) {    // 클릭한 게 없을 시
                        selectedBlock = grid[x, y]; 
                    }else {
                        if (AreBlocksNear(selectedBlock.Value, grid[x, y].Value)) {
                            AniBlock? clickedGrid = GetBlockAtPosition(x, y);
                            (selectedBlock, clickedGrid) = SwapBlocks(selectedBlock.Value, clickedGrid.Value);
                            
                            if (!IsMatched()) {
                                (selectedBlock, clickedGrid) = SwapBlocks(selectedBlock.Value, clickedGrid.Value);
                                checkingBlockList.Clear();
                            }
                            else {
                                BreakBlocks();
                            }
                        }
                        else {
                            Debug.Log("Selected Wrong Blocks");
                        }
                        selectedBlock = null;
                    }
                    return;
                }
            }
        }
    }
    
    private bool AreBlocksNear(AniBlock block1, AniBlock block2) {
        return (Mathf.Abs(block1.X - block2.X) == 1 && block1.Y == block2.Y) ||
               (Mathf.Abs(block1.Y - block2.Y) == 1 && block1.X == block2.X);
    }

    private (AniBlock, AniBlock) SwapBlocks(AniBlock block1, AniBlock block2) {
        // 블록의 좌표 변경
        (block1.X, block2.X) = (block2.X, block1.X);
        (block1.Y, block2.Y) = (block2.Y, block1.Y);
        
        // Grid 상에서 위치 변경
        grid[block1.X, block1.Y] = block1;
        grid[block2.X, block2.Y] = block2;

        // 서로 위치 변경 로직
        // TODO 필요함
        (block1.AniPangBlock.transform.position, block2.AniPangBlock.transform.position) = 
            (block2.AniPangBlock.transform.position, block1.AniPangBlock.transform.position);
        
        checkingBlockList.Add(block1);
        checkingBlockList.Add(block2);

        return (block1, block2);
    }

    
    private void IncreaseScore(int amount) {
        Managers.Score.IncreaseScore(amount);
    }
}








