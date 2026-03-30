using TMPro;
using UnityEngine;

public class NightClockUI : MonoBehaviour
{
    private const int START_HOUR = 21;
    private const int END_HOUR = 5;
    private const int TOTAL_NIGHT_MINUTES = 8 * 60;
    private const int MINUTES_PER_UPDATE = 10;
    private const int MINUTES_PER_DAY = 24 * 60;

    [Header("References")]
    [SerializeField] private NightTimer _nightTimer;
    [SerializeField] private TextMeshProUGUI _timeText;

    [Header("Debug")]
    [SerializeField] private bool _showDebugLog = false;

    private int _lastDisplayedMinuteBucket = -1;

    private void Start()
    {
        // 외부 참조를 검증한다.
        if (_nightTimer == null)
        {
            Debug.LogError($"{nameof(NightClockUI)}: NightTimer가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_timeText == null)
        {
            Debug.LogError($"{nameof(NightClockUI)}: TimeText가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        // 시작 시 현재 타이머 값으로 시계를 즉시 갱신한다.
        RefreshClock(_nightTimer.RemainingTime, true);
    }

    private void OnEnable()
    {
        // 밤 타이머 변경 이벤트를 구독한다.
        if (_nightTimer != null)
        {
            _nightTimer.OnTimeChanged += HandleTimeChanged;
        }
    }

    private void OnDisable()
    {
        // 밤 타이머 변경 이벤트 구독을 해제한다.
        if (_nightTimer != null)
        {
            _nightTimer.OnTimeChanged -= HandleTimeChanged;
        }
    }

    private void HandleTimeChanged(float remainingTime)
    {
        // 남은 시간을 시계용 시간으로 변환해 필요할 때만 갱신한다.
        RefreshClock(remainingTime, false);
    }

    private void RefreshClock(float remainingTime, bool forceUpdate)
    {
        if (_nightTimer == null || _nightTimer.NightDuration <= 0f)
        {
            return;
        }

        float normalizedRemainingTime = Mathf.Clamp01(remainingTime / _nightTimer.NightDuration);
        float elapsedProgress01 = 1f - normalizedRemainingTime;

        float elapsedMinutesFloat = elapsedProgress01 * TOTAL_NIGHT_MINUTES;
        int elapsedMinutes = Mathf.FloorToInt(elapsedMinutesFloat);

        int currentTotalMinutes = (START_HOUR * 60 + elapsedMinutes) % MINUTES_PER_DAY;

        // 10분 단위로 내림 처리한다.
        int minuteBucket = (currentTotalMinutes / MINUTES_PER_UPDATE) * MINUTES_PER_UPDATE;

        if (!forceUpdate && minuteBucket == _lastDisplayedMinuteBucket)
        {
            return;
        }

        _lastDisplayedMinuteBucket = minuteBucket;

        int displayHour = minuteBucket / 60;
        int displayMinute = minuteBucket % 60;

        _timeText.text = $"{displayHour:00}:{displayMinute:00}";

        if (_showDebugLog)
        {
            Debug.Log(
                $"Night Clock Updated - Remaining: {remainingTime:F2}, Display: {_timeText.text}",
                this
            );
        }
    }
}