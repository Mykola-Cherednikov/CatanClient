using System.Collections.Generic;
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

    public bool IsStorageHaveEnoughResources(KeyValuePair<Resource, int> resourceAmount)
    {
        return storage[resourceAmount.Key] >= resourceAmount.Value;
    }

    public void AddResourcesToUserAsGathering(User user, Dictionary<Resource, int> resourcesToAmount)
    {
        foreach (var resourceToAmount in resourcesToAmount)
        {
            AddResourceToUserAndRemoveFromStorage(user, resourceToAmount);
        }
        CallUserResourceChangedEvent();
    }

    public void AddResourcesToUserAsTrade(User user, Resource buyResource, int amount)
    {
        KeyValuePair<Resource, int> resourceToAmount = new(buyResource, amount);
        AddResourceToUserAndRemoveFromStorage(user, resourceToAmount);
        CallUserResourceChangedEvent();
    }

    public void AddResourcesToUserAsYearOfPlenty(User user, Dictionary<Resource, int> resourcesToAmount)
    {
        foreach (var resourceToAmount in resourcesToAmount)
        {
            AddResourceToUserAndRemoveFromStorage(user, resourceToAmount);
        }
        CallUserResourceChangedEvent();
    }

    public void MoveResourcesToUserAsMonopoly(User user, Resource resource)
    {
        int numOfResources = 0;
        foreach (var u in GameManager.Instance.userManager.users)
        {
            if (u == user)
            {
                continue;
            }
            else
            {
                numOfResources += u.userResources[resource];
                KeyValuePair<Resource, int> removeResources = new KeyValuePair<Resource, int>(resource, u.userResources[resource]);
                RemoveResourceFromUserAsMonopoly(u, removeResources);
            }
        }
        KeyValuePair<Resource, int> getResources = new KeyValuePair<Resource, int>(resource, numOfResources);
        AddResourceToUserAsMonopoly(user, getResources);
        CallUserResourceChangedEvent();
    }

    private void AddResourceToUserAsMonopoly(User user, KeyValuePair<Resource, int> resourceToAmount)
    {
        user.userResources[resourceToAmount.Key] += resourceToAmount.Value;
    }

    private void AddResourceToUserAndRemoveFromStorage(User user, KeyValuePair<Resource, int> resourceToAmount)
    {
        user.userResources[resourceToAmount.Key] += resourceToAmount.Value;
        storage[resourceToAmount.Key] -= resourceToAmount.Value;
    }

    private void RemoveResourceFromUserAsMonopoly(User user, KeyValuePair<Resource, int> resourceToAmount)
    {
        user.userResources[resourceToAmount.Key] -= resourceToAmount.Value;
    }

    public void RemoveResourcesFromUserAsBuying(User user, Goods goods)
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

    public void RemoveResourcesFromUserAsTrade(User user, Resource sellResource, int buyAmount)
    {
        int sellAmount = GetAmountOfSellResourceDependOnUserHarbour(user, sellResource, buyAmount);
        KeyValuePair<Resource, int> resourceToAmount = new(sellResource, sellAmount);

        RemoveResourceFromUserAndAddToStorage(user, resourceToAmount);
        CallUserResourceChangedEvent();
    }

    public void RemoveResourcesFromUserAsRobberRobbery(User user, Dictionary<Resource, int> resourcesToAmount)
    {
        foreach (var resourceToAmount in resourcesToAmount)
        {
            RemoveResourceFromUserAndAddToStorage(user, resourceToAmount);
        }
        CallUserResourceChangedEvent();
    }

    private void RemoveResourceFromUserAndAddToStorage(User user, KeyValuePair<Resource, int> resourceToAmount)
    {
        user.userResources[resourceToAmount.Key] -= resourceToAmount.Value;
        storage[resourceToAmount.Key] += resourceToAmount.Value;
    }

    public async void UserTradeRequest(Resource sellResource, Resource buyResource, int buyAmount)
    {
        int sellAmount = GetAmountOfSellResourceDependOnUserHarbour(GameManager.Instance.userManager.currentUser,
            sellResource, buyAmount);

        KeyValuePair<Resource, int> sellResourceAmount = new KeyValuePair<Resource, int>(sellResource, sellAmount);
        KeyValuePair<Resource, int> buyResourceAmount = new KeyValuePair<Resource, int>(buyResource, buyAmount);

        if (!GameManager.Instance.userManager.IsCurrentUserCanTradeNow(sellResourceAmount, buyResourceAmount))
        {
            return;
        }

        await Multiplayer.Instance.SocketSendUserTradeRequest(sellResource, buyResource, buyAmount);
    }

    public async void UserExchangeRequest(Resource initiatorResource, int initiatorResourceAmount, User targetUser, Resource targetResource, int targetResourceAmount)
    {
        KeyValuePair<Resource, int> initiatorResources = new KeyValuePair<Resource, int>(initiatorResource, initiatorResourceAmount);
        KeyValuePair<Resource, int> targetResources = new KeyValuePair<Resource, int>(targetResource, targetResourceAmount);

        if(!GameManager.Instance.userManager.IsCurrentUserCanExchangeWithUserNow(targetUser, initiatorResources, targetResources))
        {
            return;
        }

        await Multiplayer.Instance.SocketSendExchangeOfferRequest(initiatorResource, 
            initiatorResourceAmount, targetUser.id, targetResource, targetResourceAmount);
    }

    public void UserRobberResourcesFromAnotherUser(User robberUser, User victimUser, Resource resource)
    {
        victimUser.userResources[resource]--;
        robberUser.userResources[resource]++;
    }

    private void CallUserResourceChangedEvent()
    {
        RESOURCES_CHANGED_EVENT?.Invoke();
    }

    public int GetAmountOfSellResourceDependOnUserHarbour(User user, Resource sellResource, int buyAmount)
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
}
