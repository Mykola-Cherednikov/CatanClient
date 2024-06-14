using System.Collections.Generic;
using UnityEngine;

public enum EdgeBuildingType
{
    NONE,
    ROAD
}

public class Edge : Place
{
    public List<Vertex> neighborVertices;

    public List<Hex> neighborHexes;

    public EdgeBuildingType type;

    protected override void Awake()
    {
        base.Awake();
        neighborVertices = new List<Vertex>();
        neighborHexes = new List<Hex>();
    }

    public void SetEdgeBuilding(EdgeBuildingType type, User u)
    {
        this.type = type;
        user = u;
        spriteRenderer.color = Color.blue;
        ShowSpriteAndCollider();
    }
}
