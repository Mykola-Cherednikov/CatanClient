using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyRow : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyNameText;

    [SerializeField] private TMP_Text lobbyUsersCountText;

    [SerializeField] private Button lobbyRowButton;

    public void SetSmallLobbyInfo(Action<Image, int> action, int id, string lobbyName, int usersCount)
    {
        lobbyRowButton.onClick.AddListener(() => action(GetComponent<Image>(), id));
        lobbyNameText.text = lobbyName;
        lobbyUsersCountText.text = $"{usersCount}/4";
    }
}
