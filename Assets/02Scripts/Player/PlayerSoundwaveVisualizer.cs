using UnityEngine;

[RequireComponent(typeof(PlayerNoiseEmitter))]
public class PlayerSoundwaveVisualizer : MonoBehaviour
{
    [SerializeField] private ParticleSystem _soundwaveParticle;
    [SerializeField] private float _radiusToStartSizeMultiplier = 5f;
    [SerializeField] private bool _stopWhenRadiusZero = true;
    [SerializeField] private bool _showDebugLog = false;

    private PlayerNoiseEmitter _noiseEmitter;
    private ParticleSystem.MainModule _mainModule;

    private float _lastAppliedStartSize = -1f;
    private PlayerNoiseState _lastAppliedNoiseState = PlayerNoiseState.Idle;

    private void Awake()
    {
        // 같은 오브젝트 내부 컴포넌트를 캐싱한다.
        _noiseEmitter = GetComponent<PlayerNoiseEmitter>();

        if (_soundwaveParticle != null)
        {
            _mainModule = _soundwaveParticle.main;
        }
    }

    private void Start()
    {
        // 외부 참조를 검증한다.
        if (_soundwaveParticle == null)
        {
            Debug.LogError($"{nameof(PlayerSoundwaveVisualizer)}: Soundwave Particle이 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        ApplyNoiseVisual(true);
    }

    private void Update()
    {
        ApplyNoiseVisual(false);
    }

    private void ApplyNoiseVisual(bool forceUpdate)
    {
        // 현재 noise 반경과 상태를 읽는다.
        float currentRadius = _noiseEmitter.CurrentNoiseRadius;
        PlayerNoiseState currentNoiseState = _noiseEmitter.CurrentNoiseState;

        // 반경이 0 이하이면 이펙트를 정지한다.
        if (currentRadius <= 0f)
        {
            if (_stopWhenRadiusZero && _soundwaveParticle.isPlaying)
            {
                _soundwaveParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            _lastAppliedStartSize = 0f;
            _lastAppliedNoiseState = currentNoiseState;
            return;
        }

        // radius를 start size로 변환한다.
        float targetStartSize = currentRadius * _radiusToStartSizeMultiplier;

        // 값이 같으면 불필요한 갱신을 막는다.
        bool isSameStartSize = Mathf.Approximately(_lastAppliedStartSize, targetStartSize);
        bool isSameNoiseState = _lastAppliedNoiseState == currentNoiseState;

        if (!forceUpdate && isSameStartSize && isSameNoiseState)
        {
            if (!_soundwaveParticle.isPlaying)
            {
                _soundwaveParticle.Play();
            }

            return;
        }

        // 파티클 start size를 갱신한다.
        _mainModule.startSize = targetStartSize;

        // 정지 상태면 다시 재생한다.
        if (!_soundwaveParticle.isPlaying)
        {
            _soundwaveParticle.Play();
        }

        _lastAppliedStartSize = targetStartSize;
        _lastAppliedNoiseState = currentNoiseState;

        if (_showDebugLog)
        {
            Debug.Log(
                $"Soundwave Updated - NoiseState: {currentNoiseState}, Radius: {currentRadius:F2}, StartSize: {targetStartSize:F2}",
                this
            );
        }
    }
}