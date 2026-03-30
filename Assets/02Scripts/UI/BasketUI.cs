using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasketUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BasketController _basketController;
    [SerializeField] private BasketStorage _basketStorage;
    [SerializeField] private TextMeshProUGUI _storedCountText;
    [SerializeField] private Button _storeButton;
    [SerializeField] private Button _withdrawButton;
    [SerializeField] private Button _closeButton;

    private void Start()
    {
        // 외부 참조를 검증한다.
        if (_basketController == null)
        {
            Debug.LogError($"{nameof(BasketUI)}: BasketController가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_basketStorage == null)
        {
            Debug.LogError($"{nameof(BasketUI)}: BasketStorage가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_storedCountText == null)
        {
            Debug.LogError($"{nameof(BasketUI)}: StoredCountText가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        RefreshStoredCountText(_basketStorage.StoredHarvestCount);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (_basketStorage != null)
        {
            _basketStorage.OnStoredHarvestCountChanged += HandleStoredHarvestCountChanged;
        }

        if (_storeButton != null)
        {
            _storeButton.onClick.AddListener(HandleStoreClicked);
        }

        if (_withdrawButton != null)
        {
            _withdrawButton.onClick.AddListener(HandleWithdrawClicked);
        }

        if (_closeButton != null)
        {
            _closeButton.onClick.AddListener(Hide);
        }

        if (_basketStorage != null)
        {
            RefreshStoredCountText(_basketStorage.StoredHarvestCount);
        }
    }

    private void OnDisable()
    {
        if (_basketStorage != null)
        {
            _basketStorage.OnStoredHarvestCountChanged -= HandleStoredHarvestCountChanged;
        }

        if (_storeButton != null)
        {
            _storeButton.onClick.RemoveListener(HandleStoreClicked);
        }

        if (_withdrawButton != null)
        {
            _withdrawButton.onClick.RemoveListener(HandleWithdrawClicked);
        }

        if (_closeButton != null)
        {
            _closeButton.onClick.RemoveListener(Hide);
        }
    }

    public void Show()
    {
        // 바구니 UI를 열고 현재 저장 수를 즉시 갱신한다.
        gameObject.SetActive(true);

        if (_basketStorage != null)
        {
            RefreshStoredCountText(_basketStorage.StoredHarvestCount);
        }
    }

    public void Hide()
    {
        // 바구니 UI를 닫는다.
        gameObject.SetActive(false);
    }

    private void HandleStoreClicked()
    {
        // 보관 버튼 클릭 시 플레이어 소지 작물을 전부 바구니에 넣는다.
        _basketController.StoreAllHarvest();
    }

    private void HandleWithdrawClicked()
    {
        // 꺼내기 버튼 클릭 시 바구니 작물을 전부 플레이어에게 준다.
        _basketController.WithdrawAllHarvest();
    }

    private void HandleStoredHarvestCountChanged(int currentStoredCount)
    {
        // 바구니 저장 수 표시를 갱신한다.
        RefreshStoredCountText(currentStoredCount);
    }

    private void RefreshStoredCountText(int currentStoredCount)
    {
        _storedCountText.text = currentStoredCount.ToString();
    }
}