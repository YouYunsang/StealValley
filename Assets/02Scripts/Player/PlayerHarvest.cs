using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerInteraction))]
[RequireComponent(typeof(PlayerNoiseEmitter))]
[RequireComponent(typeof(PlayerInputLock))]
public class PlayerHarvest : MonoBehaviour
{
    [SerializeField] private bool _showDebugLog = true;

    private PlayerInputReader _inputReader;
    private PlayerInteraction _interaction;
    private PlayerNoiseEmitter _noiseEmitter;
    private PlayerInputLock _inputLock;

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
        _inputLock = GetComponent<PlayerInputLock>();
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
        // 전역 입력 잠금 상태면 진행 중 수확도 즉시 중단한다.
        if (_inputLock != null && _inputLock.IsGameplayInputLocked)
        {
            if (_isHarvesting || _currentHarvestTime > 0f)
            {
                ResetHarvestProgress(true);
            }

            return;
        }

        if (_currentTargetCrop == null || !_currentTargetCrop.CanHarvest)
        {
            ResetHarvestProgress(true);
            return;
        }

        if (_inputReader.IsClickPressed)
        {
            ProcessHarvest();
            return;
        }

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

        _currentHarvestTime += Time.deltaTime;

        if (_currentHarvestTime < _currentTargetCrop.GetHarvestDuration())
        {
            return;
        }

        CompleteHarvest();
    }

    private void CompleteHarvest()
    {
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
        _isHarvesting = false;
        _currentHarvestTime = 0f;

        if (restoreNoiseState)
        {
            RestoreNoiseState();
        }
    }

    private void RestoreNoiseState()
    {
        if (_inputLock != null && _inputLock.IsGameplayInputLocked)
        {
            _noiseEmitter.UpdateNoiseState(PlayerNoiseState.Idle);
            return;
        }

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