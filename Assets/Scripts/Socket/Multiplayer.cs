using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public enum EventType
{
    REQUEST_CONNECT,
    RESPONSE_CONNECT,
    REQUEST_DISCONNECT,
    REQUEST_START_GAME,
    REQUEST_READY_AND_LOAD,

    BROADCAST_USER_CONNECTED,
    BROADCAST_USER_DISCONNECTED,
    BROADCAST_NEW_HOST,
    BROADCAST_START_GAME,

    BROADCAST_PREPARE_USER_TURN,
    BROADCAST_USER_TURN,
    BROADCAST_PREPARATION_USER_TURN_BUILD_SETTLEMENTS,
    BROADCAST_PREPARATION_USER_TURN_BUILD_ROADS,
    BROADCAST_USER_GET_RESOURCE,
    REQUEST_BUILD_SETTLEMENT,
    BROADCAST_BUILD_SETTLEMENT,
    REQUEST_BUILD_ROAD,
    BROADCAST_BUILD_ROAD,
    REQUEST_BUILD_CITY,
    BROADCAST_BUILD_CITY,
    BROADCAST_DICE_THROW,
    REQUEST_USER_TURN_READY,
    REQUEST_TRADE_RESOURCE,
    BROADCAST_USER_TRADE,
    REQUEST_BUY_CARD,
    BROADCAST_BUY_CARD,
    BROADCAST_ROBBERY_START,
    BROADCAST_ROBBER_ROBBERY,
    REQUEST_USER_ROBBERY,
    BROADCAST_USER_ROBBERY,
    REQUEST_USE_KNIGHT_CARD,
    REQUEST_USE_MONOPOLY_CARD,
    REQUEST_USE_YEAR_OF_PLENTY_CARD,
    REQUEST_USE_ROAD_BUILDING_CARD,
    BROADCAST_USE_KNIGHT_CARD,
    BROADCAST_USE_MONOPOLY_CARD,
    BROADCAST_USE_YEAR_OF_PLENTY_CARD,
    BROADCAST_USE_ROAD_BUILDING_CARD,
    BROADCAST_USER_GET_LONGEST_ROAD,
    BROADCAST_USER_GET_LARGEST_ARMY,
    REQUEST_EXCHANGE_OFFER,
    RESPONSE_EXCHANGE_OFFER,
    REQUEST_EXCHANGE,
    BROADCAST_EXCHANGE
}

public class Multiplayer : MonoBehaviour
{
    public static Multiplayer Instance;

    private Socket clientSocket;

    private Dictionary<EventType, UnityEvent<object>> eventTypesToUnityEvent;

    private Dictionary<EventType, Type> eventTypesToDTOTypes;

    #region Events
    public UnityEvent<object> RESPONSE_CONNECT_TO_LOBBY_EVENT;

    public UnityEvent<object> CONNECTION_ERROR_EVENT;

    public UnityEvent<object> BROADCAST_USER_CONNECTION_TO_LOBBY_EVENT;

    public UnityEvent<object> BROADCAST_USER_DISCONNECT_FROM_LOBBY_EVENT;

    public UnityEvent<object> BROADCAST_NEW_HOST_IN_LOBBY_EVENT;

    public UnityEvent<object> BROADCAST_START_GAME_EVENT;

    public UnityEvent<object> BROADCAST_PREPARE_USER_TURN_EVENT;

    public UnityEvent<object> BROADCAST_USER_TURN_EVENT;

    public UnityEvent<object> BROADCAST_BUILD_ROAD_EVENT;

    public UnityEvent<object> BROADCAST_BUILD_SETTLEMENT_EVENT;

    public UnityEvent<object> BROADCAST_BUILD_CITY_EVENT;

    public UnityEvent<object> BROADCAST_GET_RESOURCE_EVENT;

    public UnityEvent<object> BROADCAST_DICE_THROW_EVENT;

    public UnityEvent<object> BROADCAST_USER_TRADE_EVENT;

    public UnityEvent<object> BROADCAST_BUY_CARD_EVENT;

    public UnityEvent<object> BROADCAST_ROBBERY_START_EVENT;

    public UnityEvent<object> BROADCAST_ROBBER_ROBBERY_EVENT;

    public UnityEvent<object> BROADCAST_USER_ROBBERY_EVENT;

    public UnityEvent<object> BROADCAST_USE_KNIGHT_CARD_EVENT;

    public UnityEvent<object> BROADCAST_USE_ROAD_BUILDING_CARD_EVENT;

