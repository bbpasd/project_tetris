using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniPangBlock : MonoBehaviour
{
    public Define.ColorEnum colorEnum = Define.ColorEnum.Yellow;

    private Renderer _renderer;
    
    private void Awake() {
        _renderer = GetComponent<Renderer>();
    }
    
    public void SetColor() {
        _renderer.material.color = Define.Colors[(int)colorEnum];
    }

    private void OnMouseDown() {
        AniPangManager.Instance.OnBlockClicked(this);
    }
}