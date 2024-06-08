using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;



public class MapManager : MonoBehaviour
{
    private MapBuilder mapBuilder;

    private List<Hex> hexes;
    private List<Vertex> vertices;
    private List<Edge> edges;

    public void InitializeMap(SocketBroadcastStartGameDTO dto)
    {
        var mapBuilder = gameObject.AddComponent<MapBuilder>();
        var mapInfo = mapBuilder.CreateMap(dto.hexesInRowCounts, dto.hexes);

        hexes = mapInfo.hexes;
        vertices = mapInfo.vertices;
        edges = mapInfo.edges;

        Destroy(mapBuilder);
    }

    public List<Vertex> GetUserVerticies(User user)
    {
        return vertices.Where(v => v.user == user).ToList();
    }

    public List<Edge> GetUserEdgex(User user)
    {
        return edges.Where(e => e.user == user).ToList();
    }

    #region Show/Hide Avaliable Vertices
    private void ShowAllAvaliableVertices()
    {
        foreach (var v in vertices)
        {
            if (v.type == VertexBuildingType.NONE)
            {
                v.ShowSpriteAndCollider();
            }
        }
    }

    private void HideAllAvaliableVertices()
    {
        foreach (var v in vertices)
        {
            if (v.type == VertexBuildingType.NONE)
            {
                v.HideSpriteAndCollider();
            }
        }
    }

    private void ShowAllAvaliableVerticesForUser()
    {
        List<Vertex> avaliableVertices = MapUtils.GetAvaliableVerticiesForUser(GameManager.Instance.userManager.currentUser, edges);
        foreach (var v in avaliableVertices)
        {
            v.ShowSpriteAndCollider();
        }
    }
    #endregion

    #region Show/Hide Avaliable Edges
    private void ShowAllAvaliableEdges()
    {
        foreach (var e in edges)
        {
            if (e.type == EdgeBuildingType.NONE)
            {
                e.ShowSpriteAndCollider();
            }
        }
    }

    private void HideAllAvaliableEdges()
    {
        foreach (var e in edges)
        {
            if (e.type == EdgeBuildingType.NONE)
            {
                e.HideSpriteAndCollider();
            }
        }
    }

    private void ShowAllAvaliableEdgesForUser()
    {
        List<Edge> avaliableEdges = MapUtils.GetAvaliableEdgesForUser(GameManager.Instance.userManager.currentUser, vertices, edges);
        foreach (var e in avaliableEdges)
        {
            e.ShowSpriteAndCollider();
        }
    }
    #endregion

    #region Show/Hide Places For Buildings
    public void ShowPlacesForRoads()
    {
        ShowAllAvaliableEdgesForUser();
    }

    public void ShowPlacesForSettlements()
    {
        if (GameManager.Instance.gameState == GameState.PREPARATION_BUILD_SETTLEMENTS)
        {
            ShowAllAvaliableVertices();
        }
        else
        {
            ShowAllAvaliableVerticesForUser();
        }
    }

    public void ShowPlacesForCities()
    {

    }

    public void HideAllAvaliablePlaces()
    {
        HideAllAvaliableEdges();
        HideAllAvaliableVertices();
    }
    #endregion

    #region Plan To Build
    public async void PlanToBuildRoad(int edgeId)
    {
        if (!GameManager.Instance.userManager.IsCurrentUserCanBuildRoadNow())
        {
            return;
        }

        if (!MapUtils.IsEdgeFree(edges.FirstOrDefault(e => e.id == edgeId)))
        {
            return;
        }

        await Multiplayer.Instance.SocketSendBuildRoadRequest(edgeId);
    }

    public async void PlanToBuildSettlement(int vertexId)
    {
        if (!GameManager.Instance.userManager.IsCurrentUserCanBuildSettlementNow())
        {
            return;
        }

        if (!MapUtils.IsVertexFree(vertices.FirstOrDefault(v => v.id == vertexId)))
        {
            return;
        }

        await Multiplayer.Instance.SocketSendBuildSettlementRequest(vertexId);
    }

    public async void PlanToBuildCity(int vertexId)
    {
        if (!GameManager.Instance.userManager.IsCurrentUserCanBuildCityNow())
        {
            return;
        }

        Vertex vertex = vertices.FirstOrDefault(v => v.id == vertexId);
        User currentUser = GameManager.Instance.userManager.currentUser;


        if (!MapUtils.IsCurrentUserSettlementOnThisVertex(vertex, currentUser))
        {
            return;
        }

        await Multiplayer.Instance.SocketSendBuildCityRequest(vertexId);
    }
    #endregion

    #region Build
    public void BuildRoad(int edgeId, int userId)
    {
        User u = GameManager.Instance.userManager.GetUserById(userId);
        edges.FirstOrDefault(v => v.id == edgeId).SetEdgeBuilding(EdgeBuildingType.ROAD, u);
    }

    public void BuildSettlement(int vertexId, int userId)
    {
        User u = GameManager.Instance.userManager.GetUserById(userId);
        vertices.FirstOrDefault(v => v.id == vertexId).SetVertexBuilding(VertexBuildingType.SETTLEMENT, u);
    }

    public void BuildCity(int vertexId, int userId)
    {
        User u = GameManager.Instance.userManager.GetUserById(userId);
        vertices.FirstOrDefault(v => v.id == vertexId).SetVertexBuilding(VertexBuildingType.CITY, u);
    }
    #endregion
}

