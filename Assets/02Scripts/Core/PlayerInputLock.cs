using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputLock : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager _gameManager;

    [Header("Debug")]
    [SerializeField] private bool _showDebugLog = true;

    private readonly HashSet<PlayerInputLockReason> _lockReasons = new();

    public bool IsGameplayInputLocked => _lockReasons.Count > 0;

    public event Action<bool> OnGameplayInputLockChanged;

    private void Start()
    {
        // 외부 참조를 검증한다.
        if (_gameManager == null)
        {
            Debug.LogError($"{nameof(PlayerInputLock)}: GameManager가 할당되지 않았습니다.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        // 게임 종료 이벤트를 구독한다.
        if (_gameManager == null)
        {
            return;
        }

        _gameManager.OnGameSucceeded += HandleGameSucceeded;
        _gameManager.OnGameFailed += HandleGameFailed;
    }

    private void OnDisable()
    {
        // 게임 종료 이벤트 구독을 해제한다.
        if (_gameManager == null)
        {
            return;
        }

        _gameManager.OnGameSucceeded -= HandleGameSucceeded;
        _gameManager.OnGameFailed -= HandleGameFailed;
    }

    public void Lock(PlayerInputLockReason reason)
    {
        // 유효하지 않은 이유는 무시한다.
        if (reason == PlayerInputLockReason.None)
        {
            return;
        }

        // 같은 이유의 중복 잠금을 막는다.
        bool added = _lockReasons.Add(reason);

        if (!added)
        {
            return;
        }

        if (_showDebugLog)
        {
            Debug.Log($"Player Input Locked - Reason: {reason}", this);
        }

        OnGameplayInputLockChanged?.Invoke(IsGameplayInputLocked);
    }

    public void Unlock(PlayerInputLockReason reason)
    {
        // 유효하지 않은 이유는 무시한다.
        if (reason == PlayerInputLockReason.None)
        {
            return;
        }

        // 없는 잠금 해제 요청은 무시한다.
        bool removed = _lockReasons.Remove(reason);

        if (!removed)
        {
            return;
        }

        if (_showDebugLog)
        {
            Debug.Log($"Player Input Unlocked - Reason: {reason}", this);
        }

        OnGameplayInputLockChanged?.Invoke(IsGameplayInputLocked);
    }

    public bool HasLockReason(PlayerInputLockReason reason)
    {
        return _lockReasons.Contains(reason);
    }

    public void ClearAllLocks()
    {
        // 모든 잠금 상태를 제거한다.
        if (_lockReasons.Count == 0)
        {
            return;
        }

        _lockReasons.Clear();

        if (_showDebugLog)
        {
            Debug.Log("Player Input Lock Cleared", this);
        }

        OnGameplayInputLockChanged?.Invoke(false);
    }

    private void HandleGameSucceeded()
    {
        // 성공 시 게임플레이 입력을 잠근다.
        Lock(PlayerInputLockReason.GameEnded);
    }

    private void HandleGameFailed(FailReason failReason)
    {
        // 실패 시 게임플레이 입력을 잠근다.
        Lock(PlayerInputLockReason.GameEnded);
    }
}