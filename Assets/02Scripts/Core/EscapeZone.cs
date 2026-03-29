using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EscapeZone : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private FarmProgressManager _farmProgressManager;
    [SerializeField] private LayerMask _playerLayerMask;
    [SerializeField] private bool _showDebugLog = true;

    public bool CanEscape => _farmProgressManager != null && _farmProgressManager.IsCompleted;

    void Start()
    {
        if (_gameManager == null)
        {
            Debug.LogError($"{nameof(EscapeZone)}: GameManager가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_farmProgressManager == null)
        {
            Debug.LogError($"{nameof(EscapeZone)}: FarmProgressManager가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_playerLayerMask == 0)
        {
            Debug.LogError($"{nameof(EscapeZone)}: Player Layer Mask가 설정되지 않았습니다.", this);
            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_gameManager.IsGameEnded) return;

        if (!IsPlayerLayer(other.gameObject.layer)) return;

        if (CanEscape)
        {
            if (_showDebugLog)
            {
                Debug.Log("Escape Success - Player reached escape zone", this);
            }

            _gameManager.SuccessRun();
            return;
        }

        if (_showDebugLog)
        {
            Debug.Log("Escape Blocked - Progress not completed yet", this);
        }
    }

    private bool IsPlayerLayer(int layer)
    {
        return (_playerLayerMask.value & (1 << layer)) != 0;
    }
}
