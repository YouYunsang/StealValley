using System;
using UnityEngine;

public class PlayerNoiseEmitter : MonoBehaviour
{
    [SerializeField] private PlayerNoiseDataSO _noiseData;

    private PlayerNoiseState _currentNoiseState = PlayerNoiseState.Idle;
    private float _currentNoiseRadius;

    public float CurrentNoiseRadius => _currentNoiseRadius;
    public PlayerNoiseState CurrentNoiseState => _currentNoiseState;

    public event Action<float> OnNoiseRadiusChanged;
    public static event Action<Vector2, float, PlayerNoiseState> OnNoiseBroadcasted;

    private void Start()
    {
        // 외부 데이터 참조 검증
        if (_noiseData == null)
        {
            Debug.LogError($"{nameof(PlayerNoiseEmitter)}: PlayerNoiseDataSO가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        UpdateNoiseState(PlayerNoiseState.Idle);
    }

    private void LateUpdate()
    {
        if(_currentNoiseRadius <= 0f)
        {
            return;
        }

        OnNoiseBroadcasted?.Invoke(transform.position, _currentNoiseRadius, _currentNoiseState);
    }

    public void UpdateNoiseState(PlayerNoiseState noiseState)
    {
        float nextNoiseRadius = GetNoiseRadius(noiseState);
        // 동일 상태면 불필요한 갱신을 막는다.
        if (_currentNoiseState == noiseState && Mathf.Approximately(_currentNoiseRadius, nextNoiseRadius))
        {
            return;
        }

        _currentNoiseState = noiseState;
        _currentNoiseRadius = nextNoiseRadius;

        OnNoiseRadiusChanged?.Invoke(_currentNoiseRadius);
    }

    public bool IsInNoiseRange(Vector2 targetPosition)
    {
        // 대상이 현재 소리 반경 안에 있는지 검사한다.
        float distance = Vector2.Distance(transform.position, targetPosition);
        return distance <= _currentNoiseRadius;
    }

    private float GetNoiseRadius(PlayerNoiseState noiseState)
    {
        switch (noiseState)
        {
            case PlayerNoiseState.Idle:
                return _noiseData.IdleNoiseRadius;

            case PlayerNoiseState.Move:
                return _noiseData.MoveNoiseRadius;

            case PlayerNoiseState.StealthMove:
                return _noiseData.StealthMoveNoiseRadius;
            case PlayerNoiseState.Harvest:
                return _noiseData.HarvestNoiseRadius;

            default:
                return 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 선택 중일 때 현재 소리 반경을 시각화한다.
        if (!Application.isPlaying)
        {
            return;
        }

        if (_noiseData == null || !_noiseData.ShowNoiseGizmo)
        {
            return;
        }

        if (_currentNoiseRadius <= 0f)
        {
            return;
        }

        Gizmos.color = GetGizmoColor(_currentNoiseState);
        Gizmos.DrawWireSphere(transform.position, _currentNoiseRadius);
    }

    private Color GetGizmoColor(PlayerNoiseState noiseState)
    {
        switch (noiseState)
        {
            case PlayerNoiseState.Idle:
                return Color.gray;

            case PlayerNoiseState.Move:
                return Color.yellow;

            case PlayerNoiseState.StealthMove:
                return Color.cyan;
            case PlayerNoiseState.Harvest:
                return Color.red;

            default:
                return Color.white;
        }
    }
}