    public UnityEvent<object> BROADCAST_USE_YEAR_OF_PLENTY_CARD_EVENT;

    public UnityEvent<object> BROADCAST_USE_MONOPOLY_CARD_EVENT;

    public UnityEvent<object> BROADCAST_USER_GET_LONGEST_ROAD_EVENT;

    public UnityEvent<object> BROADCAST_USER_GET_LARGEST_ARMY_EVENT;

    public UnityEvent<object> RESPONSE_EXCHANGE_OFFER_EVENT;

    public UnityEvent<object> BROADCAST_EXCHANGE_EVENT;
    #endregion

    private void Awake()
    {
        eventTypesToDTOTypes = new Dictionary<EventType, Type>
        {
            { EventType.RESPONSE_CONNECT, typeof(SocketResponseConnectDTO) },
            { EventType.BROADCAST_USER_CONNECTED, typeof(SocketBroadcastUserConnectedDTO) },
            { EventType.BROADCAST_USER_DISCONNECTED, typeof(SocketBroadcastUserDisconnectedDTO) },
            { EventType.BROADCAST_NEW_HOST, typeof(SocketBroadcastUserNewHostDTO) },
            { EventType.BROADCAST_START_GAME, typeof(SocketBroadcastStartGameDTO) },
            { EventType.BROADCAST_PREPARE_USER_TURN, typeof(SocketBroadcastUserTurnDTO) },
            { EventType.BROADCAST_USER_TURN, typeof(SocketDTOClass) },
            { EventType.BROADCAST_PREPARATION_USER_TURN_BUILD_ROADS, typeof(SocketBroadcastUserTurnDTO) },
            { EventType.BROADCAST_PREPARATION_USER_TURN_BUILD_SETTLEMENTS, typeof(SocketBroadcastUserTurnDTO) },
            { EventType.BROADCAST_BUILD_SETTLEMENT,  typeof(SocketBroadcastBuildDTO) },
            { EventType.BROADCAST_BUILD_ROAD,  typeof(SocketBroadcastBuildDTO) },
            { EventType.BROADCAST_BUILD_CITY,  typeof(SocketBroadcastBuildDTO) },
            { EventType.BROADCAST_USER_GET_RESOURCE, typeof(SocketBroadcastResourcesDTO) },
            { EventType.BROADCAST_DICE_THROW, typeof(SocketBroadcastDiceThrowDTO) },
            { EventType.BROADCAST_USER_TRADE, typeof(SocketBroadcastTradeDTO) },
            { EventType.BROADCAST_BUY_CARD, typeof (SocketBroadcastBuyCardDTO) },
            { EventType.BROADCAST_ROBBERY_START, typeof(SocketDTOClass) },
            { EventType.BROADCAST_ROBBER_ROBBERY, typeof(SocketBroadcastResourcesDTO) },
            { EventType.BROADCAST_USER_ROBBERY, typeof(SocketBroadcastUserRobberyDTO) },
            { EventType.BROADCAST_USE_KNIGHT_CARD, typeof(SocketBroadcastUseKnightCardDTO) },
            { EventType.BROADCAST_USE_MONOPOLY_CARD, typeof(SocketBroadcastUseMonopolyCardDTO) },
            { EventType.BROADCAST_USE_ROAD_BUILDING_CARD, typeof(SocketBroadcastUseRoadBuildingCardDTO) },
            { EventType.BROADCAST_USE_YEAR_OF_PLENTY_CARD, typeof(SocketBroadcastUseYearOfPlentyCardDTO) },
            { EventType.BROADCAST_USER_GET_LARGEST_ARMY, typeof(SocketBroadcastUserGetLargestArmyDTO) },
            { EventType.BROADCAST_USER_GET_LONGEST_ROAD, typeof(SocketBroadcastUserGetLongestRoadDTO) },
            { EventType.RESPONSE_EXCHANGE_OFFER, typeof(SocketResponseExchangeOfferDTO) },
            { EventType.BROADCAST_EXCHANGE, typeof(SocketBroadcastExchangeDTO) }
        };
        eventTypesToUnityEvent = new Dictionary<EventType, UnityEvent<object>>
        {
            { EventType.RESPONSE_CONNECT, RESPONSE_CONNECT_TO_LOBBY_EVENT },
            { EventType.BROADCAST_USER_CONNECTED, BROADCAST_USER_CONNECTION_TO_LOBBY_EVENT },
            { EventType.BROADCAST_USER_DISCONNECTED, BROADCAST_USER_DISCONNECT_FROM_LOBBY_EVENT },
            { EventType.BROADCAST_NEW_HOST, BROADCAST_NEW_HOST_IN_LOBBY_EVENT },
            { EventType.BROADCAST_START_GAME, BROADCAST_START_GAME_EVENT },
            { EventType.BROADCAST_PREPARE_USER_TURN, BROADCAST_PREPARE_USER_TURN_EVENT },
            { EventType.BROADCAST_USER_TURN, BROADCAST_USER_TURN_EVENT },
            { EventType.BROADCAST_PREPARATION_USER_TURN_BUILD_ROADS, BROADCAST_PREPARE_USER_TURN_EVENT },
            { EventType.BROADCAST_PREPARATION_USER_TURN_BUILD_SETTLEMENTS, BROADCAST_PREPARE_USER_TURN_EVENT },
            { EventType.BROADCAST_BUILD_SETTLEMENT,  BROADCAST_BUILD_SETTLEMENT_EVENT },
            { EventType.BROADCAST_BUILD_ROAD,  BROADCAST_BUILD_ROAD_EVENT },
            { EventType.BROADCAST_BUILD_CITY,  BROADCAST_BUILD_CITY_EVENT },
            { EventType.BROADCAST_USER_GET_RESOURCE, BROADCAST_GET_RESOURCE_EVENT},
            { EventType.BROADCAST_DICE_THROW, BROADCAST_DICE_THROW_EVENT },
            { EventType.BROADCAST_USER_TRADE, BROADCAST_USER_TRADE_EVENT },
            { EventType.BROADCAST_BUY_CARD, BROADCAST_BUY_CARD_EVENT },
            { EventType.BROADCAST_ROBBERY_START, BROADCAST_ROBBERY_START_EVENT},
            { EventType.BROADCAST_ROBBER_ROBBERY, BROADCAST_ROBBER_ROBBERY_EVENT },
            { EventType.BROADCAST_USER_ROBBERY, BROADCAST_USER_ROBBERY_EVENT },
            { EventType.BROADCAST_USE_KNIGHT_CARD, BROADCAST_USE_KNIGHT_CARD_EVENT },
            { EventType.BROADCAST_USE_MONOPOLY_CARD, BROADCAST_USE_MONOPOLY_CARD_EVENT },
            { EventType.BROADCAST_USE_ROAD_BUILDING_CARD, BROADCAST_USE_ROAD_BUILDING_CARD_EVENT },
            { EventType.BROADCAST_USE_YEAR_OF_PLENTY_CARD, BROADCAST_USE_YEAR_OF_PLENTY_CARD_EVENT },
            { EventType.BROADCAST_USER_GET_LARGEST_ARMY, BROADCAST_USER_GET_LARGEST_ARMY_EVENT },
            { EventType.BROADCAST_USER_GET_LONGEST_ROAD, BROADCAST_USER_GET_LONGEST_ROAD_EVENT },
            { EventType.RESPONSE_EXCHANGE_OFFER, RESPONSE_EXCHANGE_OFFER_EVENT },
            { EventType.BROADCAST_EXCHANGE, BROADCAST_EXCHANGE_EVENT }
        };

        RESPONSE_CONNECT_TO_LOBBY_EVENT.AddListener(ConnectFromServer);

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    #region Connect To Lobby
    public async Task<bool> ConnectToLobby()
    {
        SimixmanLogger.Log("Connect To Lobby: Trying to connect");
        try
        {
            clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            clientSocket.ReceiveBufferSize = 4096;
            clientSocket.SendBufferSize = 4096;
            IPEndPoint serverEndPoint = new(IPAddress.Parse(ConfigVariables.SERVER_IP_ADDRESS), ConfigVariables.SERVER_PORT);
            SimixmanLogger.Log("Connect To Lobby: Client socket creation successful");
            await clientSocket.ConnectAsync(serverEndPoint);
            SimixmanLogger.Log("Connect To Lobby: Client socket connection successful");
            await SocketSendConnectRequest();
            SimixmanLogger.Log("Connect To Lobby: Client socket send connection message to server");
            await Task.Run(ReceiveConnectMessageFromServer);
            SimixmanLogger.Log("Connect To Lobby: Client socket receive connection message from server");
            ListenServerAndHandleMessages();
            return true;
        }
        catch (Exception e)
        {
            SimixmanLogger.LogError("Connect To Lobby: Connection error " + e);
            CloseSocket();
            return false;
        }
    }

    private void ConnectFromServer(object data)
    {
        SocketResponseConnectDTO dto = (SocketResponseConnectDTO)data;
        SimixmanLogger.Log(dto.message);
    }

    private void ReceiveConnectMessageFromServer()
    {
        clientSocket.ReceiveTimeout = ConfigVariables.Timeout * 1000;
        byte[] receiveBuffer = new byte[4096];
        int receivedNumOfBytes = clientSocket.Receive(receiveBuffer, SocketFlags.None);
        string message = Encoding.UTF8.GetString(receiveBuffer[..receivedNumOfBytes]);
        HandleMessage(message);
        clientSocket.ReceiveTimeout = 0;
    }
    #endregion

    #region Listen Server
    private async void ListenServerAndHandleMessages()
    {
        try
        {
            SimixmanLogger.Log("Listen Server: Start listen server");
            byte[] receiveBuffer = new byte[4096];
            int receivedNumOfBytes;
            StringBuilder message = new StringBuilder();
            while ((receivedNumOfBytes = await clientSocket.ReceiveAsync(receiveBuffer, SocketFlags.None)) != 0)
            {
                //SimixmanLogger.LogError(message.ToString());
                message.Append(Encoding.UTF8.GetString(receiveBuffer[..receivedNumOfBytes]));
                if (message.ToString()[(message.Length - 2)..message.Length] == "\r\n")
                {
                    HandleMessage(message.ToString());
                    message = new StringBuilder();
                }
            }
            SimixmanLogger.LogError("Listen Server: Stop listen server");
        }
        catch (Exception e)
        {
            SimixmanLogger.LogError("Listen Server: Error stop listen server. " + e.Message);
        }

        CloseSocket();
    }

    private void HandleMessage(string message)
    {
        try
        {
            string[] jsonQueries = message.Split("\r\n");
            foreach (string jsonQuery in jsonQueries)
            {
                if (string.IsNullOrEmpty(jsonQuery)) continue;

                HandleJsonQueryAndInvokeEvent(jsonQuery);
            }
        }
        catch
        {
            SimixmanLogger.LogError("Handle Message: " + message);
        }
    }

    private void HandleJsonQueryAndInvokeEvent(string jsonQuery)
    {
        try
        {
            SocketDTOClass dto = JsonUtility.FromJson<SocketDTOClass>(jsonQuery);
            SimixmanLogger.Log("Handle Message: Receive data from server with event: " + dto.eventType);
            SimixmanLogger.Log($"{jsonQuery}");
            EventType eventType = (EventType)Enum.Parse(typeof(EventType), dto.eventType);
            object objectFromJson = JsonUtility.FromJson(jsonQuery, eventTypesToDTOTypes[eventType]);
            eventTypesToUnityEvent[eventType]?.Invoke(objectFromJson);
        }
        catch (Exception e)
        {
            SimixmanLogger.LogError("Error: " + e);
            SimixmanLogger.LogError("Error in parse eventType: " + jsonQuery);
        }
    }
    #endregion

    public void Disconnect()
    {
        SimixmanLogger.Log("Disconnect From Lobby: Trying to disconnect");

        CloseSocket();

        SimixmanLogger.Log("Disconnect From Lobby: Disconnect success");
    }

    private void CloseSocket()
    {
        if (clientSocket != null)
        {
            clientSocket.Close();
            clientSocket.Dispose();
            clientSocket = null;
            CONNECTION_ERROR_EVENT?.Invoke("");
        }
    }

    #region Socket Requests
    private async Task SocketSendConnectRequest()
    {
        SocketRequestConnectDTO dto = new SocketRequestConnectDTO();
        dto.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_CONNECT);
        await clientSocket.SendToServer(dto);
    }

