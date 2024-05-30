using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum HexType
{
    PASTURE,
    FIELD,
    FOREST,
    HILL,
    MOUNTAIN,
    DESERT
}

public enum EdgeDirection
{
    TR,
    R,
    DR,
    DL,
    L,
    TL
}

public enum VertexDirection
{
    N,
    NE,
    SE,
    S,
    SW,
    NW
}

public class Hex : MonoBehaviour
{
    public int id;

    public HexType type;

    public int numberToken;

    [SerializeField] private Sprite _desertSprite;
    [SerializeField] private Sprite _pastureSprite;
    [SerializeField] private Sprite _fieldSprite;
    [SerializeField] private Sprite _forestSprite;
    [SerializeField] private Sprite _hillSprite;
    [SerializeField] private Sprite _mountainSprite;

    public Dictionary<EdgeDirection, Hex> neighborHexs;

    public Dictionary<VertexDirection, Vertex> vertexs;

    public Dictionary<EdgeDirection, Edge> edges;

    public Dictionary<HexType, Sprite> sprites;

    public void Awake()
    {
        neighborHexs = new Dictionary<EdgeDirection, Hex>
            {
                { EdgeDirection.TR, null! },
                { EdgeDirection.R, null! },
                { EdgeDirection.DR, null! },
                { EdgeDirection.DL, null! },
                { EdgeDirection.L, null! },
                { EdgeDirection.TL, null! }
            };

        vertexs = new Dictionary<VertexDirection, Vertex>()
            {
                { VertexDirection.N, null! },
                { VertexDirection.NE, null! },
                { VertexDirection.SE, null! },
                { VertexDirection.S, null! },
                { VertexDirection.SW, null! },
                { VertexDirection.NW, null! }
            };

        edges = new Dictionary<EdgeDirection, Edge>()
            {
                { EdgeDirection.TR, null! },
                { EdgeDirection.R, null! },
                { EdgeDirection.DR, null! },
                { EdgeDirection.DL, null! },
                { EdgeDirection.L, null! },
                { EdgeDirection.TL, null! }
            };

        sprites = new Dictionary<HexType, Sprite>()
        {
            { HexType.PASTURE, _pastureSprite },
            { HexType.DESERT, _desertSprite },
            { HexType.HILL, _hillSprite },
            { HexType.MOUNTAIN, _mountainSprite },
            { HexType.FIELD, _fieldSprite },
            { HexType.FOREST, _forestSprite },
        };
    }
}