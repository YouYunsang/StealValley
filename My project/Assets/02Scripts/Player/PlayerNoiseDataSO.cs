using UnityEngine;

[CreateAssetMenu(
        fileName = "PlayerNoiseData",
        menuName = "ScriptableObjects/Player/PlayerNoiseDate"
        )]

public class PlayerNoiseDataSO : ScriptableObject
{
    [Header("Noise Radius")]
    [SerializeField] private float _idleNoiseRadius = 0.1f;
    [SerializeField] private float _moveNoiseRadius = 2f;
    [SerializeField] private float _stealthMoveNoiseRadius = 1f;
    [SerializeField] private float _harvestNoiseRadius = 3.0f;

    [Header("Debug")]
    [SerializeField] private bool _showNoiseGizmo = true;

    public float IdleNoiseRadius => _idleNoiseRadius;
    public float MoveNoiseRadius => _moveNoiseRadius;
    public float StealthMoveNoiseRadius => _stealthMoveNoiseRadius;
    public float HarvestNoiseRadius => _harvestNoiseRadius;
    public bool ShowNoiseGizmo => _showNoiseGizmo;
}   