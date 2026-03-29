using UnityEngine;

[CreateAssetMenu(
    fileName = "GuardData",
    menuName = "ScriptableObjects/Guard/GuardData"
)]
public class GuardDataSO : ScriptableObject
{
    [Header("Move Speed")]
    [SerializeField] private float _patrolMoveSpeed = 2.0f;
    [SerializeField] private float _investigateMoveSpeed = 2.8f;
    [SerializeField] private float _chaseMoveSpeed = 5f;
    [SerializeField] private float _returnMoveSpeed = 2.4f;

    [Header("State")]
    [SerializeField] private float _searchDuration = 2.0f;

    [Header("Detection")]
    [SerializeField] private float _chaseDetectRange = 0.9f;
    [SerializeField] private LayerMask _playerLayerMask;

    [Header("Distance")]
    [SerializeField] private float _arrivalThreshold = 0.1f;

    [Header("Debug")]
    [SerializeField] private bool _showSensorGizmo = true;

    public float PatrolMoveSpeed => _patrolMoveSpeed;
    public float InvestigateMoveSpeed => _investigateMoveSpeed;
    public float ChaseMoveSpeed => _chaseMoveSpeed;
    public float ReturnMoveSpeed => _returnMoveSpeed;

    public float SearchDuration => _searchDuration;
    public float ChaseDetectRange => _chaseDetectRange;
    public LayerMask PlayerLayerMask => _playerLayerMask;

    public float ArrivalThreshold => _arrivalThreshold;
    public bool ShowSensorGizmo => _showSensorGizmo;
}