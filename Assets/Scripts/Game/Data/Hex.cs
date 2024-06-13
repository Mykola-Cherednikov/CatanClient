using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private Sprite desertSprite;
    [SerializeField] private Sprite pastureSprite;
    [SerializeField] private Sprite fieldSprite;
    [SerializeField] private Sprite forestSprite;
    [SerializeField] private Sprite hillSprite;
    [SerializeField] private Sprite mountainSprite;

    public Dictionary<EdgeDirection, Hex> edgeDirectionToNeighborHexes;

    public Dictionary<VertexDirection, Vertex> vertexDirectionToContainedVertiñes;

    public Dictionary<EdgeDirection, Edge> edgeDirectionToContainedEdges;

    public Dictionary<HexType, Sprite> sprites;

    [SerializeField] public NumberToken numberToken;

    public void Awake()
    {
        edgeDirectionToNeighborHexes = new Dictionary<EdgeDirection, Hex>
            {
                { EdgeDirection.TR, null! },
                { EdgeDirection.R, null! },
                { EdgeDirection.DR, null! },
                { EdgeDirection.DL, null! },
                { EdgeDirection.L, null! },
                { EdgeDirection.TL, null! }
            };

        vertexDirectionToContainedVertiñes = new Dictionary<VertexDirection, Vertex>()
            {
                { VertexDirection.N, null! },
                { VertexDirection.NE, null! },
                { VertexDirection.SE, null! },
                { VertexDirection.S, null! },
                { VertexDirection.SW, null! },
                { VertexDirection.NW, null! }
            };

        edgeDirectionToContainedEdges = new Dictionary<EdgeDirection, Edge>()
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
            { HexType.PASTURE, pastureSprite },
            { HexType.DESERT, desertSprite },
            { HexType.HILL, hillSprite },
            { HexType.MOUNTAIN, mountainSprite },
            { HexType.FIELD, fieldSprite },
            { HexType.FOREST, forestSprite },
        };
    }
}