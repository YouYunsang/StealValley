using UnityEngine;

public class GuardPatrol : MonoBehaviour
{
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private bool _loopPatrol = true;
    [SerializeField] private bool _startFromClosestWaypoint = true;

    private int _currentWaypointIndex;

    public int CurrentWaypointIndex => _currentWaypointIndex;
    public bool HasWaypoints => _waypoints != null && _waypoints.Length > 0;

    private void Awake()
    {
        // 시작 시 가장 가까운 웨이포인트부터 순찰할지 결정한다.
        if (!HasWaypoints)
        {
            Debug.LogError($"{nameof(GuardPatrol)}: 웨이포인트가 설정되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_startFromClosestWaypoint)
        {
            _currentWaypointIndex = GetClosestWaypointIndex();
            Debug.LogFormat("start from closest waypoint: {0}",_currentWaypointIndex);
        }
        else
        {
            _currentWaypointIndex = 0;
        }
    }

    public Vector2 GetCurrentWaypointPosition()
    {
        // 현재 웨이포인트 좌표를 반환한다.
        return _waypoints[_currentWaypointIndex].position;
    }

    public void SetCurrentWaypointIndex(int waypointIndex)
    {
        if (!HasWaypoints)
        {
            return;
        }

        _currentWaypointIndex = Mathf.Clamp(waypointIndex, 0, _waypoints.Length - 1);
    }

    public void MoveToNextWaypoint()
    {
        // 다음 순찰 지점으로 인덱스를 이동한다.
        if (!HasWaypoints)
        {
            return;
        }

        if (_waypoints.Length == 1)
        {
            _currentWaypointIndex = 0;
            return;
        }

        if (_loopPatrol)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
            return;
        }

        _currentWaypointIndex++;

        if (_currentWaypointIndex >= _waypoints.Length)
        {
            _currentWaypointIndex = _waypoints.Length - 1;
        }
    }

    public int GetClosestWaypointIndex()
    {
        // 현재 위치에서 가장 가까운 웨이포인트 인덱스를 찾는다.
        int closestIndex = 0;
        float closestDistance = float.MaxValue;
        Vector2 currentPosition = transform.position;

        for (int i = 0; i < _waypoints.Length; i++)
        {
            float distance = Vector2.Distance(currentPosition, _waypoints[i].position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    private void OnDrawGizmos()
    {
        // 순찰 경로를 에디터에서 시각화한다.
        if (_waypoints == null || _waypoints.Length == 0)
        {
            return;
        }

        Gizmos.color = Color.green;

        for (int i = 0; i < _waypoints.Length; i++)
        {
            if (_waypoints[i] == null)
            {
                continue;
            }

            Gizmos.DrawWireSphere(_waypoints[i].position, 0.15f);

            if (i < _waypoints.Length - 1 && _waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(_waypoints[i].position, _waypoints[i + 1].position);
            }
        }

        if (_loopPatrol && _waypoints.Length > 1 && _waypoints[0] != null && _waypoints[_waypoints.Length - 1] != null)
        {
            Gizmos.DrawLine(_waypoints[_waypoints.Length - 1].position, _waypoints[0].position);
        }
    }
}