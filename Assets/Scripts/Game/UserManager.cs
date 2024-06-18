using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;



public class UserManager : MonoBehaviour
{
    public List<User> users;
    public User currentUser;

    [SerializeField] private bool isCurrentUserTurn;

    public int numOfAvailableDevelopmentCard;

    private Dictionary<Card, Func<bool>> cardToSpecificCondition;

    private void Awake()
    {
        isCurrentUserTurn = true;

        numOfAvailableDevelopmentCard = 5;
        Debug.Log("User Manager");

        cardToSpecificCondition = new Dictionary<Card, Func<bool>>() { {Card.KNIGHT, GameManager.Instance.mapManager.IsRobberNearbyCurrentUser } };
    }

    public void InitializeUsers(SocketBroadcastStartGameDTO dto)
    {
        users = dto.users;
        currentUser = users.FirstOrDefault(u => u.id == dto.currentUser.id);

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
            isCurrentUserTurn = false;
        }

        isCurrentUserTurn = userWhoseTurn == currentUser;
    }

    public async void UserTurnReady()
    {
        if (!IsCurrentUserTurn())
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

    public bool IsCurrentUserTurn()
    {
        return isCurrentUserTurn;
    }

    #region Check If Current User Have Enough Resources
    private bool IsCurrentUserHaveEnoughResourcesForGoods(Goods goods)
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
            if (!IsCurrentUserHaveEnoughResources(resourceToCost))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsCurrentUserHaveEnoughResources(KeyValuePair<Resource, int> resources)
    {
        if (currentUser.userResources[resources.Key] < resources.Value)
        {
            return false;
        }

        return true;
    }

    public bool IsCurrentUserCanTradeNow(KeyValuePair<Resource, int> incomeResource, KeyValuePair<Resource, int> outgoingResource)
    {
        if (!isCurrentUserTurn)
        {
            return false;
        }

        //Check if user have port with this resource type

        if (!GameManager.Instance.resourceManager.IsStorageHaveEnoughResources(outgoingResource))
        {
            return false;
        }

        if (!IsCurrentUserHaveEnoughResources(incomeResource))
        {
            return false;
        }

        return true;
    }
    #endregion

    #region Check If User Already Build Something On Preparation
    private bool IsCurrentUserAlreadyBuildRoadsOnPreparation(int numOfPreparationTurn)
    {
        int userEdgesCount = GameManager.Instance.mapManager.GetUserEdgex(currentUser).Count;

        if (userEdgesCount >= numOfPreparationTurn)
        {
            return true;
        }

        return false;
    }

    private bool IsCurrentUserAlreadyBuildSettlementOnPreparation(int numOfPreparationTurn)
    {
        int userVerticesCount = GameManager.Instance.mapManager.GetUserVerticies(currentUser).Count;

        if (userVerticesCount >= numOfPreparationTurn)
        {
            return true;
        }

        return false;
    }
    #endregion

    #region Check If Current User Can Build One Of The Buildings
    public bool IsCurrentUserCanBuildRoadNow()
    {
        if (!isCurrentUserTurn)
        {
            return false;
        }


        if (GameManager.Instance.gameState == GameState.PREPARATION_BUILD_ROADS)
        {
            if (IsCurrentUserAlreadyBuildRoadsOnPreparation(GameManager.Instance.numOfTurn))
            {
                return false;
            }

            return true;
        }
        else if (GameManager.Instance.gameState == GameState.USER_TURN)
        {
            if (!IsCurrentUserHaveEnoughResourcesForGoods(Goods.Road))
            {
                return false;
            }

            return true;
        }

        return false;
    }

    public bool IsCurrentUserCanBuildSettlementNow()
    {
        if (!isCurrentUserTurn)
        {
            return false;
        }

        if (GameManager.Instance.gameState == GameState.PREPARATION_BUILD_SETTLEMENTS)
        {
            if (IsCurrentUserAlreadyBuildSettlementOnPreparation(GameManager.Instance.numOfTurn))
            {
                return false;
            }

            return true;
        }
        else if (GameManager.Instance.gameState == GameState.USER_TURN)
        {
            if (!IsCurrentUserHaveEnoughResourcesForGoods(Goods.Settlement))
            {
                return false;
            }

            return true;
        }

        return false;
    }

    public bool IsCurrentUserCanBuildCityNow()
    {
        if (GameManager.Instance.gameState == GameState.USER_TURN)
        {
            if (!isCurrentUserTurn)
            {
                return false;
            }

            if (!IsCurrentUserHaveEnoughResourcesForGoods(Goods.City))
            {
                return false;
            }

            return true;
        }

        return false;
    }
    #endregion

    public bool IsCurrentUserCanPlaceRobberNow()
    {
        if (!isCurrentUserTurn)
        {
            return false;
        }

        if (GameManager.Instance.gameState != GameState.ROBBERY)
        {
            return false;
        }

        return true;
    }

    public bool IsCurrentUserCanBuyCardNow()
    {
        if (!isCurrentUserTurn)
        {
            return false;
        }

        if(GameManager.Instance.gameState != GameState.USER_TURN)
        {
            return false;
        }

        if (!IsCurrentUserHaveEnoughResourcesForGoods(Goods.Card))
        {
            return false;
        }

        return true;
    }

    private bool IsCurrentUserHaveCard(Card card)
    {
        return currentUser.userCards[card] > 0;
    }

    public bool IsCurrentUserCanUseCardNow(Card card)
    {
        if (!isCurrentUserTurn)
        {
            return false;
        }

        if (GameManager.Instance.gameState != GameState.USER_TURN)
        {
            return false;
        }

        if(!IsCurrentUserHaveCard(card))
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
}
