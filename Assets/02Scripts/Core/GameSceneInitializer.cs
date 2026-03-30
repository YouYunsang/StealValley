using UnityEngine;

public class GameSceneInitializer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private RunResultManager _runResultManager;
    [SerializeField] private PlayerInputLock _playerInputLock;

    [Header("Optional UI")]
    [SerializeField] private EscapeConfirmUI _escapeConfirmUI;
    [SerializeField] private GameObject _gameEndResultUIRoot;
    [SerializeField] private GameObject _basketUIRoot;

    [Header("Debug")]
    [SerializeField] private bool _showDebugLog = true;

    private void Start()
    {
        InitializeScene();
    }

    private void InitializeScene()
    {
        // 필수 참조를 먼저 검증한다.
        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        ResetTimeScale();
        ResetInputLock();
        ResetRunResult();
        ResetGameState();
        ResetUI();

        if (_showDebugLog)
        {
            Debug.Log("Game Scene Initialized", this);
        }
    }

    private bool ValidateReferences()
    {
        // 메인 게임 시작에 필수인 참조를 검증한다.
        if (_gameManager == null)
        {
            Debug.LogError($"{nameof(GameSceneInitializer)}: GameManager가 할당되지 않았습니다.", this);
            return false;
        }

        if (_runResultManager == null)
        {
            Debug.LogError($"{nameof(GameSceneInitializer)}: RunResultManager가 할당되지 않았습니다.", this);
            return false;
        }

        if (_playerInputLock == null)
        {
            Debug.LogError($"{nameof(GameSceneInitializer)}: PlayerInputLock이 할당되지 않았습니다.", this);
            return false;
        }

        return true;
    }

    private void ResetTimeScale()
    {
        // 이전 런의 슬로우 타임이나 정지 상태가 남지 않도록 시간을 복구한다.
        Time.timeScale = 1f;
    }

    private void ResetInputLock()
    {
        // 이전 런의 입력 잠금 상태를 전부 해제한다.
        _playerInputLock.ClearAllLocks();
    }

    private void ResetRunResult()
    {
        // 새 런 시작을 위해 최종 결과를 초기화한다.
        _runResultManager.ResetResult();
    }

    private void ResetGameState()
    {
        // 새 런 시작을 위해 게임 상태를 진행 중 상태로 초기화한다.
        _gameManager.ResetGameState();
    }

    private void ResetUI()
    {
        // 시작 시 열려 있으면 안 되는 UI를 닫는다.
        if (_escapeConfirmUI != null)
        {
            _escapeConfirmUI.Hide();
        }

        if (_gameEndResultUIRoot != null)
        {
            _gameEndResultUIRoot.SetActive(false);
        }

        if (_basketUIRoot != null)
        {
            _basketUIRoot.SetActive(false);
        }
    }
}