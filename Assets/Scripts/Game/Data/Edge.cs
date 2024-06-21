using System.Collections.Generic;
using UnityEngine;

public enum EdgeBuildingType
{
    NONE,
    ROAD
}

public class Edge : Place
{
    public List<Vertex> neighbourVertices;

    public List<Hex> neighbourHexes;

    public EdgeBuildingType type;

    protected override void Awake()
    {
        base.Awake();
        neighbourVertices = new List<Vertex>();
        neighbourHexes = new List<Hex>();
    }

    public void SetEdgeBuilding(EdgeBuildingType type, User u)
    {
        this.type = type;
        user = u;
        spriteRenderer.color = Color.blue;
        ShowSpriteAndCollider();
    }
}
