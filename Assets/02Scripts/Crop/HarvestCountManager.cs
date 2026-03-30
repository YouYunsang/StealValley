using System;
using UnityEngine;

public class HarvestCountManager : MonoBehaviour
{
    [SerializeField] private bool _showDebugLog = true;

    private int _currentHarvestCount;
    private CropDataSO _lastCollectedCropData;

    public int CurrentHarvestCount => _currentHarvestCount;
    public CropDataSO _LastCollectedCropData => _lastCollectedCropData;

    public event Action<int> OnHarvestCountChanged;
    public event Action<CropDataSO, int> OnHarvestCollected;
    public event Action<CropDataSO, int> OnHarvestRemoved;

    private void Awake()
    {
        _currentHarvestCount = 0;
        _lastCollectedCropData = null;
    }

    public void AddHarvest(CropDataSO cropData, int amount = 1)
    {
        if (cropData == null || amount <= 0) return;

        _currentHarvestCount += amount;
        _lastCollectedCropData = cropData;

        if (_showDebugLog)
        {
            Debug.Log(
                $"Harvest Count Added - Crop: {cropData.CropName}, Amount: {amount}, Current Total: {_currentHarvestCount}",
                this
            );
        }

        OnHarvestCollected?.Invoke(cropData, amount);
        OnHarvestCountChanged?.Invoke(_currentHarvestCount);
    }

    public bool TryRemoveHarvest(CropDataSO cropData, int amount = 1)
    {
        if (cropData == null || amount <= 0) return false;

        if(_currentHarvestCount < amount) return false;

        _currentHarvestCount -= amount;
        _lastCollectedCropData = cropData;

        OnHarvestRemoved?.Invoke(cropData, amount);
        OnHarvestCountChanged?.Invoke(_currentHarvestCount);
        return true;
    }

    public void ResetHarvestCount()
    {
        _currentHarvestCount = 0;
        _lastCollectedCropData = null;

        if (_showDebugLog)
        {
            Debug.Log("Harvest Count Reset", this);
        }

        OnHarvestCountChanged?.Invoke(_currentHarvestCount);
    }
}
