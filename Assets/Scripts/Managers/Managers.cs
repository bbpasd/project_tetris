using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



/// <summary> 각종 매니져 관리, Singleton으로 동작 </summary>
public class Managers : MonoBehaviour
{
    private static Managers _instance;

    #region Managers
    public static Managers Instance => Init();

    public static ResourceManager Resource {
        get {
            if (Instance._resource != null) return Instance._resource;
            return Instance._resource = Instance.GetOrAddComponent<ResourceManager>();
        }
    }
    public static SoundManager Sound{
        get {
            if (Instance._sound != null) return Instance._sound;
            return Instance._sound = Instance.GetOrAddComponent<SoundManager>();
        }
    }
    public static ScoreManager Score{
        get {
            if (Instance._score != null) return Instance._score;
            return Instance._score = Instance.GetOrAddComponent<ScoreManager>();
        }
    }
    public static UIManager UI{
        get {
            if (Instance._ui != null) return Instance._ui;
            return Instance._ui = Instance.GetOrAddComponent<UIManager>();
        }
    }
    public static BoardManager Board{
        get {
            if (Instance._board != null) return Instance._board;
            return Instance._board = Instance.GetOrAddComponent<BoardManager>();
        }
    }

    private ResourceManager _resource;
    private SoundManager _sound;
    private ScoreManager _score;
    private UIManager _ui;
    private BoardManager _board;
    
    #endregion
    
    #region State
    // public static StateMachine State => Instance.GetOrAddComponent<StateMachine>();
    #endregion
    
    private static Managers Init() {
        if (_instance == null) {
            _instance = FindObjectOfType<Managers>();
            
            if (_instance == null) {
                GameObject singletonObject = new GameObject("@Managers");
                _instance = singletonObject.AddComponent<Managers>();
            }
        }
        return _instance;
    }
}
