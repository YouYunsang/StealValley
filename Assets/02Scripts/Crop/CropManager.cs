using System.Collections.Generic;
using UnityEngine;


public class CropManager : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private Grid _grid;
    [SerializeField] private Transform _cropRoot;

    [Header("Drop")]
    [SerializeField] private CropDropManager _cropDropManager;

    private readonly Dictionary<Vector3Int, Crop> _cropByCell = new Dictionary<Vector3Int, Crop>();

    public Grid Grid => _grid;

    private void Start()
    {
        // 외부 참조를 검증한다.
        if (_grid == null)
        {
            Debug.LogError($"{nameof(CropManager)}: Grid가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_cropRoot == null)
        {
            Debug.LogError($"{nameof(CropManager)}: Crop Root가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_cropDropManager == null)
        {
            Debug.LogError($"{nameof(CropManager)}: CropDropManager가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        BuildCropMap();
    }

    private void OnDisable()
    {
        foreach(KeyValuePair<Vector3Int, Crop> pair in _cropByCell)
        {
            if(pair.Value == null)
            {
                continue;
            }

            pair.Value.OnHarvested -= HandleCropHarvested;
        }
    }

    public bool TryGetCropAtCell(Vector3Int cellPosition, out Crop crop)
    {
        // 해당 셀에 수확 가능한 작물이 있는지 반환한다.
        if (_cropByCell.TryGetValue(cellPosition, out Crop foundCrop) && foundCrop != null && foundCrop.CanHarvest)
        {
            crop = foundCrop;
            return true;
        }

        crop = null;
        return false;
    }

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        // 월드 좌표를 셀 좌표로 변환한다.
        return _grid.WorldToCell(worldPosition);
    }

    private void BuildCropMap()
    {
        // 씬에 배치된 Crop들을 셀 좌표 기준으로 등록한다.
        _cropByCell.Clear();

        Crop[] crops = _cropRoot.GetComponentsInChildren<Crop>(true);

        for (int i = 0; i < crops.Length; i++)
        {
            RegisterCrop(crops[i]);
        }
    }

    private void RegisterCrop(Crop crop)
    {
        // Crop의 월드 위치를 셀 좌표로 변환해 딕셔너리에 등록한다.
        if (crop == null)
        {
            return;
        }

        Vector3Int cellPosition = _grid.WorldToCell(crop.transform.position);

        if (_cropByCell.ContainsKey(cellPosition))
        {
            Debug.LogWarning(
                $"{nameof(CropManager)}: 동일한 셀에 Crop이 중복 등록되었습니다. Cell: {cellPosition}",
                crop
            );
            return;
        }

        _cropByCell.Add(cellPosition, crop);

        crop.OnHarvested += HandleCropHarvested;
    }

    private void HandleCropHarvested(Crop harvestedCrop)
    {
        if (harvestedCrop == null) return;

        _cropDropManager.SpawnHarvestDrop(
            harvestedCrop.CropData,
            harvestedCrop.DropSpawnPoint.position
        );
    }
}