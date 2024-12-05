using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
using Object = UnityEngine.Object;

public abstract class UI_Base : MonoBehaviour
{
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    public abstract void Init();

    private void Start() {
        Init();
    }

    protected void Bind<T>(Type type) where T : UnityEngine.Object {
        string[] names = Enum.GetNames(type);

        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        
        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++) {
            if (typeof(T) == typeof(GameObject))
                objects[i] = FindChild(gameObject, names[i]);   // GameObject는 Component가 아니라서 따로 처리
            else
                objects[i] = FindChild<T>(gameObject, names[i]);
        }
    }

    protected void Bind<T>(T list){
        
    }

    protected T Get<T>(int enumIdx) where T : UnityEngine.Object {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[enumIdx] as T;
    }

    protected string GetString(Type type, int enumIdx) {
        string[] names = Enum.GetNames(type);

        return names[enumIdx];
    }

    protected TextMeshProUGUI GetTMP(int enumIdx) { return Get<TextMeshProUGUI>(enumIdx); }
    protected Button GetButton(int enumIdx) { return Get<Button>(enumIdx); }
    protected Image GetImage(int enumIdx) { return Get<Image>(enumIdx); }
    protected GameObject GetObject(int enumIdx) { return Get<GameObject>(enumIdx); }

    protected T GetSomething<T>(int enumIdx) where T : Object { return Get<T>(enumIdx);}

    public static void BindEvent <T> (T go, Action<PointerEventData> action) where T: UnityEngine.Component { 
        UI_EventHandler evt = go.GetComponent<UI_EventHandler>();
        if (evt == null) evt = go.AddComponent<UI_EventHandler>();

        evt.OnClickHandler -= action;
        evt.OnClickHandler += action;
    }
    
    public static T FindChild<T>(GameObject go, string name = null) where T: UnityEngine.Object{
        if (go == null || name == null) return null;

        foreach (T component in go.GetComponentsInChildren<T>()) {
            if (string.IsNullOrEmpty(name) || component.name == name)
                return component;
        }
        
        return null;
    }
    
    public static GameObject FindChild(GameObject go, string name = null) {
        if (go == null || name == null) return null;
        
        Transform transform = FindChild<Transform>(go, name);

        if (transform != null) return transform.gameObject;
        else return null;
    }
}
