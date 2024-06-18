using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class MapUtils
{
    public static List<Edge> GetNeighborEdgesToEdgeWithBuildingType(Edge e, EdgeBuildingType edgeBuildingType)
    {
        List<Edge> neighborEdgesToEdge = new List<Edge>();
        List<Vertex> neighborVertices = e.neighborVertices;

        foreach (var v in neighborVertices)
        {
            foreach (var edge in v.neighborEdges)
            {
                if (edge.type == edgeBuildingType && edge != e)
                {
                    neighborEdgesToEdge.Add(edge);
                }
            }
        }

        return neighborEdgesToEdge;
    }

    public static List<Vertex> GetNeighborVerticesToVertex(Vertex v)
    {
        List<Vertex> neighborVerticesToVertex = new List<Vertex>();
        List<Edge> neighborEdgex = v.neighborEdges;

        foreach (var e in neighborEdgex)
        {
            foreach (var vertex in e.neighborVertices)
            {
                if (vertex != v)
                {
                    neighborVerticesToVertex.Add(vertex);
                }
            }
        }

        return neighborVerticesToVertex;
    }

    public static List<Edge> GetAvailableEdgesForUser(User user, List<Vertex> vertices, List<Edge> edges)
    {
        HashSet<Edge> availableEdges = new HashSet<Edge>();

        List<Vertex> userVertices = vertices.FindAll(v => v.user == user);
        foreach (var v in userVertices)
        {
            foreach (var edge in v.neighborEdges)
            {
                if (edge.type == EdgeBuildingType.NONE)
                {
                    availableEdges.Add(edge);
                }
            }
        }

        List<Edge> userEdges = edges.FindAll(e => e.user == user);
        foreach (var edge in userEdges)
        {
            availableEdges.AddRange(GetNeighborEdgesToEdgeWithBuildingType(edge, EdgeBuildingType.NONE));
        }

        return availableEdges.ToList();
    }

    public static List<Vertex> GetAvailableVerticiesForUser(User thisUser, List<Edge> edges)
    {
        HashSet<Vertex> availableVerticies = new HashSet<Vertex>();
        List<Edge> userEdges = edges.FindAll(e => e.user == thisUser);
        foreach (var e in userEdges)
        {
            foreach (var vertex in e.neighborVertices)
            {
                if (vertex.type == VertexBuildingType.NONE)
                {
                    availableVerticies.Add(vertex);
                }
            }
        }

        return availableVerticies.ToList();
    }

    public static bool IsVertexFree(Vertex v)
    {
        return v.user == null;
    }

    public static bool IsEdgeFree(Edge e)
    {
        return e.user == null;
    }

    public static bool IsVertexOccupiedByCurrentUser(Vertex v, User u)
    {
        return v.user == u;
    }

    public static bool IsCurrentUserSettlementOnThisVertex(Vertex v, User u)
    {
        if (!IsVertexOccupiedByCurrentUser(v, u))
        {
            return false;
        }

        if(v.type != VertexBuildingType.SETTLEMENT)
        {
            return false;
        }

        return true;
    }

    public static List<User> GetUniqueUsersInHex(Hex hex)
    {
        List<Vertex> verticesToHex = hex.vertexDirectionToContainedVertiñes.Values.ToList();
        List<Vertex> verticiesWithUser = verticesToHex.Where(v => v.user != null).ToList();
        List<User> usersInHex = verticiesWithUser.Select(v => v.user).ToList();
        List<User> uniqueUsersInHex = usersInHex.Distinct().ToList();

        return uniqueUsersInHex;
    }

    public static int GetLongestRoadLengthForUser(User u, List<Edge> edges)
    {
        List<Edge> userEdges = edges.Where(e => e.user == u).ToList();

        int maxNum = 0;
        foreach (var userEdge in userEdges)
        {
            int currentNum = FindLongestRoadLength(userEdge, new List<Edge>(), new List<Edge>(), u);
            if(currentNum > maxNum)
            {
                maxNum = currentNum;
            }
        }

        return maxNum;
    }

    public static int FindLongestRoadLength(Edge edge, List<Edge> currentRoad, List<Edge> previousNeighboursEdges, User u)
    {
        currentRoad.Add(edge);
        int maxLength = currentRoad.Count;
        List<Edge> previousEdges = GetNeighborEdgesToEdgeWithBuildingType(edge, EdgeBuildingType.ROAD);
        List<Edge> nextEdges = GetNeighborEdgesToEdgeWithBuildingType(edge, EdgeBuildingType.ROAD);


        List<Vertex> edgeVertices = new List<Vertex>(edge.neighborVertices);
        List<Vertex> previousNeighboursVertices = previousNeighboursEdges.SelectMany(e => e.neighborVertices).ToList();
        Vertex checkingVertex = edgeVertices.FirstOrDefault(e => !previousNeighboursVertices.Contains(e));

        if(checkingVertex.user != u)
        {
            return maxLength;
        }

        nextEdges = nextEdges.Except(previousNeighboursEdges).ToList();
        previousEdges.Add(edge);

        foreach(var currentEdge in nextEdges)
        {
            if(currentEdge.user != u)
            {
                continue;
            }

            int currentRoadLength = FindLongestRoadLength(currentEdge, new List<Edge>(currentRoad), new List<Edge>(previousEdges), u);

            if(currentRoadLength > maxLength)
            {
                maxLength = currentRoadLength;
            }
        }

        return maxLength;
    }
}
