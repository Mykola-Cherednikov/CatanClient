using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SocketDTOClass
{
    public string eventType;
}

[Serializable]
public class SocketRequestConnectDTO : SocketDTOClass
{
    public string token = StaticVariables.TOKEN;
}

[Serializable]
public class SocketResponseConnectDTO : SocketDTOClass
{
    public string message;
}

[Serializable]
public class SocketBroadcastUserConnectedDTO : SocketDTOClass
{
    public string message;

    public UserInLobbyDTO connectedUser;
}

[Serializable]
public class SocketBroadcastUserDisconnectedDTO : SocketDTOClass
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
    public List<int> hexesInRowCounts;

    public List<SocketHexDTO> hexes;

    public List<User> users;

    public User currentUser;
}

[Serializable]
public class SocketBroadcastUserNewHostDTO : SocketDTOClass
{
    public UserInLobbyDTO userHost;
}

[Serializable]
public class SocketHexDTO : SocketDTOClass
{
    public int id;

    public string hexType;

    public int numberToken;
}

[Serializable]
public class SocketBroadcastUserTurnDTO : SocketDTOClass
{
    public int userId;

    public int numOfTurn;
}

[SerializeField]
public class SocketRequestBuildDTO : SocketDTOClass
{
    public int fieldId;
}

[SerializeField]
public class SocketBroadcastBuildDTO : SocketDTOClass
{
    public int userId;

    public int fieldId;
}