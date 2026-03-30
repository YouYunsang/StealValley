using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerNoiseEmitter))]
[RequireComponent(typeof(PlayerHarvest))]
[RequireComponent(typeof(PlayerHide))]
[RequireComponent(typeof(PlayerCarryWeight))]
[RequireComponent(typeof(PlayerInputLock))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovementDataSO _movementData;

    private PlayerInputReader _inputReader;
    private PlayerMovement _movement;
    private PlayerNoiseEmitter _noiseEmitter;
    private PlayerHarvest _harvest;
    private PlayerHide _hide;
    private PlayerCarryWeight _carryWeight;
    private PlayerInputLock _inputLock;

    public Vector2 CurrentMoveInput { get; private set; }
    public bool IsMoving => CurrentMoveInput.sqrMagnitude > 0f;
    public bool IsStealthing { get; private set; }
    public bool IsActionLocked => (_harvest != null && _harvest.IsHarvesting) || (_hide != null && _hide.IsMovementLocked);

    private void Awake()
    {
        // 같은 오브젝트 내부 컴포넌트를 캐싱한다.
        _inputReader = GetComponent<PlayerInputReader>();
        _movement = GetComponent<PlayerMovement>();
        _noiseEmitter = GetComponent<PlayerNoiseEmitter>();
        _harvest = GetComponent<PlayerHarvest>();
        _hide = GetComponent<PlayerHide>();
        _carryWeight = GetComponent<PlayerCarryWeight>();
        _inputLock = GetComponent<PlayerInputLock>();
    }

    private void Start()
    {
        // 외부 데이터 참조를 검증한다.
        if (_movementData == null)
        {
            Debug.LogError($"{nameof(PlayerController)}: PlayerMovementDataSO가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        _movement.SetMoveSpeed(_movementData.MoveSpeed);
    }

    private void Update()
    {
        HandleMoveInput();
        UpdateMoveSpeed();
        UpdateNoiseState();
    }

    private void HandleMoveInput()
    {
        // 전역 입력 잠금 상태면 이동 입력을 막는다.
        if (_inputLock != null && _inputLock.IsGameplayInputLocked)
        {
            CurrentMoveInput = Vector2.zero;
            IsStealthing = false;
            _movement.SetMoveDirection(Vector2.zero);
            return;
        }

        if (IsActionLocked)
        {
            CurrentMoveInput = Vector2.zero;
            IsStealthing = false;
            _movement.SetMoveDirection(Vector2.zero);
            return;
        }

        // 입력값을 정제한다.
        Vector2 rawInput = _inputReader.MoveInput;

        if (rawInput.magnitude < _movementData.InputDeadZone)
        {
            rawInput = Vector2.zero;
        }

        Vector2 moveDirection = rawInput == Vector2.zero ? Vector2.zero : rawInput.normalized;

        CurrentMoveInput = moveDirection;
        _movement.SetMoveDirection(CurrentMoveInput);

        IsStealthing = IsMoving && _inputReader.IsStealthPressed;
    }

    private void UpdateMoveSpeed()
    {
        if (IsActionLocked)
        {
            _movement.SetMoveSpeed(_movementData.MoveSpeed);
            return;
        }

        float speedPenalty = _carryWeight != null ? _carryWeight.CurrentSpeedPenalty : 0f;

        if (IsStealthing)
        {
            float finalStealthSpeed = Mathf.Max(
                _movementData.StealthMoveSpeed - speedPenalty,
                _movementData.MinStealthMoveSpeed
            );

            _movement.SetMoveSpeed(finalStealthSpeed);
            return;
        }

        float finalMoveSpeed = Mathf.Max(
            _movementData.MoveSpeed - speedPenalty,
            _movementData.MinMoveSpeed
        );

        _movement.SetMoveSpeed(finalMoveSpeed);
    }

    private void UpdateNoiseState()
    {
        if (_inputLock != null && _inputLock.IsGameplayInputLocked)
        {
            _noiseEmitter.UpdateNoiseState(PlayerNoiseState.Idle);
            return;
        }

        if (_harvest != null && _harvest.IsHarvesting)
        {
            _noiseEmitter.UpdateNoiseState(PlayerNoiseState.Harvest);
            return;
        }

        if (_hide != null && _hide.IsMovementLocked)
        {
            _noiseEmitter.UpdateNoiseState(PlayerNoiseState.Idle);
            return;
        }

        if (IsStealthing)
        {
            _noiseEmitter.UpdateNoiseState(PlayerNoiseState.StealthMove);
            return;
        }

        if (IsMoving)
        {
            _noiseEmitter.UpdateNoiseState(PlayerNoiseState.Move);
            return;
        }

        _noiseEmitter.UpdateNoiseState(PlayerNoiseState.Idle);
    }
}