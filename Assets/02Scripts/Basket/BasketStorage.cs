using System;
using UnityEngine;

public class BasketStorage : MonoBehaviour
{
    [SerializeField] private bool _showDebugLog = true;

    private int _storedHarvestCount;

    public int StoredHarvestCount => _storedHarvestCount;

    public event Action<int> OnStoredHarvestCountChanged;

    private void Awake()
    {
        // 시작 시 저장 개수를 0으로 초기화한다.
        _storedHarvestCount = 0;
    }

    public void AddStoredHarvest(int amount)
    {
        // 유효하지 않은 추가 요청은 무시한다.
        if (amount <= 0)
        {
            return;
        }

        _storedHarvestCount += amount;

        if (_showDebugLog)
        {
            Debug.Log(
                $"Basket Stored Added - Amount: {amount}, Current Stored: {_storedHarvestCount}",
                this
            );
        }

        OnStoredHarvestCountChanged?.Invoke(_storedHarvestCount);
    }

    public int RemoveAllStoredHarvest()
    {
        // 현재 저장된 작물을 전부 꺼내고 저장 개수를 0으로 만든다.
        if (_storedHarvestCount <= 0)
        {
            return 0;
        }

        int removedAmount = _storedHarvestCount;
        _storedHarvestCount = 0;

        if (_showDebugLog)
        {
            Debug.Log(
                $"Basket Stored Removed All - Removed: {removedAmount}, Current Stored: {_storedHarvestCount}",
                this
            );
        }

        OnStoredHarvestCountChanged?.Invoke(_storedHarvestCount);

        return removedAmount;
    }
}