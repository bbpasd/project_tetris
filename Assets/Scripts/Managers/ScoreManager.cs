using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Observer 패턴을 적용한 점수 관리 </summary>
public class ScoreManager : MonoBehaviour
{
    private List<IMyObserver> _observers = new();
    public int score = 0;

    public void AddObserver(IMyObserver myObserver) {
        if (!_observers.Contains(myObserver)) {
            _observers.Add(myObserver);
        }
    }

    public void RemoveObserver(IMyObserver myObserver) {
        _observers.Remove(myObserver);
    }

    public void NotifyScoreObservers(int amount) {
        score += amount;
        foreach (IMyObserver observer in _observers) {
            observer.OnNotify(amount, score);
        }
    }

    public void Init() {
        score = 0;
        NotifyScoreObservers(0);
    }
    
    public void IncreaseScore(int amount) {
        score += amount;
        NotifyScoreObservers(amount);
    }
}