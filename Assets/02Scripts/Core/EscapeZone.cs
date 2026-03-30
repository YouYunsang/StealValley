using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EscapeZone : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private RunResultManager _runResultManager;
    [SerializeField] private EscapeConfirmUI _escapeConfirmUI;
    [SerializeField] private PlayerInputLock _playerInputLock;
    [SerializeField] private LayerMask _playerLayerMask;

    [Header("Settings")]
    [SerializeField] private float _escapeSlowTimeScale = 0.5f;
    [SerializeField] private float _reactivationDelay = 1.0f;

    [Header("Debug")]
    [SerializeField] private bool _showDebugLog = true;

    private bool _isEscapePromptOpen;
    private bool _isEscapeZoneTemporarilyBlocked;
    private Coroutine _reactivationCoroutine;

    private void Start()
    {
        if (_gameManager == null)
        {
            Debug.LogError($"{nameof(EscapeZone)}: GameManager가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_runResultManager == null)
        {
            Debug.LogError($"{nameof(EscapeZone)}: RunResultManager가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_escapeConfirmUI == null)
        {
            Debug.LogError($"{nameof(EscapeZone)}: EscapeConfirmUI가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_playerInputLock == null)
        {
            Debug.LogError($"{nameof(EscapeZone)}: PlayerInputLock이 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_playerLayerMask == 0)
        {
            Debug.LogError($"{nameof(EscapeZone)}: Player Layer Mask가 설정되지 않았습니다.", this);
            enabled = false;
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 이미 게임이 끝났으면 무시한다.
        if (_gameManager.IsGameEnded)
        {
            return;
        }

        // 플레이어가 아니면 무시한다.
        if (!IsPlayerLayer(other.gameObject.layer))
        {
            return;
        }

        // 이미 UI가 열려 있으면 다시 열지 않는다.
        if (_isEscapePromptOpen)
        {
            return;
        }

        // 취소 직후 재활성화 대기 중이면 무시한다.
        if (_isEscapeZoneTemporarilyBlocked)
        {
            return;
        }

        OpenEscapePrompt();
    }

    public void ConfirmEscape()
    {
        // 이미 게임이 끝났다면 중복 성공 처리하지 않는다.
        if (_gameManager.IsGameEnded)
        {
            return;
        }

        if (_showDebugLog)
        {
            Debug.Log("Escape Confirmed - Player chose to leave", this);
        }

        _isEscapePromptOpen = false;
        Time.timeScale = 1f;
        _escapeConfirmUI.Hide();
        _playerInputLock.Unlock(PlayerInputLockReason.EscapePrompt);

        _runResultManager.RecordSuccessResult();
        _gameManager.SuccessRun();
    }

    public void CancelEscape()
    {
        if (_showDebugLog)
        {
            Debug.Log("Escape Cancelled - Player chose to stay", this);
        }

        _isEscapePromptOpen = false;
        Time.timeScale = 1f;
        _escapeConfirmUI.Hide();
        _playerInputLock.Unlock(PlayerInputLockReason.EscapePrompt);

        StartReactivationDelay();
    }

    private void OpenEscapePrompt()
    {
        // 탈출 확인 UI를 열고 시간 감속 및 입력 잠금을 건다.
        _isEscapePromptOpen = true;
        Time.timeScale = _escapeSlowTimeScale;
        _escapeConfirmUI.Show(this);
        _playerInputLock.Lock(PlayerInputLockReason.EscapePrompt);

        if (_showDebugLog)
        {
            Debug.Log("Escape Prompt Opened", this);
        }
    }

    private void StartReactivationDelay()
    {
        if (_reactivationCoroutine != null)
        {
            StopCoroutine(_reactivationCoroutine);
        }

        _reactivationCoroutine = StartCoroutine(ReactivationDelayRoutine());
    }

    private IEnumerator ReactivationDelayRoutine()
    {
        _isEscapeZoneTemporarilyBlocked = true;

        float elapsed = 0f;

        while (elapsed < _reactivationDelay)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        _isEscapeZoneTemporarilyBlocked = false;
        _reactivationCoroutine = null;

        if (_showDebugLog)
        {
            Debug.Log("Escape Zone Reactivated", this);
        }
    }

    private bool IsPlayerLayer(int layer)
    {
        return (_playerLayerMask.value & (1 << layer)) != 0;
    }
}