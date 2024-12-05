using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary> 데이터 세이브 로드, 오브젝트 생성 제거 관리 </summary>
public class ResourceManager : MonoBehaviour
{
    public GameObject LoadPrefab(string path) {
        return Resources.Load<GameObject>(path);
    }

    public T Load<T>(string path) where T : Object {
        return Resources.Load<T>(path);
    }

    public T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object {
        return (T)Object.Instantiate((Object)original, position, rotation);
    }
    
    /// <summary>
    /// My Instantiate
    /// </summary>
    public T Instantiate<T>(T original, Vector3 position) where T : Object {
        return (T)Instantiate((Object)original, position, Quaternion.identity);
    }

    public void Destroy(GameObject go) {
        if (go == null) return;
        Object.Destroy(go);
    }

    public GameObject CreateEmpty(string name) {
        GameObject go = new GameObject(name);
        return go;
    }
}