    public async Task SocketSendStartGameRequest()
    {
        SocketStartGameRequestDTO dto = new SocketStartGameRequestDTO();
        dto.numHexesInMapRow = new List<int> { 4, 5, 6, 7, 6, 5, 4 };
        dto.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_START_GAME);
        await clientSocket.SendToServer(dto);
    }

    public async Task SocketSendReadyAndLoadRequest()
    {
        SocketDTOClass dto = new SocketDTOClass();
        dto.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_READY_AND_LOAD);
        await clientSocket.SendToServer(dto);
    }

    public async Task SocketSendBuildSettlementRequest(int vertexId)
    {
        SocketRequestBuildDTO dto = new SocketRequestBuildDTO();
        dto.fieldId = vertexId;
        dto.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_BUILD_SETTLEMENT);
        await clientSocket.SendToServer(dto);
    }

    public async Task SocketSendBuildRoadRequest(int edgeId)
    {
        SocketRequestBuildDTO dto = new SocketRequestBuildDTO();
        dto.fieldId = edgeId;
        dto.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_BUILD_ROAD);
        await clientSocket.SendToServer(dto);
    }

    public async Task SocketSendBuildCityRequest(int vertexId)
    {
        SocketRequestBuildDTO dto = new SocketRequestBuildDTO();
        dto.fieldId = vertexId;
        dto.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_BUILD_CITY);
        await clientSocket.SendToServer(dto);
    }

    public async Task SocketSendUserTurnReadyRequest()
    {
        SocketDTOClass dto = new SocketDTOClass();
        dto.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_USER_TURN_READY);
        await clientSocket.SendToServer(dto);
    }

    public async Task SocketSendUserTradeRequest(Resource sellResource, Resource buyResource, int amount)
    {
        SocketRequestTradeDTO dto = new SocketRequestTradeDTO();
        dto.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_TRADE_RESOURCE);
        dto.sellResource = Enum.GetName(typeof(Resource), sellResource);
        dto.buyResource = Enum.GetName(typeof(Resource), buyResource);
        dto.requestedAmountOfBuyResource = amount;
        await clientSocket.SendToServer(dto);
    }

    public async Task SocketSendUserRobberyRequest(int hexId, int robbedUserId)
    {
        SocketRequestUserRobberyDTO dto = new SocketRequestUserRobberyDTO();
        dto.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_USER_ROBBERY);
        dto.hexId = hexId;
        dto.victimUserId = robbedUserId;
        await clientSocket.SendToServer(dto);
    }

    public async Task SocketSendBuyCardRequest()
    {
        SocketDTOClass dto = new SocketDTOClass();
        dto.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_BUY_CARD);
        await clientSocket.SendToServer(dto);
    }

    public async Task SocketSendUseRoadBuildingCardRequest()
    {
        SocketDTOClass dto = new SocketDTOClass();
        dto.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_USE_ROAD_BUILDING_CARD);
        await clientSocket.SendToServer(dto);
    }

    public async Task SocketSendUseMonopolyCardRequest(Resource resource)
    {
        SocketRequestUseMonopolyCardDTO dto = new SocketRequestUseMonopolyCardDTO();
        dto.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_USE_MONOPOLY_CARD);
        dto.resource = Enum.GetName(typeof(Resource), resource);
        await clientSocket.SendToServer(dto);
    }

    public async Task SocketSendUseYearOfPlentyCardRequest(List<Resource> resources)
    {
        SocketRequestUseYearOfPlentyCardDTO dto = new SocketRequestUseYearOfPlentyCardDTO();
        dto.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_USE_YEAR_OF_PLENTY_CARD);
        List<string> resourcesString = new List<string>();
        foreach (var resource in resources)
        {
            resourcesString.Add(Enum.GetName(typeof(Resource), resource));
        }
        dto.resources = resourcesString;
        await clientSocket.SendToServer(dto);
    }

    public async Task SocketSendUseKnightCardRequest(int hexId)
    {
        SocketRequestUseKnightCardDTO dto = new SocketRequestUseKnightCardDTO();
        dto.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_USE_KNIGHT_CARD);
        dto.hexId = hexId;
        await clientSocket.SendToServer(dto);
    }

    public async Task SocketSendExchangeOfferRequest(Resource initiatorResource, int initiatorAmountOfResource, int targetUserId, Resource targetResource, int targetAmountOfResource)
    {
        SocketRequestExchangeOfferDTO dto = new SocketRequestExchangeOfferDTO();
        dto.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_EXCHANGE_OFFER);
        dto.targetUserId = targetUserId;
        dto.targetAmountOfResource = targetAmountOfResource;
        dto.initiatorAmountOfResource = initiatorAmountOfResource;
        dto.targetResource = Enum.GetName(typeof(Resource), targetResource);
        dto.initiatorResource = Enum.GetName(typeof(Resource), initiatorResource);

        await clientSocket.SendToServer(dto);
    }

    public async Task SocketSendExchangeRequest(int exchangeId, bool isAccept)
    {
        SocketRequestExchangeDTO dto = new SocketRequestExchangeDTO();
        dto.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_EXCHANGE);
        dto.exchangeId = exchangeId;
        dto.isAccept = isAccept;
        await clientSocket.SendToServer(dto);
    }
    #endregion
}
