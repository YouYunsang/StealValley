using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GuardMovement : MonoBehaviour
{
    [SerializeField] private GuardDataSO _guardData;

    private Rigidbody2D _rigidbody2D;

    private Vector2 _targetPosition;
    private float _currentMoveSpeed;
    private bool _hasMoveTarget;

    public Vector2 TargetPosition => _targetPosition;
    public bool HasMoveTarget => _hasMoveTarget;
    public float CurrentMoveSpeed => _currentMoveSpeed;

    private void Awake()
    {
        // 같은 오브젝트 내부 컴포넌트는 Awake에서 캐싱한다.
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // 외부 데이터 참조를 검증한다.
        if (_guardData == null)
        {
            Debug.LogError($"{nameof(GuardMovement)}: GuardDataSO가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        SetMoveType(GuardMoveType.Patrol);
    }

    private void FixedUpdate()
    {
        MoveToTarget();
    }

    public void SetMoveType(GuardMoveType moveType)
    {
        // 이동 타입에 따라 현재 속도를 설정한다.
        _currentMoveSpeed = GetMoveSpeed(moveType);
    }

    public void SetTargetPosition(Vector2 targetPosition)
    {
        // 이동할 목표 좌표를 설정한다.
        _targetPosition = targetPosition;
        _hasMoveTarget = true;
    }

    public void ClearTargetPosition()
    {
        // 현재 목표 좌표를 제거한다.
        _hasMoveTarget = false;
    }

    public bool HasArrived()
    {
        // 목표 지점 도착 여부를 판단한다.
        if (!_hasMoveTarget)
        {
            return false;
        }

        float distance = Vector2.Distance(_rigidbody2D.position, _targetPosition);
        return distance <= _guardData.ArrivalThreshold;
    }

    private void MoveToTarget()
    {
        // 목표가 없으면 이동하지 않는다.
        if (!_hasMoveTarget)
        {
            return;
        }

        // 이미 도착했다면 정확한 좌표로 보정하고 정지한다.
        if (HasArrived())
        {
            _rigidbody2D.MovePosition(_targetPosition);
            return;
        }

        // 목표 지점을 향해 일정 속도로 이동한다.
        Vector2 currentPosition = _rigidbody2D.position;
        Vector2 moveDirection = (_targetPosition - currentPosition).normalized;
        Vector2 nextPosition = currentPosition + (moveDirection * _currentMoveSpeed * Time.fixedDeltaTime);

        _rigidbody2D.MovePosition(nextPosition);
    }

    private float GetMoveSpeed(GuardMoveType moveType)
    {
        switch (moveType)
        {
            case GuardMoveType.Patrol:
                return _guardData.PatrolMoveSpeed;

            case GuardMoveType.Investigate:
                return _guardData.InvestigateMoveSpeed;

            case GuardMoveType.Chase:
                return _guardData.ChaseMoveSpeed;

            case GuardMoveType.Return:
                return _guardData.ReturnMoveSpeed;

            default:
                return _guardData.PatrolMoveSpeed;
        }
    }
}