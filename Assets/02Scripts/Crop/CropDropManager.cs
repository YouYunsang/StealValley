using UnityEngine;

public class CropDropManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CropDropItem _dropItemPrefab;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private HarvestCountManager _harvestCountManager;

    [Header("Dropped Harvest")]
    [SerializeField] private float _droppedHarvestPickupDelay = 0.4f;
    [SerializeField] private float _droppedHarvestForwardOffset = 0.35f;

    public void SpawnHarvestDrop(CropDataSO cropData, Vector3 dropPosition)
    {
        // 수확 완료로 생기는 드롭 아이템을 생성한다.
        SpawnDropInternal(cropData, dropPosition, 0f);
    }

    public void SpawnDroppedHarvest(CropDataSO cropData, Vector3 dropPosition, Vector2 dropDirection)
    {
        // 플레이어가 버린 드롭 아이템은 짧은 재획득 불가 시간을 가진다.
        Vector2 normalizedDirection = dropDirection.sqrMagnitude > 0.0001f
            ? dropDirection.normalized
            : Vector2.right;

        Vector3 adjustedDropPosition = dropPosition + (Vector3)(normalizedDirection * _droppedHarvestForwardOffset);

        SpawnDropInternal(cropData, adjustedDropPosition, _droppedHarvestPickupDelay);
    }

    private void SpawnDropInternal(CropDataSO cropData, Vector3 dropPosition, float pickupDelay)
    {
        // 유효하지 않은 요청은 무시한다.
        if (cropData == null)
        {
            Debug.LogWarning($"{nameof(CropDropManager)}: CropDataSO가 null이라 드롭을 생성할 수 없습니다.", this);
            return;
        }

        if (_dropItemPrefab == null)
        {
            Debug.LogError($"{nameof(CropDropManager)}: Drop Item Prefab이 할당되지 않았습니다.", this);
            return;
        }

        if (_playerTransform == null)
        {
            Debug.LogError($"{nameof(CropDropManager)}: Player Transform이 할당되지 않았습니다.", this);
            return;
        }

        if (_harvestCountManager == null)
        {
            Debug.LogError($"{nameof(CropDropManager)}: HarvestCountManager가 할당되지 않았습니다.", this);
            return;
        }

        CropDropItem dropItemInstance = Instantiate(
            _dropItemPrefab,
            dropPosition,
            Quaternion.identity
        );

        dropItemInstance.Initialize(
            _playerTransform,
            () => HandleDropCollected(cropData)
        );

        dropItemInstance.SetPickupDelay(pickupDelay);
        dropItemInstance.PlayDrop(dropPosition);
    }

    private void HandleDropCollected(CropDataSO cropData)
    {
        // 드롭 아이템을 실제로 먹었을 때만 보유 개수를 증가시킨다.
        if (cropData == null)
        {
            return;
        }

        _harvestCountManager.AddHarvest(cropData, 1);
    }
}