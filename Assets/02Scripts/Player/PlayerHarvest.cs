using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerInteraction))]
[RequireComponent(typeof(PlayerNoiseEmitter))]
public class PlayerHarvest : MonoBehaviour
{
    [SerializeField] private bool _showDebugLog = true;

    private PlayerInputReader _inputReader;
    private PlayerInteraction _interaction;
    private PlayerNoiseEmitter _noiseEmitter;

    private Crop _currentTargetCrop;
    private float _currentHarvestTime;
    private bool _isHarvesting;

    public Crop CurrentTargetCrop => _currentTargetCrop;
    public float CurrentHarvestTime => _currentHarvestTime;
    public bool IsHarvesting => _isHarvesting;
    public float CurrentTargetHarvestDuration => _currentTargetCrop != null ? _currentTargetCrop.GetHarvestDuration() : 0f;
    public float CurrentHarvestNormalized =>
        _currentTargetCrop != null && _currentTargetCrop.GetHarvestDuration() > 0f
            ? _currentHarvestTime / _currentTargetCrop.GetHarvestDuration()
            : 0f;

    private void Awake()
    {
        // 같은 오브젝트 내부 컴포넌트를 캐싱한다.
        _inputReader = GetComponent<PlayerInputReader>();
        _interaction = GetComponent<PlayerInteraction>();
        _noiseEmitter = GetComponent<PlayerNoiseEmitter>();
    }

    private void OnEnable()
    {
        // 상호작용 이벤트를 구독한다.
        if (_interaction != null)
        {
            _interaction.OnCropSelected += HandleCropSelected;
            _interaction.OnCropSelectionCleared += HandleCropSelectionCleared;
        }
    }

    private void OnDisable()
    {
        // 상호작용 이벤트 구독을 해제한다.
        if (_interaction != null)
        {
            _interaction.OnCropSelected -= HandleCropSelected;
            _interaction.OnCropSelectionCleared -= HandleCropSelectionCleared;
        }
    }

    private void Update()
    {
        HandleHarvest();
    }

    private void HandleCropSelected(Crop crop)
    {
        // 새로운 작물이 선택되면 현재 수확 대상을 갱신하고 진행을 초기화한다.
        if (_currentTargetCrop == crop)
        {
            return;
        }

        _currentTargetCrop = crop;
        ResetHarvestProgress(false);

        if (_showDebugLog && _currentTargetCrop != null)
        {
            Debug.Log(
                $"Harvest Target Set - Crop Name: {_currentTargetCrop.GetCropName()}, Duration: {_currentTargetCrop.GetHarvestDuration()}",
                _currentTargetCrop
            );
        }
    }

    private void HandleCropSelectionCleared()
    {
        // 선택이 해제되면 수확도 즉시 중단하고 초기화한다.
        if (_showDebugLog && _currentTargetCrop != null && _isHarvesting)
        {
            Debug.Log(
                $"Harvest Cancelled - Selection Cleared, Crop Name: {_currentTargetCrop.GetCropName()}",
                _currentTargetCrop
            );
        }

        _currentTargetCrop = null;
        ResetHarvestProgress(true);
    }

    private void HandleHarvest()
    {
        // 현재 유효한 수확 대상이 없으면 아무 것도 하지 않는다.
        if (_currentTargetCrop == null || !_currentTargetCrop.CanHarvest)
        {
            ResetHarvestProgress(true);
            return;
        }

        // 좌클릭을 홀드하는 동안만 수확을 진행한다.
        if (_inputReader.IsClickPressed)
        {
            ProcessHarvest();
            return;
        }

        // 홀드를 해제하면 수확 진행도를 초기화한다.
        if (_isHarvesting || _currentHarvestTime > 0f)
        {
            if (_showDebugLog)
            {
                Debug.Log(
                    $"Harvest Cancelled - Hold Released, Crop Name: {_currentTargetCrop.GetCropName()}, Progress: {_currentHarvestTime:F2}/{_currentTargetCrop.GetHarvestDuration():F2}",
                    _currentTargetCrop
                );
            }

            ResetHarvestProgress(true);
        }
    }

    private void ProcessHarvest()
    {
        // 수확 시작 시 한 번만 상태를 전환한다.
        if (!_isHarvesting)
        {
            _isHarvesting = true;
            _noiseEmitter.UpdateNoiseState(PlayerNoiseState.Harvest);

            if (_showDebugLog)
            {
                Debug.Log(
                    $"Harvest Started - Crop Name: {_currentTargetCrop.GetCropName()}",
                    _currentTargetCrop
                );
            }
        }

        // 수확 시간을 누적한다.
        _currentHarvestTime += Time.deltaTime;

        // 목표 시간을 채우면 수확 완료 처리한다.
        if (_currentHarvestTime < _currentTargetCrop.GetHarvestDuration())
        {
            return;
        }

        CompleteHarvest();
    }

    private void CompleteHarvest()
    {
        // 작물 수확을 완료한다.
        if (_showDebugLog)
        {
            Debug.Log(
                $"Harvest Completed - Crop Name: {_currentTargetCrop.GetCropName()}, Progress Amount: {_currentTargetCrop.GetProgressAmount()}",
                _currentTargetCrop
            );
        }

        _currentTargetCrop.Harvest();
        _interaction.ClearSelection();

        _currentTargetCrop = null;
        ResetHarvestProgress(true);
    }

    private void ResetHarvestProgress(bool restoreNoiseState)
    {
        // 현재 수확 진행 상태를 초기화한다.
        _isHarvesting = false;
        _currentHarvestTime = 0f;

        if (restoreNoiseState)
        {
            RestoreNoiseState();
        }
    }

    private void RestoreNoiseState()
    {
        bool isMoving = _inputReader.MoveInput.sqrMagnitude > 0f;

        if (_inputReader.IsStealthPressed && isMoving)
        {
            _noiseEmitter.UpdateNoiseState(PlayerNoiseState.StealthMove);
            return;
        }

        if (isMoving)
        {
            _noiseEmitter.UpdateNoiseState(PlayerNoiseState.Move);
            return;
        }

        _noiseEmitter.UpdateNoiseState(PlayerNoiseState.Idle);
    }
}