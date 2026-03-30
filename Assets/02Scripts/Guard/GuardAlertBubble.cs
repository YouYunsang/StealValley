using DG.Tweening;
using TMPro;
using UnityEngine;

public class GuardAlertBubble : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _bubbleRoot;
    [SerializeField] private TextMeshProUGUI _alertText;

    [Header("Text")]
    [SerializeField] private string _defaultAlertMessage = "거기 누구야!";

    [Header("Timing")]
    [SerializeField] private float _showDuration = 0.8f;

    [Header("Animation")]
    [SerializeField] private float _showScaleDuration = 0.15f;
    [SerializeField] private float _hideDuration = 0.12f;
    [SerializeField] private float _moveUpDistance = 12f;
    [SerializeField] private float _moveDuration = 0.18f;
    [SerializeField] private float _startScale = 0.85f;

    private const float HIDDEN_ALPHA = 0f;
    private const float VISIBLE_ALPHA = 1f;
    private const float NORMAL_SCALE = 1f;

    private Tween _showTween;
    private Tween _hideTween;
    private Tween _delayTween;
    private Tween _moveTween;

    private Vector2 _initialAnchoredPosition;
    private bool _isShowing;
    private bool _isPersistent;
    private string _currentMessage = string.Empty;

    public bool IsShowing => _isShowing;

    private void Awake()
    {
        // 같은 오브젝트 내부 및 인스펙터 참조를 초기 상태로 맞춘다.
        CacheInitialState();
        HideVisualImmediate();
    }

    private void OnDisable()
    {
        // 비활성화될 때 진행 중인 트윈을 정리한다.
        KillAllTweens();
        HideVisualImmediate();
    }

    public void ShowAlertText()
    {
        // 기본 Alert 메시지를 말풍선에 출력한다.
        ShowAlertText(_defaultAlertMessage);
    }

    public void ShowAlertText(string message)
    {
        // 자동 숨김이 있는 일반 표시 모드로 보여준다.
        ShowInternal(message, false);
    }

    public void ShowPersistentText(string message)
    {
        // Chase 등 지속 표시가 필요한 경우 자동 숨김 없이 보여준다.
        ShowInternal(message, true);
    }

    public void HideImmediate()
    {
        // Chase 전환 해제 등 즉시 숨겨야 하는 상황에서 말풍선을 바로 끈다.
        KillAllTweens();
        HideVisualImmediate();
    }

    private void ShowInternal(string message, bool isPersistent)
    {
        // 빈 문자열 요청이 들어오면 기본 문구를 사용한다.
        string nextMessage = string.IsNullOrWhiteSpace(message) ? _defaultAlertMessage : message;

        // 같은 문구와 같은 표시 모드가 이미 떠 있다면 필요한 처리만 갱신한다.
        if (_isShowing && _currentMessage == nextMessage && _isPersistent == isPersistent)
        {
            if (!_isPersistent)
            {
                RefreshShowTimer();
            }

            return;
        }

        _currentMessage = nextMessage;
        _isPersistent = isPersistent;
        _alertText.text = _currentMessage;

        PlayShowAnimation();
    }

    private void PlayShowAnimation()
    {
        // 기존 재생 중인 연출을 정리하고 처음부터 다시 표시한다.
        KillAllTweens();

        _isShowing = true;
        gameObject.SetActive(true);

        _canvasGroup.alpha = HIDDEN_ALPHA;
        _bubbleRoot.localScale = Vector3.one * _startScale;
        _bubbleRoot.anchoredPosition = _initialAnchoredPosition;

        // 말풍선을 살짝 위로 올리며 자연스럽게 나타나게 한다.
        _moveTween = _bubbleRoot.DOAnchorPosY(
            _initialAnchoredPosition.y + _moveUpDistance,
            _moveDuration
        );

        // 팝업되듯 스케일을 키워 말풍선 등장감을 만든다.
        _showTween = DOTween.Sequence()
            .Append(_canvasGroup.DOFade(VISIBLE_ALPHA, _showScaleDuration))
            .Join(_bubbleRoot.DOScale(NORMAL_SCALE, _showScaleDuration).SetEase(Ease.OutBack))
            .OnComplete(HandleShowAnimationCompleted);
    }

    private void HandleShowAnimationCompleted()
    {
        // 지속 표시가 아니면 일반 표시 시간 타이머를 시작한다.
        if (_isPersistent)
        {
            return;
        }

        RefreshShowTimer();
    }

    private void RefreshShowTimer()
    {
        // 연속 감지 시에는 표시 유지 시간만 갱신한다.
        if (_delayTween != null && _delayTween.IsActive())
        {
            _delayTween.Kill();
        }

        _delayTween = DOVirtual.DelayedCall(_showDuration, PlayHideAnimation);
    }

    private void PlayHideAnimation()
    {
        // 말풍선을 짧게 사라지게 만들고 상태를 정리한다.
        KillHideTweenOnly();

        _hideTween = DOTween.Sequence()
            .Append(_canvasGroup.DOFade(HIDDEN_ALPHA, _hideDuration))
            .Join(_bubbleRoot.DOScale(_startScale, _hideDuration))
            .OnComplete(HideVisualImmediate);
    }

    private void CacheInitialState()
    {
        // 최초 기준 위치를 저장해 매번 같은 자리에서 연출이 시작되게 한다.
        if (_bubbleRoot != null)
        {
            _initialAnchoredPosition = _bubbleRoot.anchoredPosition;
        }
    }

    private void HideVisualImmediate()
    {
        // 말풍선 비주얼을 즉시 초기 숨김 상태로 되돌린다.
        _isShowing = false;
        _isPersistent = false;
        _currentMessage = string.Empty;

        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = HIDDEN_ALPHA;
        }

        if (_bubbleRoot != null)
        {
            _bubbleRoot.localScale = Vector3.one * _startScale;
            _bubbleRoot.anchoredPosition = _initialAnchoredPosition;
        }
    }

    private void KillAllTweens()
    {
        // 현재 재생 중인 모든 말풍선 트윈을 안전하게 중지한다.
        KillTween(ref _showTween);
        KillTween(ref _hideTween);
        KillTween(ref _delayTween);
        KillTween(ref _moveTween);
    }

    private void KillHideTweenOnly()
    {
        // Hide 재생 전에 중복 hide 트윈만 정리한다.
        KillTween(ref _hideTween);
    }

    private void KillTween(ref Tween targetTween)
    {
        // 유효한 트윈만 종료하고 참조를 비운다.
        if (targetTween != null && targetTween.IsActive())
        {
            targetTween.Kill();
        }

        targetTween = null;
    }
}