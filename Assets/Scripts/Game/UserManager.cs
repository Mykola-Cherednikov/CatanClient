using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;



public class UserManager : MonoBehaviour
{
    public List<User> users;
    public User thisUser;

    [SerializeField] private bool isThisUserTurn;

    public int numOfAvailableDevelopmentCard;

    private Dictionary<Card, Func<bool>> cardToSpecificCondition;

    private void Awake()
    {
        isThisUserTurn = false;

        numOfAvailableDevelopmentCard = 5;
        SimixmanLogger.Log("User Manager");

        cardToSpecificCondition = new Dictionary<Card, Func<bool>>() { 
            { Card.KNIGHT, GameManager.Instance.mapManager.IsRobberNearbyThisUser },
            { Card.VICTORY_POINT, VictoryPointCardCondition }
        };
    }

    public void InitializeUsers(SocketBroadcastStartGameDTO dto)
    {
        users = dto.users;
        thisUser = users.FirstOrDefault(u => u.id == dto.currentUser.id);

        JavaRandom random = new JavaRandom(dto.seed);
        foreach (User user in users)
        {
            user.color = new Color(random.NextInt(255) / 255f, random.NextInt(255) / 255f, random.NextInt(255) / 255f);
        }
    }

    public void UpdateUserTurnStatus(User userWhoseTurn)
    {
        if (userWhoseTurn == null)
        {
            isThisUserTurn = false;
            return;
        }

        isThisUserTurn = userWhoseTurn == thisUser;
    }

    public async void UserTurnReady()
    {
        if (!IsThisUserTurn())
        {
            return;
        }

        UpdateUserTurnStatus(null);
        await Multiplayer.Instance.SocketSendUserTurnReadyRequest();
    }

    public User GetUserById(int userId)
    {
        return users.FirstOrDefault(user => user.id == userId);
    }

    public bool IsThisUserTurn()
    {
        return isThisUserTurn;
    }

