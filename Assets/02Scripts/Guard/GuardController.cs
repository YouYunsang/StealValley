using UnityEngine;

[RequireComponent(typeof(GuardMovement))]
[RequireComponent(typeof(GuardPatrol))]
[RequireComponent(typeof(GuardSensor))]
public class GuardController : MonoBehaviour
{
    [SerializeField] private GuardDataSO _guardData;
    [SerializeField] private GuardAlertBubble _alertBubble;

    private GuardMovement _movement;
    private GuardPatrol _patrol;
    private GuardSensor _sensor;

    private GuardState _currentState = GuardState.Patrol;
    private Vector2 _lastHeardPosition;
    private float _searchTimer;

    private Vector2 _facingDirection = Vector2.down;

    private Vector2 _searchBaseFacingDirection = Vector2.down;
    private float _searchSweepTimer;

    private const string ALERT_MESSAGE = "거기 누구야!";
    private const string LOST_TARGET_MESSAGE = "어디갔어, 나와!";

    private static readonly string[] CHASE_MESSAGES =
    {
        "제발 가져가지 마..",
        "그게 없으면 우린 굶어..",
        "딱 걸렸어, 거기서!"
    };

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
        // 외부 데이터 참조를 검증한다.
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
        if (_sensor != null)
        {
            _sensor.OnHeardNoise -= HandleHeardNoise;
            _sensor.OnPlayerDetected -= HandlePlayerDetected;
            _sensor.OnPlayerLost -= HandlePlayerLost;
        }

