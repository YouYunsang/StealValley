using UnityEngine;

public class PlayerBasketInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BasketUI _basketUI;

    private PlayerInputReader _inputReader;
    private PlayerController _playerController;
    private PlayerInputLock _inputLock;

    private BasketController _nearbyBasket;

    public BasketController NearbyBasket => _nearbyBasket;
    public bool HasNearbyBasket => _nearbyBasket != null;

    private void Awake()
    {
        _inputReader = GetComponentInParent<PlayerInputReader>();
        _playerController = GetComponentInParent<PlayerController>();
        _inputLock = GetComponentInParent<PlayerInputLock>();
    }

    private void Start()
    {
        if (_basketUI == null)
        {
            Debug.LogError($"{nameof(PlayerBasketInteraction)}: BasketUI가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        HandleBasketInteraction();
    }

    public void SetNearbyBasket(BasketController basketController)
    {
        _nearbyBasket = basketController;
    }

    public void ClearNearbyBasket(BasketController basketController)
    {
        if (_nearbyBasket != basketController)
        {
            return;
        }

        _nearbyBasket = null;
    }

    private void HandleBasketInteraction()
    {
        // 전역 입력 잠금 상태면 바구니 상호작용을 막는다.
        if (_inputLock != null && _inputLock.IsGameplayInputLocked)
        {
            return;
        }

        if (!_inputReader.WasInteractPressedThisFrame)
        {
            return;
        }

        if (_playerController != null && _playerController.IsActionLocked)
        {
            return;
        }

        if (_nearbyBasket == null)
        {
            return;
        }

        if (_basketUI.gameObject.activeSelf)
        {
            _basketUI.Hide();
            return;
        }

        _basketUI.Show();
    }
}