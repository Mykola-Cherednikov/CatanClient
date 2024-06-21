using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.Events;

public enum Resource
{
    BRICK,
    LUMBER,
    ORE,
    GRAIN,
    WOOL
}

public enum Goods
{
    Road,
    Settlement,
    City,
    Card
}

public class ResourceManager : MonoBehaviour
{
    public Dictionary<Resource, int> storage;

    private Dictionary<Resource, HarborType> resourceTypeToHarborType;

    public UnityAction RESOURCES_CHANGED_EVENT;

    private void Awake()
    {
        storage = new Dictionary<Resource, int>()
        {
            { Resource.WOOL, 20 },
            { Resource.ORE, 20 },
            { Resource.BRICK, 20 },
            { Resource.LUMBER, 20 },
            { Resource.GRAIN, 20 }
        };

        resourceTypeToHarborType = new Dictionary<Resource, HarborType>()
        {
            { Resource.WOOL, HarborType.WOOL },
            { Resource.ORE, HarborType.ORE },
            { Resource.GRAIN, HarborType.GRAIN },
            { Resource.LUMBER, HarborType.LUMBER },
            { Resource.BRICK, HarborType.BRICK }
        };
    }

    #region Gathering
    public void GatheringMoveResources(User user, Dictionary<Resource, int> resourcesToAmount)
    {
        foreach (var resourceToAmount in resourcesToAmount)
        {
            AddResourceToUserAndRemoveFromStorage(user, resourceToAmount);
        }
        CallUserResourceChangedEvent();
    }
    #endregion

    #region Trade
    private void AddResourcesToUserAsTrade(User user, Resource buyResource, int amount)
    {
        KeyValuePair<Resource, int> resourceToAmount = new(buyResource, amount);
        AddResourceToUserAndRemoveFromStorage(user, resourceToAmount);
    }

    private void RemoveResourcesFromUserAsTrade(User user, Resource sellResource, int buyAmount)
    {
        int sellAmount = GetAmountOfTradingSellResourceDependOnUserHarbour(user, sellResource, buyAmount);
        KeyValuePair<Resource, int> resourceToAmount = new(sellResource, sellAmount);
        RemoveResourceFromUserAndAddToStorage(user, resourceToAmount);
    }

    public async void UserTradeRequest(Resource sellResource, Resource buyResource, int buyAmount)
    {
        int sellAmount = GetAmountOfTradingSellResourceDependOnUserHarbour(GameManager.Instance.userManager.currentUser,
            sellResource, buyAmount);

        KeyValuePair<Resource, int> sellResourceAmount = new KeyValuePair<Resource, int>(sellResource, sellAmount);
        KeyValuePair<Resource, int> buyResourceAmount = new KeyValuePair<Resource, int>(buyResource, buyAmount);

        if (!GameManager.Instance.userManager.IsCurrentUserCanTradeNow(sellResourceAmount, buyResourceAmount))
        {
            return;
        }

        await Multiplayer.Instance.SocketSendUserTradeRequest(sellResource, buyResource, buyAmount);
    }

    public void UserTradeMoveResources(User user, Resource buyResource, Resource sellResource, int buyAmount)
    {
        RemoveResourcesFromUserAsTrade(user, sellResource, buyAmount);
        AddResourcesToUserAsTrade(user, buyResource, buyAmount);
        CallUserResourceChangedEvent();
    }
    #endregion

    #region YearOfPlenty Card
    public void YearOfPlentyCardMoveResources(User user, Dictionary<Resource, int> resourcesToAmount)
    {
        foreach (var resourceToAmount in resourcesToAmount)
        {
            AddResourceToUserAndRemoveFromStorage(user, resourceToAmount);
        }
        CallUserResourceChangedEvent();
    }
    #endregion

    #region Monopoly Card
    public void MonopolyCardMoveResources(User userWhichUseCard, Resource resource)
    {
        foreach (var user in GameManager.Instance.userManager.users)
        {
            if (user == userWhichUseCard)
            {
                continue;
            }
            else
            {
                KeyValuePair<Resource, int> removeResources = new KeyValuePair<Resource, int>(resource, user.userResources[resource]);
                RemoveResourcesFromUserAndAddToAnotherUser(user, userWhichUseCard, removeResources);
            }
        }
        CallUserResourceChangedEvent();
    }
    #endregion

