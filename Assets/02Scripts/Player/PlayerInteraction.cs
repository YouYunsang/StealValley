using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
public class PlayerInteraction : MonoBehaviour
{
    private const int INTERACTION_CELL_RANGE = 1;

    [SerializeField] private CropManager _cropManager;
    [SerializeField] private Camera _targetCamera;

    private PlayerInputReader _inputReader;
    private PlayerHide _hide;

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
        if (_hide != null && _hide.IsInteractionLocked)
        {
            return;
        }

        // 클릭이 시작된 프레임에만 선택을 시도한다.
        if (!_inputReader.WasClickPressedThisFrame)
        {
            return;
        }

        Vector3 pointerWorldPosition = _targetCamera.ScreenToWorldPoint(_inputReader.PointerScreenPosition);
        pointerWorldPosition.z = 0f;

        Vector3Int clickedCellPosition = _cropManager.WorldToCell(pointerWorldPosition);
        Vector3Int playerCellPosition = _cropManager.WorldToCell(transform.position);

        // 플레이어 기준 3x3 범위 밖이면 선택을 무시한다.
        if (!IsWithinInteractionRange(playerCellPosition, clickedCellPosition))
        {
            ClearSelection();
            return;
        }

        // 클릭한 셀에 수확 가능한 Crop이 있으면 선택한다.
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
        // 체비쇼 거리 1 이내인지 검사한다.
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