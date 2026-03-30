using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerInteraction))]
[RequireComponent(typeof(PlayerInputLock))]
public class PlayerHide : MonoBehaviour
{
    [SerializeField] private PlayerHideDatdSO _hideData;
    [SerializeField] private float _hideCheckRadius = 10f;
    [SerializeField] private LayerMask _guardLayerMask;
    [SerializeField] private bool _showDebugLog = true;

    private PlayerInputReader _inputReader;
    private PlayerInteraction _interaction;
    private PlayerInputLock _inputLock;

    private Haystack _nearbyHaystack;
    private Haystack _activeHaystack;

    private bool _isHidden;
    private bool _isHideTransitioning;
    private float _hideTimer;

    public bool IsHidden => _isHidden;
    public bool IsHideTransitioning => _isHideTransitioning;
    public bool IsMovementLocked => _isHidden || _isHideTransitioning;
    public bool IsInteractionLocked => _isHidden || _isHideTransitioning;
    public Haystack NearbyHaystack => _nearbyHaystack;
    public Haystack ActiveHaystack => _activeHaystack;

    public event Action<bool> OnHiddenStateChanged;

    private void Awake()
    {
        _inputReader = GetComponent<PlayerInputReader>();
        _interaction = GetComponent<PlayerInteraction>();
        _inputLock = GetComponent<PlayerInputLock>();
    }

    private void Start()
    {
        // 외부 데이터 참조를 검증한다.
        if (_hideData == null)
        {
            Debug.LogError($"{nameof(PlayerHide)}: PlayerHideDataSO가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_guardLayerMask == 0)
        {
            Debug.LogError($"{nameof(PlayerHide)}: Guard Layer Mask가 설정되지 않았습니다.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        HandleHideState();
    }

    public void SetNearbyHaystack(Haystack haystack)
    {
        _nearbyHaystack = haystack;
    }

    public void ClearNearbyHaystack(Haystack haystack)
    {
        if (_nearbyHaystack != haystack)
        {
            return;
        }

        _nearbyHaystack = null;
    }

    private void HandleHideState()
    {
        // 전역 입력 잠금 상태면 숨기 관련 입력을 받지 않는다.
        if (_inputLock != null && _inputLock.IsGameplayInputLocked)
        {
            return;
        }

        if (_isHidden)
        {
            HandleHiddenState();
            return;
        }

        if (_isHideTransitioning)
        {
            UpdateHideTransition();
            return;
        }

        TryBeginHide();
    }

    private void TryBeginHide()
    {
        if (_nearbyHaystack == null) return;

        if (!_inputReader.WasInteractPressedThisFrame) return;

        _activeHaystack = _nearbyHaystack;
        _isHideTransitioning = true;
        _hideTimer = 0f;

        _interaction.ClearSelection();

        if (_showDebugLog)
        {
            Debug.Log($"Hide Transition Started - Haystack: {_activeHaystack.name}", this);
        }
    }

    private void UpdateHideTransition()
    {
        _hideTimer += Time.deltaTime;

        if (_hideTimer < _hideData.HideEnterDelay) return;

        if (CanFullyHide())
        {
            EnterHiddenState();
            return;
        }

        CancelHideTransition();
    }

    private void HandleHiddenState()
    {
        if (_inputReader.MoveInput.sqrMagnitude <= 0f) return;

        ExitHiddenState();
    }

    private bool CanFullyHide()
    {
        Collider2D[] guardColliders = Physics2D.OverlapCircleAll(transform.position, _hideCheckRadius, _guardLayerMask);

        for (int i = 0; i < guardColliders.Length; i++)
        {
            if (guardColliders[i] == null) continue;

            GuardSensor guardSensor = guardColliders[i].GetComponentInParent<GuardSensor>();

            if (guardSensor == null) continue;

            if (guardSensor.CanSeeTarget(transform.position)) return false;
        }

        return true;
    }

    private void EnterHiddenState()
    {
        _isHideTransitioning = false;
        _isHidden = true;
        _hideTimer = 0f;

        if (_activeHaystack != null)
        {
            transform.position = _activeHaystack.HidePosition;
        }

        if (_showDebugLog)
        {
            Debug.Log($"Hidden Entered - IsHidden: {_isHidden}", this);
            Debug.Log($"Hide Transition Started - Haystack: {_activeHaystack.name}", this);
        }

        OnHiddenStateChanged?.Invoke(true);
    }

    private void CancelHideTransition()
    {
        _isHideTransitioning = false;
        _hideTimer = 0f;

        if (_showDebugLog)
        {
            Debug.Log("Hide Transition Cancelled - Guard Too Close", this);
        }
    }

    private void ExitHiddenState()
    {
        _isHidden = false;

        if (_activeHaystack != null)
        {
            transform.position = _activeHaystack.ExitPosition;
        }

        if (_showDebugLog)
        {
            Debug.Log($"Hidden Exited - Haystack: {_activeHaystack?.name}", this);
        }

        OnHiddenStateChanged?.Invoke(false);
        _activeHaystack = null;
    }
}