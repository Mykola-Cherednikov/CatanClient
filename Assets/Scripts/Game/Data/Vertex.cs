using System.Collections.Generic;
using UnityEngine;

public enum VertexBuildingType
{
    NONE,
    SETTLEMENT,
    CITY
}

public class Vertex : PlaceForBuildings
{
    public List<Hex> neighborHexes;

    public List<Edge> neighborEdges;

    public VertexBuildingType type;

    protected override void Awake()
    {
        base.Awake();
        neighborHexes = new();
        neighborEdges = new();
        HideSpriteAndCollider();
    }

    public void SetVertexBuilding(VertexBuildingType type, User u)
    {
        this.type = type;
        user = u;
        sprite.color = Color.blue;
        ShowSpriteAndCollider();
    }
}
