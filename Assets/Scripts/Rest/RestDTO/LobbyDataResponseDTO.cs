using System;


[Serializable]
public class LobbyDataResponseDTO : RestDTOClass
{
    public int lobbyId;

    public string lobbyName;

    public int userIdWhoSendRequest;

    public UserInLobbyDTO[] users;
}

