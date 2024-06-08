using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    private List<User> users;
    public User currentUser;

    [SerializeField] private bool isCurrentUserTurn;

    private void Awake()
    {
        isCurrentUserTurn = true;
    }

    public void InitializeUsers(SocketBroadcastStartGameDTO dto)
    {
        users = dto.users;
        currentUser = users.FirstOrDefault(u => u.id == dto.currentUser.id);
    }

    public void UpdateUserTurnStatus(User userWhoseTurn)
    {
        isCurrentUserTurn = userWhoseTurn == currentUser;
    }

    public User GetUserById(int userId)
    {
        return users.FirstOrDefault(user => user.id == userId);
    }

    #region Check If Current User Have Enough Resources For Buildings
    private bool isCurrentUserHaveEnoughResourcesForRoad()
    {
        return true;
    }

    private bool isCurrentUserHaveEnoughResourcesForSettlement()
    {
        return true;
    }

    private bool isCurrentUserHaveEnoughResourcesForCity()
    {
        return true;
    }
    #endregion

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

        if (!isCurrentUserHaveEnoughResourcesForRoad())
        {
            return false;
        }

        return true;
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

        if (!isCurrentUserHaveEnoughResourcesForSettlement())
        {
            return false;
        }

        return true;
    }

    public bool IsCurrentUserCanBuildCityNow()
    {
        if (!isCurrentUserTurn)
        {
            return false;
        }

        if (!isCurrentUserHaveEnoughResourcesForCity())
        {
            return false;
        }

        return true;
    }
    #endregion
}
