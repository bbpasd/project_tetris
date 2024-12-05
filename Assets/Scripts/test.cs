using System;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject targetObject; // 투명도를 변경할 오브젝트
    [Range(0, 1)]
    public float transparency = 0.5f; // 투명도 값 (0은 완전 투명, 1은 불투명)

    void Start()
    {
        if (targetObject != null)
        {
            SetTransparency(targetObject, transparency);
        }
    }

    private void Update() {
        if (targetObject != null)
        {
            SetTransparency(targetObject, transparency);
        }
    }

    void SetTransparency(GameObject obj, float alpha)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = renderer.material;
            if (material != null)
            {
                // 머티리얼의 투명도 설정
                Color color = material.color;
                color.a = alpha;
                material.color = color;

                // 쉐이더의 알파 설정
                material.SetFloat("_Mode", 3);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
            }
        }
    }
}