using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BuildingsUI : MonoBehaviour
{
    public bool isDragging;
    public GameObject infoMenuGO;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private GameObject settlementPrefab;
    [SerializeField] private GameObject cityPrefab;
    [SerializeField] private GameObject robberPrefab;
    [SerializeField] private Transform roadTransformPosition;
    [SerializeField] private Transform settlementTransformPosition;
    [SerializeField] private Transform cityTransformPosition;
    private GameObject roadGO;
    private GameObject settlementGO;
    private GameObject cityGO;
    private GameObject robberGO;
    [SerializeField] private TMP_Text hideAndShowButtonText;
    private RectTransform rect;
    private bool isHiden;

    private void Awake()
    {
        isHiden = false;
        rect = GetComponent<RectTransform>();
        GameManager.Instance.uiManager.CHANGE_UI_STATE += ChangeUIToAnotherGameState;
    }

    #region Create Drag And Drop Object
    private GameObject CreateNewBuildingGameObject(GameObject prefab, Transform transformPosition, UnityAction onBeginDragCreateNew,
        UnityAction onBeginDragShowAllPlaces, UnityAction onDropFindAndBuild, UnityAction onDropHideAll, Func<bool> isAvailableFunc)
    {
        GameObject dragAndDropGO = Instantiate(prefab, transformPosition);
        DragAndDropObject dragAndDropBuilding = dragAndDropGO.GetComponent<DragAndDropObject>();
        dragAndDropBuilding.buildingsUI = this;
        dragAndDropBuilding.onBeginDrag.AddListener(onBeginDragCreateNew);
        dragAndDropBuilding.onBeginDrag.AddListener(onBeginDragShowAllPlaces);
        dragAndDropBuilding.onDrop.AddListener(onDropFindAndBuild);
        dragAndDropBuilding.onDrop.AddListener(onDropHideAll);
        dragAndDropBuilding.isAvailable = isAvailableFunc;

        return dragAndDropGO;
    }

    public void CreateNewRoadGameObject()
    {
        roadGO = CreateNewBuildingGameObject(
            roadPrefab,
            roadTransformPosition,
            CreateNewRoadGameObject,
            GameManager.Instance.mapManager.ShowPlacesForRoads,
            FindEdgeAndBuildRoad,
            GameManager.Instance.mapManager.HideAllAvailablePlaces,
            GameManager.Instance.userManager.IsCurrentUserCanBuildRoadNow
        );
    }

    public void CreateNewSettlementGameObject()
    {
        settlementGO = CreateNewBuildingGameObject(
            settlementPrefab,
            settlementTransformPosition,
            CreateNewSettlementGameObject,
            GameManager.Instance.mapManager.ShowPlacesForSettlements,
            FindVertexAndBuildSettlement,
            GameManager.Instance.mapManager.HideAllAvailablePlaces,
            GameManager.Instance.userManager.IsCurrentUserCanBuildSettlementNow
        );
    }

    public void CreateNewCityGameObject()
    {
        cityGO = CreateNewBuildingGameObject(
            cityPrefab,
            cityTransformPosition,
            CreateNewCityGameObject,
            GameManager.Instance.mapManager.ShowPlacesForCities,
            FindVertexAndBuildCity,
            GameManager.Instance.mapManager.HideAllAvailablePlaces,
            GameManager.Instance.userManager.IsCurrentUserCanBuildCityNow
        );
    }

    public void CreateNewRobberForRobberyGameObject()
    {
        robberGO = CreateNewBuildingGameObject(
            robberPrefab,
            settlementTransformPosition,
            CreateNewRobberForRobberyGameObject,
            GameManager.Instance.mapManager.ShowPlacesForRobber,
            FindNumberTokenAndPlaceRobberForRob,
            GameManager.Instance.mapManager.HideAllPlacesForRobber,
            GameManager.Instance.userManager.IsCurrentUserCanPlaceRobberNow
        );
    }

    public void CreateNewRobberForMovingGameObject()
    {
        robberGO = CreateNewBuildingGameObject(
            robberPrefab,
            settlementTransformPosition,
            CreateNewRobberForMovingGameObject,
            GameManager.Instance.mapManager.ShowPlacesForRobber,
            FindNumberTokenAndPlaceRobberForMove,
            GameManager.Instance.mapManager.HideAllPlacesForRobber,
            () => { return GameManager.Instance.userManager.IsCurrentUserCanUseCardNow(Card.KNIGHT); }
        );
    }
    #endregion

    #region Find Place And Do Action
    public void FindObjectAndDoAction<T>(UnityAction<int> planToBuild) where T : Place
    {
        T objectCollider = GetColliderFromMousePositionIfNotPlacedAtDragAndDropBlockPlace<T>();

        if (objectCollider != null)
        {
            int id = objectCollider.id;
            planToBuild(id);
        }
    }

    private T GetColliderFromMousePositionIfNotPlacedAtDragAndDropBlockPlace<T>() where T : Place
    {
        if (IsUIBlockPlacement())
        {
            return default;
        }

        Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        List<RaycastHit2D> hits = Physics2D.BoxCastAll(position, new Vector2(0.5f, 0.5f), 90f, Vector2.zero).ToList();
        RaycastHit2D hit = hits.FirstOrDefault(h => h.collider.GetComponent<T>() != null);
        if (hit.collider == null)
        {
            return default;
        }
        return hit.collider.GetComponent<T>();
    }

    public void FindEdgeAndBuildRoad()
    {
        FindObjectAndDoAction<Edge>(id => GameManager.Instance.mapManager.PlanToBuildRoadAndEndTurnIfPlaceOnPreparation(id));
    }

    public void FindVertexAndBuildSettlement()
    {
        FindObjectAndDoAction<Vertex>(id => GameManager.Instance.mapManager.PlanToBuildSettlementAndEndTurnIfPlaceOnPreparation(id));
    }

    public void FindVertexAndBuildCity()
    {
        FindObjectAndDoAction<Vertex>(id => GameManager.Instance.mapManager.PlanToBuildCity(id));
    }

    public void FindNumberTokenAndPlaceRobberForRob()
    {
        FindObjectAndDoAction<NumberToken>(id => SelectPlaceForRobberAndSelectVictimUser(id));
    }

    public void FindNumberTokenAndPlaceRobberForMove()
    {
        FindObjectAndDoAction<NumberToken>(id => StopMoveRobberAndSelectPlaceForRobber(id));
    }
    #endregion

    private void SelectPlaceForRobberAndSelectVictimUser(int hexId)
    {
        if (!GameManager.Instance.userManager.IsCurrentUserCanPlaceRobberNow())
        {
            return;
        }

        Hex hex = GameManager.Instance.mapManager.GetHexById(hexId);
        List<User> uniqueUsers = MapUtils.GetUniqueUsersInHex(hex);
        uniqueUsers.Remove(GameManager.Instance.userManager.currentUser);

        switch (uniqueUsers.Count)
        {
            case > 1:
                GameManager.Instance.uiManager.windowUI.OpenRobberyFormWithUniqueUsers(hexId, uniqueUsers);
                break;
            case 1:
                GameManager.Instance.mapManager.PlanPlaceRobberAndPlanToRobUser(hexId, uniqueUsers[0].id);
                break;
            default:
                GameManager.Instance.mapManager.PlanPlaceRobberAndPlanToRobUser(hexId, -1);
                break;
        }
    }

    private void StopMoveRobberAndSelectPlaceForRobber(int hexId)
    {
        GameManager.Instance.uiManager.StopUserMoveRobberState();
        GameManager.Instance.cardManager.PlanUseKnightCard(hexId);
    }

    public void CancelMoveRobber()
    {
        GameManager.Instance.uiManager.StopUserMoveRobberState();
    }

    public void HideOrShow()
    {
        float rectHeight = rect.sizeDelta.y - 20f;
        transform.position = new Vector2(transform.position.x, transform.position.y + (rectHeight * (isHiden ? 1 : -1)));
        hideAndShowButtonText.text = isHiden ? "Hide" : "Show";
        isHiden = !isHiden;
    }

    #region Change UI
    public void ChangeUIToAnotherGameState()
    {
        switch (GameManager.Instance.uiManager.uiState)
        {
            case UIState.PREPARATION_BUILD_SETTLEMENTS:
                ChangeUIToPreparationSettlementState();
                break;
            case UIState.PREPARATION_BUILD_ROADS:
                ChangeUIToPreparationRoadState();
                break;
            case UIState.USER_TURN:
                ChangeUIToMainGameState();
                break;
            case UIState.USER_ROBBERY:
                ChangeUIToUserRobbery();
                break;
            case UIState.USER_MOVING_ROBBER:
                ChangeUIToMoveRobbery();
                break;

        }
    }

    private void ChangeUIToPreparationSettlementState()
    {
        DestroyAllUI();
        CreateNewSettlementGameObject();
    }

    private void ChangeUIToPreparationRoadState()
    {
        DestroyAllUI();
        CreateNewRoadGameObject();
    }

    private void ChangeUIToMainGameState()
    {
        DestroyAllUI();
        CreateNewRoadGameObject();
        CreateNewCityGameObject();
        CreateNewSettlementGameObject();
    }

    private void ChangeUIToUserRobbery()
    {
        DestroyAllUI();
        CreateNewRobberForRobberyGameObject();
    }

    private void ChangeUIToMoveRobbery()
    {
        DestroyAllUI();
        CreateNewRobberForMovingGameObject();
    }

    private void DestroyAllUI()
    {
        if (roadGO != null)
        {
            Destroy(roadGO);
        }
        if (settlementGO != null)
        {
            Destroy(settlementGO);
        }
        if (cityGO != null)
        {
            Destroy(cityGO);
        }
        if (robberGO != null)
        {
            Destroy(robberGO);
        }
    }
    #endregion

    private bool IsUIBlockPlacement()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> uiRaycastResults = new();
        EventSystem.current.RaycastAll(pointerEventData, uiRaycastResults);
        uiRaycastResults.RemoveAll(u => u.gameObject.GetComponent<DragAndDropObject>() != null);
        uiRaycastResults.RemoveAll(u => u.gameObject.GetComponent<GameNotificationText>() != null);

        if (uiRaycastResults.Count > 0)
        {
            return true;
        }

        return false;
    }

    private void OnDestroy()
    {
        GameManager.Instance.uiManager.CHANGE_UI_STATE -= ChangeUIToAnotherGameState;
    }
}
