using UnityEngine;

public class GameResultMessageUI : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private EscapeZone _escapeZone;
    [SerializeField] private float _blockedEscapeMessageDuration = 1.5f;

    private float _blockedEscapeMessageTimer;

    private GUIStyle _centerMessageStyle;
    private GUIStyle _subMessageStyle;
    private GUIStyle _shadowStyle;

    private void Start()
    {
        // 외부 참조를 검증한다.
        if (_gameManager == null)
        {
            Debug.LogError($"{nameof(GameResultMessageUI)}: GameManager가 할당되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (_escapeZone == null)
        {
            Debug.LogError($"{nameof(GameResultMessageUI)}: EscapeZone이 할당되지 않았습니다.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        UpdateBlockedEscapeMessageTimer();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 탈출구에 닿았지만 아직 탈출 불가능할 때만 메시지를 띄운다.
        if (_gameManager == null || _escapeZone == null)
        {
            return;
        }

        if (_gameManager.IsGameEnded)
        {
            return;
        }

        if (!IsPlayerCollider(other))
        {
            return;
        }

        if (_escapeZone.CanEscape)
        {
            return;
        }

        _blockedEscapeMessageTimer = _blockedEscapeMessageDuration;
    }

    private void OnGUI()
    {
        if (_centerMessageStyle == null || _subMessageStyle == null || _shadowStyle == null)
        {
            CreateGuiStyles();
        }

        if (_gameManager == null)
        {
            return;
        }

        DrawResultMessage();
        DrawBlockedEscapeMessage();
    }

    private void UpdateBlockedEscapeMessageTimer()
    {
        if (_blockedEscapeMessageTimer <= 0f)
        {
            return;
        }

        _blockedEscapeMessageTimer -= Time.deltaTime;

        if (_blockedEscapeMessageTimer < 0f)
        {
            _blockedEscapeMessageTimer = 0f;
        }
    }

    private void DrawResultMessage()
    {
        // 성공/실패 메시지를 화면 중앙에 크게 출력한다.
        if (_gameManager.IsSucceeded)
        {
            DrawCenteredMessage("SUCCESS", "탈출에 성공했습니다.");
            return;
        }

        if (_gameManager.IsFailed)
        {
            string failSubMessage = GetFailSubMessage(_gameManager.CurrentFailReason);
            DrawCenteredMessage("FAILED", failSubMessage);
        }
    }

    private void DrawBlockedEscapeMessage()
    {
        // 탈출 불가 메시지를 화면 중앙에 잠깐 출력한다.
        if (_blockedEscapeMessageTimer <= 0f)
        {
            return;
        }

        DrawCenteredMessage("ESCAPE BLOCKED", "아직 충분히 수확하지 못했습니다.");
    }

    private void DrawCenteredMessage(string mainMessage, string subMessage)
    {
        float centerX = Screen.width * 0.5f;
        float centerY = Screen.height * 0.5f;

        Rect mainRect = new Rect(centerX - 300f, centerY - 60f, 600f, 80f);
        Rect subRect = new Rect(centerX - 300f, centerY + 10f, 600f, 40f);

        Rect mainShadowRect = new Rect(mainRect.x + 2f, mainRect.y + 2f, mainRect.width, mainRect.height);
        Rect subShadowRect = new Rect(subRect.x + 2f, subRect.y + 2f, subRect.width, subRect.height);

        GUI.Label(mainShadowRect, mainMessage, _shadowStyle);
        GUI.Label(subShadowRect, subMessage, _shadowStyle);

        GUI.Label(mainRect, mainMessage, _centerMessageStyle);
        GUI.Label(subRect, subMessage, _subMessageStyle);
    }

    private string GetFailSubMessage(FailReason failReason)
    {
        switch (failReason)
        {
            case FailReason.TimeOut:
                return "밤이 끝나기 전에 탈출하지 못했습니다.";

            case FailReason.CaughtByGuard:
                return "감시자에게 붙잡혔습니다.";

            default:
                return "실패했습니다.";
        }
    }

    private bool IsPlayerCollider(Collider2D other)
    {
        // 플레이어 레이어 이름을 기준으로 간단히 판정한다.
        return other.gameObject.layer == LayerMask.NameToLayer("Player");
    }

    private void CreateGuiStyles()
    {
        _centerMessageStyle = new GUIStyle(GUI.skin.label);
        _centerMessageStyle.fontSize = 40;
        _centerMessageStyle.alignment = TextAnchor.MiddleCenter;
        _centerMessageStyle.normal.textColor = Color.white;
        _centerMessageStyle.fontStyle = FontStyle.Bold;

        _subMessageStyle = new GUIStyle(GUI.skin.label);
        _subMessageStyle.fontSize = 22;
        _subMessageStyle.alignment = TextAnchor.MiddleCenter;
        _subMessageStyle.normal.textColor = Color.white;

        _shadowStyle = new GUIStyle(GUI.skin.label);
        _shadowStyle.fontSize = 40;
        _shadowStyle.alignment = TextAnchor.MiddleCenter;
        _shadowStyle.normal.textColor = Color.black;
        _shadowStyle.fontStyle = FontStyle.Bold;
    }
}