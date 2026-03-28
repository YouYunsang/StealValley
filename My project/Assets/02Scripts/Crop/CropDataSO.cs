using UnityEngine;

[CreateAssetMenu(
    fileName = "CropData",
    menuName = "ScriptableObjects/Crop/CropData"
)]
public class CropDataSO : ScriptableObject
{
    [Header("Info")]
    [SerializeField] private string _cropName = "Crop";

    [Header("Harvest")]
    [SerializeField] private float _harvestDuration = 1.5f;
    [SerializeField] private float _progressAmount = 10f;

    public string CropName => _cropName;
    public float HarvestDuration => _harvestDuration;
    public float ProgressAmount => _progressAmount;

    private void OnValidate()
    {
        // 수확 시간은 음수가 될 수 없도록 보정한다.
        if (_harvestDuration < 0f)
        {
            _harvestDuration = 0f;
        }

        // 진행도 증가량은 음수가 될 수 없도록 보정한다.
        if (_progressAmount < 0f)
        {
            _progressAmount = 0f;
        }
    }
}