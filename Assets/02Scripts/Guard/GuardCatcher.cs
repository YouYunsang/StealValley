using UnityEngine;

public class GuardCatcher : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private RunResultManager _runResultManager;
    [SerializeField] private LayerMask _playerLayerMask;
    [SerializeField] private bool _showDebugLog = true;

    private void Start()
    {
        if (_gameManager == null)
        {
            Debug.LogError($"{nameof(GuardCatcher)}: GameManager가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_runResultManager == null)
        {
            Debug.LogError($"{nameof(GuardCatcher)}: RunResultManager가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_playerLayerMask == 0)
        {
            Debug.LogError($"{nameof(GuardCatcher)}: Player Layer Mask가 설정되지 않았습니다.", this);
            enabled = false;
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_gameManager.IsGameEnded)
        {
            return;
        }

        if (!IsPlayerLayer(other.gameObject.layer))
        {
            return;
        }

        PlayerHide playerHide = other.GetComponentInParent<PlayerHide>();

        if (playerHide != null && playerHide.IsHidden)
        {
            return;
        }

        if (_showDebugLog)
        {
            Debug.Log($"Player Caught By Guard - Guard: {name}, Player: {other.name}", this);
        }

        // 감시자에게 잡혔을 때 최종 점수를 0으로 먼저 확정한다.
        _runResultManager.RecordFailResult();

        // 그 다음 게임 오버 상태로 전환한다.
        _gameManager.FailRun(FailReason.CaughtByGuard);
    }

    private bool IsPlayerLayer(int layer)
    {
        return (_playerLayerMask.value & (1 << layer)) != 0;
    }
}