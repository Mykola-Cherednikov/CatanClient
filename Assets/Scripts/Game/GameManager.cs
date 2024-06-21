using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameState
{
    WAITING_FOR_PLAYERS,
    PREPARATION_BUILD_SETTLEMENTS,
    PREPARATION_BUILD_ROADS,
    ROBBERY,
    USER_TURN,
    PREPARING_USER_TURN
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
    public ResourceManager resourceManager;
    public CardManager cardManager;

    public GameState gameState;
    public GameState previousGameState;

    public int numOfTurn;

    private Dictionary<EventType, GameState> eventTypeToGameStates;

    public UnityAction CHANGED_GAME_STATE;

    private void Awake()
    {
        Instance = this;

        eventTypeToGameStates = new Dictionary<EventType, GameState>() { 
            { EventType.BROADCAST_PREPARATION_USER_TURN_BUILD_ROADS, GameState.PREPARATION_BUILD_ROADS },
            { EventType.BROADCAST_PREPARATION_USER_TURN_BUILD_SETTLEMENTS, GameState.PREPARATION_BUILD_SETTLEMENTS },
            { EventType.BROADCAST_USER_TURN, GameState.USER_TURN },
            { EventType.BROADCAST_ROBBERY_START, GameState.ROBBERY },
            { EventType.BROADCAST_PREPARE_USER_TURN, GameState.PREPARING_USER_TURN }
        };

        Multiplayer.Instance.CONNECTION_ERROR_EVENT.AddListener(OnConnectionError);
        Multiplayer.Instance.BROADCAST_PREPARE_USER_TURN_EVENT.AddListener(OnUserPreparationTurn);
        Multiplayer.Instance.BROADCAST_USER_TURN_EVENT.AddListener(OnUserTurn);
        Multiplayer.Instance.BROADCAST_BUILD_ROAD_EVENT.AddListener(OnBuildRoad);
        Multiplayer.Instance.BROADCAST_BUILD_SETTLEMENT_EVENT.AddListener(OnBuildSettlement);
        Multiplayer.Instance.BROADCAST_BUILD_CITY_EVENT.AddListener(OnBuildCity);
        Multiplayer.Instance.BROADCAST_DICE_THROW_EVENT.AddListener(OnDiceThrow);
        Multiplayer.Instance.BROADCAST_GET_RESOURCE_EVENT.AddListener(OnResourceGet);
        Multiplayer.Instance.BROADCAST_USER_TRADE_EVENT.AddListener(OnTrade);
        Multiplayer.Instance.BROADCAST_ROBBERY_START_EVENT.AddListener(OnRobberyStart);
        Multiplayer.Instance.BROADCAST_ROBBER_ROBBERY_EVENT.AddListener(OnRobberRobbery);
        Multiplayer.Instance.BROADCAST_USER_ROBBERY_EVENT.AddListener(OnUserRobbery);
        Multiplayer.Instance.BROADCAST_BUY_CARD_EVENT.AddListener(OnBuyCard);
        Multiplayer.Instance.BROADCAST_USE_KNIGHT_CARD_EVENT.AddListener(OnUseKnightCard);
        Multiplayer.Instance.BROADCAST_USE_MONOPOLY_CARD_EVENT.AddListener(OnUseMonopolyCard);
        Multiplayer.Instance.BROADCAST_USE_ROAD_BUILDING_CARD_EVENT.AddListener(OnUseRoadBuildingCard);
        Multiplayer.Instance.BROADCAST_USE_YEAR_OF_PLENTY_CARD_EVENT.AddListener(OnUseYearOfPlentyCard);
        Multiplayer.Instance.BROADCAST_USER_GET_LARGEST_ARMY.AddListener(OnUserGetLargestArmy);
        Multiplayer.Instance.BROADCAST_USER_GET_LONGEST_ROAD.AddListener(OnUserGetLongestRoad);
        cameraGO = Camera.main.gameObject;
        cameraGO.transform.position = new Vector3(0f, 0f, -10f);

        
        mapManager = gameObject.AddComponent<MapManager>();
        userManager = gameObject.AddComponent<UserManager>();
        resourceManager = gameObject.AddComponent<ResourceManager>();
        cardManager = gameObject.AddComponent<CardManager>();
        uiManager = gameObject.AddComponent<UIManager>();
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

    private void OnUserPreparationTurn(object dtoObject)
    {
        SocketBroadcastUserTurnDTO dto = (SocketBroadcastUserTurnDTO)dtoObject;  

        SetGameStateOnSpecificEventTypeAndChangeUIForGameState(dto.eventType);

        User userWhoseTurn = userManager.GetUserById(dto.userId);
        userManager.UpdateUserTurnStatus(userWhoseTurn);

        numOfTurn = dto.numOfTurn;
    }

    private void SetGameStateOnSpecificEventTypeAndChangeUIForGameState(string stringEventType)
    {
        EventType eventType = (EventType)Enum.Parse(typeof(EventType), stringEventType);
        previousGameState = gameState;
        gameState = eventTypeToGameStates[eventType];
        if (gameState != previousGameState)
        {
            CHANGED_GAME_STATE?.Invoke();
        }
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
    }

    private void OnResourceGet(object dtoObject)
    {
        SocketBroadcastResourcesDTO dto = (SocketBroadcastResourcesDTO)dtoObject;
        Dictionary<Resource, int> resourcesToAmount = SimixmanUtils.ResourceListToResourceDictionary(dto.resources);
        resourceManager.AddResourcesToUserAsGathering(userManager.GetUserById(dto.userId), resourcesToAmount);
    }

    private void OnTrade(object dtoObject)
    {
        SocketBroadcastTradeDTO dto = (SocketBroadcastTradeDTO)dtoObject;
        Resource sellResource = (Resource) Enum.Parse(typeof(Resource), dto.userSellResource);
        Resource buyResource = (Resource)Enum.Parse(typeof(Resource), dto.userBuyResource);

        resourceManager.RemoveResourcesFromUserAsTrade(userManager.GetUserById(dto.userId), sellResource, dto.requestedAmountOfBuyResource);
        resourceManager.AddResourcesToUserAsTrade(userManager.GetUserById(dto.userId), buyResource, dto.requestedAmountOfBuyResource);
    }

    private void OnRobberyStart(object dtoObject)
    {
        SocketDTOClass dto = (SocketDTOClass)dtoObject;
        SetGameStateOnSpecificEventTypeAndChangeUIForGameState(dto.eventType);
    }

    private void OnRobberRobbery(object dtoObject)
    {
        SocketBroadcastResourcesDTO dto = (SocketBroadcastResourcesDTO)dtoObject;
        Dictionary<Resource, int> resourcesToAmount = SimixmanUtils.ResourceListToResourceDictionary(dto.resources);
        resourceManager.RemoveResourcesFromUserAsRobberRobbery(userManager.GetUserById(dto.userId), resourcesToAmount);
    }

    private void OnUserRobbery(object dtoObject)
    {
        SocketBroadcastUserRobberyDTO dto = (SocketBroadcastUserRobberyDTO)dtoObject;

        if (dto.victimUserId != -1)
        {
            resourceManager.UserRobberResourcesFromAnotherUser(userManager.GetUserById(dto.victimUserId),
                userManager.GetUserById(dto.robberUserId), (Resource)Enum.Parse(typeof(Resource), dto.resource));
        }

        mapManager.PlaceRobber(dto.hexId);
    }

    private void OnUserTurn(object dtoObject)
    {
        SocketDTOClass dto = (SocketDTOClass)dtoObject;
        SetGameStateOnSpecificEventTypeAndChangeUIForGameState(dto.eventType);
    }

    private void OnBuyCard(object dtoObject)
    {
        SocketBroadcastBuyCardDTO dto = (SocketBroadcastBuyCardDTO)dtoObject;
        Card card = (Card) Enum.Parse(typeof(Card), dto.card);
        cardManager.BuyCard(userManager.GetUserById(dto.userId), card);
    }

    private void OnUseKnightCard(object dtoObject)
    {
        SocketBroadcastUseKnightCardDTO dto = (SocketBroadcastUseKnightCardDTO)dtoObject;
        cardManager.UseKnightCard(userManager.GetUserById(dto.userId), dto.hexId);
    }

    private void OnUseMonopolyCard(object dtoObject)
    {
        SocketBroadcastUseMonopolyCardDTO dto = (SocketBroadcastUseMonopolyCardDTO)dtoObject;
        Resource resource = (Resource)Enum.Parse(typeof(Resource), dto.resource);
        cardManager.UseMonopolyCard(userManager.GetUserById(dto.userId), resource);
    }

    private void OnUseYearOfPlentyCard(object dtoObject)
    {
        SocketBroadcastUseYearOfPlentyCardDTO dto = (SocketBroadcastUseYearOfPlentyCardDTO)dtoObject;
        cardManager.UseYearOfPlentyCard(userManager.GetUserById(dto.userId), dto.resources);
    }

    private void OnUseRoadBuildingCard(object dtoObject)
    {
        SocketBroadcastUseRoadBuildingCardDTO dto = (SocketBroadcastUseRoadBuildingCardDTO)dtoObject;
        cardManager.UseRoadBuildingCard(userManager.GetUserById(dto.userId));
    }

    private void OnUserGetLongestRoad(object dtoObject)
    {
        SocketBroadcastUserGetLongestRoadDTO dto = (SocketBroadcastUserGetLongestRoadDTO)dtoObject;
        userManager.SetLongestRoadToUserAndClearFromOthers(userManager.GetUserById(dto.userId));
    }

    private void OnUserGetLargestArmy(object dtoObject)
    {
        SocketBroadcastUserGetLargestArmyDTO dto = (SocketBroadcastUserGetLargestArmyDTO)dtoObject;
        userManager.SetLargestArmyToUserAndClearFromOthers(userManager.GetUserById(dto.userId));
    }

    private void OnDestroy()
    {
        Multiplayer.Instance.CONNECTION_ERROR_EVENT.RemoveListener(OnConnectionError);
        Multiplayer.Instance.BROADCAST_PREPARE_USER_TURN_EVENT.RemoveListener(OnUserPreparationTurn);
        Multiplayer.Instance.BROADCAST_USER_TURN_EVENT.RemoveListener(OnUserTurn);
        Multiplayer.Instance.BROADCAST_BUILD_ROAD_EVENT.RemoveListener(OnBuildRoad);
        Multiplayer.Instance.BROADCAST_BUILD_SETTLEMENT_EVENT.RemoveListener(OnBuildSettlement);
        Multiplayer.Instance.BROADCAST_BUILD_CITY_EVENT.RemoveListener(OnBuildCity);
        Multiplayer.Instance.BROADCAST_DICE_THROW_EVENT.RemoveListener(OnDiceThrow);
        Multiplayer.Instance.BROADCAST_GET_RESOURCE_EVENT.RemoveListener(OnResourceGet);
        Multiplayer.Instance.BROADCAST_USER_TRADE_EVENT.RemoveListener(OnTrade);
        Multiplayer.Instance.BROADCAST_ROBBERY_START_EVENT.RemoveListener(OnRobberyStart);
        Multiplayer.Instance.BROADCAST_ROBBER_ROBBERY_EVENT.RemoveListener(OnRobberRobbery);
        Multiplayer.Instance.BROADCAST_USER_ROBBERY_EVENT.RemoveListener(OnUserRobbery);
        Multiplayer.Instance.BROADCAST_BUY_CARD_EVENT.RemoveListener(OnBuyCard);
        Multiplayer.Instance.BROADCAST_USE_KNIGHT_CARD_EVENT.RemoveListener(OnUseKnightCard);
        Multiplayer.Instance.BROADCAST_USE_MONOPOLY_CARD_EVENT.RemoveListener(OnUseMonopolyCard);
        Multiplayer.Instance.BROADCAST_USE_ROAD_BUILDING_CARD_EVENT.RemoveListener(OnUseRoadBuildingCard);
        Multiplayer.Instance.BROADCAST_USE_YEAR_OF_PLENTY_CARD_EVENT.RemoveListener(OnUseYearOfPlentyCard);
        Multiplayer.Instance.BROADCAST_USER_GET_LARGEST_ARMY.RemoveListener(OnUserGetLargestArmy);
        Multiplayer.Instance.BROADCAST_USER_GET_LONGEST_ROAD.RemoveListener(OnUserGetLongestRoad);
    }
}
