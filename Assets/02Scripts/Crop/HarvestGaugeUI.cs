using UnityEngine;
using UnityEngine.UI;

public class HarvestGaugeUI : MonoBehaviour
{
    [SerializeField] private PlayerHarvest _playerHarvest;
    [SerializeField] private Camera _targetCamera;
    [SerializeField] private Canvas _targetCanvas;

    [Header("UI")]
    [SerializeField] private RectTransform _gaugeRoot;
    [SerializeField] private Image _fillImage;

    [Header("Position")]
    [SerializeField] private Vector3 _worldOffset = new Vector3(0f, 0.8f, 0f);

    private RectTransform _canvasRectTransform;

    private void Awake()
    {
        // Canvas의 RectTransform을 캐싱한다.
        if (_targetCanvas != null)
        {
            _canvasRectTransform = _targetCanvas.GetComponent<RectTransform>();
        }
    }

    private void Start()
    {
        // 외부 참조를 검증한다.
        if (_playerHarvest == null)
        {
            Debug.LogError($"{nameof(HarvestGaugeUI)}: PlayerHarvest가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_targetCamera == null)
        {
            Debug.LogError($"{nameof(HarvestGaugeUI)}: Target Camera가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_targetCanvas == null)
        {
            Debug.LogError($"{nameof(HarvestGaugeUI)}: Target Canvas가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_gaugeRoot == null)
        {
            Debug.LogError($"{nameof(HarvestGaugeUI)}: Gauge Root가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_fillImage == null)
        {
            Debug.LogError($"{nameof(HarvestGaugeUI)}: Fill Image가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        // 시작 시 게이지를 숨긴다.
        _gaugeRoot.gameObject.SetActive(false);
        _fillImage.fillAmount = 0f;
    }

    private void Update()
    {
        UpdateGaugeVisibility();
        UpdateGaugePosition();
        UpdateGaugeFill();
    }

    private void UpdateGaugeVisibility()
    {
        // 현재 채집 대상이 있고 실제 채집 중일 때만 게이지를 표시한다.
        bool shouldShow = _playerHarvest.CurrentTargetCrop != null && _playerHarvest.IsHarvesting;

        if (_gaugeRoot.gameObject.activeSelf != shouldShow)
        {
            _gaugeRoot.gameObject.SetActive(shouldShow);
        }
    }

    private void UpdateGaugePosition()
    {
        // 표시 중이 아닐 때는 위치를 갱신하지 않는다.
        if (!_gaugeRoot.gameObject.activeSelf)
        {
            return;
        }

        Crop currentCrop = _playerHarvest.CurrentTargetCrop;

        if (currentCrop == null)
        {
            return;
        }

        // 현재 채집 대상 작물의 월드 위치를 화면 위치로 변환한다.
        Vector3 worldPosition = currentCrop.transform.position + _worldOffset;
        Vector3 screenPosition = _targetCamera.WorldToScreenPoint(worldPosition);

        // 카메라 뒤에 있으면 게이지를 숨긴다.
        if (screenPosition.z < 0f)
        {
            _gaugeRoot.gameObject.SetActive(false);
            return;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRectTransform,
            screenPosition,
            null,
            out Vector2 localPoint
        );

        _gaugeRoot.anchoredPosition = localPoint;
    }

    private void UpdateGaugeFill()
    {
        // 표시 중이 아닐 때는 진행도를 초기화한다.
        if (!_gaugeRoot.gameObject.activeSelf)
        {
            _fillImage.fillAmount = 0f;
            return;
        }

        // 현재 채집 진행도를 0~1 값으로 반영한다.
        _fillImage.fillAmount = Mathf.Clamp01(_playerHarvest.CurrentHarvestNormalized);
    }
}