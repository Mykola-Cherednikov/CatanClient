using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    WAITING_FOR_PLAYERS,
    PREPARATION_BUILD_SETTLEMENTS,
    PREPARATION_BUILD_ROADS,
    GAME
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject lobbiesFormPrefab;

    private GameObject cameraGO;

    public UIManager uiManager;
    public MapManager mapManager;
    public UserManager userManager;

    public GameState gameState;

    public int numOfTurn;

    private Dictionary<EventType, GameState> userTurnToGameStates;

    private void Awake()
    {
        Instance = this;

        userTurnToGameStates = new Dictionary<EventType, GameState>() { 
            { EventType.BROADCAST_PREPARATION_USER_TURN_BUILD_ROADS, GameState.PREPARATION_BUILD_ROADS },
            { EventType.BROADCAST_PREPARATION_USER_TURN_BUILD_SETTLEMENTS, GameState.PREPARATION_BUILD_SETTLEMENTS },
            { EventType.BROADCAST_USER_TURN, GameState.GAME }
        };

        Multiplayer.Instance.CONNECTION_ERROR_EVENT.AddListener(OnConnectionError);
        Multiplayer.Instance.BROADCAST_USER_TURN_EVENT.AddListener(OnUserTurn);
        Multiplayer.Instance.BROADCAST_BUILD_ROAD_EVENT.AddListener(OnBuildRoad);
        Multiplayer.Instance.BROADCAST_BUILD_SETTLEMENT_EVENT.AddListener(OnBuildSettlement);
        Multiplayer.Instance.BROADCAST_BUILD_CITY_EVENT.AddListener(OnBuildCity);
        cameraGO = Camera.main.gameObject;
        cameraGO.transform.position = new Vector3(0f, 0f, -10f);

        uiManager = gameObject.AddComponent<UIManager>();
        mapManager = gameObject.AddComponent<MapManager>();
        userManager = gameObject.AddComponent<UserManager>();
    }

    public async void StartGame(SocketBroadcastStartGameDTO dto, GameObject canvas)
    {
        mapManager.InitializeMap(dto);
        userManager.InitializeUsers(dto);
        uiManager.InitializeUI(canvas);

        await Multiplayer.Instance.SocketSendReadyAndLoadRequest();
    }

    private void OnConnectionError(object dtoObject)
    {
        GoToLobbiesForm();
    }

    private void GoToLobbiesForm()
    {
        uiManager.ClearAllElementsFromCanvas();
        Instantiate(lobbiesFormPrefab, uiManager.uiCanvas.transform);
        Destroy(gameObject);
    }

    private void OnUserTurn(object dtoObject)
    {
        SocketBroadcastUserTurnDTO dto = (SocketBroadcastUserTurnDTO)dtoObject;

        SetGameStateOnSpecificUserTurn(dto.eventType);

        User userWhoseTurn = userManager.GetUserById(dto.userId);
        numOfTurn = dto.numOfTurn;

        uiManager.DisplayUserTurnText(userWhoseTurn);
        userManager.UpdateUserTurnStatus(userWhoseTurn);
    }

    private void SetGameStateOnSpecificUserTurn(string stringEventType)
    {
        EventType eventType = (EventType)Enum.Parse(typeof(EventType), stringEventType);
        gameState = userTurnToGameStates[eventType];
    }

    private void OnBuildRoad(object dtoObject)
    {
        SocketBroadcastBuildDTO dto = (SocketBroadcastBuildDTO)dtoObject;
        mapManager.BuildRoad(dto.fieldId, dto.userId);
    }

    private void OnBuildSettlement(object dtoObject)
    {
        SocketBroadcastBuildDTO dto = (SocketBroadcastBuildDTO)dtoObject;
        mapManager.BuildSettlement(dto.fieldId, dto.userId);
    }

    private void OnBuildCity(object dtoObject)
    {
        SocketBroadcastBuildDTO dto = (SocketBroadcastBuildDTO)dtoObject;
        mapManager.BuildCity(dto.fieldId, dto.userId);
    }
}
