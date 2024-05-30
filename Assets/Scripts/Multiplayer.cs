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

    private Socket _client;

    private bool _connected;

    private Dictionary<EventType, UnityEvent<object>> _eventsToUnityEvent;

    private Dictionary<EventType, Type> _eventsToTypes;

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
        _eventsToTypes = new Dictionary<EventType, Type>
        {
            { EventType.RESPONSE_CONNECT_TO_LOBBY, typeof(SocketResponseConnectToLobbyDTO) },
            { EventType.BROADCAST_USER_CONNECTION_TO_LOBBY, typeof(SocketBroadcastUserConnectionToLobbyDTO) },
            { EventType.BROADCAST_USER_DISCONNECT_FROM_LOBBY, typeof(SocketBroadcastUserDisconnectFromLobbyDTO) },
            { EventType.BROADCAST_NEW_HOST_IN_LOBBY, typeof(SocketBroadcastNewHostDTO) },
            { EventType.BROADCAST_START_GAME, typeof(SocketBroadcastStartGameDTO) }
        };
        _eventsToUnityEvent = new Dictionary<EventType, UnityEvent<object>>
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
            _client = new Socket(SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new(IPAddress.Parse(StaticVariables.SERVER_ADDRESS), StaticVariables.SERVER_PORT);
            Debug.Log("Connect To Lobby: Client socket creation successful");
            await _client.ConnectAsync(ep);
            Debug.Log("Connect To Lobby: Client socket connection successful");
            await SocketConnectMessage();
            Debug.Log("Connect To Lobby: Client socket send connection message to server");
            await Task.Run(GetConnectMessageFromServer);
            Debug.Log("Connect To Lobby: Client socket receive connection message from server");
            ListenServer();
            _connected = true;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Connect To Lobby: Connection error " + e);
            ClearSocket();
            return false;
        }
    }

    private void ConnectFromServer(object data)
    {
        SocketResponseConnectToLobbyDTO s = (SocketResponseConnectToLobbyDTO)data;
        Debug.Log(s.message);
    }

    private void GetConnectMessageFromServer()
    {
        _client.ReceiveTimeout = StaticVariables.Timeout * 1000;
        byte[] buffer = new byte[4096];
        int numOfBytes = _client.Receive(buffer, SocketFlags.None);
        string message = Encoding.UTF8.GetString(buffer[..numOfBytes]);
        HandleMessage(message);
        _client.ReceiveTimeout = 0;
    }
    #endregion

    private async void ListenServer()
    {
        try
        {
            Debug.Log("Listen Server: Start listen server");
            byte[] buffer = new byte[4096];
            int numOfBytes;
            while ((numOfBytes = await _client.ReceiveAsync(buffer, SocketFlags.None)) != 0)
            {
                string message = Encoding.UTF8.GetString(buffer[..numOfBytes]);
                HandleMessage(message);
            }
        }
        catch (Exception)
        {
            CONNECTION_ERROR_EVENT?.Invoke("");
            Debug.LogError("Listen Server: Stop listen server");
            ClearSocket();
        }
    }

    private void HandleMessage(string message)
    {
        try
        {
            string[] stringQueries = message.Split("\r\n");
            foreach (string query in stringQueries)
            {
                if (string.IsNullOrEmpty(query)) break;

                SocketDTOClass socketDTO = JsonUtility.FromJson<SocketDTOClass>(query);
                Debug.Log("Handle Message: Receive data from server with event: " + socketDTO.eventType);
                Debug.Log($"{query}");
                EventType eventType = (EventType)Enum.Parse(typeof(EventType), socketDTO.eventType);
                _eventsToUnityEvent[eventType]?.Invoke(JsonUtility.FromJson(query, _eventsToTypes[eventType]));
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
            ClearSocket();
            Debug.Log("Disconnect From Lobby: Disconnect successful");
        }
        catch (Exception e)
        {
            Debug.LogError("Disconnect From Lobby: Disconnect with error " + e);
        }
    }

    #region DO NOT OPEN
    private void ClearSocket()
    {
        if (_connected)
        {
            _client.Close();
            _client.Dispose();
            _client = null;
            _connected = false;
        }
    }
    #endregion

    #region Socket Requests
    private async Task SocketConnectMessage()
    {
        SocketRequestConnectToLobbyDTO socketDTOClass = new SocketRequestConnectToLobbyDTO();
        socketDTOClass.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_CONNECT_TO_LOBBY);
        await _client.SendToServer(socketDTOClass);
    }

    public async Task SocketStartGameMessage()
    {
        SocketStartGameRequestDTO socketDTOClass = new SocketStartGameRequestDTO();
        socketDTOClass.map = new List<int> { 4, 5, 6, 7, 6, 5, 4 };
        socketDTOClass.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_START_GAME);
        await _client.SendToServer(socketDTOClass);
    }

    public async Task SocketReadyAndLoadMessage()
    {
        SocketDTOClass socketDTOClass = new SocketDTOClass();
        socketDTOClass.eventType = Enum.GetName(typeof(EventType), EventType.REQUEST_READY_AND_LOAD);
        await _client.SendToServer(socketDTOClass);
    }
    #endregion
}
