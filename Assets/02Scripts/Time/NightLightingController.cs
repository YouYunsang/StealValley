using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NightLightingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light2D _globalLight;
    [SerializeField] private NightTimer _nightTimer;

    [Header("Light")]
    [SerializeField] private float _startIntensity = 0.85f;
    [SerializeField] private float _endIntensity = 0.45f;

    [SerializeField] private Color _startColor = new Color(0.75f, 0.8f, 1f, 1f);
    [SerializeField] private Color _endColor = new Color(0.2f, 0.24f, 0.35f, 1f);

    [Header("Curve")]
    [SerializeField] private AnimationCurve _darknessCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Debug")]
    [SerializeField] private bool _showDebugLog = false;

    private void Start()
    {
        // 외부 참조를 검증한다.
        if (_globalLight == null)
        {
            Debug.LogError($"{nameof(NightLightingController)}: Global Light 2D가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_nightTimer == null)
        {
            Debug.LogError($"{nameof(NightLightingController)}: NightTimer가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        // 시작 시 현재 남은 시간을 기준으로 조명 상태를 즉시 맞춘다.
        ApplyLightingFromRemainingTime(_nightTimer.RemainingTime);
    }

    private void OnEnable()
    {
        // 타이머 이벤트를 구독한다.
        if (_nightTimer != null)
        {
            _nightTimer.OnTimeChanged += HandleTimeChanged;
        }
    }

    private void OnDisable()
    {
        // 타이머 이벤트 구독을 해제한다.
        if (_nightTimer != null)
        {
            _nightTimer.OnTimeChanged -= HandleTimeChanged;
        }
    }

    private void HandleTimeChanged(float remainingTime)
    {
        // 타이머의 남은 시간을 조명 진행률로 변환해 반영한다.
        ApplyLightingFromRemainingTime(remainingTime);
    }

    private void ApplyLightingFromRemainingTime(float remainingTime)
    {
        if (_nightTimer == null || _nightTimer.NightDuration <= 0f)
        {
            return;
        }

        float normalizedRemainingTime = Mathf.Clamp01(remainingTime / _nightTimer.NightDuration);
        float elapsedProgress01 = 1f - normalizedRemainingTime;

        ApplyLighting(elapsedProgress01);

        if (_showDebugLog)
        {
            Debug.Log(
                $"Night Lighting Updated - Remaining: {remainingTime:F2}, Progress: {elapsedProgress01:F2}",
                this
            );
        }
    }

    public void SetNightProgress(float progress01)
    {
        // 외부에서 직접 진행률을 넣어 테스트할 수 있게 한다.
        float clampedProgress = Mathf.Clamp01(progress01);
        ApplyLighting(clampedProgress);
    }

    private void ApplyLighting(float progress01)
    {
        // 진행률에 따라 밝기와 색을 함께 보간한다.
        float darkness01 = _darknessCurve.Evaluate(progress01);

        _globalLight.intensity = Mathf.Lerp(_startIntensity, _endIntensity, darkness01);
        _globalLight.color = Color.Lerp(_startColor, _endColor, darkness01);
    }
}