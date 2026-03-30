using UnityEngine;

public class GuardAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private GuardController _guardController;
    [SerializeField] private GuardMovement _guardMovement;

    private const string IS_MOVING_PARAM = "IsMoving";
    private const string FACING_INDEX_PARAM = "FacingIndex";

    private const int DOWN_INDEX = 0;
    private const int UP_INDEX = 1;
    private const int LEFT_INDEX = 2;
    private const int RIGHT_INDEX = 3;

    private int _currentFacingIndex = DOWN_INDEX;

    private void Awake()
    {
        // 같은 오브젝트 또는 부모/자식 관계의 참조를 캐싱한다.
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }

        if (_guardController == null)
        {
            _guardController = GetComponentInParent<GuardController>();
        }

        if (_guardMovement == null)
        {
            _guardMovement = GetComponentInParent<GuardMovement>();
        }
    }

    private void Start()
    {
        // 필수 참조를 검증한다.
        if (_animator == null)
        {
            Debug.LogError($"{nameof(GuardAnimator)}: Animator가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_guardController == null)
        {
            Debug.LogError($"{nameof(GuardAnimator)}: GuardController가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_guardMovement == null)
        {
            Debug.LogError($"{nameof(GuardAnimator)}: GuardMovement가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        // 시작 시 기본 방향을 Animator에 반영한다.
        _animator.SetBool(IS_MOVING_PARAM, false);
        _animator.SetInteger(FACING_INDEX_PARAM, _currentFacingIndex);
    }

    private void Update()
    {
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        // 현재 이동 중인지 판단한다.
        bool isMoving = _guardMovement.HasMoveTarget && !_guardMovement.HasArrived();

        // 이동 중일 때만 마지막 방향을 갱신한다.
        if (isMoving)
        {
            _currentFacingIndex = GetFacingIndex(_guardController.FacingDirection);
        }
        else
        {
            // 멈춰 있어도 제자리에서 방향만 바뀌는 상태를 반영하고 싶다면 아래 라인을 사용한다.
            _currentFacingIndex = GetFacingIndex(_guardController.FacingDirection);
        }

        _animator.SetBool(IS_MOVING_PARAM, isMoving);
        _animator.SetInteger(FACING_INDEX_PARAM, _currentFacingIndex);
    }

    private int GetFacingIndex(Vector2 direction)
    {
        // 방향 벡터가 너무 작으면 기존 방향을 유지한다.
        if (direction.sqrMagnitude <= 0.0001f)
        {
            return _currentFacingIndex;
        }

        // 대각선은 좌우 우선으로 처리한다.
        if (Mathf.Abs(direction.x) > 0.01f)
        {
            return direction.x < 0f ? LEFT_INDEX : RIGHT_INDEX;
        }

        return direction.y >= 0f ? UP_INDEX : DOWN_INDEX;
    }
}