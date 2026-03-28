using UnityEngine;

public class Crop : MonoBehaviour
{
    [SerializeField] private CropDataSO _cropData;

    private bool _isHarvested;

    public CropDataSO CropData => _cropData;
    public bool IsHarvested => _isHarvested;
    public bool CanHarvest => _cropData != null && !_isHarvested;

    private void Awake()
    {
        // 시작 시 수확되지 않은 상태로 초기화한다.
        _isHarvested = false;
    }

    private void Start()
    {
        // 외부 데이터 참조를 검증한다.
        if (_cropData == null)
        {
            Debug.LogError($"{nameof(Crop)}: CropDataSO가 할당되지 않았습니다.", this);
            enabled = false;
        }
    }

    public string GetCropName()
    {
        // 작물 이름을 반환한다.
        return _cropData.CropName;
    }

    public float GetHarvestDuration()
    {
        // 수확에 필요한 시간을 반환한다.
        return _cropData.HarvestDuration;
    }

    public float GetProgressAmount()
    {
        // 수확 시 증가할 진행도 값을 반환한다.
        return _cropData.ProgressAmount;
    }

    public void Harvest()
    {
        // 수확 불가능한 상태면 처리하지 않는다.
        if (!CanHarvest)
        {
            return;
        }

        // 수확 완료 상태로 전환한다.
        _isHarvested = true;

        // 테스트 단계에서는 오브젝트를 비활성화한다.
        gameObject.SetActive(false);
    }
}