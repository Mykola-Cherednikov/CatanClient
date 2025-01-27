using UnityEngine;
using UnityEngine.Events;

public enum UIState
{
    WAITING_FOR_PLAYERS,
    PREPARATION_BUILD_SETTLEMENTS,
    PREPARATION_BUILD_ROADS,
    USER_ROBBERY,
    USER_TURN,
    USER_MOVING_ROBBER
}


public class UIManager : MonoBehaviour
{
    public GameObject uiCanvas;

    private GameObject windowUIPrefab;
    private GameObject buildingsUIPrefab;
    private GameObject gameNotificationsUIPrefab;
    private GameObject resourceBoxUIPrefab;
    private GameObject readyButtonUIPrefab;
    private GameObject userTurnTextUIPrefab;

    public BuildingsUI buildingsUI;
    public GameNotificationsUI gameNotificationsUI;
    public WindowUI windowUI;
    public ResourceBoxUI resourceBoxUI;

    public UnityEvent UPDATE_UI_EVENT;
    [SerializeField] public UIState uiState;

    private UnityEvent ESCAPE_PRESSED_EVENT;

    private void Awake()
    {
        UPDATE_UI_EVENT = new();
        ESCAPE_PRESSED_EVENT = new();
        windowUIPrefab = Resources.Load<GameObject>("Prefabs/Game/WindowUI");
        buildingsUIPrefab = Resources.Load<GameObject>("Prefabs/Game/BuildingsUI");
        gameNotificationsUIPrefab = Resources.Load<GameObject>("Prefabs/Game/GameNotificationsUI");
        resourceBoxUIPrefab = Resources.Load<GameObject>("Prefabs/Game/ResourceBoxUI");
        readyButtonUIPrefab = Resources.Load<GameObject>("Prefabs/Game/ReadyButtonUI");
        userTurnTextUIPrefab = Resources.Load<GameObject>("Prefabs/Game/UserTurnTextUI");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ESCAPE_PRESSED_EVENT?.Invoke();
        }
    }

    public void InitializeUI(GameObject uiCanvas)
    {
        this.uiCanvas = uiCanvas;
        buildingsUI = Instantiate(buildingsUIPrefab, uiCanvas.transform).GetComponent<BuildingsUI>();
        gameNotificationsUI = Instantiate(gameNotificationsUIPrefab, uiCanvas.transform).GetComponent<GameNotificationsUI>();
        resourceBoxUI = Instantiate(resourceBoxUIPrefab, uiCanvas.transform).GetComponent<ResourceBoxUI>();
        windowUI = Instantiate(windowUIPrefab, uiCanvas.transform).GetComponent<WindowUI>();
        Instantiate(readyButtonUIPrefab, uiCanvas.transform);
        Instantiate(userTurnTextUIPrefab, uiCanvas.transform);

        GameManager.Instance.CHANGED_GAME_STATE.AddListener(ChangeUIStateToGameState);
        ESCAPE_PRESSED_EVENT .AddListener( windowUI.OnEscape);
    }

    public void ClearAllElementsFromCanvas()
    {
        foreach (Transform transform in uiCanvas.transform)
        {
            Destroy(transform.gameObject);
        }
    }

    private void ChangeUIStateToGameState()
    {
        switch (GameManager.Instance.gameState)
        {
            case GameState.WAITING_FOR_PLAYERS:
                uiState = UIState.WAITING_FOR_PLAYERS;
                break;
            case GameState.PREPARATION_BUILD_SETTLEMENTS:
                uiState = UIState.PREPARATION_BUILD_SETTLEMENTS;
                break;
            case GameState.PREPARATION_BUILD_ROADS:
                uiState = UIState.PREPARATION_BUILD_ROADS;
                break;
            case GameState.ROBBERY:
                if (!GameManager.Instance.userManager.IsThisUserTurn())
                {
                    return;
                }

                uiState = UIState.USER_ROBBERY;
                break;
            case GameState.USER_TURN:
                uiState = UIState.USER_TURN;
                break;
        }
        UPDATE_UI_EVENT?.Invoke();
    }

    public void StartUserMoveRobberState()
    {
        uiState = UIState.USER_MOVING_ROBBER;
        UPDATE_UI_EVENT?.Invoke();
        ESCAPE_PRESSED_EVENT.RemoveListener(windowUI.OnEscape);
        ESCAPE_PRESSED_EVENT.AddListener(buildingsUI.CancelMoveRobber);
    }

    public void StopUserMoveRobberState()
    {
        uiState = UIState.USER_TURN;
        UPDATE_UI_EVENT?.Invoke();
        ESCAPE_PRESSED_EVENT.AddListener(windowUI.OnEscape);
        ESCAPE_PRESSED_EVENT.RemoveListener(buildingsUI.CancelMoveRobber);
    }

    private void OnDestroy()
    {
        GameManager.Instance.CHANGED_GAME_STATE.RemoveListener(ChangeUIStateToGameState);
        ESCAPE_PRESSED_EVENT.RemoveListener(windowUI.OnEscape);
        ESCAPE_PRESSED_EVENT.RemoveListener(buildingsUI.CancelMoveRobber);
    }
}
