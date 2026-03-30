using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAnimator : MonoBehaviour
{
    private const string IS_MOVING_HASH_NAME = "IsMoving";
    private const string FACING_INDEX_HASH_NAME = "FacingIndex";

    private const int FACING_DOWN = 0;
    private const int FACING_UP = 1;
    private const int FACING_LEFT = 2;
    private const int FACING_RIGHT = 3;

    [SerializeField] private Animator _animator;

    private PlayerController _playerController;

    private int _isMovingHash;
    private int _facingIndexHash;

    private int _currentFacingIndex = FACING_DOWN;

    private void Awake()
    {
        // 같은 오브젝트 내부 컴포넌트를 캐싱한다.
        _playerController = GetComponent<PlayerController>();

        _isMovingHash = Animator.StringToHash(IS_MOVING_HASH_NAME);
        _facingIndexHash = Animator.StringToHash(FACING_INDEX_HASH_NAME);
    }

    private void Start()
    {
        // 외부 참조를 검증한다.
        if (_animator == null)
        {
            Debug.LogError($"{nameof(PlayerAnimator)}: Animator가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        ApplyAnimatorParameters(false, _currentFacingIndex);
    }

    private void Update()
    {
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        // 현재 이동 입력을 기준으로 애니메이션 방향을 결정한다.
        Vector2 moveInput = _playerController.CurrentMoveInput;
        bool isMoving = moveInput.sqrMagnitude > 0.0001f;

        if (isMoving)
        {
            _currentFacingIndex = GetFacingIndex(moveInput);
        }

        ApplyAnimatorParameters(isMoving, _currentFacingIndex);
    }

    private int GetFacingIndex(Vector2 moveInput)
    {
        // 좌우 입력이 있으면 좌우를 우선하고, 없을 때만 상하를 판정한다.
        if (moveInput.x < 0f)
        {
            return FACING_LEFT;
        }

        if (moveInput.x > 0f)
        {
            return FACING_RIGHT;
        }

        if (moveInput.y > 0f)
        {
            return FACING_UP;
        }

        return FACING_DOWN;
    }

    private void ApplyAnimatorParameters(bool isMoving, int facingIndex)
    {
        // 현재 이동 여부와 마지막 방향을 Animator에 반영한다.
        _animator.SetBool(_isMovingHash, isMoving);
        _animator.SetInteger(_facingIndexHash, facingIndex);
    }
}