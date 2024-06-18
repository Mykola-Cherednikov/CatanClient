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

    public void AddResourcesToUserAsTrade(User user, KeyValuePair<Resource, int> resourceToAmount)
    {
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
        foreach(var u in GameManager.Instance.userManager.users)
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

    public void RemoveResourcesFromUserAsTrade(User user, KeyValuePair<Resource, int> resourceToAmount)
    {
        //Check if user have port with this resource type
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

    public async void UserTradeResource(Resource incomeResource, Resource outgoingResource, int outgoingCount)
    {
        int incomeCount = outgoingCount * 4;

        KeyValuePair<Resource, int> incomeResourceAmount = new KeyValuePair<Resource, int>(incomeResource, incomeCount);
        KeyValuePair<Resource, int> outgoingResourceAmount = new KeyValuePair<Resource, int>(outgoingResource, outgoingCount);

        if (!GameManager.Instance.userManager.IsCurrentUserCanTradeNow(incomeResourceAmount, outgoingResourceAmount))
        {
            return;
        }

        await Multiplayer.Instance.SocketSendUserTradeRequest(incomeResource, outgoingResource, outgoingCount);
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
}
