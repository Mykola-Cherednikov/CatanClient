using Assets.Scripts;
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
    REQUEST_CONNECT_TO_LOBBY,
    RESPONSE_CONNECT_TO_LOBBY,
    REQUEST_DISCONNECT_FROM_LOBBY,
    REQUEST_START_GAME,
    REQUEST_READY_AND_LOAD,

    BROADCAST_USER_CONNECTION_TO_LOBBY,
    BROADCAST_USER_DISCONNECT_FROM_LOBBY,
    BROADCAST_NEW_HOST_IN_LOBBY,
    BROADCAST_START_GAME
}

public class Multiplayer : MonoBehaviour
{
    public static Multiplayer Instance;

    private Socket socketClient;

    private bool isConnected;

    private Dictionary<EventType, UnityEvent<object>> eventTypesToUnityEvent;

    private Dictionary<EventType, Type> eventTypesToDTOTypes;

    #region Events
    public UnityEvent<object> RESPONSE_CONNECT_TO_LOBBY_EVENT;

    public UnityEvent<object> CONNECTION_ERROR_EVENT;

    public UnityEvent<object> BROADCAST_USER_CONNECTION_TO_LOBBY_EVENT;

    public UnityEvent<object> BROADCAST_USER_DISCONNECT_FROM_LOBBY_EVENT;

    public UnityEvent<object> BROADCAST_NEW_HOST_IN_LOBBY_EVENT;

    public UnityEvent<object> BROADCAST_START_GAME_EVENT;
    #endregion

    private void Awake()
    {
        eventTypesToDTOTypes = new Dictionary<EventType, Type>
        {
            { EventType.RESPONSE_CONNECT_TO_LOBBY, typeof(SocketResponseConnectToLobbyDTO) },
            { EventType.BROADCAST_USER_CONNECTION_TO_LOBBY, typeof(SocketBroadcastUserConnectedToLobbyDTO) },
            { EventType.BROADCAST_USER_DISCONNECT_FROM_LOBBY, typeof(SocketBroadcastUserDisconnectedFromLobbyDTO) },
            { EventType.BROADCAST_NEW_HOST_IN_LOBBY, typeof(SocketBroadcastUserNewHostDTO) },
            { EventType.BROADCAST_START_GAME, typeof(SocketBroadcastStartGameDTO) }
        };
        eventTypesToUnityEvent = new Dictionary<EventType, UnityEvent<object>>
        {
            { EventType.RESPONSE_CONNECT_TO_LOBBY, RESPONSE_CONNECT_TO_LOBBY_EVENT },
            { EventType.BROADCAST_USER_CONNECTION_TO_LOBBY, BROADCAST_USER_CONNECTION_TO_LOBBY_EVENT },
            { EventType.BROADCAST_USER_DISCONNECT_FROM_LOBBY, BROADCAST_USER_DISCONNECT_FROM_LOBBY_EVENT },
            { EventType.BROADCAST_NEW_HOST_IN_LOBBY, BROADCAST_NEW_HOST_IN_LOBBY_EVENT },
            { EventType.BROADCAST_START_GAME, BROADCAST_START_GAME_EVENT }
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
            socketClient = new Socket(SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEndPoint = new(IPAddress.Parse(StaticVariables.SERVER_ADDRESS), StaticVariables.SERVER_PORT);
            Debug.Log("Connect To Lobby: Client socket creation successful");
            await socketClient.ConnectAsync(serverEndPoint);
            Debug.Log("Connect To Lobby: Client socket connection successful");
            await SocketSendConnectMessage();
            Debug.Log("Connect To Lobby: Client socket send connection message to server");
            await Task.Run(ReceiveConnectMessageFromServer);
            Debug.Log("Connect To Lobby: Client socket receive connection message from server");
            ListenServer();
            isConnected = true;
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
        SocketResponseConnectToLobbyDTO dto = (SocketResponseConnectToLobbyDTO)data;
        Debug.Log(dto.message);
    }

    private void ReceiveConnectMessageFromServer()
    {
        socketClient.ReceiveTimeout = StaticVariables.Timeout * 1000;
        byte[] receiveBuffer = new byte[4096];
        int receivedNumOfBytes = socketClient.Receive(receiveBuffer, SocketFlags.None);
        string message = Encoding.UTF8.GetString(receiveBuffer[..receivedNumOfBytes]);
        HandleMessage(message);
        socketClient.ReceiveTimeout = 0;
    }
    #endregion

    private async void ListenServer()
    {
        try
        {
            Debug.Log("Listen Server: Start listen server");
            byte[] receiveBuffer = new byte[4096];
            int receivedNumOfBytes;
            while ((receivedNumOfBytes = await socketClient.ReceiveAsync(receiveBuffer, SocketFlags.None)) != 0)
            {
                string message = Encoding.UTF8.GetString(receiveBuffer[..receivedNumOfBytes]);
                HandleMessage(message);
            }
        }
        catch (Exception)
        {
            CONNECTION_ERROR_EVENT?.Invoke("");
            Debug.LogError("Listen Server: Stop listen server");
            CloseSocket();
        }
    }

    private void HandleMessage(string message)
    {
        try
        {
            string[] jsonQueries = message.Split("\r\n");
            foreach (string jsonQuery in jsonQueries)
            {
                if (string.IsNullOrEmpty(jsonQuery)) break;

                SocketDTOClass dto = JsonUtility.FromJson<SocketDTOClass>(jsonQuery);
                Debug.Log("Handle Message: Receive data from server with event: " + dto.eventType);
                Debug.Log($"{jsonQuery}");
                EventType eventType = (EventType)Enum.Parse(typeof(EventType), dto.eventType);
                object objectFromJson = JsonUtility.FromJson(jsonQuery, eventTypesToDTOTypes[eventType]);
                eventTypesToUnityEvent[eventType]?.Invoke(objectFromJson);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Handle Message: " + e);
        }
    }

    public void Disconnect()
    {
        Debug.Log("Disconnect From Lobby: Trying to disconnect");

        try
        {
            CloseSocket();
            Debug.Log("Disconnect From Lobby: Disconnect successful");
        }
        catch (Exception e)
        {
            Debug.LogError("Disconnect From Lobby: Disconnect with error " + e);
        }
    }

    #region DO NOT OPEN
    private void CloseSocket()
    {
        if (isConnected)
        {
            socketClient.Close();
            socketClient.Dispose();
            socketClient = null;
            isConnected = false;
        }
    }
    #endregion

    #region Socket Requests
    private async Task SocketSendConnectMessage()
    {
        SocketRequestConnectToLobbyDTO dto = new SocketRequestConnectToLobbyDTO();
        dto.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_CONNECT_TO_LOBBY);
        await socketClient.SendToServer(dto);
    }

    public async Task SocketSendStartGameMessage()
    {
        SocketStartGameRequestDTO dto = new SocketStartGameRequestDTO();
        dto.numHexesInMapRow = new List<int> { 4, 5, 6, 7, 6, 5, 4 };
        dto.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_START_GAME);
        await socketClient.SendToServer(dto);
    }

    public async Task SocketSendReadyAndLoadMessage()
    {
        SocketDTOClass dto = new SocketDTOClass();
        dto.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_READY_AND_LOAD);
        await socketClient.SendToServer(dto);
    }
    #endregion
}
