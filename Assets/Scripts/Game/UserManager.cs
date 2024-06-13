using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum Goods
{
    Road,
    Settlement,
    City,
    DevelopmentCard
}

public class UserManager : MonoBehaviour
{
    public List<User> users;
    public User currentUser;

    [SerializeField] private bool isCurrentUserTurn;
    public Dictionary<Resource, int> storage;
    public int numOfAvailableDevelopmentCard;

    public UnityEvent CURRENT_USER_RESOURCES_CHANGED_EVENT;

    private void Awake()
    {
        isCurrentUserTurn = true;
        storage = new Dictionary<Resource, int>()
        {
            { Resource.WOOL, 20 },
            { Resource.ORE, 20 },
            { Resource.BRICK, 20 },
            { Resource.LUMBER, 20 },
            { Resource.GRAIN, 20 }
        };
        numOfAvailableDevelopmentCard = 5;
    }

    public void InitializeUsers(SocketBroadcastStartGameDTO dto)
    {
        users = dto.users;
        currentUser = users.FirstOrDefault(u => u.id == dto.currentUser.id);

        /*List<Color> colors = new List<Color>() { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.white };
        Queue<Color> colorQueue = new Queue<Color>(colors);
        foreach (User user in users)
        {
            user.color = colorQueue.Dequeue();
        }*/

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

    private bool IsStorageHaveEnoughResources(KeyValuePair<Resource, int> resourceAmount)
    {
        return storage[resourceAmount.Key] >= resourceAmount.Value;
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
            case Goods.DevelopmentCard:
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

    public bool IsCurrentUserCanTrade(KeyValuePair<Resource, int> incomeResource, KeyValuePair<Resource, int> outgoingResource)
    {
        //Check if user have port with this resource type
        if (!IsStorageHaveEnoughResources(outgoingResource))
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
        else if (GameManager.Instance.gameState == GameState.GAME)
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
        else if (GameManager.Instance.gameState == GameState.GAME)
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
        if (GameManager.Instance.gameState == GameState.GAME)
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

    #region Resource Operations
    public void AddResourcesToUserAsGathering(User user, Dictionary<Resource, int> resourcesToAmount)
    {
        foreach (var resourceToAmount in resourcesToAmount)
        {
            AddResourceToUserAndRemoveFromStorage(user, resourceToAmount);
        }
    }

    public void AddResourcesToUserAsTrade(User user, KeyValuePair<Resource, int> resourceToAmount)
    {
        AddResourceToUserAndRemoveFromStorage(user, resourceToAmount);
    }

    private void AddResourceToUserAndRemoveFromStorage(User user, KeyValuePair<Resource, int> resourceToAmount)
    {
        user.userResources[resourceToAmount.Key] += resourceToAmount.Value;
        storage[resourceToAmount.Key] -= resourceToAmount.Value;
        CallCurrentUserResourceChangedEvent(user);
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
            case Goods.DevelopmentCard:
                cost = ResourceCost.GetDevelopmentCardCost();
                break;
        }

        foreach (var resourceToAmount in cost)
        {
            RemoveResourceFromUserAndAddToStorage(user, resourceToAmount);
        }
    }

    public void RemoveResourcesFromUserAsTrade(User user, KeyValuePair<Resource, int> resourceToAmount)
    {
        //Check if user have port with this resource type
        RemoveResourceFromUserAndAddToStorage(user, resourceToAmount);
    }

    public void RemoveResourcesFromUserAsRobberRobbery(User user, Dictionary<Resource, int> resourcesToAmount)
    {
        foreach (var resourceToAmount in resourcesToAmount)
        {
            RemoveResourceFromUserAndAddToStorage(user, resourceToAmount);
        }
    }

    private void RemoveResourceFromUserAndAddToStorage(User user, KeyValuePair<Resource, int> resourceToAmount)
    {
        user.userResources[resourceToAmount.Key] -= resourceToAmount.Value;
        storage[resourceToAmount.Key] += resourceToAmount.Value;
        CallCurrentUserResourceChangedEvent(user);
    }

    public async void UserTradeResource(Resource incomeResource, Resource outgoingResource, int outgoingCount)
    {
        int incomeCount = outgoingCount * 4;

        if (!IsCurrentUserTurn())
        {
            return;
        }

        KeyValuePair<Resource, int> incomeResourceAmount = new KeyValuePair<Resource, int>(incomeResource, incomeCount);
        KeyValuePair<Resource, int> outgoingResourceAmount = new KeyValuePair<Resource, int>(outgoingResource, outgoingCount);

        if (!IsCurrentUserCanTrade(incomeResourceAmount, outgoingResourceAmount))
        {
            return;
        }

        await Multiplayer.Instance.SocketSendUserTradeRequest(incomeResource, outgoingResource, outgoingCount);
    }
    #endregion

    public bool IsCurrentUserCanPlaceRobber()
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

    private void CallCurrentUserResourceChangedEvent(User u)
    {
        if (u == currentUser)
        {
            CURRENT_USER_RESOURCES_CHANGED_EVENT.Invoke();
        }
    }
}
