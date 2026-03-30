using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerInputLock))]
public class PlayerInteraction : MonoBehaviour
{
    private const int INTERACTION_CELL_RANGE = 1;

    [SerializeField] private CropManager _cropManager;
    [SerializeField] private Camera _targetCamera;

    private PlayerInputReader _inputReader;
    private PlayerHide _hide;
    private PlayerInputLock _inputLock;

    private Crop _selectedCrop;
    private Vector3Int _selectedCellPosition;

    public Crop SelectedCrop => _selectedCrop;
    public Vector3Int SelectedCellPosition => _selectedCellPosition;
    public bool HasSelectedCrop => _selectedCrop != null;

    public event Action<Crop> OnCropSelected;
    public event Action OnCropSelectionCleared;

    private void Awake()
    {
        // 같은 오브젝트 내부 컴포넌트를 캐싱한다.
        _inputReader = GetComponent<PlayerInputReader>();
        _hide = GetComponent<PlayerHide>();
        _inputLock = GetComponent<PlayerInputLock>();
    }

    private void Start()
    {
        // 외부 참조를 검증한다.
        if (_cropManager == null)
        {
            Debug.LogError($"{nameof(PlayerInteraction)}: CropManager가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_targetCamera == null)
        {
            Debug.LogError($"{nameof(PlayerInteraction)}: Target Camera가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        HandleCropSelection();
    }

    private void HandleCropSelection()
    {
        // 전역 입력 잠금 상태면 선택을 막고 현재 선택을 해제한다.
        if (_inputLock != null && _inputLock.IsGameplayInputLocked)
        {
            ClearSelection();
            return;
        }

        if (_hide != null && _hide.IsInteractionLocked)
        {
            return;
        }

        if (!_inputReader.WasClickPressedThisFrame)
        {
            return;
        }

        Vector3 pointerWorldPosition = _targetCamera.ScreenToWorldPoint(_inputReader.PointerScreenPosition);
        pointerWorldPosition.z = 0f;

        Vector3Int clickedCellPosition = _cropManager.WorldToCell(pointerWorldPosition);
        Vector3Int playerCellPosition = _cropManager.WorldToCell(transform.position);

        if (!IsWithinInteractionRange(playerCellPosition, clickedCellPosition))
        {
            ClearSelection();
            return;
        }

        if (_cropManager.TryGetCropAtCell(clickedCellPosition, out Crop crop))
        {
            _selectedCrop = crop;
            _selectedCellPosition = clickedCellPosition;

            OnCropSelected?.Invoke(_selectedCrop);
            return;
        }

        ClearSelection();
    }

    private bool IsWithinInteractionRange(Vector3Int playerCellPosition, Vector3Int targetCellPosition)
    {
        int deltaX = Mathf.Abs(playerCellPosition.x - targetCellPosition.x);
        int deltaY = Mathf.Abs(playerCellPosition.y - targetCellPosition.y);

        return deltaX <= INTERACTION_CELL_RANGE && deltaY <= INTERACTION_CELL_RANGE;
    }

    public void ClearSelection()
    {
        // 현재 선택된 Crop을 해제한다.
        if (_selectedCrop == null)
        {
            return;
        }

        _selectedCrop = null;
        _selectedCellPosition = default;
        OnCropSelectionCleared?.Invoke();
    }
}