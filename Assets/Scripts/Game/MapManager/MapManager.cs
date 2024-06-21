using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;



public class MapManager : MonoBehaviour
{
    private List<Hex> hexes;
    private List<Vertex> vertices;
    private List<Edge> edges;

    public void InitializeMap(SocketBroadcastStartGameDTO dto)
    {
        var mapBuilder = gameObject.AddComponent<MapBuilder>();
        var mapInfo = mapBuilder.CreateMap(dto.hexesInRowCounts, dto.seed);

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

    public Hex GetHexById(int hexId)
    {
        return hexes.FirstOrDefault(h => h.id == hexId);
    }

    #region Show/Hide Available Vertices
    private void ShowAllAvailableVertices()
    {
        foreach (var v in vertices)
        {
            if (v.type == VertexBuildingType.NONE)
            {
                v.ShowSpriteAndCollider();
            }
        }
    }

    private void HideAllAvailableVertices()
    {
        foreach (var v in vertices)
        {
            if (v.type == VertexBuildingType.NONE)
            {
                v.HideSpriteAndCollider();
            }
        }
    }

    private void ShowAllAvailableVerticesForUser()
    {
        List<Vertex> availableVertices = MapUtils.GetAvailableVerticiesForUser(GameManager.Instance.userManager.currentUser, edges);
        foreach (var v in availableVertices)
        {
            v.ShowSpriteAndCollider();
        }
    }
    #endregion

    #region Show/Hide Available Edges
    private void ShowAllAvailableEdges()
    {
        foreach (var e in edges)
        {
            if (e.type == EdgeBuildingType.NONE)
            {
                e.ShowSpriteAndCollider();
            }
        }
    }

    private void HideAllAvailableEdges()
    {
        foreach (var e in edges)
        {
            if (e.type == EdgeBuildingType.NONE)
            {
                e.HideSpriteAndCollider();
            }
        }
    }

    private void ShowAllAvailableEdgesForUser()
    {
        List<Edge> availableEdges = MapUtils.GetAvailableEdgesForUser(GameManager.Instance.userManager.currentUser, vertices, edges);
        foreach (var e in availableEdges)
        {
            e.ShowSpriteAndCollider();
        }
    }
    #endregion

    #region Show/Hide Places For Buildings
    public void ShowPlacesForRoads()
    {
        ShowAllAvailableEdgesForUser();
    }

    public void ShowPlacesForSettlements()
    {
        if (GameManager.Instance.gameState == GameState.PREPARATION_BUILD_SETTLEMENTS)
        {
            ShowAllAvailableVertices();
        }
        else
        {
            ShowAllAvailableVerticesForUser();
        }
    }

    public void ShowPlacesForCities()
    {

    }

    public void HideAllAvailablePlaces()
    {
        HideAllAvailableEdges();
        HideAllAvailableVertices();
    }
    #endregion

    #region Plan To Build
    public async void PlanToBuildRoadAndEndTurnIfPlaceOnPreparation(int edgeId)
    {
        if (!GameManager.Instance.userManager.IsCurrentUserCanBuildRoadNow())
        {
            return;
        }

        if (!MapUtils.IsEdgeFree(edges.FirstOrDefault(e => e.id == edgeId)))
        {
            return;
        }

        if (GameManager.Instance.gameState == GameState.PREPARATION_BUILD_ROADS)
        {
            GameManager.Instance.userManager.UserTurnReady();
        }

        await Multiplayer.Instance.SocketSendBuildRoadRequest(edgeId);
    }

    public async void PlanToBuildSettlementAndEndTurnIfPlaceOnPreparation(int vertexId)
    {
        if (!GameManager.Instance.userManager.IsCurrentUserCanBuildSettlementNow())
        {
            return;
        }

        if (!MapUtils.IsVertexFree(vertices.FirstOrDefault(v => v.id == vertexId)))
        {
            return;
        }

        if (GameManager.Instance.gameState == GameState.PREPARATION_BUILD_SETTLEMENTS)
        {
            GameManager.Instance.userManager.UserTurnReady();
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
    public void BuildRoad(int edgeId, User user)
    {
        if (GameManager.Instance.gameState == GameState.USER_TURN)
        {
            if (user.buffDuration[Buff.ROAD_BUILDING] > 0)
            {
                user.buffDuration[Buff.ROAD_BUILDING]--;
            }
            else
            {
                GameManager.Instance.resourceManager.BuyGoods(user, Goods.Road);
            }            
        }
        Edge edge = edges.FirstOrDefault(v => v.id == edgeId);
        edge.SetEdgeBuilding(EdgeBuildingType.ROAD, user);
        SetColorToUserBuilding(edge, user);
    }

    public void BuildSettlement(int vertexId, User user)
    {
        if (GameManager.Instance.gameState == GameState.USER_TURN)
        {
            GameManager.Instance.resourceManager.BuyGoods(user, Goods.Settlement);
        }
        Vertex vertex = vertices.FirstOrDefault(v => v.id == vertexId);
        vertex.SetVertexBuilding(VertexBuildingType.SETTLEMENT, user);
        SetColorToUserBuilding(vertex, user);
    }

    public void BuildCity(int vertexId, User user)
    {
        if (GameManager.Instance.gameState == GameState.USER_TURN)
        {
            GameManager.Instance.resourceManager.BuyGoods(user, Goods.City);
        }
        Vertex vertex = vertices.FirstOrDefault(v => v.id == vertexId);
        vertex.SetVertexBuilding(VertexBuildingType.CITY, user);
        SetColorToUserBuilding(vertex, user);
    }
    #endregion

    private void SetColorToUserBuilding<T>(T placeForBuilding, User user) where T : Place
    {
        placeForBuilding.spriteRenderer.color = user.color;
    }

    public void ShowPlacesForRobber()
    {
        foreach (var hex in hexes)
        {
            if(hex.numberToken == null)
            {
                continue;
            }
            NumberToken numberToken = hex.numberToken;
            numberToken.ShowCollider();
        }
    }

    public void HideAllPlacesForRobber()
    {
        foreach (var hex in hexes)
        {
            if (hex.numberToken == null)
            {
                continue;
            }
            NumberToken numberToken = hex.numberToken;
            numberToken.HideCollider();
        }
    }

    public async void PlanPlaceRobberAndPlanToRobUser(int hexId, int userId)
    {
        if (!GameManager.Instance.userManager.IsCurrentUserCanPlaceRobberNow())
        {
            return;
        }

        await Multiplayer.Instance.SocketSendUserRobberyRequest(hexId, userId);
    }

    public void PlaceRobber(int hexId)
    {
        foreach(var hex in hexes)
        {
            hex.numberToken.SetBanditOff();
        }

        hexes.FirstOrDefault(h => h.id == hexId).numberToken.SetBanditOn();
    }

    public bool IsRobberNearbyCurrentUser() 
    {
        HashSet<Hex> hexes = new HashSet<Hex>();
        List<Vertex> vertices = GetUserVerticies(GameManager.Instance.userManager.currentUser);
        foreach (var vertex in vertices)
        {
            hexes.AddRange(vertex.neighbourHexes);
        }
        
        foreach (var hex in hexes)
        {
            if (hex.numberToken.isBandit)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsUserHaveHarbourWithType(User user, HarborType harborType)
    {
        List<Vertex> harborVertices = GetUserVerticies(user).Where(h => h.harborType == harborType).ToList();

        return harborVertices.Count > 0;
    }
}

