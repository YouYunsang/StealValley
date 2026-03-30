using TMPro;
using UnityEngine;

public class GameEndResultUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private RunResultManager _runResultManager;

    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _countLabelText;
    [SerializeField] private TextMeshProUGUI _countValueText;

    [Header("Message")]
    [SerializeField] private string _successMessage = "무사히 훔쳤습니다!";
    [SerializeField] private string _failedMessage = "무사히 빠져나오지 못했습니다.";
    [SerializeField] private string _countLabelMessage = "훔친 작물";

    private void Start()
    {
        // 외부 참조를 검증한다.
        if (_gameManager == null)
        {
            Debug.LogError($"{nameof(GameEndResultUI)}: GameManager가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_runResultManager == null)
        {
            Debug.LogError($"{nameof(GameEndResultUI)}: RunResultManager가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_resultPanel == null)
        {
            Debug.LogError($"{nameof(GameEndResultUI)}: ResultPanel이 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_titleText == null)
        {
            Debug.LogError($"{nameof(GameEndResultUI)}: TitleText가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_countLabelText == null)
        {
            Debug.LogError($"{nameof(GameEndResultUI)}: CountLabelText가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_countValueText == null)
        {
            Debug.LogError($"{nameof(GameEndResultUI)}: CountValueText가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        _countLabelText.text = _countLabelMessage;
        _resultPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (_gameManager != null)
        {
            _gameManager.OnGameSucceeded += HandleGameSucceeded;
            _gameManager.OnGameFailed += HandleGameFailed;
        }
    }

    private void OnDisable()
    {
        if (_gameManager != null)
        {
            _gameManager.OnGameSucceeded -= HandleGameSucceeded;
            _gameManager.OnGameFailed -= HandleGameFailed;
        }
    }

    private void HandleGameSucceeded()
    {
        // 성공 시 성공 메시지와 최종 작물 개수를 표시한다.
        ShowResult(_successMessage, _runResultManager.FinalHarvestCount);
    }

    private void HandleGameFailed(FailReason failReason)
    {
        // 실패 시 실패 메시지와 최종 작물 개수를 표시한다.
        ShowResult(_failedMessage, _runResultManager.FinalHarvestCount);
    }

    private void ShowResult(string titleMessage, int harvestCount)
    {
        _titleText.text = titleMessage;
        _countLabelText.text = _countLabelMessage;
        _countValueText.text = $"{harvestCount}개";

        _resultPanel.SetActive(true);
    }
}