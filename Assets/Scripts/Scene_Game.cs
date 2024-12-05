using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Scene_Game : MonoBehaviour
{
    private void Awake() {
        // TODO Awake 순서 변경으로 제거 가능한 코드
        StartCoroutine(StartGameSceneCoroutine());
        MinigameManager.Instance.Init();
    }

    IEnumerator StartGameSceneCoroutine() {
        yield return new WaitForSeconds(0.1f);
        
        GameObject.Find("@UI_Canvas").GetComponent<UI_GameScene>().DisplayMenuButton(true);
    }
}