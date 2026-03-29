using System;
using UnityEngine;

public class FarmProgressManager : MonoBehaviour
{
    private const float MiN_PROGRESS = 0f;
    private const float MAX_PROGRESS = 100f;

    [SerializeField] private float _startProgress = 0f;
    [SerializeField] private float _targetProgress = 100f;
    [SerializeField] private bool _showDebugLog = true;

    private float _currentProgress;
    private bool _isCompleted;

    public float CurrentProgress => _currentProgress;
    public float TargetProgress => _targetProgress;
    public bool IsCompleted => _isCompleted;
    public float NormalizedProgress => _targetProgress > 0f ? _currentProgress / _targetProgress : 0f;

    public event Action<float, float> OnProgressChanged;
    public event Action OnProgressCompleted;

    private void Awake()
    {
        _currentProgress = Mathf.Clamp(_startProgress, MiN_PROGRESS, _targetProgress);
        _isCompleted = _currentProgress >= _targetProgress;
    }

    private void Start()
    {
        if(_targetProgress <= 0f)
        {
            Debug.LogError($"{nameof(FarmProgressManager)}: Target Progress는 0보다 커야 합니다.", this);
            enabled = false;
            return;
        }

        OnProgressChanged?.Invoke(_currentProgress, _targetProgress);

        if (_isCompleted)
        {
            OnProgressCompleted?.Invoke();
        }
    }

    public void AddProgress(float amount)
    {
        if (_isCompleted || amount <= 0f)
        {
            return;
        }

        _currentProgress = Mathf.Clamp(_currentProgress + amount, MiN_PROGRESS, _targetProgress);

        if (_showDebugLog)
        {
            Debug.Log(
                $"Farm Progress Added - Amount: {amount}, Current: {_currentProgress}/{_targetProgress}",
                this
            );
        }

        OnProgressChanged?.Invoke(_currentProgress, _targetProgress);

        if(_currentProgress < _targetProgress)
        {
            return;
        }

        _isCompleted = true;

        if (_showDebugLog)
        {
            Debug.Log("Farm Progress Completed", this);
        }

        OnProgressCompleted?.Invoke();
    }

    public void ResetProgress()
    {
        _currentProgress = Mathf.Clamp(_startProgress, MiN_PROGRESS, _targetProgress);
        _isCompleted = _currentProgress >= _targetProgress;

        if (_showDebugLog)
        {
            Debug.Log(
                $"Farm Progress Reset - Current: {_currentProgress}/{_targetProgress}",
                this
            );
        }

        OnProgressChanged?.Invoke(_currentProgress, _targetProgress);

        if (_isCompleted)
        {
            OnProgressCompleted?.Invoke();
        }
    }

    private void OnValidate()
    {
        if(_targetProgress <= 0f)
        {
            _targetProgress = MAX_PROGRESS;
        }

        _startProgress = Mathf.Clamp(_startProgress, MiN_PROGRESS, _targetProgress);
    }
}
