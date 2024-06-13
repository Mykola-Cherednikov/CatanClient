using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    WAITING_FOR_PLAYERS,
    PREPARATION_BUILD_SETTLEMENTS,
    PREPARATION_BUILD_ROADS,
    ROBBERY,
    GAME
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject lobbiesFormPrefab;
    //[SerializeField] private GameObject errorFormPrefab;

    private GameObject cameraGO;

    public UIManager uiManager;
    public MapManager mapManager;
    public UserManager userManager;

    public GameState gameState;

    public int numOfTurn;

    private Dictionary<EventType, GameState> eventTypeToGameStates;

    private void Awake()
    {
        Instance = this;

        eventTypeToGameStates = new Dictionary<EventType, GameState>() { 
            { EventType.BROADCAST_PREPARATION_USER_TURN_BUILD_ROADS, GameState.PREPARATION_BUILD_ROADS },
            { EventType.BROADCAST_PREPARATION_USER_TURN_BUILD_SETTLEMENTS, GameState.PREPARATION_BUILD_SETTLEMENTS },
            { EventType.BROADCAST_USER_TURN, GameState.GAME },
            { EventType.BROADCAST_ROBBERY_START, GameState.ROBBERY },
            { EventType.BROADCAST_ROBBERY_END, GameState.GAME }
        };

        Multiplayer.Instance.CONNECTION_ERROR_EVENT.AddListener(OnConnectionError);
        Multiplayer.Instance.BROADCAST_USER_TURN_EVENT.AddListener(OnUserTurn);
        Multiplayer.Instance.BROADCAST_BUILD_ROAD_EVENT.AddListener(OnBuildRoad);
        Multiplayer.Instance.BROADCAST_BUILD_SETTLEMENT_EVENT.AddListener(OnBuildSettlement);
        Multiplayer.Instance.BROADCAST_BUILD_CITY_EVENT.AddListener(OnBuildCity);
        Multiplayer.Instance.BROADCAST_DICE_THROW_EVENT.AddListener(OnDiceThrow);
        Multiplayer.Instance.BROADCAST_GET_RESOURCE_EVENT.AddListener(OnResourceGet);
        Multiplayer.Instance.BROADCAST_USER_TRADE_EVENT.AddListener(OnTrade);
        Multiplayer.Instance.BROADCAST_ROBBERY_START_EVENT.AddListener(OnRobberyStartOrEnd);
        Multiplayer.Instance.BROADCAST_ROBBERY_END_EVENT.AddListener(OnRobberyStartOrEnd);
        Multiplayer.Instance.BROADCAST_ROBBER_ROBBERY_EVENT.AddListener(OnRobberRobbery);
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
        /*ErrorForm errorForm = Instantiate(errorFormPrefab, uiManager.uiCanvas.transform).GetComponent<ErrorForm>();
        errorForm.SetErrorText("TOLYA FIX SERVER!");*/
        Destroy(gameObject);
    }

    private void OnUserTurn(object dtoObject)
    {
        SocketBroadcastUserTurnDTO dto = (SocketBroadcastUserTurnDTO)dtoObject;

        SetGameStateOnSpecificEventType(dto.eventType);

        User userWhoseTurn = userManager.GetUserById(dto.userId);
        numOfTurn = dto.numOfTurn;

        uiManager.DisplayUserTurnText(userWhoseTurn);
        userManager.UpdateUserTurnStatus(userWhoseTurn);
    }

    private void SetGameStateOnSpecificEventType(string stringEventType)
    {
        EventType eventType = (EventType)Enum.Parse(typeof(EventType), stringEventType);
        gameState = eventTypeToGameStates[eventType];
        uiManager.ChangeUIToGameState();
    }

    private void OnBuildRoad(object dtoObject)
    {
        SocketBroadcastBuildDTO dto = (SocketBroadcastBuildDTO)dtoObject;
        mapManager.BuildRoad(dto.fieldId, userManager.GetUserById(dto.userId));
    }

    private void OnBuildSettlement(object dtoObject)
    {
        SocketBroadcastBuildDTO dto = (SocketBroadcastBuildDTO)dtoObject;
        mapManager.BuildSettlement(dto.fieldId, userManager.GetUserById(dto.userId));
    }

    private void OnBuildCity(object dtoObject)
    {
        SocketBroadcastBuildDTO dto = (SocketBroadcastBuildDTO)dtoObject;
        mapManager.BuildCity(dto.fieldId, userManager.GetUserById(dto.userId));
    }

    private void OnDiceThrow(object dtoObject)
    {
        SocketBroadcastDiceThrowDTO dto = (SocketBroadcastDiceThrowDTO)dtoObject;
        uiManager.DisplayUserDiceThrow(userManager.GetUserById(dto.userId), dto.firstDiceNum + dto.secondDiceNum);
    }

    private void OnResourceGet(object dtoObject)
    {
        SocketBroadcastResourcesDTO dto = (SocketBroadcastResourcesDTO)dtoObject;
        Dictionary<Resource, int> resourcesToAmount = SimixmanUtils.ResourceListToResourceDictionary(dto.resources);
        userManager.AddResourcesToUserAsGathering(userManager.GetUserById(dto.userId), resourcesToAmount);
    }

    private void OnTrade(object dtoObject)
    {
        SocketBroadcastTradeDTO dto = (SocketBroadcastTradeDTO)dtoObject;
        Resource incomeResource = (Resource) Enum.Parse(typeof(Resource), dto.userIncomingResources);
        Resource outgoingResource = (Resource)Enum.Parse(typeof(Resource), dto.userOutgoingResources);
        int incomeCount = dto.requestedCountOfOutgoingResource * 4;
        int outgoingCount = dto.requestedCountOfOutgoingResource;
        KeyValuePair<Resource, int> addResources = new KeyValuePair<Resource, int>(outgoingResource, outgoingCount);
        KeyValuePair<Resource, int> removeResources = new KeyValuePair<Resource, int>(incomeResource, incomeCount);

        userManager.RemoveResourcesFromUserAsTrade(userManager.GetUserById(dto.userId), removeResources);
        userManager.AddResourcesToUserAsTrade(userManager.GetUserById(dto.userId), addResources);
    }

    private void OnRobberyStartOrEnd(object dtoObject)
    {
        SocketDTOClass dto = (SocketDTOClass)dtoObject;
        SetGameStateOnSpecificEventType(dto.eventType);
    }

    private void OnRobberRobbery(object dtoObject)
    {
        SocketBroadcastResourcesDTO dto = (SocketBroadcastResourcesDTO)dtoObject;
        Dictionary<Resource, int> resourcesToAmount = SimixmanUtils.ResourceListToResourceDictionary(dto.resources);
        userManager.RemoveResourcesFromUserAsRobberRobbery(userManager.GetUserById(dto.userId), resourcesToAmount);
    }
}
