using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NightLightingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light2D _globalLight;

    [Header("Time")]
    [SerializeField] private float _nightDuration = 180f;

    [Header("Light")]
    [SerializeField] private float _startIntensity = 0.85f;
    [SerializeField] private float _endIntensity = 0.45f;

    [SerializeField] private Color _startColor = new Color(0.75f, 0.8f, 1f, 1f);
    [SerializeField] private Color _endColor = new Color(0.2f, 0.24f, 0.35f, 1f);

    [Header("Curve")]
    [SerializeField] private AnimationCurve _darknessCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Debug")]
    [SerializeField] private bool _playOnStart = true;
    [SerializeField] private bool _showDebugLog = false;

    private float _elapsedTime;
    private bool _isRunning;

    public float ElapsedTime => _elapsedTime;
    public float NightProgress01 => _nightDuration > 0f ? Mathf.Clamp01(_elapsedTime / _nightDuration) : 1f;

    private void Start()
    {
        // 외부 참조를 검증한다.
        if (_globalLight == null)
        {
            Debug.LogError($"{nameof(NightLightingController)}: Global Light 2D가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_nightDuration <= 0f)
        {
            Debug.LogError($"{nameof(NightLightingController)}: Night Duration은 0보다 커야 합니다.", this);
            enabled = false;
            return;
        }

        _elapsedTime = 0f;
        _isRunning = _playOnStart;

        ApplyLighting(0f);
    }

    private void Update()
    {
        if (!_isRunning)
        {
            return;
        }

        _elapsedTime += Time.deltaTime;

        float progress01 = NightProgress01;
        ApplyLighting(progress01);

        if (_showDebugLog)
        {
            Debug.Log($"Night Progress: {progress01:F2}", this);
        }
    }

    public void SetNightProgress(float progress01)
    {
        // 외부 시스템에서 밤 진행도를 직접 설정할 수 있게 한다.
        float clampedProgress = Mathf.Clamp01(progress01);
        _elapsedTime = clampedProgress * _nightDuration;
        ApplyLighting(clampedProgress);
    }

    public void StartNight()
    {
        // 밤 진행을 시작한다.
        _isRunning = true;
    }

    public void StopNight()
    {
        // 밤 진행을 멈춘다.
        _isRunning = false;
    }

    private void ApplyLighting(float progress01)
    {
        // 커브를 적용한 어둠 보간값으로 조명 색과 밝기를 갱신한다.
        float darkness01 = _darknessCurve.Evaluate(progress01);

        _globalLight.intensity = Mathf.Lerp(_startIntensity, _endIntensity, darkness01);
        _globalLight.color = Color.Lerp(_startColor, _endColor, darkness01);
    }
}