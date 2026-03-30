using UnityEngine;

[CreateAssetMenu(
    fileName = "CropDropItemData",
    menuName = "ScriptableObjects/Crop/CropDropItemData")]
public class CropDropItemDataSO : ScriptableObject
{
    [Header("Spawn Animation")]
    [SerializeField] private float _spawnHeight = 0.6f;
    [SerializeField] private float _spawnDuration = 0.3f;
    [SerializeField] private float _spawnRandomOffsetX = 0.35f;
    [SerializeField] private float _landBounceScale = 1.08f;
    [SerializeField] private float _landBounceDuration = 0.1f;

    [Header("Detection")]
    [SerializeField] private float _absorbRange = 1.2f;

    [Header("Absorb")]
    [SerializeField] private float _absorbDuration = 0.18f;
    [SerializeField] private float _absorbScale = 0.2f;

    [Header("Lifetime")]
    [SerializeField] private float _lifeTime = 6.0f;
    [SerializeField] private float _expireFadeDuration = 0.2f;

    public float SpawnHeight => _spawnHeight;
    public float SpawnDuration => _spawnDuration;
    public float SpawnRandomOffsetX => _spawnRandomOffsetX;
    public float LandBounceScale => _landBounceScale;
    public float LandBounceDuration => _landBounceDuration;

    public float AbsorbRange => _absorbRange;
    public float AbsorbDuration => _absorbDuration;
    public float AbsorbScale => _absorbScale;

    public float LifeTime => _lifeTime;
    public float ExpireFadeDuration => _expireFadeDuration;

    private void OnValidate()
    {
        if (_spawnHeight < 0f)
        {
            _spawnHeight = 0f;
        }

        if (_spawnDuration < 0.01f)
        {
            _spawnDuration = 0.01f;
        }

        if (_spawnRandomOffsetX < 0f)
        {
            _spawnRandomOffsetX = 0f;
        }

        if (_landBounceScale < 1f)
        {
            _landBounceScale = 1f;
        }

        if (_landBounceDuration < 0.01f)
        {
            _landBounceDuration = 0.01f;
        }

        if (_absorbRange < 0f)
        {
            _absorbRange = 0f;
        }

        if (_absorbDuration < 0.01f)
        {
            _absorbDuration = 0.01f;
        }

        _absorbScale = Mathf.Clamp(_absorbScale, 0.01f, 1f);

        if (_lifeTime < 0.1f)
        {
            _lifeTime = 0.1f;
        }

        if (_expireFadeDuration < 0.01f)
        {
            _expireFadeDuration = 0.01f;
        }
    }
}
