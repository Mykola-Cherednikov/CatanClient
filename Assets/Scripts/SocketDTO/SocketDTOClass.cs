using Assets.Scripts.Game;
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
public class SocketBroadcastUserConnectedToLobbyDTO : SocketDTOClass
{
    public string message;

    public UserInLobbyDTO connectedUser;
}

[Serializable]
public class SocketBroadcastUserDisconnectedFromLobbyDTO : SocketDTOClass
{
    public string message;

    public UserInLobbyDTO disconnectedUser;
}

[Serializable]
public class SocketStartGameRequestDTO : SocketDTOClass
{
    public List<int> numHexesInMapRow;
}

[Serializable]
public class SocketBroadcastStartGameDTO : SocketDTOClass
{
    public List<int> numHexesInMapRow;

    public List<HexDTO> hexes;
}

[Serializable]
public class SocketBroadcastUserNewHostDTO : SocketDTOClass
{
    public UserInLobbyDTO userHost;
}