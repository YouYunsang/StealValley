using UnityEngine;

public class BasketController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BasketStorage _basketStorage;
    [SerializeField] private HarvestCountManager _harvestCountManager;
    [SerializeField] private CropDataSO _cropData;

    [Header("Debug")]
    [SerializeField] private bool _showDebugLog = true;

    public BasketStorage BasketStorage => _basketStorage;

    private void Start()
    {
        // 외부 참조를 검증한다.
        if (_basketStorage == null)
        {
            Debug.LogError($"{nameof(BasketController)}: BasketStorage가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_harvestCountManager == null)
        {
            Debug.LogError($"{nameof(BasketController)}: HarvestCountManager가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_cropData == null)
        {
            Debug.LogError($"{nameof(BasketController)}: CropDataSO가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }
    }

    public void StoreAllHarvest()
    {
        // 플레이어가 들고 있는 작물을 전부 바구니에 보관한다.
        int removedAmount = _harvestCountManager.RemoveAllHarvest(_cropData);

        if (removedAmount <= 0)
        {
            if (_showDebugLog)
            {
                Debug.Log("Basket Store Failed - No carried harvest.", this);
            }

            return;
        }

        _basketStorage.AddStoredHarvest(removedAmount);

        if (_showDebugLog)
        {
            Debug.Log($"Basket Store Success - Stored Amount: {removedAmount}", this);
        }
    }

    public void WithdrawAllHarvest()
    {
        // 바구니에 들어 있는 작물을 전부 플레이어에게 꺼내준다.
        int withdrawnAmount = _basketStorage.RemoveAllStoredHarvest();

        if (withdrawnAmount <= 0)
        {
            if (_showDebugLog)
            {
                Debug.Log("Basket Withdraw Failed - No stored harvest.", this);
            }

            return;
        }

        _harvestCountManager.AddHarvest(_cropData, withdrawnAmount);

        if (_showDebugLog)
        {
            Debug.Log($"Basket Withdraw Success - Withdrawn Amount: {withdrawnAmount}", this);
        }
    }
}