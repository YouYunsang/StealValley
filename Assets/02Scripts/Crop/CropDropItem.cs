using System;
using DG.Tweening;
using UnityEngine;

public class CropDropItem : MonoBehaviour
{
    private enum DropState
    {
        None,
        SpawnAnimating,
        Dropped,
        Absorbing,
        Collected,
        Expired
    }

    [Header("References")]
    [SerializeField] private CropDropItemDataSO _dropItemData;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Transform _visualRoot;

    [Header("Debug")]
    [SerializeField] private bool _showDebugGizmo = true;

    private DropState _currentState = DropState.None;

    private Transform _playerTransform;
    private Action _onCollected;

    private Tween _spawnTween;
    private Tween _bounceTween;
    private Tween _absorbTween;
    private Tween _expireTween;

    private Vector3 _spawnStartPosition;
    private Vector3 _dropTargetPosition;
    private Vector3 _initialScale;

    private float _lifeTimer;
    private float _pickupDelayTimer;

    public bool IsDropped => _currentState == DropState.Dropped;

    private void Awake()
    {
        // 같은 오브젝트 내부 참조를 캐싱한다.
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (_visualRoot == null)
        {
            _visualRoot = transform;
        }

        _initialScale = _visualRoot.localScale;
    }

    private void Start()
    {
        // 외부 데이터 참조를 검증한다.
        if (_dropItemData == null)
        {
            Debug.LogError($"{nameof(CropDropItem)}: DropItemData가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        UpdateDroppedState();
    }

    private void OnDisable()
    {
        // 비활성화 시 진행 중인 트윈을 정리한다.
        KillAllTweens();
    }

    public void Initialize(Transform playerTransform, Action onCollected)
    {
        // 드롭 아이템이 흡수할 대상을 저장하고 수집 완료 콜백을 등록한다.
        _playerTransform = playerTransform;
        _onCollected = onCollected;
    }

    public void SetPickupDelay(float pickupDelay)
    {
        // 드롭된 뒤 자동 흡수되기까지의 지연 시간을 설정한다.
        _pickupDelayTimer = Mathf.Max(0f, pickupDelay);
    }

    public void PlayDrop(Vector3 worldPosition)
    {
        // 월드 위치에서 드롭 연출을 시작한다.
        _spawnStartPosition = worldPosition;

        float randomOffsetX = UnityEngine.Random.Range(
            -_dropItemData.SpawnRandomOffsetX,
            _dropItemData.SpawnRandomOffsetX
        );

        _dropTargetPosition = worldPosition + new Vector3(randomOffsetX, 0f, 0f);

        transform.position = _spawnStartPosition;
        _visualRoot.localScale = _initialScale;
        SetAlpha(1f);

        _lifeTimer = _dropItemData.LifeTime;
        _currentState = DropState.SpawnAnimating;

        PlaySpawnAnimation();
    }

    private void UpdateDroppedState()
    {
        // 드롭 대기 상태가 아니면 흡수/만료 판정을 하지 않는다.
        if (_currentState != DropState.Dropped)
        {
            return;
        }

        _lifeTimer -= Time.deltaTime;

        if (_lifeTimer <= 0f)
        {
            Expire();
            return;
        }

        // 재획득 불가 시간이 남아 있으면 아직 흡수하지 않는다.
        if (_pickupDelayTimer > 0f)
        {
            _pickupDelayTimer -= Time.deltaTime;
            return;
        }

        if (_playerTransform == null)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, _playerTransform.position);

        if (distance > _dropItemData.AbsorbRange)
        {
            return;
        }

        StartAbsorb();
    }

    private void PlaySpawnAnimation()
    {
        // 아이템이 살짝 위로 튀었다가 바닥에 떨어지는 연출을 재생한다.
        KillAllTweens();

        Vector3 peakPosition = _spawnStartPosition + new Vector3(
            (_dropTargetPosition.x - _spawnStartPosition.x) * 0.5f,
            _dropItemData.SpawnHeight,
            0f
        );

        Sequence spawnSequence = DOTween.Sequence();

        spawnSequence.Append(
            transform.DOMove(peakPosition, _dropItemData.SpawnDuration * 0.5f).SetEase(Ease.OutQuad)
        );

        spawnSequence.Append(
            transform.DOMove(_dropTargetPosition, _dropItemData.SpawnDuration * 0.5f).SetEase(Ease.InQuad)
        );

        spawnSequence.OnComplete(HandleSpawnAnimationCompleted);

        _spawnTween = spawnSequence;

        // 아이템이 튀어나올 때 살짝 커졌다가 돌아오게 해 탄력을 준다.
        _bounceTween = _visualRoot.DOScale(
            _initialScale * _dropItemData.LandBounceScale,
            _dropItemData.LandBounceDuration
        ).SetLoops(2, LoopType.Yoyo);
    }

    private void HandleSpawnAnimationCompleted()
    {
        // 스폰 연출이 끝났다면 이제 바닥에 드롭된 상태로 전환한다.
        _currentState = DropState.Dropped;
        transform.position = _dropTargetPosition;
        _visualRoot.localScale = _initialScale;
    }

    private void StartAbsorb()
    {
        // 흡수 중복 시작을 방지한다.
        if (_currentState != DropState.Dropped)
        {
            return;
        }

        if (_playerTransform == null)
        {
            return;
        }

        _currentState = DropState.Absorbing;

        KillAllTweens();

        Sequence absorbSequence = DOTween.Sequence();

        absorbSequence.Append(
            transform.DOMove(_playerTransform.position, _dropItemData.AbsorbDuration)
                .SetEase(Ease.InQuad)
        );

        absorbSequence.Join(
            _visualRoot.DOScale(_initialScale * _dropItemData.AbsorbScale, _dropItemData.AbsorbDuration)
                .SetEase(Ease.InQuad)
        );

        absorbSequence.OnComplete(Collect);

        _absorbTween = absorbSequence;
    }

    private void Collect()
    {
        // 실제 획득 처리를 호출하고 아이템을 제거한다.
        if (_currentState == DropState.Collected)
        {
            return;
        }

        _currentState = DropState.Collected;
        _onCollected?.Invoke();
        Destroy(gameObject);
    }

    private void Expire()
    {
        // 제한 시간이 지나면 페이드 아웃 후 아이템을 제거한다.
        if (_currentState == DropState.Expired || _currentState == DropState.Collected)
        {
            return;
        }

        _currentState = DropState.Expired;

        KillAllTweens();

        _expireTween = DOVirtual.Float(
            1f,
            0f,
            _dropItemData.ExpireFadeDuration,
            SetAlpha
        ).OnComplete(() => Destroy(gameObject));
    }

    private void SetAlpha(float alpha)
    {
        // 스프라이트 알파를 조절해 만료 페이드 연출에 사용한다.
        if (_spriteRenderer == null)
        {
            return;
        }

        Color color = _spriteRenderer.color;
        color.a = alpha;
        _spriteRenderer.color = color;
    }

    private void KillAllTweens()
    {
        // 드롭 아이템에 걸린 모든 트윈을 종료한다.
        KillTween(ref _spawnTween);
        KillTween(ref _bounceTween);
        KillTween(ref _absorbTween);
        KillTween(ref _expireTween);
    }

    private void KillTween(ref Tween targetTween)
    {
        // 유효한 트윈만 종료하고 참조를 비운다.
        if (targetTween != null && targetTween.IsActive())
        {
            targetTween.Kill();
        }

        targetTween = null;
    }

    private void OnDrawGizmosSelected()
    {
        // 드롭 아이템의 자동 흡수 반경을 에디터에서 시각화한다.
        if (!_showDebugGizmo || _dropItemData == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _dropItemData.AbsorbRange);
    }
}