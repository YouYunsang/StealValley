using System;
using UnityEngine;

public class GuardSensor : MonoBehaviour
{
    [SerializeField] private GuardDataSO _guardData;

    private bool _isPlayerInChaseRange;
    private Vector2 _lastDetectedPlayerPosition;

    public Vector2 LastDetectedPlayerPosition => _lastDetectedPlayerPosition;

    public event Action<Vector2> OnHeardNoise;
    public event Action<Vector2> OnPlayerDetected;
    public event Action OnPlayerLost;

    private void Start()
    {
        // 외부 데이터 참조를 검증한다.
        if (_guardData == null)
        {
            Debug.LogError($"{nameof(GuardSensor)}: GuardDataSO가 할당되지 않았습니다.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        // 플레이어 소리 이벤트를 구독한다.
        PlayerNoiseEmitter.OnNoiseBroadcasted += HandleNoiseBroadcasted;
    }

    private void OnDisable()
    {
        // 플레이어 소리 이벤트 구독을 해제한다.
        PlayerNoiseEmitter.OnNoiseBroadcasted -= HandleNoiseBroadcasted;
    }

    private void Update()
    {
        CheckChaseDetection();
    }

    private void HandleNoiseBroadcasted(Vector2 noisePosition, float noiseRadius, PlayerNoiseState noiseState)
    {
        // 감시자가 플레이어 소리 반경 안에 있으면 소리를 들은 것으로 처리한다.
        if (!enabled)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, noisePosition);

        if (distance > noiseRadius)
        {
            return;
        }

        _lastDetectedPlayerPosition = noisePosition;
        OnHeardNoise?.Invoke(_lastDetectedPlayerPosition);
    }

    private void CheckChaseDetection()
    {
        // 근접 범위 안에 플레이어가 있으면 확정 추격 감지로 처리한다.
        Collider2D playerCollider = Physics2D.OverlapCircle(
            transform.position,
            _guardData.ChaseDetectRange,
            _guardData.PlayerLayerMask
        );

        if (playerCollider != null)
        {
            PlayerHide playerHide = playerCollider.GetComponentInParent<PlayerHide>();

            if (playerHide != null && playerHide.IsHidden)
            {
                HandlePlayerLostState();
                return;
            }

            _lastDetectedPlayerPosition = playerCollider.transform.position;

            if (!_isPlayerInChaseRange)
            {
                _isPlayerInChaseRange = true;
                OnPlayerDetected?.Invoke(_lastDetectedPlayerPosition);
            }

            return;
        }

        HandlePlayerLostState();
    }

    private void HandlePlayerLostState()
    {
        if (_isPlayerInChaseRange)
        {
            _isPlayerInChaseRange = false;
            OnPlayerLost?.Invoke();
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 근접 확정 감지 범위를 시각화한다.
        if (!Application.isPlaying)
        {
            return;
        }

        if (_guardData == null || !_guardData.ShowSensorGizmo)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _guardData.ChaseDetectRange);
    }
}