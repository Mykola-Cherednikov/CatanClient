using Assets.Scripts;
using Assets.Scripts.RestDTO;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LobbiesForm : Form
{
    [SerializeField] private GameObject _content;

    [SerializeField] private GameObject _lobbyRowGO;

    [SerializeField] private GameObject _loginFormGO;

    [SerializeField] private GameObject _lobbyFormGO;

    [SerializeField] private Button _joinButton;


    private Image _currentLobbyImage;
    private int _currentLobbyID;

    private void Start()
    {
        RefreshLobbies();
    }

    public void LogOut()
    {
        StaticVariables.TOKEN = null;
        Instantiate(_loginFormGO, transform.parent);
        Destroy(gameObject);
    }

    public void SetCurrentLobby(Image currentLobbyImage, int id)
    {
        if (_currentLobbyImage != null)
        {
            _currentLobbyImage.color = new Color(135f / 255f, 90f / 255f, 50f / 255f);
        }
        _currentLobbyImage = currentLobbyImage;
        _currentLobbyImage.color = new Color(90f / 255f, 60f / 255f, 40f / 255f);
        _currentLobbyID = id;
        _joinButton.interactable = true;
    }


    #region Join Lobby
    public async void Join()
    {
        TurnOffInteractables();
        await RestRequests.JoinLobby(_currentLobbyID, JoinLobbySuccess, JoinLobbyError);
    }

    private async void JoinLobbySuccess(string resultData)
    {
        if (await Multiplayer.Instance.ConnectToLobby())
        {
            Instantiate(_lobbyFormGO, transform.parent).GetComponent<LobbyForm>();
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Connection to lobby failed");
        }
    }

    private void JoinLobbyError(string resultData)
    {
        CreateErrorForm(resultData);
        Debug.Log(resultData);
        TurnOnInteractables();
        _joinButton.interactable = false;
    }
    #endregion

    #region Get Lobby
    public async void RefreshLobbies()
    {
        TurnOffInteractables();
        foreach (Transform t in _content.transform)
        {
            _interactiveItems.Remove(t.gameObject.GetComponent<Button>());

            Destroy(t.gameObject);
        }

        _currentLobbyID = -1;

        await RestRequests.GetLobbies(GetLobbiesSuccess, GetLobbiesError);
        
    }

    private void GetLobbiesSuccess(string resultData)
    {
        Debug.Log(resultData);

        var lobbies = JsonUtility.FromJson<SmallLobbiesResponseDTO>(SimixmanUtils.FixArrayJson(resultData));

        foreach (var l in lobbies.Items)
        {
            var lobbyRow = Instantiate(_lobbyRowGO, _content.transform).GetComponent<LobbyRow>();

            lobbyRow.SetSmallLobbyInfo(SetCurrentLobby, l.lobbyId, l.lobbyName, l.usersCount);

            _interactiveItems.Add(lobbyRow.GetComponent<Button>());
        }
        TurnOnInteractables();
        _joinButton.interactable = false;
    }

    private void GetLobbiesError(string resultData)
    {
        CreateErrorForm(resultData);
        Debug.Log(resultData);
        TurnOnInteractables();
        _joinButton.interactable = false;
    }
    #endregion

    #region Create Lobby
    public async void CreateLobby()
    {
        TurnOffInteractables();
        await RestRequests.CreateLobby(Guid.NewGuid().ToString(), CreateLobbySuccessful, CreateLobbyError);
    }

    private void CreateLobbySuccessful(string resultData)
    {
        LobbyCreateResponseDTO dto = JsonUtility.FromJson<LobbyCreateResponseDTO>(resultData);
        _currentLobbyID = dto.lobbyId;
        Join();
    }

    private void CreateLobbyError(string resultData)
    {
        CreateErrorForm(resultData);
        Debug.Log(resultData);
        TurnOnInteractables();
        _joinButton.interactable = false;
    }
    #endregion
}
