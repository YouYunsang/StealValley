using UnityEngine;

public class Haystack : MonoBehaviour
{
    [SerializeField] private Transform _hidePoint;
    [SerializeField] private Transform _exitPoint;

    public Vector3 HidePosition => _hidePoint != null ? _hidePoint.position : transform.position;
    public Vector3 ExitPosition => _exitPoint != null ? _exitPoint.position : transform.position;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHide playerHide = other.GetComponentInParent<PlayerHide>();

        if (playerHide == null) return;

        playerHide.SetNearbyHaystack(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerHide playerHide = other.GetComponentInParent<PlayerHide>();

        if(playerHide == null) return;

        playerHide.ClearNearbyHaystack(this);
    }
}
