using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LobbyForm : Form
{
    [SerializeField] private GameObject usersContent;

    [SerializeField] private GameObject userRowPrefab;

    [SerializeField] private GameObject lobbiesFormPrefab;
    [SerializeField] private GameObject gamePrefab;
    [SerializeField] private TMP_Text lobbyNameText;

    private List<UserRow> users;
    [NonSerialized] private int userId;
    [NonSerialized] private int hostId;

    private async void Start()
    {
        users = new();
        await RestRequests.GetLobbyData(GetLobbyDataSuccess, GetLobbyDataError);
        Multiplayer.Instance.CONNECTION_ERROR_EVENT.AddListener(OnConnectionErrorOrSocketClose);
        Multiplayer.Instance.BROADCAST_USER_CONNECTION_TO_LOBBY_EVENT.AddListener(OnUserConnectToLobby);
        Multiplayer.Instance.BROADCAST_USER_DISCONNECT_FROM_LOBBY_EVENT.AddListener(OnUserDisconnectFromLobby);
        Multiplayer.Instance.BROADCAST_START_GAME_EVENT.AddListener(OnStartGame);
        Multiplayer.Instance.BROADCAST_NEW_HOST_IN_LOBBY_EVENT.AddListener(OnNewHost);
    }

    private void GetLobbyDataSuccess(string json)
    {
        LobbyDataResponseDTO lobbyDataResponseDTO = JsonUtility.FromJson<LobbyDataResponseDTO>(json);
        lobbyNameText.text = lobbyDataResponseDTO.lobbyName;
        userId = lobbyDataResponseDTO.userIdWhoSendRequest;
        hostId = lobbyDataResponseDTO.users.FirstOrDefault(u => u.host).id;
        foreach (var user in lobbyDataResponseDTO.users)
        {
            var userRow = Instantiate(userRowPrefab, usersContent.transform).GetComponent<UserRow>();

            userRow.SetUserInLobbyInfo(user.id, user.name, new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));

            SetRowColor(userRow, user.id);

            users.Add(userRow);
        }
    }

    private void GetLobbyDataError(string json)
    {
        GoToLobbiesForm();
    }

    public void DisconnectFromLobby()
    {
        Multiplayer.Instance.Disconnect();
    }

    public void OnConnectionErrorOrSocketClose(object dtoObject)
    {
        GoToLobbiesForm();
    }

    private void GoToLobbiesForm()
    {
        Instantiate(lobbiesFormPrefab, transform.parent);
        Destroy(gameObject);
    }

    public async void StartGame()
    {
        TurnOffInteractables();
        await Multiplayer.Instance.SocketSendStartGameRequest();
    }

    private void OnStartGame(object dtoObject)
    {
        SocketBroadcastStartGameDTO dto = (SocketBroadcastStartGameDTO)dtoObject;
        GameManager g = Instantiate(gamePrefab, transform.parent.parent).GetComponent<GameManager>();
        g.StartGame(dto, transform.parent.gameObject);
        Destroy(gameObject);
    }

    private void OnUserConnectToLobby(object dtoObject)
    {
        SocketBroadcastUserConnectedDTO dto = (SocketBroadcastUserConnectedDTO)dtoObject;
        if (users.FirstOrDefault(u => u.id == dto.connectedUser.id) != null)
        {
            return;
        }
        var lobbyRow = Instantiate(userRowPrefab, usersContent.transform).GetComponent<UserRow>();
        lobbyRow.SetUserInLobbyInfo(dto.connectedUser.id, dto.connectedUser.name, new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        users.Add(lobbyRow);
    }

    private void OnUserDisconnectFromLobby(object dtoObject)
    {
        SocketBroadcastUserDisconnectedDTO dto = (SocketBroadcastUserDisconnectedDTO)dtoObject;
        var lobbyRow = users.FirstOrDefault(u => u.id == dto.disconnectedUser.id);
        users.Remove(lobbyRow);
        Destroy(lobbyRow.gameObject);
    }

    private void OnNewHost(object dtoObject)
    {
        SocketBroadcastUserNewHostDTO dto = (SocketBroadcastUserNewHostDTO)dtoObject;
        hostId = dto.userHost.id;
        UserRow userInLobbyRow = users.FirstOrDefault(u => u.id == dto.userHost.id);
        SetRowColor(userInLobbyRow, dto.userHost.id);
    }

    private void SetRowColor(UserRow userRow, int newId)
    {
        if (userRow != null)
        {
            if (userId == newId && hostId == newId)
            {
                userRow.GetComponent<Image>().color = Color.red;
            }
            else if (hostId == newId)
            {
                userRow.GetComponent<Image>().color = Color.cyan;
            }
            else if (userId == newId)
            {
                userRow.GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                userRow.GetComponent<Image>().color = Color.white;
            }
        }
    }

    private void OnDestroy()
    {
        Multiplayer.Instance.CONNECTION_ERROR_EVENT.RemoveListener(OnConnectionErrorOrSocketClose);
        Multiplayer.Instance.BROADCAST_USER_CONNECTION_TO_LOBBY_EVENT.RemoveListener(OnUserConnectToLobby);
        Multiplayer.Instance.BROADCAST_USER_DISCONNECT_FROM_LOBBY_EVENT.RemoveListener(OnUserDisconnectFromLobby);
        Multiplayer.Instance.BROADCAST_START_GAME_EVENT.RemoveListener(OnStartGame);
        Multiplayer.Instance.BROADCAST_NEW_HOST_IN_LOBBY_EVENT.RemoveListener(OnNewHost);
    }
}
