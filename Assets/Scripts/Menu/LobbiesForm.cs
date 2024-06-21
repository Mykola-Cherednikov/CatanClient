using System;
using UnityEngine;
using UnityEngine.UI;

public class LobbiesForm : Form
{
    [SerializeField] private GameObject lobbiesContent;

    [SerializeField] private GameObject lobbyRowPrefab;

    [SerializeField] private GameObject loginFormPrefab;

    [SerializeField] private GameObject lobbyFormPrefab;

    [SerializeField] private Button joinButton;


    private Image currentLobbyImage;
    private int currentLobbyId;

    private void Start()
    {
        RefreshLobbies();
    }

    public void LogOut()
    {
        ConfigVariables.TOKEN = null;
        Instantiate(loginFormPrefab, transform.parent);
        Destroy(gameObject);
    }

    public void MarkCurrentLobbyAfterChoseAndDemarkOld(Image lobbyImage, int lobbyId)
    {
        if (currentLobbyImage != null)
        {
            currentLobbyImage.color = new Color(135f / 255f, 90f / 255f, 50f / 255f);
        }

        currentLobbyImage = lobbyImage;
        currentLobbyImage.color = new Color(90f / 255f, 60f / 255f, 40f / 255f);
        currentLobbyId = lobbyId;
        joinButton.interactable = true;
    }


    #region Join Lobby
    public async void Join()
    {
        TurnOffInteractables();
        await RestRequests.JoinLobby(currentLobbyId, OnJoinLobbySuccess, OnJoinLobbyError);
    }

    private async void OnJoinLobbySuccess(string json)
    {
        if (await Multiplayer.Instance.ConnectToLobby())
        {
            Instantiate(lobbyFormPrefab, transform.parent).GetComponent<LobbyForm>();
            Destroy(gameObject);
        }
        else
        {
            SimixmanLogger.Log("Connection to lobby failed");
            TurnOnInteractables();
            joinButton.interactable = false;
        }
    }

    private void OnJoinLobbyError(string json)
    {
        CreateErrorForm(json);
        SimixmanLogger.Log(json);
        TurnOnInteractables();
        joinButton.interactable = false;
    }
    #endregion

    #region Get Lobby
    public async void RefreshLobbies()
    {
        TurnOffInteractables();
        foreach (Transform t in lobbiesContent.transform)
        {
            interactiveItems.Remove(t.gameObject.GetComponent<Button>());

            Destroy(t.gameObject);
        }

        currentLobbyId = -1;

        await RestRequests.GetLobbies(OnGetLobbiesSuccess, OnGetLobbiesError);
    }

    private void OnGetLobbiesSuccess(string json)
    {
        SimixmanLogger.Log(json);

        var lobbies = JsonUtility.FromJson<SmallLobbiesResponseDTO>(SimixmanUtils.FixArrayJson(json));

        foreach (var lobby in lobbies.Items)
        {
            var lobbyRow = Instantiate(lobbyRowPrefab, lobbiesContent.transform).GetComponent<LobbyRow>();

            lobbyRow.SetSmallLobbyInfo(MarkCurrentLobbyAfterChoseAndDemarkOld, lobby.lobbyId, lobby.lobbyName, lobby.usersCount);

            interactiveItems.Add(lobbyRow.GetComponent<Button>());
        }
        TurnOnInteractables();
        joinButton.interactable = false;
    }

    private void OnGetLobbiesError(string json)
    {
        CreateErrorForm(json);
        SimixmanLogger.Log(json);
        TurnOnInteractables();
        joinButton.interactable = false;
    }
    #endregion

    #region Create Lobby
    public async void CreateLobby()
    {
        TurnOffInteractables();
        await RestRequests.CreateLobby(Guid.NewGuid().ToString(), OnCreateLobbySuccess, OnCreateLobbyError);
    }

    private void OnCreateLobbySuccess(string json)
    {
        LobbyCreateResponseDTO dto = JsonUtility.FromJson<LobbyCreateResponseDTO>(json);
        currentLobbyId = dto.lobbyId;
        Join();
    }

    private void OnCreateLobbyError(string json)
    {
        CreateErrorForm(json);
        SimixmanLogger.Log(json);
        TurnOnInteractables();
        joinButton.interactable = false;
    }
    #endregion
}
