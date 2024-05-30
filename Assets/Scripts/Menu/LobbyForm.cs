using Assets.Scripts;
using Assets.Scripts.RestDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LobbyForm : Form
{
    [SerializeField] private GameObject _content;

    [SerializeField] private GameObject _userRowGO;

    [SerializeField] private GameObject _lobbiesFormGO;
    [SerializeField] private GameObject _gameGO;
    [SerializeField] private TMP_Text _lobbyNameText;

    private List<UserInLobbyRow> userList;
    [NonSerialized] private int userId;
    [NonSerialized] private int hostId;

    private async void Start()
    {
        userList = new();
        Multiplayer.Instance.CONNECTION_ERROR_EVENT.AddListener(OnConnectionError);
        await RestRequests.GetLobbyData(GetLobbyDataSuccess, GetLobbyDataError);
        Multiplayer.Instance.BROADCAST_USER_CONNECTION_TO_LOBBY_EVENT.AddListener(OnUserConnectToLobby);
        Multiplayer.Instance.BROADCAST_USER_DISCONNECT_FROM_LOBBY_EVENT.AddListener(OnUserDisconnectFromLobby);
        Multiplayer.Instance.BROADCAST_START_GAME_EVENT.AddListener(OnStartGame);
        Multiplayer.Instance.BROADCAST_NEW_HOST_IN_LOBBY_EVENT.AddListener(OnNewHost);
    }

    private void GetLobbyDataSuccess(string resultData)
    {
        LobbyDataResponseDTO lobbyDataResponseDTO = JsonUtility.FromJson<LobbyDataResponseDTO>(resultData);
        _lobbyNameText.text = lobbyDataResponseDTO.lobbyName;
        userId = lobbyDataResponseDTO.requestUserId;
        hostId = lobbyDataResponseDTO.users.FirstOrDefault(u => u.host).id;
        foreach (var user in lobbyDataResponseDTO.users)
        {
            var lobbyRow = Instantiate(_userRowGO, _content.transform).GetComponent<UserInLobbyRow>();

            lobbyRow.SetUserInLobbyInfo(user.id, user.name, new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));

            SetRowColor(lobbyRow, user.id);

            userList.Add(lobbyRow);
        }
    }

    private void GetLobbyDataError(string resultData)
    {
        Instantiate(_lobbiesFormGO, transform.parent);
        Destroy(gameObject);
    }

    public void DisconnectFromLobby()
    {
        Multiplayer.Instance.Disconnect();
        Instantiate(_lobbiesFormGO, transform.parent);
        Destroy(gameObject);
    }

    public void OnConnectionError(object data)
    {
        Instantiate(_lobbiesFormGO, transform.parent);
        Destroy(gameObject);
    }

    public async void StartGame()
    {
        TurnOffInteractables();
        await Multiplayer.Instance.SocketStartGameMessage();
    }

    private void OnStartGame(object data)
    {
        SocketBroadcastStartGameDTO dto = (SocketBroadcastStartGameDTO)data;
        Game g = Instantiate(_gameGO, transform.parent.parent).GetComponent<Game>();
        g.SetStartGameData(dto.map, dto.hexes, transform.parent.gameObject);
        UIGameHandler uiGameHandler = g.GetComponent<UIGameHandler>();
        Destroy(gameObject);
    }

    private void OnUserConnectToLobby(object data)
    {
        SocketBroadcastUserConnectionToLobbyDTO dto = (SocketBroadcastUserConnectionToLobbyDTO)data;
        if (userList.FirstOrDefault(u => u.id == dto.user.id) != null)
        {
            return;
        }
        var lobbyRow = Instantiate(_userRowGO, _content.transform).GetComponent<UserInLobbyRow>();
        lobbyRow.SetUserInLobbyInfo(dto.user.id, dto.user.name, new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        userList.Add(lobbyRow);
    }

    private void OnUserDisconnectFromLobby(object data)
    {
        SocketBroadcastUserDisconnectFromLobbyDTO dto = (SocketBroadcastUserDisconnectFromLobbyDTO)data;
        var lobbyRow = userList.FirstOrDefault(u => u.id == dto.user.id);
        userList.Remove(lobbyRow);
        Destroy(lobbyRow.gameObject);
    }

    private void OnNewHost(object data)
    {
        SocketBroadcastNewHostDTO dto = (SocketBroadcastNewHostDTO)data;
        hostId = dto.user.id;
        UserInLobbyRow userInLobbyRow = userList.FirstOrDefault(u => u.id == dto.user.id);
        SetRowColor(userInLobbyRow, dto.user.id);
    }

    private void SetRowColor(UserInLobbyRow row, int newId)
    {
        if (userId == newId && hostId == newId)
        {
            row.GetComponent<Image>().color = Color.red;
        }
        else if (hostId == newId)
        {
            row.GetComponent<Image>().color = Color.cyan;
        }
        else if (userId == newId)
        {
            row.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            row.GetComponent<Image>().color = Color.white;
        }
    }
}
