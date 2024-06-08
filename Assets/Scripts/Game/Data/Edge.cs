using System.Collections.Generic;
using UnityEngine;

public enum EdgeBuildingType
{
    NONE,
    ROAD
}

public class Edge : PlaceForBuildings
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
        sprite.color = Color.blue;
        ShowSpriteAndCollider();
    }
}
