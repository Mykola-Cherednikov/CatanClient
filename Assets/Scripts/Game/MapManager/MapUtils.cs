using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class MapUtils
{
    public static List<Edge> GetneighbourEdgesToEdge(Edge e)
    {
        List<Edge> neighbourEdgesToEdge = new List<Edge>();
        List<Vertex> neighbourVertices = e.neighbourVertices;

        foreach (var v in neighbourVertices)
        {
            foreach (var edge in v.neighbourEdges)
            {
                if (edge != e)
                {
                    neighbourEdgesToEdge.Add(edge);
                }
            }
        }

        return neighbourEdgesToEdge;
    }

    public static List<Edge> GetneighbourEdgesToEdgeWithBuildingType(Edge e, EdgeBuildingType edgeBuildingType)
    {
        List<Edge> neighbourEdgesToEdge = GetneighbourEdgesToEdge(e);

        neighbourEdgesToEdge = neighbourEdgesToEdge.Where(edge => edge.type == edgeBuildingType).ToList();

        return neighbourEdgesToEdge;
    }

    public static List<Vertex> GetneighbourVerticesToVertex(Vertex v)
    {
        List<Vertex> neighbourVerticesToVertex = new List<Vertex>();
        List<Edge> neighbourEdgex = v.neighbourEdges;

        foreach (var e in neighbourEdgex)
        {
            foreach (var vertex in e.neighbourVertices)
            {
                if (vertex != v)
                {
                    neighbourVerticesToVertex.Add(vertex);
                }
            }
        }

        return neighbourVerticesToVertex;
    }

    public static List<Edge> GetAvailableEdgesForUser(User user, List<Vertex> vertices, List<Edge> edges)
    {
        HashSet<Edge> availableEdges = new HashSet<Edge>();

        List<Vertex> userVertices = vertices.FindAll(v => v.user == user);
        foreach (var v in userVertices)
        {
            foreach (var edge in v.neighbourEdges)
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
            availableEdges.AddRange(GetneighbourEdgesToEdgeWithBuildingType(edge, EdgeBuildingType.NONE));
        }

        return availableEdges.ToList();
    }

    public static List<Vertex> GetAvailableVerticiesForUser(User thisUser, List<Edge> edges)
    {
        HashSet<Vertex> availableVerticies = new HashSet<Vertex>();
        List<Edge> userEdges = edges.FindAll(e => e.user == thisUser);
        foreach (var e in userEdges)
        {
            foreach (var vertex in e.neighbourVertices)
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

    public static bool IsVertexOccupiedByThisUser(Vertex v, User u)
    {
        return v.user == u;
    }

    public static bool IsThisUserSettlementOnThisVertex(Vertex v, User u)
    {
        if (!IsVertexOccupiedByThisUser(v, u))
        {
            return false;
        }

        if (v.type != VertexBuildingType.SETTLEMENT)
        {
            return false;
        }

        return true;
    }

    public static List<User> GetUniqueUsersInHex(Hex hex)
    {
        List<Vertex> verticesToHex = hex.vertexDirectionToContainedVertices.Values.ToList();
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
            int currentNum = FindLongestRoadLength(userEdge, null, new List<Edge>(), u);
            if (currentNum > maxNum)
            {
                maxNum = currentNum;
            }
        }

        return maxNum;
    }

    public static int FindLongestRoadLength(Edge currentEdge, Edge previousEdge, List<Edge> currentRoad, User u)
    {
        currentRoad.Add(currentEdge);
        int maxLength = currentRoad.Count;

        List<Vertex> nextVerteces = currentEdge.neighbourVertices;

        if (previousEdge != null)
        {
            nextVerteces = nextVerteces.Except(previousEdge.neighbourVertices).ToList();
        }

        List<Vertex> nextUserOrFreeVerteces = nextVerteces.Where(v => v.user == null || v.user == u).ToList();
        List<Edge> nextUserEdges = nextUserOrFreeVerteces.SelectMany(v => v.neighbourEdges)
            .Where(e => e.user == u && e != currentEdge).ToList();

        foreach (var nextUserEdge in nextUserEdges)
        {
            int currentRoadLength = FindLongestRoadLength(nextUserEdge, currentEdge, new List<Edge>(currentRoad), u);

            if (currentRoadLength > maxLength)
            {
                maxLength = currentRoadLength;
            }
        }

        return maxLength;
    }
}
