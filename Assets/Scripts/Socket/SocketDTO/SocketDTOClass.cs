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
    public string token = ConfigVariables.TOKEN;
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

    public int seed;

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

[SerializeField]
public class SocketBroadcastDiceThrowDTO : SocketDTOClass
{
    public int userId;

    public int firstDiceNum;

    public int secondDiceNum;
}

[SerializeField]
public class SocketBroadcastResourcesDTO : SocketDTOClass
{
    public int userId;

    public List<string> resources;
}

[SerializeField]
public class SocketRequestTradeDTO : SocketDTOClass
{
    public int requestedAmountOfBuyResource;

    public string sellResource;

    public string buyResource;
}

[SerializeField]
public class SocketBroadcastTradeDTO : SocketDTOClass
{
    public int userId;

    public string userSellResource;

    public string userBuyResource;

    public int requestedAmountOfBuyResource;
}

[SerializeField]
public class SocketRequestUserRobberyDTO : SocketDTOClass
{
    public int victimUserId;

    public int hexId;
}

[SerializeField]
public class SocketBroadcastUserRobberyDTO : SocketDTOClass
{
    public int robberUserId;

    public int victimUserId;

    public int hexId;

    public string resource;
}

[SerializeField]
public class SocketBroadcastBuyCardDTO : SocketDTOClass
{
    public int userId;

    public string card;
}

[SerializeField]
public class SocketRequestUseKnightCardDTO : SocketDTOClass
{
    public int hexId;
}

[SerializeField]
public class SocketRequestUseMonopolyCardDTO : SocketDTOClass
{
    public string resource;
}

[SerializeField]
public class SocketRequestUseYearOfPlentyCardDTO : SocketDTOClass
{
    public List<string> resources;
}

[SerializeField]
public class SocketBroadcastUseYearOfPlentyCardDTO : SocketDTOClass
{
    public int userId;

    public List<string> resources;
}

[SerializeField]
public class SocketBroadcastUseRoadBuildingCardDTO : SocketDTOClass
{
    public int userId;
}

[SerializeField]
public class SocketBroadcastUseMonopolyCardDTO : SocketDTOClass
{
    public int userId;

    public string resource;
}

[SerializeField]
public class SocketBroadcastUseKnightCardDTO : SocketDTOClass
{
    public int userId;

    public int hexId;
}

[SerializeField]
public class SocketBroadcastUserGetLongestRoadDTO : SocketDTOClass
{
    public int userId;
}

[SerializeField]
public class SocketBroadcastUserGetLargestArmyDTO : SocketDTOClass
{
    public int userId;
}

[SerializeField]
public class SocketRequestExchangeOfferDTO : SocketDTOClass
{
    public int targetUserId;

    public int targetAmountOfResource;

    public int initiatorAmountOfResource;

    public string initiatorResource;

    public string targetResource;
}

[SerializeField]
public class SocketResponseExchangeOfferDTO : SocketDTOClass
{
    public int initiatorUserId;

    public int targetAmountOfResource;

    public int initiatorAmountOfResource;

    public string initiatorResource;

    public string targetResource;

    public int exchangeId;
}

[SerializeField]
public class SocketBroadcastExchangeDTO : SocketDTOClass
{
    public int initiatorId;

    public int targetId;

    public int initiatorAmountOfResource;

    public int targetAmountOfResource;

    public string initiatorResource;

    public string targetResource;
}

[SerializeField]
public class SocketRequestExchangeDTO : SocketDTOClass
{
    public int exchangeId;

    public bool isAccept;
}