using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BasketInteractZone : MonoBehaviour
{
    [SerializeField] private BasketController _basketController;

    private void Start()
    {
        if (_basketController == null)
        {
            Debug.LogError($"{nameof(BasketInteractZone)}: BasketController가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerBasketInteraction playerBasketInteraction = other.GetComponent<PlayerBasketInteraction>();

        if (playerBasketInteraction == null)
        {
            return;
        }

        playerBasketInteraction.SetNearbyBasket(_basketController);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerBasketInteraction playerBasketInteraction = other.GetComponent<PlayerBasketInteraction>();

        if (playerBasketInteraction == null)
        {
            return;
        }

        playerBasketInteraction.ClearNearbyBasket(_basketController);
    }
}