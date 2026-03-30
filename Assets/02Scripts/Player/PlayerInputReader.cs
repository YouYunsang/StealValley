using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _stealthAction;
    [SerializeField] private InputActionReference _pointerPositionAction;
    [SerializeField] private InputActionReference _clickAction;
    [SerializeField] private InputActionReference _interactAction;
    [SerializeField] private InputActionReference _dropAction;

    private Vector2 _moveInput;
    private bool _isStealthPressed;
    private Vector2 _pointerScreenPosition;
    private bool _isClickPressed;
    private bool _wasClickPressedThisFrame;
    private bool _isInteractPressed;
    private bool _wasInteractPressedThisFrame;
    private bool _isDropPressed;
    private bool _wasDropPressedThisFrame;

    public Vector2 MoveInput => _moveInput;
    public bool IsStealthPressed => _isStealthPressed;
    public Vector2 PointerScreenPosition => _pointerScreenPosition;
    public bool IsClickPressed => _isClickPressed;
    public bool WasClickPressedThisFrame => _wasClickPressedThisFrame;
    public bool IsInteractPressed => _isInteractPressed;
    public bool WasInteractPressedThisFrame => _wasInteractPressedThisFrame;
    public bool IsDropPressed => _isDropPressed;
    public bool WasDropPressedThisFrame => _wasDropPressedThisFrame;

    private void OnEnable()
    {
        if (_moveAction != null)
        {
            _moveAction.action.Enable();
        }

        if (_stealthAction != null)
        {
            _stealthAction.action.Enable();
        }

        if (_pointerPositionAction != null)
        {
            _pointerPositionAction.action.Enable();
        }

        if (_clickAction != null)
        {
            _clickAction.action.Enable();
        }

        if (_interactAction != null)
        {
            _interactAction.action.Enable();
        }

        if (_dropAction != null)
        {
            _dropAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (_moveAction != null)
        {
            _moveAction.action.Disable();
        }

        if (_stealthAction != null)
        {
            _stealthAction.action.Disable();
        }

        if (_pointerPositionAction != null)
        {
            _pointerPositionAction.action.Disable();
        }

        if (_clickAction != null)
        {
            _clickAction.action.Disable();
        }

        if (_interactAction != null)
        {
            _interactAction.action.Disable();
        }

        if (_dropAction != null)
        {
            _dropAction.action.Disable();
        }
    }

    private void Update()
    {
        ReadMoveInput();
        ReadStealthInput();
        ReadPointerInput();
        ReadClickInput();
        ReadInteractInput();
        ReadDropInput();
    }

    #region Read Input
    private void ReadMoveInput()
    {
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

    private void ReadInteractInput()
    {
        if (_interactAction != null)
        {
            _isInteractPressed = _interactAction.action.IsPressed();
            _wasInteractPressedThisFrame = _interactAction.action.WasPressedThisFrame();
            return;
        }

        _isInteractPressed = false;
        _wasInteractPressedThisFrame = false;
    }

    private void ReadDropInput()
    {
        // F키 버리기 입력 상태를 읽는다.
        if (_dropAction != null)
        {
            _isDropPressed = _dropAction.action.IsPressed();
            _wasDropPressedThisFrame = _dropAction.action.WasPressedThisFrame();
            return;
        }

        _isDropPressed = false;
        _wasDropPressedThisFrame = false;
    }
    #endregion
}