    #region Check If This User Have Enough Resources
    private bool IsThisUserHaveEnoughResourcesForGoods(Goods goods)
    {
        Dictionary<Resource, int> cost = new();

        switch (goods)
        {
            case Goods.Road:
                cost = ResourceCost.GetRoadCost();
                break;
            case Goods.Settlement:
                cost = ResourceCost.GetSettlementCost();
                break;
            case Goods.City:
                cost = ResourceCost.GetCityCost();
                break;
            case Goods.Card:
                cost = ResourceCost.GetDevelopmentCardCost();
                break;
        }

        foreach (var resourceToCost in cost)
        {
            if (!IsThisUserHaveEnoughResources(resourceToCost))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsThisUserHaveEnoughResources(KeyValuePair<Resource, int> resources)
    {
        if (!IsUserHaveEnoughResources(thisUser, resources))
        {
            return false;
        }

        return true;
    }

    private bool IsUserHaveEnoughResources(User user, KeyValuePair<Resource, int> resources)
    {
        if (user.userResources[resources.Key] < resources.Value)
        {
            return false;
        }

        return true;
    }

    public bool IsThisUserCanTradeNow(Resource sellResource, KeyValuePair<Resource, int> buyResource)
    {
        if (!isThisUserTurn)
        {
            return false;
        }

        int sellAmount = GameManager.Instance.resourceManager.GetAmountOfTradingSellResourceDependOnUserHarbour(
            GameManager.Instance.userManager.thisUser, sellResource, buyResource.Value);

        KeyValuePair<Resource, int> sellResourceAmount = new KeyValuePair<Resource, int>(sellResource, sellAmount);

        if (!GameManager.Instance.resourceManager.IsStorageHaveEnoughResources(buyResource))
        {
            return false;
        }

        if (!IsThisUserHaveEnoughResources(sellResourceAmount))
        {
            return false;
        }

        return true;
    }

    public bool IsThisUserCanExchangeWithUserNow(User targetUser, KeyValuePair<Resource, int> initiatorResources, KeyValuePair<Resource, int> targetResources)
    {
        if(!isThisUserTurn)
        {
            return false;
        }

        if(!IsThisUserHaveEnoughResources(initiatorResources) || !IsUserHaveEnoughResources(targetUser, targetResources))
        {
            return false;
        }

        return true;
    }
    #endregion

    #region Check If User Already Build Something On Preparation
    private bool IsThisUserAlreadyBuildRoadsOnPreparation(int numOfPreparationTurn)
    {
        int userEdgesCount = GameManager.Instance.mapManager.GetUserEdgex(thisUser).Count;

        if (userEdgesCount >= numOfPreparationTurn)
        {
            return true;
        }

        return false;
    }

    private bool IsThisUserAlreadyBuildSettlementOnPreparation(int numOfPreparationTurn)
    {
        int userVerticesCount = GameManager.Instance.mapManager.GetUserVerticies(thisUser).Count;

        if (userVerticesCount >= numOfPreparationTurn)
        {
            return true;
        }

        return false;
    }
    #endregion

    #region Check If This User Can Build One Of The Buildings
    public bool IsThisUserCanBuildRoadNow()
    {
        if (!isThisUserTurn)
        {
            return false;
        }


        if (GameManager.Instance.gameState == GameState.PREPARATION_BUILD_ROADS)
        {
            if (IsThisUserAlreadyBuildRoadsOnPreparation(GameManager.Instance.numOfTurn))
            {
                return false;
            }

            return true;
        }
        else if (GameManager.Instance.gameState == GameState.USER_TURN)
        {
            if (!IsThisUserHaveEnoughResourcesForGoods(Goods.Road) && thisUser.buffDuration[Buff.ROAD_BUILDING] <= 0)
            {
                return false;
            }

            return true;
        }

        return false;
    }

    public bool IsThisUserCanBuildSettlementNow()
    {
        if (!isThisUserTurn)
        {
            return false;
        }

        if (GameManager.Instance.gameState == GameState.PREPARATION_BUILD_SETTLEMENTS)
        {
            if (IsThisUserAlreadyBuildSettlementOnPreparation(GameManager.Instance.numOfTurn))
            {
                return false;
            }

            return true;
        }
        else if (GameManager.Instance.gameState == GameState.USER_TURN)
        {
            if (!IsThisUserHaveEnoughResourcesForGoods(Goods.Settlement))
            {
                return false;
            }

            return true;
        }

        return false;
    }

    public bool IsThisUserCanBuildCityNow()
    {
        if (GameManager.Instance.gameState == GameState.USER_TURN)
        {
            if (!isThisUserTurn)
            {
                return false;
            }

            if (!IsThisUserHaveEnoughResourcesForGoods(Goods.City))
            {
                return false;
            }

            return true;
        }

        return false;
    }
    #endregion

    public bool IsThisUserCanPlaceRobberNow()
    {
        if (!isThisUserTurn)
        {
            return false;
        }

        if (GameManager.Instance.gameState != GameState.ROBBERY)
        {
            return false;
        }

        return true;
    }

    #region Check User Card
    public bool IsThisUserCanBuyCardNow()
    {
        if (!isThisUserTurn)
        {
            return false;
        }

        if(GameManager.Instance.gameState != GameState.USER_TURN)
        {
            return false;
        }

        if (!IsThisUserHaveEnoughResourcesForGoods(Goods.Card))
        {
            return false;
        }

        return true;
    }

    private bool IsThisUserHaveCard(Card card)
    {
        return thisUser.userCards[card] > 0;
    }

    public bool IsThisUserCanUseCardNow(Card card)
    {
        if (!isThisUserTurn)
        {
            return false;
        }

        if (GameManager.Instance.gameState != GameState.USER_TURN)
        {
            return false;
        }

        if(!IsThisUserHaveCard(card))
        {
            return false;
        }

        if (!IsCardConditionSuccess(card))
        {
            return false;
        }

        return true;
    }

    private bool IsCardConditionSuccess(Card card)
    {
        if(!cardToSpecificCondition.ContainsKey(card))
        {
            return true;
        }

        if (!cardToSpecificCondition[card].Invoke())
        {
            return false;
        }

        return true;
    }

    private bool VictoryPointCardCondition()
    {
        return false;
    }
    #endregion

    public int GetUserVictoryPoints(User user)
    {
        int victoryPoint = 0;

        List<Vertex> vertices = GameManager.Instance.mapManager.GetUserVerticies(user);

        foreach (Vertex v in vertices)
        {
            if(v.type == VertexBuildingType.SETTLEMENT)
            {
                victoryPoint++;
            }
            else if(v.type==VertexBuildingType.CITY)
            {
                victoryPoint += 2;
            }
        }

        victoryPoint += user.isLongestRoad ? 2 : 0;

        victoryPoint += user.isLargestArmy ? 2 : 0;

        victoryPoint += user.userCards[Card.VICTORY_POINT];

        return victoryPoint;
    }

    public void SetLongestRoadToUserAndClearFromOthers(User u)
    {
        foreach(var user in users)
        {
            user.isLongestRoad = false;
        }

        u.isLongestRoad = true;
    }

    public void SetLargestArmyToUserAndClearFromOthers(User u)
    {
        foreach (var user in users)
        {
            user.isLargestArmy = false;
        }

        u.isLargestArmy = true;
    }
}
