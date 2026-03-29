using UnityEngine;

public class GuardCatcher : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private LayerMask _playerLayerMask;
    [SerializeField] private bool _showDebugLog = true;

    private void Start()
    {
        if(_gameManager == null)
        {
            Debug.LogError($"{nameof(GuardCatcher)}: GameManager가 할당되지 않았습니다.", this);
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

        if(playerHide != null && playerHide.IsHidden)
        {
            return;
        }

        if (_showDebugLog)
        {
            Debug.Log($"Player Caught By Guard - Guard: {name}, Player: {other.name}", this);
        }

        _gameManager.FailRun(FailReason.CaughtByGuard);
    }

    private bool IsPlayerLayer(int layer)
    {
        return (_playerLayerMask.value & (1 << layer)) != 0;
    }
}
