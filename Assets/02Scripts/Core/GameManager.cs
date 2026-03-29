using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool _showDebugLog = true;

    private bool _isGameEnded;
    private bool _isFailed;
    private bool _isSucceeded;
    private FailReason _currentFailReason = FailReason.None;

    public bool IsGameEnded => _isGameEnded;
    public bool IsFailed => _isFailed;
    public bool IsSucceeded => _isSucceeded;
    public FailReason CurrentFailReason => _currentFailReason;

    public event Action<FailReason> OnGameFailed;
    public event Action OnGameSucceeded;
    public void FailRun(FailReason failReason)
    {
        if (_isGameEnded)
        {
            return;
        }

        if(failReason == FailReason.None)
        {
            return;
        }

        _isGameEnded = true;
        _isFailed = true;
        _isSucceeded = false;
        _currentFailReason = failReason;

        if (_showDebugLog)
        {
            Debug.Log($"Game Failed - Reason: {_currentFailReason}", this);
        }

        OnGameFailed?.Invoke(_currentFailReason);
    }

    public void SuccessRun()
    {
        if (_isGameEnded) return;

        _isGameEnded = true;
        _isFailed = false;
        _isSucceeded = true;
        _currentFailReason = FailReason.None;

        if (_showDebugLog)
        {
            Debug.Log("Game Succeeded", this);
        }

        OnGameSucceeded?.Invoke();
    }

    public void ResetGameState()
    {
        _isGameEnded = false;
        _isFailed = false;
        _isSucceeded = false;
        _currentFailReason = FailReason.None;

        if(_showDebugLog)
        {
            Debug.Log("Game State Reset", this);
        }
    }
}
