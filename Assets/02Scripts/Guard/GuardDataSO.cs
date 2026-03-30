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
    [SerializeField] private float _patrolSearchDuration = 1.0f;

    [Header("Search Vision")]
    [SerializeField] private float _searchSweepAngle = 90f;
    [SerializeField] private float _searchSweepSpeed = 1.5f;

    [Header("Patrol Search Vision")]
    [SerializeField] private float _patrolSearchSweepAngle = 60f;
    [SerializeField] private float _patrolSearchSweepSpeed = 1.5f;

    [Header("Detection")]
    [SerializeField] private LayerMask _playerLayerMask;

    [Header("Distance")]
    [SerializeField] private float _arrivalThreshold = 0.1f;

    [Header("Vision")]
    [SerializeField] private float _viewDistance = 3.5f;
    [SerializeField] private float _viewAngle = 70f;
    [SerializeField] private bool _showVisionGizmo = true;

    [Header("Debug")]
    [SerializeField] private bool _showSensorGizmo = true;

    public float PatrolMoveSpeed => _patrolMoveSpeed;
    public float InvestigateMoveSpeed => _investigateMoveSpeed;
    public float ChaseMoveSpeed => _chaseMoveSpeed;
    public float ReturnMoveSpeed => _returnMoveSpeed;

    public float SearchDuration => _searchDuration;
    public float PatrolSearchDuration => _patrolSearchDuration;
    public float SearchSweepAngle => _searchSweepAngle;
    public float SearchSweepSpeed => _searchSweepSpeed;
    public float PatrolSearchSweepAngle => _patrolSearchSweepAngle;
    public float PatrolSearchSweepSpeed => _patrolSearchSweepSpeed;
    public LayerMask PlayerLayerMask => _playerLayerMask;

    public float ArrivalThreshold => _arrivalThreshold;
    public bool ShowSensorGizmo => _showSensorGizmo;
    public float ViewDistance => _viewDistance;
    public float ViewAngle => _viewAngle;
    public bool ShowVisionGizmo => _showVisionGizmo;

    private void OnValidate()
    {
        if(_viewDistance < 0f)
        {
            _viewDistance = 0f;
        }

        _viewAngle = Mathf.Clamp(_viewAngle, 0f, 360f);

        if(_arrivalThreshold < 0f)
        {
            _arrivalThreshold = 0f;
        }

        if(_searchDuration < 0f)
        {
            _searchDuration = 0f;
        }

        _searchSweepAngle = Mathf.Clamp(_searchSweepAngle, 0f, 360f);

        if (_patrolSearchDuration < 0f)
        {
            _patrolSearchDuration = 0f;
        }

        // 수색 회전 속도는 음수가 될 수 없도록 보정한다.
        if (_searchSweepSpeed < 0f)
        {
            _searchSweepSpeed = 0f;
        }

        if(_patrolSearchSweepSpeed < 0f)
        {
            _patrolSearchSweepSpeed = 0f;
        }
    }
}