using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BuildingsUI : MonoBehaviour
{
    public bool isDragging;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private GameObject settlementPrefab;
    [SerializeField] private GameObject cityPrefab;
    [SerializeField] private Transform roadTransformPosition;
    [SerializeField] private Transform settlementTransformPosition;
    [SerializeField] private Transform cityTransformPosition;

    private void Awake()
    {
        CreateNewRoadGameObject();
        CreateNewSettlementGameObject();
        CreateNewCityGameObject();
    }

    private void CreateNewBuildingGameObject(GameObject prefab, Transform transformPosition, UnityAction onBeginDragCreateNew, 
        UnityAction onBeginDragShowAll, UnityAction onDropFindAndBuild, UnityAction onDropHideAll, Func<bool> isAvaliableFunc)
    {
        DragAndDropBuilding dragAndDropBuilding = Instantiate(prefab, transformPosition).GetComponent<DragAndDropBuilding>();
        dragAndDropBuilding.buildingsUI = this;
        dragAndDropBuilding.onBeginDrag.AddListener(onBeginDragCreateNew);
        dragAndDropBuilding.onBeginDrag.AddListener(onBeginDragShowAll);
        dragAndDropBuilding.onDrop.AddListener(onDropFindAndBuild);
        dragAndDropBuilding.onDrop.AddListener(onDropHideAll);
        dragAndDropBuilding.isAvaliable = isAvaliableFunc;
    }

    public void CreateNewRoadGameObject()
    {
        CreateNewBuildingGameObject(
            roadPrefab,
            roadTransformPosition,
            CreateNewRoadGameObject,
            GameManager.Instance.mapManager.ShowPlacesForRoads,
            FindEdgeAndBuildRoad,
            GameManager.Instance.mapManager.HideAllAvaliablePlaces,
            GameManager.Instance.userManager.IsCurrentUserCanBuildRoadNow
        );
    }

    public void CreateNewSettlementGameObject()
    {
        CreateNewBuildingGameObject(
            settlementPrefab,
            settlementTransformPosition,
            CreateNewSettlementGameObject,
            GameManager.Instance.mapManager.ShowPlacesForSettlements,
            FindVertexAndBuildSettlement,
            GameManager.Instance.mapManager.HideAllAvaliablePlaces,
            GameManager.Instance.userManager.IsCurrentUserCanBuildSettlementNow
        );
    }

    public void CreateNewCityGameObject()
    {
        CreateNewBuildingGameObject(
            cityPrefab,
            cityTransformPosition,
            CreateNewCityGameObject,
            GameManager.Instance.mapManager.ShowPlacesForCities,
            FindVertexAndBuildCity,
            GameManager.Instance.mapManager.HideAllAvaliablePlaces,
            GameManager.Instance.userManager.IsCurrentUserCanBuildCityNow
        );
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
        if(hit.collider == null)
        {
            return null;
        }
        return hit.collider.GetComponent<T>();
    }
}
