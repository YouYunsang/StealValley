using UnityEngine;

public class RunResultManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HarvestCountManager _harvestCountManager;

    [Header("Debug")]
    [SerializeField] private bool _showDebugLog = true;

    private int _finalHarvestCount;
    private bool _hasFinalResult;

    public int FinalHarvestCount => _finalHarvestCount;
    public bool HasFinalResult => _hasFinalResult;

    private void Start()
    {
        // 외부 참조를 검증한다.
        if (_harvestCountManager == null)
        {
            Debug.LogError($"{nameof(RunResultManager)}: HarvestCountManager가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        ResetResult();
    }

    public void RecordSuccessResult()
    {
        // 이미 최종 결과가 기록되었다면 중복 기록하지 않는다.
        if (_hasFinalResult)
        {
            return;
        }

        _finalHarvestCount = _harvestCountManager.CurrentHarvestCount;
        _hasFinalResult = true;

        if (_showDebugLog)
        {
            Debug.Log(
                $"Run Result Success Recorded - Final Harvest Count: {_finalHarvestCount}",
                this
            );
        }
    }

    public void RecordFailResult()
    {
        // 이미 최종 결과가 기록되었다면 중복 기록하지 않는다.
        if (_hasFinalResult)
        {
            return;
        }

        _finalHarvestCount = 0;
        _hasFinalResult = true;

        if (_showDebugLog)
        {
            Debug.Log("Run Result Failed Recorded - Final Harvest Count: 0", this);
        }
    }

    public void ResetResult()
    {
        // 새 실행을 위해 결과 상태를 초기화한다.
        _finalHarvestCount = 0;
        _hasFinalResult = false;

        if (_showDebugLog)
        {
            Debug.Log("Run Result Reset", this);
        }
    }
}