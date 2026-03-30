using UnityEngine;

public class PlayerCarryWeight : MonoBehaviour
{
    [SerializeField] private PlayerMovementDataSO _movementData;
    [SerializeField] private HarvestCountManager _harvestCountManager;
    [SerializeField] private bool _showDebugLog = true;

    private int _currentHarvestCount;
    private float _currentSpeedPenalty;

    public int CurrentHarvestCount => _currentHarvestCount;
    public float CurrentSpeedPenalty => _currentSpeedPenalty;

    private void Start()
    {
        // 외부 데이터 참조를 검증한다.
        if (_movementData == null)
        {
            Debug.LogError($"{nameof(PlayerCarryWeight)}: PlayerMovementDataSO가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_harvestCountManager == null)
        {
            Debug.LogError($"{nameof(PlayerCarryWeight)}: HarvestCountManager가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        RefreshPenalty(_harvestCountManager.CurrentHarvestCount);
    }

    private void OnEnable()
    {
        // 수확 개수 변경 이벤트를 구독한다.
        if (_harvestCountManager != null)
        {
            _harvestCountManager.OnHarvestCountChanged += HandleHarvestCountChanged;
        }
    }

    private void OnDisable()
    {
        // 수확 개수 변경 이벤트 구독을 해제한다.
        if (_harvestCountManager != null)
        {
            _harvestCountManager.OnHarvestCountChanged -= HandleHarvestCountChanged;
        }
    }

    private void HandleHarvestCountChanged(int currentHarvestCount)
    {
        // 현재 보유 작물 개수에 맞춰 속도 페널티를 다시 계산한다.
        RefreshPenalty(currentHarvestCount);
    }

    private void RefreshPenalty(int currentHarvestCount)
    {
        _currentHarvestCount = Mathf.Max(0, currentHarvestCount);
        _currentSpeedPenalty = _currentHarvestCount * _movementData.MoveSpeedDecreasePerCrop;

        if (_showDebugLog)
        {
            Debug.Log(
                $"Carry Weight Updated - Count: {_currentHarvestCount}, Speed Penalty: {_currentSpeedPenalty:F2}",
                this
            );
        }
    }
}
