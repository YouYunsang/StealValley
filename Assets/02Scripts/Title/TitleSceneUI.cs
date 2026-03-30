using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneUI : MonoBehaviour
{
    private const string DEFAULT_GAME_SCENE_NAME = "SampleScene";

    [Header("Scene")]
    [SerializeField] private string _gameSceneName = DEFAULT_GAME_SCENE_NAME;

    [Header("Debug")]
    [SerializeField] private bool _showDebugLog = true;

    public void OnClickStartGame()
    {
        // 씬 이름이 비어 있으면 잘못된 설정이므로 로드를 중단한다.
        if (string.IsNullOrWhiteSpace(_gameSceneName))
        {
            Debug.LogError($"{nameof(TitleSceneUI)}: 게임 씬 이름이 비어 있습니다.", this);
            return;
        }

        if (_showDebugLog)
        {
            Debug.Log($"Load Game Scene - Scene Name: {_gameSceneName}", this);
        }

        SceneManager.LoadScene(_gameSceneName);
    }
}