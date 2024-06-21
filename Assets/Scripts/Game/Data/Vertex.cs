using System.Collections.Generic;
using UnityEngine;

public enum VertexBuildingType
{
    NONE,
    SETTLEMENT,
    CITY
}

public enum HarborType
{
    NONE,
    GENERIC,
    BRICK,
    LUMBER,
    ORE,
    GRAIN,
    WOOL
}

public class Vertex : Place
{
    public List<Hex> neighbourHexes;

    public List<Edge> neighbourEdges;

    public VertexBuildingType type;

    public HarborType harborType;

    [SerializeField] private Sprite noneSprite;
    [SerializeField] private Sprite citySprite;
    [SerializeField] private Sprite settlementSprite;

    protected override void Awake()
    {
        base.Awake();
        neighbourHexes = new();
        neighbourEdges = new();
        spriteRenderer.sprite = noneSprite;
        HideSpriteAndCollider();
    }

    private void ChangeBuildingSpriteType()
    {
        switch (type)
        {
            case VertexBuildingType.NONE:
                spriteRenderer.sprite = noneSprite; 
                break;
            case VertexBuildingType.SETTLEMENT:
                spriteRenderer.sprite = settlementSprite; 
                break;
            case VertexBuildingType.CITY:
                spriteRenderer.sprite = citySprite;
                break;
        }
    }

    public void SetVertexBuilding(VertexBuildingType type, User u)
    {
        this.type = type;
        user = u;
        spriteRenderer.color = Color.blue;
        ShowSpriteAndCollider();
        ChangeBuildingSpriteType();
    }
}
