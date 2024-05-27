using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyRow : MonoBehaviour
{
    [SerializeField] private TMP_Text _lobbyNameText;

    [SerializeField] private TMP_Text _lobbyUsersCountText;

    [SerializeField] private Button _lobbyRowButton;

    public void SetSmallLobbyInfo(Action<Image, int> action, int id, string lobbyName, int usersCount)
    {
        _lobbyRowButton.onClick.AddListener(() => action(GetComponent<Image>(), id));
        _lobbyNameText.text = lobbyName;
        _lobbyUsersCountText.text = $"{usersCount}/4";
    }
}
