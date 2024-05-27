using Assets.Scripts.RestDTO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SocketDTOClass
{
    public string eventType;
}

[Serializable]
public class SocketRequestConnectToLobbyDTO : SocketDTOClass
{
    public string token = StaticVariables.TOKEN;
}

[Serializable]
public class SocketResponseConnectToLobbyDTO : SocketDTOClass
{
    public string message;
}

[Serializable]
public class SocketBroadcastUserConnectionToLobbyDTO : SocketDTOClass
{
    public string message;

    public UserInLobbyDTO user;
}

[Serializable]
public class SocketBroadcastUserDisconnectFromLobbyDTO : SocketDTOClass
{
    public string message;

    public UserInLobbyDTO user;
}

[Serializable]
public class SocketBroadcastStartGameDTO : SocketDTOClass
{
    public UserInLobbyDTO[] users;

    public int userId;
}

[Serializable]
public class SocketBroadcastNewHostDTO : SocketDTOClass
{
    public UserInLobbyDTO user;
}