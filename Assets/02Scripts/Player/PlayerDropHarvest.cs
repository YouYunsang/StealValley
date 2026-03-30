using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerController))]
public class PlayerDropHarvest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HarvestCountManager _harvestCountManager;
    [SerializeField] private CropDropManager _cropDropManager;
    [SerializeField] private CropDataSO _cropData;

    [Header("Drop")]
    [SerializeField] private float _dropForwardDistance = 0.6f;
    [SerializeField] private bool _showDebugLog = true;

    private PlayerInputReader _inputReader;
    private PlayerController _playerController;

    private void Awake()
    {
        // 같은 오브젝트 내부 컴포넌트를 캐싱한다.
        _inputReader = GetComponent<PlayerInputReader>();
        _playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        // 외부 참조를 검증한다.
        if (_harvestCountManager == null)
        {
            Debug.LogError($"{nameof(PlayerDropHarvest)}: HarvestCountManager가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_cropDropManager == null)
        {
            Debug.LogError($"{nameof(PlayerDropHarvest)}: CropDropManager가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_cropData == null)
        {
            Debug.LogError($"{nameof(PlayerDropHarvest)}: CropDataSO가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        HandleDropInput();
    }

    private void HandleDropInput()
    {
        // F키를 누른 프레임에만 버리기를 시도한다.
        if (!_inputReader.WasDropPressedThisFrame)
        {
            return;
        }

        // 현재 행동이 잠겨 있으면 버릴 수 없다.
        if (_playerController != null && _playerController.IsActionLocked)
        {
            return;
        }

        TryDropHarvest();
    }

    private void TryDropHarvest()
    {
        // 보유 작물 1개 감소에 성공했을 때만 드롭 아이템을 생성한다.
        bool removed = _harvestCountManager.TryRemoveHarvest(_cropData, 1);

        if (!removed)
        {
            if (_showDebugLog)
            {
                Debug.Log("Drop Failed - No harvest item to drop.", this);
            }

            return;
        }

        Vector2 dropDirection = GetDropDirection();
        Vector3 dropPosition = transform.position + (Vector3)(dropDirection * _dropForwardDistance);

        _cropDropManager.SpawnDroppedHarvest(_cropData, dropPosition, dropDirection);

        if (_showDebugLog)
        {
            Debug.Log(
                $"Harvest Dropped - Crop: {_cropData.CropName}, Position: {dropPosition}",
                this
            );
        }
    }

    private Vector2 GetDropDirection()
    {
        // 이동 중이면 이동 방향 앞으로 버리고, 멈춰 있으면 오른쪽으로 기본 버리기 방향을 사용한다.
        Vector2 moveInput = _playerController != null ? _playerController.CurrentMoveInput : Vector2.zero;

        if (moveInput.sqrMagnitude > 0.0001f)
        {
            return -moveInput.normalized;
        }

        return Vector2.right;
    }
}