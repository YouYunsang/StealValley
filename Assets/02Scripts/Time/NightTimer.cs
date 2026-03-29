using System;
using UnityEngine;

public class NightTimer : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private float _nightDuration = 120f;
    [SerializeField] private bool _startTimerOnPlay = true;
    [SerializeField] private bool _showDebugLog = true;

    private float _remainingTime;
    private bool _isRunning;
    private bool _isExpired;

    public float RemainingTime => _remainingTime;
    public float NightDuration => _nightDuration;
    public bool IsRunning => _isRunning;
    public bool IsExpired => _isExpired;
    public float NormalizedRemainingTime => _nightDuration > 0f ? _remainingTime / _nightDuration : 0f;

    public event Action<float> OnTimeChanged;
    public event Action OnTimeExpired;

    private void Awake()
    {
        _remainingTime = Mathf.Max(0f, _nightDuration);
    }

    private void Start()
    {
        if (_gameManager == null)
        {
            Debug.LogError($"{nameof(NightTimer)}: GameManager가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_nightDuration <= 0f)
        {
            Debug.LogError($"{nameof(NightTimer)}: Night Duration은 0보다 커야 합니다.", this);
            enabled = false;
            return;
        }

        if (_startTimerOnPlay)
        {
            StartTimer();
        }

        OnTimeChanged?.Invoke(_remainingTime);
    }

    private void Update()
    {
        TickTimer();
    }

    public void StartTimer()
    {
        if (_isExpired)
        {
            return;
        }

        _isRunning = true;

        if (_showDebugLog)
        {
            Debug.Log($"Night Timer Started - Duration: {_nightDuration}", this);
        }
    }

    public void StopTimer()
    {
        _isRunning = false;

        if (_showDebugLog)
        {
            Debug.Log("Night Timer Stopped", this);
        }
    }

    public void ResetTimer()
    {
        _remainingTime = Mathf.Max(0f, _nightDuration);
        _isRunning = false;
        _isExpired = false;

        if (_showDebugLog)
        {
            Debug.Log($"Night Timer Reset - Remaining: {_remainingTime}", this);
        }

        OnTimeChanged?.Invoke(_remainingTime);
    }

    private void TickTimer()
    {
        if(!_isRunning || _isExpired || _gameManager.IsGameEnded)
        {
            return;
        }

        _remainingTime -= Time.deltaTime;
        _remainingTime = Mathf.Max(0f, _remainingTime);

        OnTimeChanged?.Invoke(_remainingTime);

        if(_remainingTime > 0f)
        {
            return;
        }

        ExpireTimer();
    }

    private void ExpireTimer()
    {
        if (_isExpired)
        {
            return;
        }

        _isExpired = true;
        _isRunning = false;

        if (_showDebugLog)
        {
            Debug.Log("Night Timer Expired", this);
        }

        OnTimeExpired?.Invoke();
        _gameManager.FailRun(FailReason.TimeOut);
    }

    private void OnValidate()
    {
        if(_nightDuration <= 0f)
        {
            _nightDuration = 1f;
        }
    }
}
