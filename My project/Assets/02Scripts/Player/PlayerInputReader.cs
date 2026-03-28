using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _stealthAction;
    [SerializeField] private InputActionReference _pointerPositionAction;
    [SerializeField] private InputActionReference _clickAction;

    private Vector2 _moveInput;
    private bool _isStealthPressed;
    private Vector2 _pointerScreenPosition;
    private bool _isClickPressed;
    private bool _wasClickPressedThisFrame;

    public Vector2 MoveInput => _moveInput;
    public bool IsStealthPressed => _isStealthPressed;
    public Vector2 PointerScreenPosition => _pointerScreenPosition;
    public bool IsClickPressed => _isClickPressed;
    public bool WasClickPressedThisFrame => _wasClickPressedThisFrame;

    private void OnEnable()
    {
        if(_moveAction != null)
        {
            _moveAction.action.Enable();
        }

        if (_stealthAction != null)
        {
            _stealthAction.action.Enable();
        }

        // 마우스 위치 입력 액션 활성화
        if (_pointerPositionAction != null)
        {
            _pointerPositionAction.action.Enable();
        }

        // 클릭 입력 액션 활성화
        if (_clickAction != null)
        {
            _clickAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if(_moveAction != null)
        {
            _moveAction.action.Disable();
        }

        if (_stealthAction != null)
        {
            _stealthAction.action.Disable();
        }

        // 마우스 위치 입력 액션 비활성화
        if (_pointerPositionAction != null)
        {
            _pointerPositionAction.action.Disable();
        }

        // 클릭 입력 액션 비활성화
        if (_clickAction != null)
        {
            _clickAction.action.Disable();
        }
    }

    private void Update()
    {
        ReadMoveInput();
        ReadStealthInput();
        ReadPointerInput();
        ReadClickInput();
    }

    private void ReadMoveInput()
    {
        // 이동 입력값을 읽는다.
        if (_moveAction != null)
        {
            _moveInput = _moveAction.action.ReadValue<Vector2>();
        }
        else
        {
            _moveInput = Vector2.zero;
        }
    }

    private void ReadStealthInput()
    {
        // 은신 키 입력 상태를 읽는다.
        if (_stealthAction != null)
        {
            _isStealthPressed = _stealthAction.action.IsPressed();
        }
        else
        {
            _isStealthPressed = false;
        }
    }

    private void ReadPointerInput()
    {
        // 마우스 스크린 좌표를 읽는다.
        if (_pointerPositionAction != null)
        {
            _pointerScreenPosition = _pointerPositionAction.action.ReadValue<Vector2>();
        }
        else
        {
            _pointerScreenPosition = Vector2.zero;
        }
    }

    private void ReadClickInput()
    {
        // 좌클릭 홀드/단발 입력 상태를 읽는다.
        if (_clickAction != null)
        {
            _isClickPressed = _clickAction.action.IsPressed();
            _wasClickPressedThisFrame = _clickAction.action.WasPressedThisFrame();
        }
        else
        {
            _isClickPressed = false;
            _wasClickPressedThisFrame = false;
        }
    }
}
