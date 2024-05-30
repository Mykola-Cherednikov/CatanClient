using Assets.Scripts.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class MapBuilder : MonoBehaviour
{
    [SerializeField] private GameObject _hexGO;
    [SerializeField] private GameObject _vertexGO;
    [SerializeField] private GameObject _edgeGO;
    [SerializeField] private GameObject _numberTokenGO;

    private Dictionary<VertexDirection, Vector2> _vertexOffsets;
    private Dictionary<EdgeDirection, Vector2> _edgeOffsets;
    private Dictionary<EdgeDirection, List<VertexDirection>> _edgeNeighborVertex;

    private float hexOffsetX = 4.7f;
    private float hexOffsetY = 4f;
    private float vertixOffsetX = 2.35f;
    private float vertixMiddleOffsetY = 2f;
    private float vertixOffsetY = 0.7f;

    private void Awake()
    {
        _hexGO = Resources.Load<GameObject>("Prefabs/Game/Hex");
        _vertexGO = Resources.Load<GameObject>("Prefabs/Game/Vertex");
        _edgeGO = Resources.Load<GameObject>("Prefabs/Game/Edge");
        _numberTokenGO = Resources.Load<GameObject>("Prefabs/Game/NumberToken");

        _vertexOffsets = new Dictionary<VertexDirection, Vector2>()
        {
            { VertexDirection.N, new Vector2(0f, vertixMiddleOffsetY + vertixOffsetY) },
            { VertexDirection.NE, new Vector2(vertixOffsetX, vertixMiddleOffsetY - vertixOffsetY)},
            { VertexDirection.SE, new Vector2(vertixOffsetX, -vertixMiddleOffsetY + vertixOffsetY)},
            { VertexDirection.S, new Vector2(0f, -vertixMiddleOffsetY - vertixOffsetY)},
            { VertexDirection.SW, new Vector2(-vertixOffsetX, -vertixMiddleOffsetY + vertixOffsetY)},
            { VertexDirection.NW, new Vector2(-vertixOffsetX, vertixMiddleOffsetY - vertixOffsetY)}
        };

        _edgeOffsets = new Dictionary<EdgeDirection, Vector2>()
        {
            { EdgeDirection.TR, new Vector2(vertixOffsetX / 2f, vertixMiddleOffsetY) },
            { EdgeDirection.R, new Vector2(vertixOffsetX, 0f)},
            { EdgeDirection.DR, new Vector2(vertixOffsetX / 2f, -vertixMiddleOffsetY)},
            { EdgeDirection.DL, new Vector2(-vertixOffsetX / 2f, -vertixMiddleOffsetY)},
            { EdgeDirection.L, new Vector2(-vertixOffsetX, 0f)},
            { EdgeDirection.TL, new Vector2(-vertixOffsetX / 2f, vertixMiddleOffsetY)}
        };

        _edgeNeighborVertex = new Dictionary<EdgeDirection, List<VertexDirection>>() 
        {
            { EdgeDirection.TR, new(){ VertexDirection.N, VertexDirection.NE} },
            { EdgeDirection.R, new(){ VertexDirection.NE, VertexDirection.SE} },
            { EdgeDirection.DR, new(){ VertexDirection.SE, VertexDirection.S} },
            { EdgeDirection.DL, new(){ VertexDirection.S, VertexDirection.SW} },
            { EdgeDirection.L, new(){ VertexDirection.SW, VertexDirection.NW} },
            { EdgeDirection.TL, new(){ VertexDirection.NW, VertexDirection.N} },
        };
    }

    public MapData CreateMap(List<int> numsInRow, List<HexDTO> hexesDTO)
    {
        Dictionary<Vector2, Hex> hexes = new();
        Dictionary<Vector2, Vertex> vertices = new();
        Dictionary<Vector2, Edge> edges = new();

        CreateHexes(hexes, numsInRow);
        SetHexNeighbor(hexes);
        CreateVertices(hexes, vertices);
        CreateEdges(hexes, edges);
        LinkEdgesAndVertices(hexes);
        SetHexData(hexes, hexesDTO);

        MapData mapData = new MapData();
        mapData.hexes = hexes.Values.ToList();
        mapData.vertices = vertices.Values.ToList();
        mapData.edges = edges.Values.ToList();

        return mapData;
    }

    private void CreateHexes(Dictionary<Vector2, Hex> hexes, List<int> numsInRow)
    {
        int i = 0;
        for (float y = ((numsInRow.Count - 1) / 2f); y >= -((numsInRow.Count - 1) / 2f); y--)
        {
            for (float x = -(numsInRow[i] - 1) / 2f; x <= (numsInRow[i] - 1) / 2f; x++)
            {
                GameObject go = Instantiate(_hexGO, transform);
                go.transform.position = transform.position + new Vector3(x * hexOffsetX, y * hexOffsetY);
                Hex hex = go.GetComponent<Hex>();
                hex.id = hexes.Count;
                hex.name = "Hex"+hex.id;
                hexes.Add(new Vector2(go.transform.position.x, go.transform.position.y), hex);
            }
            i++;
        }
    }

    private void SetHexNeighbor(Dictionary<Vector2, Hex> hexes)
    {
        foreach (Hex hex in hexes.Values)
        {
            if (hexes.TryGetValue(new Vector2(hex.transform.position.x + (hexOffsetX / 2f),
                hex.transform.position.y + hexOffsetY), out Hex hexTR))
            {
                hex.neighborHexs[EdgeDirection.TR] = hexTR;
            }

            if (hexes.TryGetValue(new Vector2(hex.transform.position.x + hexOffsetX,
                hex.transform.position.y), out Hex hexR))
            {
                hex.neighborHexs[EdgeDirection.R] = hexR;
            }

            if (hexes.TryGetValue(new Vector2(hex.transform.position.x + (hexOffsetX / 2f),
                hex.transform.position.y - hexOffsetY), out Hex hexDR))
            {
                hex.neighborHexs[EdgeDirection.DR] = hexDR;
            }

            if (hexes.TryGetValue(new Vector2(hex.transform.position.x - (hexOffsetX / 2f),
                hex.transform.position.y - hexOffsetY), out Hex hexDL))
            {
                hex.neighborHexs[EdgeDirection.DL] = hexDL;
            }

            if (hexes.TryGetValue(new Vector2(hex.transform.position.x - hexOffsetX,
                hex.transform.position.y), out Hex hexL))
            {
                hex.neighborHexs[EdgeDirection.L] = hexL;
            }

            if (hexes.TryGetValue(new Vector2(hex.transform.position.x - (hexOffsetX / 2f),
                hex.transform.position.y + hexOffsetY), out Hex hexTL))
            {
                hex.neighborHexs[EdgeDirection.TL] = hexTL;
            }
        }
    }

    private void CreateVertices(Dictionary<Vector2, Hex> hexes, Dictionary<Vector2, Vertex> vertices)
    {
        foreach (Hex hex in hexes.Values)
        {
            foreach (var vertexOffset in _vertexOffsets)
            {
                Vector3 position = hex.transform.position + new Vector3(vertexOffset.Value.x, vertexOffset.Value.y);
                if (!vertices.TryGetValue(new Vector2(position.x, position.y), out Vertex ver))
                {
                    Vertex vertex = Instantiate(_vertexGO, transform).GetComponent<Vertex>();
                    vertex.transform.position = position;
                    hex.vertexs[vertexOffset.Key] = vertex;
                    vertex.id = vertices.Count;
                    vertex.name = "Vertex" + vertex.id;
                    vertex.hexes.Add(hex);
                    vertices.Add(new Vector2(position.x, position.y), vertex);
                }
                else
                {
                    hex.vertexs[vertexOffset.Key] = ver;
                    ver.hexes.Add(hex);
                }
            }
        }
    }

    private void CreateEdges(Dictionary<Vector2, Hex> hexes, Dictionary<Vector2, Edge> edges)
    {
        foreach (Hex hex in hexes.Values)
        {
            foreach (var edgeOffset in _edgeOffsets)
            {
                Vector3 position = hex.transform.position + new Vector3(edgeOffset.Value.x, edgeOffset.Value.y);

                if (!edges.TryGetValue(new Vector2(position.x, position.y), out Edge e))
                {
                    Edge edge = Instantiate(_edgeGO, transform).GetComponent<Edge>();
                    edge.transform.position = position;
                    hex.edges[edgeOffset.Key] = edge;
                    edge.id = edges.Count;
                    edge.name = "Edge" + edge.id;
                    edge.hexes.Add(hex);
                    edges.Add(new Vector2(position.x, position.y), edge);
                }
                else
                {
                    hex.edges[edgeOffset.Key] = e;
                    e.hexes.Add(hex);
                }
            }
        }
    }

    private void LinkEdgesAndVertices(Dictionary<Vector2, Hex> hexes)
    {
        foreach (Hex hex in hexes.Values)
        {
            foreach(var edge in hex.edges)
            {
                foreach(VertexDirection vertexDirection in _edgeNeighborVertex[edge.Key])
                {
                    Vertex vertex = hex.vertexs[vertexDirection];
                    if (!edge.Value.vertices.Contains(vertex))
                    {
                        edge.Value.vertices.Add(vertex);
                        vertex.edges.Add(edge.Value);
                    }
                }
            }
        }
    }

    private void SetHexData(Dictionary<Vector2, Hex> hexes, List<HexDTO> hexesDTO)
    {
        var listHex = hexes.Values.ToList();
        for (int i = 0; i < hexesDTO.Count; i++)
        {
            listHex[i].type = (HexType)Enum.Parse(typeof(HexType), hexesDTO[i].hexType);
            listHex[i].gameObject.GetComponent<SpriteRenderer>().sprite = listHex[i].sprites[listHex[i].type];
            listHex[i].numberToken = hexesDTO[i].numberToken;
            if (i != hexesDTO.Count / 2)
            {
                NumberToken numberToken = Instantiate(_numberTokenGO, listHex[i].transform).GetComponent<NumberToken>();
                numberToken.numberText.text = hexesDTO[i].numberToken.ToString();
            }
        }
    }
}
