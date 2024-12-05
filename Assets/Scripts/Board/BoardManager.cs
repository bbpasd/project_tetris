using System.Collections.Generic;
using UnityEngine;

/// <summary> 보드 생성 관리 </summary>
public class BoardManager : MonoBehaviour
{
    private static string _blankPrefabPath = "Board/BoardBlank";
    
    private List<IBoardObserver> _observers = new();

    public GameObject BoardPaent;

    public Vector3[,] CreateBlankBoard(int width, int height, float blockSize, float interval) {
        // Vector3 startPos;
        
        if (BoardPaent == null) {
            BoardPaent = Managers.Resource.CreateEmpty("@Board");
        }
        
        GameObject blankPrefab = Managers.Resource.LoadPrefab(_blankPrefabPath);
        Vector3[,] grid = new Vector3[width, height];
        
        for (int wIdx = 0; wIdx < width; wIdx++) {
            for (int hIdx = 0; hIdx < height; hIdx++) {
                float xPos = blockSize * wIdx + interval * wIdx;
                float yPos = blockSize * hIdx + interval * hIdx;
                Vector3 blockPos = new Vector3(xPos, yPos, 0.0f);
                GameObject go = Managers.Resource.Instantiate(blankPrefab, blockPos);
                go.name = $"({wIdx},{hIdx})";
                go.transform.parent = BoardPaent.transform;
                go.transform.localScale = new Vector3(blockSize, blockSize, blockSize);
                grid[wIdx, hIdx] = go.transform.position;
            }
        }

        return grid;
    }

    public GameObject CreateAniPangFloorBlock() {
        GameObject prefab = Managers.Resource.LoadPrefab("Board/AniPangFloor");
        GameObject go = Managers.Resource.Instantiate(prefab, new Vector3(32, -10, 0));  // TODO

        return go;
    }
    
    

    #region Observer
    public void AddObserver(IBoardObserver myObserver) {
        _observers.Add(myObserver);
    }

    public void RemoveObserver(IBoardObserver myObserver) {
        _observers.Remove(myObserver);
    }

    public void NotifyObservers() {
        foreach (IBoardObserver observer in _observers) {
            observer.ClearBoard();
        }
    }

    public void ClearBoard() {
        NotifyObservers();
    }

    public void ClearObservers() {
        _observers.Clear();
    }
    
    #endregion
}