using System.Collections.Generic;
using UnityEngine;

public enum VertexBuildingType
{
    NONE,
    SETTLEMENT,
    CITY
}

public class Vertex : Place
{
    public List<Hex> neighborHexes;

    public List<Edge> neighborEdges;

    public VertexBuildingType type;

    [SerializeField] private Sprite noneSprite;
    [SerializeField] private Sprite citySprite;
    [SerializeField] private Sprite settlementSprite;

    protected override void Awake()
    {
        base.Awake();
        neighborHexes = new();
        neighborEdges = new();
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
