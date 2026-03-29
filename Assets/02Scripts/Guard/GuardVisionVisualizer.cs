using UnityEngine;

[RequireComponent(typeof(GuardController))]
public class GuardVisionVisualizer : MonoBehaviour
{
    [SerializeField] private GuardDataSO _guardData;
    [SerializeField] private Transform _visionVisualRoot;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;

    [Header("Flashlight Shape")]
    [SerializeField] private int _segmentCount = 20;
    [SerializeField] private float _innerRadius = 0.2f;

    [Header("Rendering")]
    [SerializeField] private string _sortingLayerName = "Default";
    [SerializeField] private int _sortingOrder = 2;
    [SerializeField] private Color _visionColor = new Color(1f, 0.96f, 0.008f, 0.28f);

    [SerializeField] private bool _showDebugLog = false;

    private GuardController _guardController;
    private Mesh _visionMesh;
    private MaterialPropertyBlock _propertyBlock;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private void Awake()
    {
        _guardController = GetComponent<GuardController>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        if (_guardData == null)
        {
            Debug.LogError($"{nameof(GuardVisionVisualizer)}: GuardDataSO가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_visionVisualRoot == null)
        {
            Debug.LogError($"{nameof(GuardVisionVisualizer)}: Vision Visual Root가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_meshFilter == null)
        {
            Debug.LogError($"{nameof(GuardVisionVisualizer)}: MeshFilter가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_meshRenderer == null)
        {
            Debug.LogError($"{nameof(GuardVisionVisualizer)}: MeshRenderer가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_segmentCount < 2)
        {
            _segmentCount = 2;
        }

        if (_innerRadius < 0f)
        {
            _innerRadius = 0f;
        }

        CreateVisionMesh();
        UpdateVisionMesh();
        UpdateVisionRotation();
        ApplyRendererSettings();
    }

    private void LateUpdate()
    {
        UpdateVisionMesh();
        UpdateVisionRotation();
    }

    private void CreateVisionMesh()
    {
        _visionMesh = new Mesh();
        _visionMesh.name = "GuardFlashlightVisionMesh";
        _meshFilter.mesh = _visionMesh;

        if (_showDebugLog)
        {
            Debug.Log("Guard flashlight vision mesh created.", this);
        }
    }

    private void UpdateVisionMesh()
    {
        if (_visionMesh == null)
        {
            return;
        }

        float outerRadius = _guardData.ViewDistance;
        float angle = _guardData.ViewAngle;
        float clampedInnerRadius = Mathf.Clamp(_innerRadius, 0f, outerRadius);

        int ringPointCount = _segmentCount + 1;
        int vertexCount = ringPointCount * 2;

        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount];
        int[] triangles = new int[_segmentCount * 6];

        float startAngle = -angle * 0.5f;
        float angleStep = angle / _segmentCount;

        for (int i = 0; i <= _segmentCount; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            float radians = currentAngle * Mathf.Deg2Rad;

            Vector2 direction = new Vector2(Mathf.Sin(radians), Mathf.Cos(radians));

            int innerIndex = i * 2;
            int outerIndex = innerIndex + 1;

            vertices[innerIndex] = direction * clampedInnerRadius;
            vertices[outerIndex] = direction * outerRadius;

            float normalizedT = (float)i / _segmentCount;
            uvs[innerIndex] = new Vector2(normalizedT, 0f);
            uvs[outerIndex] = new Vector2(normalizedT, 1f);
        }

        for (int i = 0; i < _segmentCount; i++)
        {
            int triangleIndex = i * 6;

            int currentInner = i * 2;
            int currentOuter = currentInner + 1;
            int nextInner = currentInner + 2;
            int nextOuter = currentInner + 3;

            triangles[triangleIndex] = currentInner;
            triangles[triangleIndex + 1] = currentOuter;
            triangles[triangleIndex + 2] = nextOuter;

            triangles[triangleIndex + 3] = currentInner;
            triangles[triangleIndex + 4] = nextOuter;
            triangles[triangleIndex + 5] = nextInner;
        }

        _visionMesh.Clear();
        _visionMesh.vertices = vertices;
        _visionMesh.triangles = triangles;
        _visionMesh.uv = uvs;
        _visionMesh.RecalculateBounds();
        _visionMesh.RecalculateNormals();
    }

    private void UpdateVisionRotation()
    {
        Vector2 facingDirection = _guardController.FacingDirection;

        if (facingDirection.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        float angle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg;
        _visionVisualRoot.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    private void ApplyRendererSettings()
    {
        _meshRenderer.sortingLayerName = _sortingLayerName;
        _meshRenderer.sortingOrder = _sortingOrder;

        _meshRenderer.GetPropertyBlock(_propertyBlock);

        // URP Unlit는 _BaseColor, Built-in Unlit은 _Color를 주로 사용한다.
        _propertyBlock.SetColor(BaseColorId, _visionColor);
        _propertyBlock.SetColor(ColorId, _visionColor);

        _meshRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void OnValidate()
    {
        if (_segmentCount < 2)
        {
            _segmentCount = 2;
        }

        if (_innerRadius < 0f)
        {
            _innerRadius = 0f;
        }
    }
}