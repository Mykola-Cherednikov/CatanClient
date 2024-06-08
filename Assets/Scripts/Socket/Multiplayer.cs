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

    BROADCAST_USER_TURN,
    BROADCAST_PREPARATION_USER_TURN_BUILD_SETTLEMENTS,
    BROADCAST_PREPARATION_USER_TURN_BUILD_ROADS,
    BROADCAST_GIVE_RESOURCE,
    REQUEST_BUILD_SETTLEMENT,
    BROADCAST_BUILD_SETTLEMENT,
    REQUEST_BUILD_ROAD,
    BROADCAST_BUILD_ROAD,
    REQUEST_BUILD_CITY,
    BROADCAST_BUILD_CITY
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

    public UnityEvent<object> BROADCAST_USER_TURN_EVENT;

    public UnityEvent<object> BROADCAST_BUILD_ROAD_EVENT;

    public UnityEvent<object> BROADCAST_BUILD_SETTLEMENT_EVENT;

    public UnityEvent<object> BROADCAST_BUILD_CITY_EVENT;
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
            { EventType.BROADCAST_USER_TURN, typeof(SocketBroadcastUserTurnDTO) },
            { EventType.BROADCAST_PREPARATION_USER_TURN_BUILD_ROADS, typeof(SocketBroadcastUserTurnDTO) },
            { EventType.BROADCAST_PREPARATION_USER_TURN_BUILD_SETTLEMENTS, typeof(SocketBroadcastUserTurnDTO) },
            { EventType.BROADCAST_BUILD_SETTLEMENT,  typeof(SocketBroadcastBuildDTO) },
            { EventType.BROADCAST_BUILD_ROAD,  typeof(SocketBroadcastBuildDTO) },
            { EventType.BROADCAST_BUILD_CITY,  typeof(SocketBroadcastBuildDTO) }
        };
        eventTypesToUnityEvent = new Dictionary<EventType, UnityEvent<object>>
        {
            { EventType.RESPONSE_CONNECT, RESPONSE_CONNECT_TO_LOBBY_EVENT },
            { EventType.BROADCAST_USER_CONNECTED, BROADCAST_USER_CONNECTION_TO_LOBBY_EVENT },
            { EventType.BROADCAST_USER_DISCONNECTED, BROADCAST_USER_DISCONNECT_FROM_LOBBY_EVENT },
            { EventType.BROADCAST_NEW_HOST, BROADCAST_NEW_HOST_IN_LOBBY_EVENT },
            { EventType.BROADCAST_START_GAME, BROADCAST_START_GAME_EVENT },
            { EventType.BROADCAST_USER_TURN, BROADCAST_USER_TURN_EVENT },
            { EventType.BROADCAST_PREPARATION_USER_TURN_BUILD_ROADS, BROADCAST_USER_TURN_EVENT },
            { EventType.BROADCAST_PREPARATION_USER_TURN_BUILD_SETTLEMENTS, BROADCAST_USER_TURN_EVENT },
            { EventType.BROADCAST_BUILD_SETTLEMENT,  BROADCAST_BUILD_SETTLEMENT_EVENT },
            { EventType.BROADCAST_BUILD_ROAD,  BROADCAST_BUILD_ROAD_EVENT },
            { EventType.BROADCAST_BUILD_CITY,  BROADCAST_BUILD_CITY_EVENT }
        };

        RESPONSE_CONNECT_TO_LOBBY_EVENT.AddListener(ConnectFromServer);

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    #region Connect To Lobby
    public async Task<bool> ConnectToLobby()
    {
        Debug.Log("Connect To Lobby: Trying to connect");
        try
        {
            clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            clientSocket.ReceiveBufferSize = 4096;
            clientSocket.SendBufferSize = 4096;
            IPEndPoint serverEndPoint = new(IPAddress.Parse(StaticVariables.SERVER_ADDRESS), StaticVariables.SERVER_PORT);
            Debug.Log("Connect To Lobby: Client socket creation successful");
            await clientSocket.ConnectAsync(serverEndPoint);
            Debug.Log("Connect To Lobby: Client socket connection successful");
            await SocketSendConnectRequest();
            Debug.Log("Connect To Lobby: Client socket send connection message to server");
            await Task.Run(ReceiveConnectMessageFromServer);
            Debug.Log("Connect To Lobby: Client socket receive connection message from server");
            ListenServerAndHandleMessages();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Connect To Lobby: Connection error " + e);
            CloseSocket();
            return false;
        }
    }

    private void ConnectFromServer(object data)
    {
        SocketResponseConnectDTO dto = (SocketResponseConnectDTO)data;
        Debug.Log(dto.message);
    }

    private void ReceiveConnectMessageFromServer()
    {
        clientSocket.ReceiveTimeout = StaticVariables.Timeout * 1000;
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
            Debug.Log("Listen Server: Start listen server");
            byte[] receiveBuffer = new byte[4096];
            int receivedNumOfBytes;
            StringBuilder message = new StringBuilder();
            while ((receivedNumOfBytes = await clientSocket.ReceiveAsync(receiveBuffer, SocketFlags.None)) != 0)
            {
                Debug.LogError(message.ToString());
                message.Append(Encoding.UTF8.GetString(receiveBuffer[..receivedNumOfBytes]));
                if (message.ToString().Contains("\r\n"))
                {
                    HandleMessage(message.ToString());
                    message = new StringBuilder();
                }
            }
            Debug.LogError("Listen Server: Stop listen server");
        }
        catch (Exception e)
        {
            Debug.LogError("Listen Server: Error stop listen server. " + e.Message);
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
        catch (Exception e)
        {
            Debug.LogError("Handle Message: " + e);
        }
    }

    private void HandleJsonQueryAndInvokeEvent(string jsonQuery)
    {
        SocketDTOClass dto = JsonUtility.FromJson<SocketDTOClass>(jsonQuery);
        Debug.Log("Handle Message: Receive data from server with event: " + dto.eventType);
        Debug.Log($"{jsonQuery}");
        EventType eventType = (EventType)Enum.Parse(typeof(EventType), dto.eventType);
        object objectFromJson = JsonUtility.FromJson(jsonQuery, eventTypesToDTOTypes[eventType]);
        eventTypesToUnityEvent[eventType]?.Invoke(objectFromJson);
    }
    #endregion


    public void Disconnect()
    {
        Debug.Log("Disconnect From Lobby: Trying to disconnect");

        CloseSocket();

        Debug.Log("Disconnect From Lobby: Disconnect success");
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
    #endregion
}
