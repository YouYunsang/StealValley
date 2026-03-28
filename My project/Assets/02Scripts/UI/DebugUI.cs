using UnityEngine;

public class DebugUI : MonoBehaviour
{
    [SerializeField] private FarmProgressManager _farmProgressManager;
    [SerializeField] private PlayerHarvest _playerHarvest;
    [SerializeField] private PlayerNoiseEmitter _noiseEmitter;
    [SerializeField] private NightTimer _nightTimer;

    private GUIStyle _labelStyle;

    private void Awake()
    {
        _labelStyle = new GUIStyle();
        _labelStyle.fontSize = 30;
        _labelStyle.normal.textColor = Color.white;
    }

    private void OnGUI()
    {
        int y = 10;

        DrawLabel(ref y, $"=== FARM PROGRESS ===");

        if (_farmProgressManager != null)
        {
            DrawLabel(ref y,
                $"Progress: {_farmProgressManager.CurrentProgress:F1} / {_farmProgressManager.TargetProgress}");

            DrawLabel(ref y,
                $"Normalized: {_farmProgressManager.NormalizedProgress:P1}");

            DrawLabel(ref y,
                $"Completed: {_farmProgressManager.IsCompleted}");
        }

        DrawLabel(ref y, $"");

        DrawLabel(ref y, $"=== HARVEST ===");

        if (_playerHarvest != null)
        {
            if (_playerHarvest.CurrentTargetCrop != null)
            {
                DrawLabel(ref y,
                    $"Target: {_playerHarvest.CurrentTargetCrop.GetCropName()}");

                DrawLabel(ref y,
                    $"Time: {_playerHarvest.CurrentHarvestTime:F2} / {_playerHarvest.CurrentTargetHarvestDuration:F2}");

                DrawLabel(ref y,
                    $"Normalized: {_playerHarvest.CurrentHarvestNormalized:P1}");

                DrawLabel(ref y,
                    $"Is Harvesting: {_playerHarvest.IsHarvesting}");
            }
            else
            {
                DrawLabel(ref y, "Target: None");
            }
        }

        DrawLabel(ref y, $"");

        DrawLabel(ref y, $"=== NOISE ===");

        if (_noiseEmitter != null)
        {
            DrawLabel(ref y,
                $"State: {_noiseEmitter.CurrentNoiseState}");

            DrawLabel(ref y,
                $"Radius: {_noiseEmitter.CurrentNoiseRadius:F2}");
        }

        DrawLabel(ref y, $"=== NIGHT TIMER ===");

        if (_nightTimer != null)
        {
            DrawLabel(ref y, $"Remaining: {_nightTimer.RemainingTime:F1}");
            DrawLabel(ref y, $"Normalized: {_nightTimer.NormalizedRemainingTime:P1}");
            DrawLabel(ref y, $"Running: {_nightTimer.IsRunning}");
            DrawLabel(ref y, $"Expired: {_nightTimer.IsExpired}");
        }
    }

    private void DrawLabel(ref int y, string text)
    {
        GUI.Label(new Rect(10, y, 400, 30), text, _labelStyle);
        y += 30;
    }
}