        // 비활성화 시 남아 있는 말풍선을 즉시 숨긴다.
        if (_alertBubble != null)
        {
            _alertBubble.HideImmediate();
        }
    }

    private void UpdateState()
    {
        switch (_currentState)
        {
            case GuardState.Patrol:
                UpdatePatrolState();
                break;

            case GuardState.PatrolSearch:
                UpdatePatrolSearchState();
                break;

            case GuardState.Alert:
                UpdateAlertState();
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
        // 마지막으로 들은 소리 위치를 갱신한다.
        _lastHeardPosition = heardPosition;

        // 추격 중에는 소리보다 시야 추적이 우선이다.
        if (_currentState == GuardState.Chase)
        {
            return;
        }

        // 소리를 들었을 때는 즉시 조사하지 않고 Alert 상태로 먼저 반응한다.
        EnterAlertState(_lastHeardPosition);
    }

    private void HandlePlayerDetected(Vector2 detectedPlayerPosition)
    {
        // 플레이어를 직접 시야로 포착하면 Alert를 건너뛰고 즉시 추격한다.
        _lastHeardPosition = detectedPlayerPosition;
        EnterChaseState();
    }

    private void HandlePlayerLost()
    {
        // 추격 중 플레이어를 놓쳤을 때만 마지막 위치를 조사하러 간다.
        if (_currentState != GuardState.Chase)
        {
            return;
        }

        _lastHeardPosition = _sensor.LastDetectedPlayerPosition;
        EnterInvestigateState(_lastHeardPosition, true, LOST_TARGET_MESSAGE);
    }

    private void UpdateFacingDirectionTo(Vector2 targetPosition)
    {
        // 목표 위치를 기준으로 정면 방향을 갱신한다.
        Vector2 direction = targetPosition - (Vector2)transform.position;
        UpdateFacingDirection(direction);
    }

    private void UpdateFacingDirection(Vector2 direction)
    {
        // 유효한 방향일 때만 정면 방향을 갱신한다.
        if (direction.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        _facingDirection = direction.normalized;
    }

    #region 상태 도중 업데이트
    private void UpdatePatrolState()
    {
        if (!_movement.HasArrived())
        {
            return;
        }

        EnterPatrolSearchState();
    }

    private void UpdatePatrolSearchState()
    {
        UpdateSweepFacingDirection(
            _searchBaseFacingDirection,
            _guardData.PatrolSearchSweepAngle,
            _guardData.PatrolSearchSweepSpeed
        );

        _searchTimer -= Time.deltaTime;

        if (_searchTimer > 0f)
        {
            return;
        }

        _patrol.MoveToNextWaypoint();
        EnterPatrolState();
    }

    private void UpdateAlertState()
    {
        UpdateFacingDirectionTo(_lastHeardPosition);

        _searchTimer -= Time.deltaTime;

        if (_searchTimer > 0f)
        {
            return;
        }

        EnterInvestigateState(_lastHeardPosition, false, string.Empty);
    }

    private void UpdateInvestigateState()
    {
        UpdateFacingDirectionTo(_lastHeardPosition);

        if (!_movement.HasArrived())
        {
            return;
        }

        EnterSearchState();
    }

    private void UpdateSearchState()
    {
        UpdateSweepFacingDirection(
            _searchBaseFacingDirection,
            _guardData.SearchSweepAngle,
            _guardData.SearchSweepSpeed
        );

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

        if (!_movement.HasArrived())
        {
            return;
        }

        EnterPatrolState();
    }

    private void UpdateChaseState()
    {
        _lastHeardPosition = _sensor.LastDetectedPlayerPosition;

        UpdateFacingDirectionTo(_lastHeardPosition);
        _movement.SetTargetPosition(_lastHeardPosition);
    }
    #endregion

    #region 상태 진입
    private void EnterPatrolState()
    {
        _currentState = GuardState.Patrol;
        _movement.SetMoveType(GuardMoveType.Patrol);
        _movement.SetTargetPosition(_patrol.GetCurrentWaypointPosition());

        UpdateFacingDirectionTo(_patrol.GetCurrentWaypointPosition());
    }

    private void EnterPatrolSearchState()
    {
        _currentState = GuardState.PatrolSearch;
        _searchTimer = _guardData.PatrolSearchDuration;
        _searchSweepTimer = 0f;
        _searchBaseFacingDirection = _facingDirection;

        _movement.ClearTargetPosition();
    }

    private void EnterAlertState(Vector2 alertPosition)
    {
        _currentState = GuardState.Alert;
        _lastHeardPosition = alertPosition;
        _searchTimer = _guardData.AlertDuration;

        _movement.ClearTargetPosition();
        UpdateFacingDirectionTo(_lastHeardPosition);

        if (_alertBubble != null)
        {
            _alertBubble.ShowAlertText(ALERT_MESSAGE);
        }
    }

    private void EnterInvestigateState(Vector2 investigatePosition, bool showBubble, string bubbleMessage)
    {
        _currentState = GuardState.Investigate;
        _lastHeardPosition = investigatePosition;
        _movement.SetMoveType(GuardMoveType.Investigate);
        _movement.SetTargetPosition(_lastHeardPosition);

        UpdateFacingDirectionTo(_lastHeardPosition);

        if (showBubble && _alertBubble != null)
        {
            _alertBubble.ShowAlertText(bubbleMessage);
        }
    }

    private void EnterSearchState()
    {
        _currentState = GuardState.Search;
        _searchTimer = _guardData.SearchDuration;
        _searchSweepTimer = 0f;
        _searchBaseFacingDirection = _facingDirection;

        _movement.ClearTargetPosition();
    }

    private void EnterReturnState()
    {
        int closestWaypointIndex = _patrol.GetClosestWaypointIndex();

        _patrol.SetCurrentWaypointIndex(closestWaypointIndex);

        _currentState = GuardState.Return;
        _movement.SetMoveType(GuardMoveType.Return);
        _movement.SetTargetPosition(_patrol.GetCurrentWaypointPosition());

        UpdateFacingDirectionTo(_patrol.GetCurrentWaypointPosition());
    }

    private void EnterChaseState()
    {
        // 추격 상태 진입 시 랜덤 문구를 골라 지속 표시한다.
        if (_alertBubble != null)
        {
            _alertBubble.ShowPersistentText(GetRandomChaseMessage());
        }

        _currentState = GuardState.Chase;
        _movement.SetMoveType(GuardMoveType.Chase);
        _movement.SetTargetPosition(_sensor.LastDetectedPlayerPosition);

        UpdateFacingDirectionTo(_sensor.LastDetectedPlayerPosition);
    }

    private string GetRandomChaseMessage()
    {
        // Chase 문구 3개 중 하나를 랜덤하게 선택한다.
        int randomIndex = Random.Range(0, CHASE_MESSAGES.Length);
        return CHASE_MESSAGES[randomIndex];
    }
    #endregion

    #region 회전
    private void UpdateSweepFacingDirection(Vector2 baseFacingDirection, float sweepAngle, float sweepSpeed)
    {
        if (sweepAngle <= 0f || sweepSpeed <= 0f)
        {
            _facingDirection = baseFacingDirection;
            return;
        }

        _searchSweepTimer += Time.deltaTime * sweepSpeed;

        float halfAngle = sweepAngle * 0.5f;
        float pingPong = Mathf.PingPong(_searchSweepTimer, 1f);
        float currentOffsetAngle = Mathf.Lerp(-halfAngle, halfAngle, pingPong);

        _facingDirection = RotateVector(baseFacingDirection, currentOffsetAngle);
    }

    private Vector2 RotateVector(Vector2 vector, float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos
        ).normalized;
    }
    #endregion
}