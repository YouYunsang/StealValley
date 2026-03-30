using UnityEngine;
using UnityEngine.UI;

public class EscapeConfirmUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;

    private EscapeZone _currentEscapeZone;

    private void Start()
    {
        // 외부 참조를 검증한다.
        if (_yesButton == null)
        {
            Debug.LogError($"{nameof(EscapeConfirmUI)}: Yes Button이 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_noButton == null)
        {
            Debug.LogError($"{nameof(EscapeConfirmUI)}: No Button이 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _yesButton.onClick.AddListener(HandleYesClicked);
        _noButton.onClick.AddListener(HandleNoClicked);
    }

    private void OnDisable()
    {
        _yesButton.onClick.RemoveListener(HandleYesClicked);
        _noButton.onClick.RemoveListener(HandleNoClicked);
    }

    public void Show(EscapeZone escapeZone)
    {
        // 현재 탈출 확인을 요청한 EscapeZone을 기억하고 UI를 연다.
        _currentEscapeZone = escapeZone;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        // 현재 UI를 닫고 EscapeZone 참조를 정리한다.
        _currentEscapeZone = null;
        gameObject.SetActive(false);
    }

    private void HandleYesClicked()
    {
        // '네'를 누르면 현재 EscapeZone에 탈출 확정을 전달한다.
        if (_currentEscapeZone == null)
        {
            return;
        }

        _currentEscapeZone.ConfirmEscape();
    }

    private void HandleNoClicked()
    {
        // '아니오'를 누르면 현재 EscapeZone에 탈출 취소를 전달한다.
        if (_currentEscapeZone == null)
        {
            return;
        }

        _currentEscapeZone.CancelEscape();
    }
}