using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HarvestCountUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HarvestCountManager _harvestCountManager;
    [SerializeField] private Image _cropIconImage;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private RectTransform _countTextRect;

    [Header("Icon")]
    [SerializeField] private Sprite _cropIconSprite;

    [Header("Animation")]
    [SerializeField] private float _popScale = 1.15f;
    [SerializeField] private float _popDuration = 0.12f;

    private Tween _countPopTween;
    private Vector3 _countTextInitialScale = Vector3.one;

    private void Awake()
    {
        // 같은 오브젝트 또는 인스펙터 참조를 초기 상태로 맞춘다.
        if (_countText != null)
        {
            _countTextInitialScale = _countText.rectTransform.localScale;
        }

        if (_countTextRect == null && _countText != null)
        {
            _countTextRect = _countText.rectTransform;
        }
    }

    private void Start()
    {
        // 외부 참조를 검증한다.
        if (_harvestCountManager == null)
        {
            Debug.LogError($"{nameof(HarvestCountUI)}: HarvestCountManager가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_cropIconImage == null)
        {
            Debug.LogError($"{nameof(HarvestCountUI)}: Crop Icon Image가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_countText == null)
        {
            Debug.LogError($"{nameof(HarvestCountUI)}: Count Text가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_countTextRect == null)
        {
            Debug.LogError($"{nameof(HarvestCountUI)}: Count Text RectTransform이 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        ApplyIcon();
        RefreshCountText(_harvestCountManager.CurrentHarvestCount);
    }

    private void OnEnable()
    {
        // 수확 개수 변경 이벤트를 구독한다.
        if (_harvestCountManager != null)
        {
            _harvestCountManager.OnHarvestCountChanged += HandleHarvestCountChanged;
        }
    }

    private void OnDisable()
    {
        // 수확 개수 변경 이벤트 구독을 해제한다.
        if (_harvestCountManager != null)
        {
            _harvestCountManager.OnHarvestCountChanged -= HandleHarvestCountChanged;
        }

        KillCountPopTween();
    }

    private void HandleHarvestCountChanged(int currentHarvestCount)
    {
        // 개수 표시를 갱신하고 숫자 팝 연출을 재생한다.
        RefreshCountText(currentHarvestCount);
        PlayCountPopAnimation();
    }

    private void ApplyIcon()
    {
        // UI에 표시할 작물 아이콘 스프라이트를 적용한다.
        _cropIconImage.sprite = _cropIconSprite;
        _cropIconImage.enabled = _cropIconSprite != null;
    }

    private void RefreshCountText(int currentHarvestCount)
    {
        // 현재 훔친 작물 개수를 텍스트에 반영한다.
        _countText.text = currentHarvestCount.ToString();
    }

    private void PlayCountPopAnimation()
    {
        // 숫자 텍스트만 짧게 팝 연출해 획득 피드백을 준다.
        KillCountPopTween();

        _countTextRect.localScale = _countTextInitialScale;

        _countPopTween = _countTextRect
            .DOScale(_countTextInitialScale * _popScale, _popDuration)
            .SetEase(Ease.OutBack)
            .SetLoops(2, LoopType.Yoyo);
    }

    private void KillCountPopTween()
    {
        // 진행 중인 숫자 팝 트윈을 정리한다.
        if (_countPopTween != null && _countPopTween.IsActive())
        {
            _countPopTween.Kill();
        }

        _countPopTween = null;
    }
}