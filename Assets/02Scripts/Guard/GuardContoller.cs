using UnityEngine;

[RequireComponent(typeof(GuardMovement))]
[RequireComponent(typeof(GuardPatrol))]
[RequireComponent(typeof(GuardSensor))]
public class GuardController : MonoBehaviour
{
    [SerializeField] private GuardDataSO _guardData;

    private GuardMovement _movement;
    private GuardPatrol _patrol;
    private GuardSensor _sensor;

    private GuardState _currentState = GuardState.Patrol;
    private Vector2 _lastHeardPosition;
    private float _searchTimer;

    private Vector2 _facingDirection = Vector2.down;

    public GuardState CurrentState => _currentState;
    public Vector2 LastHearPosition => _lastHeardPosition;
    public Vector2 FacingDirection => _facingDirection;

    private void Awake()
    {
        // 같은 오브젝트 내부 컴포넌트를 캐싱한다.
        _movement = GetComponent<GuardMovement>();
        _patrol = GetComponent<GuardPatrol>();
        _sensor = GetComponent<GuardSensor>();
    }

    private void Start()
    {
        // 외부 데이터 참조 검증
        if (_guardData == null)
        {
            Debug.LogError($"{nameof(GuardController)}: GuardDataSO가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        // 시작 시 순찰 상태로 진입한다.
        EnterPatrolState();
    }

    private void Update()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        // 센서 이벤트를 구독한다.
        if (_sensor == null)
        {
            return;
        }

        _sensor.OnHeardNoise += HandleHeardNoise;
        _sensor.OnPlayerDetected += HandlePlayerDetected;
        _sensor.OnPlayerLost += HandlePlayerLost;
    }

    private void OnDisable()
    {
        // 센서 이벤트 구독을 해제한다.
        if (_sensor == null)
        {
            return;
        }

        _sensor.OnHeardNoise -= HandleHeardNoise;
        _sensor.OnPlayerDetected -= HandlePlayerDetected;
        _sensor.OnPlayerLost -= HandlePlayerLost;
    }

    private void UpdateState()
    {
        switch (_currentState)
        {
            case GuardState.Patrol:
                UpdatePatrolState();
                break;

            case GuardState.Investigate:
                UpdateInvestigateState();
                break;

            case GuardState.Search:
                UpdateSearchState();
                break;

            case GuardState.Return:
                UpdateReturnState();
                break;

            case GuardState.Chase:
                UpdateChaseState();
                break;
        }
    }

    private void HandleHeardNoise(Vector2 heardPosition)
    {
        // 추격 중이 아닐 때만 소리 조사 상태로 반응한다.
        _lastHeardPosition = heardPosition;

        if (_currentState == GuardState.Chase)
        {
            return;
        }

        EnterInvestigateState(_lastHeardPosition);
    }

    private void HandlePlayerDetected(Vector2 detectedPlayerPosition)
    {
        // 플레이어를 확실히 포착하면 실시간 추격 상태로 전환한다.
        _lastHeardPosition = detectedPlayerPosition;
        EnterChaseState();
    }

    private void HandlePlayerLost()
    {
        // 추격 중 플레이어를 놓치면 마지막 위치를 조사하러 간다.
        if (_currentState != GuardState.Chase)
        {
            return;
        }

        EnterInvestigateState(_lastHeardPosition);
    }

    private void UpdateFacingDirectionTo(Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - (Vector2)transform.position;
        UpdateFacingDirection(direction);
    }

    private void UpdateFacingDirection(Vector2 direction)
    {
        if(direction.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        _facingDirection = direction.normalized;
    }

    #region 상태 도중 업데이트
    private void UpdatePatrolState()
    {
        // 순찰 지점에 도착하면 다음 웨이포인트로 이동한다.
        if (!_movement.HasArrived())
        {
            return;
        }

        _patrol.MoveToNextWaypoint();
        _movement.SetTargetPosition(_patrol.GetCurrentWaypointPosition());

        UpdateFacingDirectionTo(_patrol.GetCurrentWaypointPosition());
    }

    private void UpdateInvestigateState()
    {
        UpdateFacingDirectionTo(_lastHeardPosition);

        // 마지막으로 들은 위치에 도착하면 수색 상태로 전환한다.
        if (!_movement.HasArrived())
        {
            return;
        }

        EnterSearchState();
    }

    private void UpdateSearchState()
    {
        // 일정 시간 동안 수색 후 순찰 경로로 복귀한다.
        _searchTimer -= Time.deltaTime;

        if (_searchTimer > 0f)
        {
            return;
        }

        EnterReturnState();
    }

    private void UpdateReturnState()
    {
        UpdateFacingDirectionTo(_patrol.GetCurrentWaypointPosition());

        // 가장 가까운 순찰 지점으로 복귀하면 다시 순찰을 재개한다.
        if (!_movement.HasArrived())
        {
            return;
        }

        EnterPatrolState();
    }

    private void UpdateChaseState()
    {
        _lastHeardPosition = _sensor.LastDetectedPlayerPosition;

        UpdateFacingDirectionTo(_sensor.LastDetectedPlayerPosition);

        // 추격 중에는 마지막으로 감지한 플레이어 위치를 계속 목표로 갱신한다.
        _movement.SetTargetPosition(_sensor.LastDetectedPlayerPosition);
    }
    #endregion

    #region 상태 진입
    private void EnterPatrolState()
    {
        // 순찰 상태 초기화
        _currentState = GuardState.Patrol;
        _movement.SetMoveType(GuardMoveType.Patrol);
        _movement.SetTargetPosition(_patrol.GetCurrentWaypointPosition());

        UpdateFacingDirectionTo(_patrol.GetCurrentWaypointPosition());
    }

    private void EnterInvestigateState(Vector2 investigatePosition)
    {
        // 조사 상태 초기화
        _currentState = GuardState.Investigate;
        _lastHeardPosition = investigatePosition;
        _movement.SetMoveType(GuardMoveType.Investigate);
        _movement.SetTargetPosition(_lastHeardPosition);

        UpdateFacingDirectionTo(_lastHeardPosition);
    }

    private void EnterSearchState()
    {
        // 수색 상태 초기화
        _currentState = GuardState.Search;
        _searchTimer = _guardData.SearchDuration;
        _movement.ClearTargetPosition();
    }

    private void EnterReturnState()
    {
        // 가장 가까운 웨이포인트를 기준으로 순찰로 복귀한다.
        int closestWaypointIndex = _patrol.GetClosestWaypointIndex();

        _patrol.SetCurrentWaypointIndex(closestWaypointIndex);

        _currentState = GuardState.Return;
        _movement.SetMoveType(GuardMoveType.Return);
        _movement.SetTargetPosition(_patrol.GetCurrentWaypointPosition());

        UpdateFacingDirectionTo(_patrol.GetCurrentWaypointPosition());
    }

    private void EnterChaseState()
    {
        // 추격 상태 초기화
        _currentState = GuardState.Chase;
        _movement.SetMoveType(GuardMoveType.Chase);
        _movement.SetTargetPosition(_sensor.LastDetectedPlayerPosition);

        UpdateFacingDirectionTo(_sensor.LastDetectedPlayerPosition);
    }
    #endregion
}