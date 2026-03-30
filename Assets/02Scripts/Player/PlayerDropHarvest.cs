using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerInputLock))]
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
    private PlayerInputLock _inputLock;

    private void Awake()
    {
        // 같은 오브젝트 내부 컴포넌트를 캐싱한다.
        _inputReader = GetComponent<PlayerInputReader>();
        _playerController = GetComponent<PlayerController>();
        _inputLock = GetComponent<PlayerInputLock>();
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
        // 전역 입력 잠금 상태면 버리기를 막는다.
        if (_inputLock != null && _inputLock.IsGameplayInputLocked)
        {
            return;
        }

        if (!_inputReader.WasDropPressedThisFrame)
        {
            return;
        }

        if (_playerController != null && _playerController.IsActionLocked)
        {
            return;
        }

        TryDropHarvest();
    }

    private void TryDropHarvest()
    {
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
        // 이동 중이면 이동 반대 방향으로 버리고, 멈춰 있으면 오른쪽을 기본 방향으로 사용한다.
        Vector2 moveInput = _playerController != null ? _playerController.CurrentMoveInput : Vector2.zero;

        if (moveInput.sqrMagnitude > 0.0001f)
        {
            return -moveInput.normalized;
        }

        return Vector2.right;
    }
}