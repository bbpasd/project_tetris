﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance {
        get {
            if (_instance == null) {
                GameObject singletonObject = new GameObject($"@{typeof(T).Name}");
                _instance = singletonObject.AddComponent<T>();
            }

            return _instance;
        }
    }

    protected virtual void Awake() {
        if (_instance == null) {
            _instance = this as T;
        }
    }

    public virtual void Init() {
        
    }
}