using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class MapUtils
{
    public static List<Edge> GetNeighborEdgesToEdge(Edge e)
    {
        List<Edge> neighborEdgesToEdge = new List<Edge>();
        List<Vertex> neighborVertices = e.neighborVertices;

        foreach (var v in neighborVertices)
        {
            foreach (var edge in v.neighborEdges)
            {
                if (edge.type == EdgeBuildingType.NONE && edge != e)
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
            availableEdges.AddRange(GetNeighborEdgesToEdge(edge));
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
}
