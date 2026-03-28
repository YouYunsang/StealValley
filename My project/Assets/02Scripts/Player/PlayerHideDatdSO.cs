using UnityEngine;

[CreateAssetMenu(
    fileName = "PlayerHideData",
    menuName = "ScriptableObjects/Player/PlayerHideData"
    )]

public class PlayerHideDatdSO : ScriptableObject
{
    [Header("Hide")]
    [SerializeField] private float _hideEnterDelay = 0.3f;

    public float HideEnterDelay => _hideEnterDelay;

    private void OnValidate()
    {
        if(_hideEnterDelay < 0f)
        {
            _hideEnterDelay = 0f;
        }
    }
}