    #region Buying
    public void BuyGoods(User user, Goods goods)
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

        foreach (var resourceToAmount in cost)
        {
            RemoveResourceFromUserAndAddToStorage(user, resourceToAmount);
        }
        CallUserResourceChangedEvent();
    }
    #endregion

    #region Robbery
    public void UserRobberyMoveResources(User robberUser, User victimUser, Resource resource)
    {
        RemoveResourcesFromUserAndAddToAnotherUser(victimUser, robberUser, new(resource, 1));
        CallUserResourceChangedEvent();
    }

    public void RobberRobberyMoveResources(User user, Dictionary<Resource, int> resourcesToAmount)
    {
        foreach (var resourceToAmount in resourcesToAmount)
        {
            RemoveResourceFromUserAndAddToStorage(user, resourceToAmount);
        }
        CallUserResourceChangedEvent();
    }
    #endregion

    #region Exchange
    public async void UserExchangeRequest(Resource initiatorResource, int initiatorResourceAmount, User targetUser, Resource targetResource, int targetResourceAmount)
    {
        KeyValuePair<Resource, int> initiatorResources = new KeyValuePair<Resource, int>(initiatorResource, initiatorResourceAmount);
        KeyValuePair<Resource, int> targetResources = new KeyValuePair<Resource, int>(targetResource, targetResourceAmount);

        if (!GameManager.Instance.userManager.IsCurrentUserCanExchangeWithUserNow(targetUser, initiatorResources, targetResources))
        {
            return;
        }

        await Multiplayer.Instance.SocketSendExchangeOfferRequest(initiatorResource,
            initiatorResourceAmount, targetUser.id, targetResource, targetResourceAmount);
    }

    public void UserExchangeMoveResources(User initiatorUser, Resource initiatorResource, int initiatorResourceAmount, User targetUser, Resource targetResource, int targetResourceAmount)
    {
        KeyValuePair<Resource, int> initiatorResources = new(initiatorResource, initiatorResourceAmount);
        KeyValuePair<Resource, int> targetResources = new(targetResource, targetResourceAmount);
        RemoveResourcesFromUserAndAddToAnotherUser(initiatorUser, targetUser, initiatorResources);
        RemoveResourcesFromUserAndAddToAnotherUser(targetUser, initiatorUser, targetResources);

        CallUserResourceChangedEvent();
    }
    #endregion

    #region Resource Operations
    private void AddResourceToUserAndRemoveFromStorage(User user, KeyValuePair<Resource, int> resourceToAmount)
    {
        user.userResources[resourceToAmount.Key] += resourceToAmount.Value;
        storage[resourceToAmount.Key] -= resourceToAmount.Value;
    }

    private void RemoveResourcesFromUserAndAddToAnotherUser(User takeFromUser, User getToUser, KeyValuePair<Resource, int> resourceToAmount)
    {
        takeFromUser.userResources[resourceToAmount.Key] -= resourceToAmount.Value;
        getToUser.userResources[resourceToAmount.Key] += resourceToAmount.Value;
    }

    private void RemoveResourceFromUserAndAddToStorage(User user, KeyValuePair<Resource, int> resourceToAmount)
    {
        user.userResources[resourceToAmount.Key] -= resourceToAmount.Value;
        storage[resourceToAmount.Key] += resourceToAmount.Value;
    }
    #endregion

    private void CallUserResourceChangedEvent()
    {
        RESOURCES_CHANGED_EVENT?.Invoke();
    }

    public int GetAmountOfTradingSellResourceDependOnUserHarbour(User user, Resource sellResource, int buyAmount)
    {
        if (GameManager.Instance.mapManager.IsUserHaveHarbourWithType(user, resourceTypeToHarborType[sellResource]))
        {
            buyAmount *= 2;
        }
        else if (GameManager.Instance.mapManager.IsUserHaveHarbourWithType(user, HarborType.GENERIC))
        {
            buyAmount *= 3;
        }
        else
        {
            buyAmount *= 4;
        }

        return buyAmount;
    }

    public bool IsStorageHaveEnoughResources(KeyValuePair<Resource, int> resourceAmount)
    {
        return storage[resourceAmount.Key] >= resourceAmount.Value;
    }
}
