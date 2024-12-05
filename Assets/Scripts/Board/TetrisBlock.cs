using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    public Define.ColorEnum colorEnum = Define.ColorEnum.Yellow;

    private Renderer _renderer;
    
    private void Awake() {
        // foreach (Transform child in gameObject.transform) {
        //     SetChildColor(child.gameObject);
        // }
        _renderer = GetComponent<Renderer>();
    }

    private void SetChildColor(GameObject target) {
        target.GetComponent<Renderer>().material.color = Define.Colors[(int)colorEnum];
    }
    
    public void SetColor() {
        _renderer.material.color = Define.Colors[(int)colorEnum];
    }

    public void SetTransparency(float amount) {
        Material material = _renderer.material;

        Color color = material.color;
        color.a = amount;
        material.color = color;
        
        material.SetFloat("_Mode", 3);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
        
        //_renderer.material = material;
    }
}
