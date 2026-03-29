using System;
using UnityEngine;

[RequireComponent(typeof(GuardController))]
public class GuardSensor : MonoBehaviour
{
    [SerializeField] private GuardDataSO _guardData;

    private GuardController _guardController;

    private bool _isPlayerInsight;
    private Vector2 _lastDetectedPlayerPosition;

    public Vector2 LastDetectedPlayerPosition => _lastDetectedPlayerPosition;

    public event Action<Vector2> OnHeardNoise;
    public event Action<Vector2> OnPlayerDetected;
    public event Action OnPlayerLost;

    private void Awake()
    {
        _guardController = GetComponent<GuardController>();
    }

    private void Start()
    {
        // 외부 데이터 참조를 검증한다.
        if (_guardData == null)
        {
            Debug.LogError($"{nameof(GuardSensor)}: GuardDataSO가 할당되지 않았습니다.", this);
            enabled = false;
            return;
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
        CheckVisionDetection();
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

    private void CheckVisionDetection()
    {
        Collider2D[] playerColliders = Physics2D.OverlapCircleAll(
            transform.position,
            _guardData.ViewDistance,
            _guardData.PlayerLayerMask
        );

        Collider2D visiblePlayerCollider = null;
        float closestSqrDistance = float.MaxValue;

        for(int i = 0; i < playerColliders.Length; i++)
        {
            Collider2D playerCollider = playerColliders[i];

            if (playerCollider == null) continue;

            PlayerHide playerHide = playerCollider.GetComponentInParent<PlayerHide>();

            if (playerHide != null && playerHide.IsHidden) continue;

            Vector2 toPlayer = (Vector2)playerCollider.transform.position - (Vector2)transform.position;

            float angleToPlyaer = Vector2.Angle(_guardController.FacingDirection, toPlayer.normalized);

            if (angleToPlyaer > _guardData.ViewAngle * 0.5f) continue;

            float sqrDistance = toPlayer.sqrMagnitude;

            if(sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                visiblePlayerCollider = playerCollider;
            }
        }

        if(visiblePlayerCollider != null)
        {
            _lastDetectedPlayerPosition = visiblePlayerCollider.transform.position;

            if (!_isPlayerInsight)
            {
                _isPlayerInsight = true;
                OnPlayerDetected?.Invoke(_lastDetectedPlayerPosition);
            }

            return;
        }

        if (_isPlayerInsight)
        {
            _isPlayerInsight = false;
            OnPlayerLost?.Invoke();
        }
    }

    private Vector2 RotateVector(Vector2 vector, float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        return new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos).normalized;
    }

    public bool CanSeeTarget(Vector2 targetPosition)
    {
        Vector2 toTarget = targetPosition - (Vector2)transform.position;
        float distance = toTarget.magnitude;

        if(distance > _guardData.ViewDistance)
        {
            return false;
        }

        float angle = Vector2.Angle(_guardController.FacingDirection, toTarget.normalized);

        if(angle > _guardData.ViewAngle * 0.5f)
        {
            return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        // 시야 거리와 시야각을 에디터에서 시각화한다.
        if (!Application.isPlaying)
        {
            return;
        }

        if (_guardData == null || !_guardData.ShowVisionGizmo)
        {
            return;
        }

        Vector2 facingDirection = Vector2.down;

        if (_guardController != null)
        {
            facingDirection = _guardController.FacingDirection;
        }

        Vector2 leftBoundary = RotateVector(facingDirection, -_guardData.ViewAngle * 0.5f);
        Vector2 rightBoundary = RotateVector(facingDirection, _guardData.ViewAngle * 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _guardData.ViewDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + facingDirection * _guardData.ViewDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + leftBoundary * _guardData.ViewDistance);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + rightBoundary * _guardData.ViewDistance);
    }
}