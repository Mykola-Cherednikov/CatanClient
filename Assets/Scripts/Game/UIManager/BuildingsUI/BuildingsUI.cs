using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField] private GameObject roadGO;
    [SerializeField] private GameObject settlementGO;
    [SerializeField] private GameObject cityGO;
    [SerializeField] private GameObject robberGO;
    [SerializeField] private TMP_Text hideAndShowButtonText;
    private RectTransform rect;
    private bool isHiden;

    private void Awake()
    {
        isHiden = false;
        rect = GetComponent<RectTransform>();
    }

    private GameObject CreateNewBuildingGameObject(GameObject prefab, Transform transformPosition, UnityAction onBeginDragCreateNew,
        UnityAction onBeginDragShowAllPlaces, UnityAction onDropFindAndBuild, UnityAction onDropHideAll, Func<bool> isAvailableFunc)
    {
        GameObject dragAndDropGO = Instantiate(prefab, transformPosition);
        DragAndDropBuilding dragAndDropBuilding = dragAndDropGO.GetComponent<DragAndDropBuilding>();
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

    public void CreateNewRobberGameObject()
    {
        /*robberGO = CreateNewBuildingGameObject(
            robberPrefab,
            settlementTransformPosition,
            CreateNewRobberGameObject,
            GameManager.Instance.mapManager.ShowPlacesForRobber,
            FindVertexAndBuildCity,
            GameManager.Instance.mapManager.HideAllAvailablePlacesForRobber,
            GameManager.Instance.userManager.IsCurrentUserCanPlaceRobber
        );*/
    }

    public void FindObjectAndBuild<T>(Action<int> planToBuild) where T : PlaceForBuildings
    {
        T objectCollider = GetColliderFromMousePosition<T>();
        if (objectCollider != null)
        {
            int id = objectCollider.id;
            planToBuild(id);
        }
    }

    public void FindEdgeAndBuildRoad()
    {
        FindObjectAndBuild<Edge>(id => GameManager.Instance.mapManager.PlanToBuildRoad(id));
    }

    public void FindVertexAndBuildSettlement()
    {
        FindObjectAndBuild<Vertex>(id => GameManager.Instance.mapManager.PlanToBuildSettlement(id));
    }

    public void FindVertexAndBuildCity()
    {
        FindObjectAndBuild<Vertex>(id => GameManager.Instance.mapManager.PlanToBuildCity(id));
    }

    private T GetColliderFromMousePosition<T>() where T : PlaceForBuildings
    {
        Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        List<RaycastHit2D> hits = Physics2D.BoxCastAll(position, new Vector2(0.5f, 0.5f), 90f, Vector2.zero).ToList();
        RaycastHit2D hit = hits.FirstOrDefault(h => h.collider.GetComponent<T>() != null);
        if (hit.collider == null)
        {
            return null;
        }
        return hit.collider.GetComponent<T>();
    }

    public void HideOrShow()
    {
        float rectHeight = rect.sizeDelta.y - 20f;
        transform.position = new Vector2(transform.position.x, transform.position.y + (rectHeight * (isHiden ? 1 : - 1)));
        hideAndShowButtonText.text = isHiden ? "Hide" : "Show";
        isHiden = !isHiden;
    }

    public void ChangeUIToGameState()
    {
        DestroyAllUI();
        switch(GameManager.Instance.gameState)
        {
            case GameState.PREPARATION_BUILD_SETTLEMENTS:
                ChangeUIToPreparationSettlementState();
                break;
            case GameState.PREPARATION_BUILD_ROADS:
                ChangeUIToPreparationRoadState();
                break;
            case GameState.GAME:
                ChangeUIToMainGameState();
                break;
            case GameState.ROBBERY:
                ChangeCurrentUserUIToRobberyOrRemainInMainGameState();
                break;
        }
    }

    private void ChangeUIToPreparationSettlementState()
    {
        CreateNewSettlementGameObject();
    }

    private void ChangeUIToPreparationRoadState()
    {
        CreateNewRoadGameObject();
    }

    private void ChangeUIToMainGameState()
    {
        CreateNewRoadGameObject();
        CreateNewCityGameObject();
        CreateNewSettlementGameObject();
    }

    private void ChangeCurrentUserUIToRobberyOrRemainInMainGameState()
    {
        if (GameManager.Instance.userManager.IsCurrentUserTurn())
        {
            CreateNewRobberGameObject();
        }
        else
        {
            ChangeUIToMainGameState();
        }
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
    }
}
