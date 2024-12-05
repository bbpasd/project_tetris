using UnityEngine;

public class UIManager : MonoBehaviour
{
    public UI_Base _sceneUI;

    public UI_Base SceneUI {
        get {
            if (_sceneUI == null) SetSceneUI();
            return _sceneUI;
        }
    }

    public void Init() {
        
    }

    public void SetSceneUI(string uiName = "@UI_Canvas") {
        _sceneUI = GameObject.Find(uiName).GetComponent<UI_Base>();
    }

}