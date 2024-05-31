using Assets.Scripts.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    [SerializeField] private GameObject hexPrefab;
    [SerializeField] private GameObject vertexPrefab;
    [SerializeField] private GameObject edgePrefab;
    [SerializeField] private GameObject numberTokenPrefab;

    private Dictionary<VertexDirection, Vector2> vertexOffsetsFromHexCenter;
    private Dictionary<EdgeDirection, Vector2> edgeOffsetsFromHexCenter;
    private Dictionary<EdgeDirection, List<VertexDirection>> edgeDirectionToNeighborVertexDirections;

    private float xIndendBetweenHex = 4.7f;
    private float yIndendBetweenHex = 4f;
    private float xVertexOffsetFromHexCentre = 2.35f;
    private float yTopVertexOffsetFromHexCentre = 2.7f;
    private float yDownVertexOffsetFromHexCentre = 1.3f;

    private void Awake()
    {
        hexPrefab = Resources.Load<GameObject>("Prefabs/Game/Hex");
        vertexPrefab = Resources.Load<GameObject>("Prefabs/Game/Vertex");
        edgePrefab = Resources.Load<GameObject>("Prefabs/Game/Edge");
        numberTokenPrefab = Resources.Load<GameObject>("Prefabs/Game/NumberToken");

        vertexOffsetsFromHexCenter = new Dictionary<VertexDirection, Vector2>()
        {
            { VertexDirection.N, new Vector2(0f, yTopVertexOffsetFromHexCentre) },
            { VertexDirection.NE, new Vector2(xVertexOffsetFromHexCentre, yDownVertexOffsetFromHexCentre)},
            { VertexDirection.SE, new Vector2(xVertexOffsetFromHexCentre, -yDownVertexOffsetFromHexCentre)},
            { VertexDirection.S, new Vector2(0f, -yTopVertexOffsetFromHexCentre)},
            { VertexDirection.SW, new Vector2(-xVertexOffsetFromHexCentre, -yDownVertexOffsetFromHexCentre)},
            { VertexDirection.NW, new Vector2(-xVertexOffsetFromHexCentre, yDownVertexOffsetFromHexCentre)}
        };

        edgeOffsetsFromHexCenter = new Dictionary<EdgeDirection, Vector2>()
        {
            { EdgeDirection.TR, new Vector2(xVertexOffsetFromHexCentre / 2f, yTopVertexOffsetFromHexCentre) },
            { EdgeDirection.R, new Vector2(xVertexOffsetFromHexCentre, 0f)},
            { EdgeDirection.DR, new Vector2(xVertexOffsetFromHexCentre / 2f, -yTopVertexOffsetFromHexCentre)},
            { EdgeDirection.DL, new Vector2(-xVertexOffsetFromHexCentre / 2f, -yTopVertexOffsetFromHexCentre)},
            { EdgeDirection.L, new Vector2(-xVertexOffsetFromHexCentre, 0f)},
            { EdgeDirection.TL, new Vector2(-xVertexOffsetFromHexCentre / 2f, yTopVertexOffsetFromHexCentre)}
        };

        edgeDirectionToNeighborVertexDirections = new Dictionary<EdgeDirection, List<VertexDirection>>()
        {
            { EdgeDirection.TR, new(){ VertexDirection.N, VertexDirection.NE} },
            { EdgeDirection.R, new(){ VertexDirection.NE, VertexDirection.SE} },
            { EdgeDirection.DR, new(){ VertexDirection.SE, VertexDirection.S} },
            { EdgeDirection.DL, new(){ VertexDirection.S, VertexDirection.SW} },
            { EdgeDirection.L, new(){ VertexDirection.SW, VertexDirection.NW} },
            { EdgeDirection.TL, new(){ VertexDirection.NW, VertexDirection.N} },
        };
    }

    public MapInfo CreateMap(List<int> numHexesInMapRow, List<HexDTO> hexesDTO)
    {
        Dictionary<Vector2, Hex> coordinatesToHexes = new();
        Dictionary<Vector2, Vertex> coordinatesToVertices = new();
        Dictionary<Vector2, Edge> coordinatesToEdges = new();

        CreateHexes(coordinatesToHexes, numHexesInMapRow);
        SetHexNeighbor(coordinatesToHexes);
        CreateVertices(coordinatesToHexes, coordinatesToVertices);
        CreateEdges(coordinatesToHexes, coordinatesToEdges);
        LinkEdgesAndVertices(coordinatesToHexes);
        SetHexData(coordinatesToHexes, hexesDTO);

        MapInfo mapInfo = new MapInfo();
        mapInfo.hexes = coordinatesToHexes.Values.ToList();
        mapInfo.vertices = coordinatesToVertices.Values.ToList();
        mapInfo.edges = coordinatesToEdges.Values.ToList();

        return mapInfo;
    }

    private void CreateHexes(Dictionary<Vector2, Hex> coordinatesToHexes, List<int> numHexesInMapRow)
    {
        int rowNum = 0;
        float yCoordinateFromCentreMap = (numHexesInMapRow.Count - 1) / 2f;
        for (float yCoordinate = yCoordinateFromCentreMap; yCoordinate >= -yCoordinateFromCentreMap; yCoordinate--)
        {
            float xCoordinateFromCentreMap = (numHexesInMapRow[rowNum] - 1) / 2f;
            for (float xCoordinate = -xCoordinateFromCentreMap; xCoordinate <= xCoordinateFromCentreMap; xCoordinate++)
            {
                GameObject go = Instantiate(hexPrefab, transform);
                go.transform.position = transform.position + new Vector3(xCoordinate * xIndendBetweenHex, yCoordinate * yIndendBetweenHex);
                Hex hex = go.GetComponent<Hex>();
                hex.id = coordinatesToHexes.Count;
                hex.name = "Hex" + hex.id;
                coordinatesToHexes.Add(new Vector2(go.transform.position.x, go.transform.position.y), hex);
            }
            rowNum++;
        }
    }

    private void SetHexNeighbor(Dictionary<Vector2, Hex> coordinatesToHexes)
    {
        foreach (Hex hex in coordinatesToHexes.Values)
        {
            if (coordinatesToHexes.TryGetValue(new Vector2(hex.transform.position.x + (xIndendBetweenHex / 2f),
                hex.transform.position.y + yIndendBetweenHex), out Hex hexTR))
            {
                hex.edgeDirectionToNeighborHexes[EdgeDirection.TR] = hexTR;
            }

            if (coordinatesToHexes.TryGetValue(new Vector2(hex.transform.position.x + xIndendBetweenHex,
                hex.transform.position.y), out Hex hexR))
            {
                hex.edgeDirectionToNeighborHexes[EdgeDirection.R] = hexR;
            }

            if (coordinatesToHexes.TryGetValue(new Vector2(hex.transform.position.x + (xIndendBetweenHex / 2f),
                hex.transform.position.y - yIndendBetweenHex), out Hex hexDR))
            {
                hex.edgeDirectionToNeighborHexes[EdgeDirection.DR] = hexDR;
            }

            if (coordinatesToHexes.TryGetValue(new Vector2(hex.transform.position.x - (xIndendBetweenHex / 2f),
                hex.transform.position.y - yIndendBetweenHex), out Hex hexDL))
            {
                hex.edgeDirectionToNeighborHexes[EdgeDirection.DL] = hexDL;
            }

            if (coordinatesToHexes.TryGetValue(new Vector2(hex.transform.position.x - xIndendBetweenHex,
                hex.transform.position.y), out Hex hexL))
            {
                hex.edgeDirectionToNeighborHexes[EdgeDirection.L] = hexL;
            }

            if (coordinatesToHexes.TryGetValue(new Vector2(hex.transform.position.x - (xIndendBetweenHex / 2f),
                hex.transform.position.y + yIndendBetweenHex), out Hex hexTL))
            {
                hex.edgeDirectionToNeighborHexes[EdgeDirection.TL] = hexTL;
            }
        }
    }

    private void CreateVertices(Dictionary<Vector2, Hex> coordinatesToHexes, Dictionary<Vector2, Vertex> coordinatesToVertices)
    {
        foreach (Hex hex in coordinatesToHexes.Values)
        {
            foreach (var vertexOffset in vertexOffsetsFromHexCenter)
            {
                Vector3 vertexPosition = hex.transform.position + new Vector3(vertexOffset.Value.x, vertexOffset.Value.y);
                if (!coordinatesToVertices.TryGetValue(new Vector2(vertexPosition.x, vertexPosition.y), out Vertex ver))
                {
                    Vertex vertex = Instantiate(vertexPrefab, transform).GetComponent<Vertex>();
                    vertex.transform.position = vertexPosition;
                    hex.vertexDirectionToContainedVertiñes[vertexOffset.Key] = vertex;
                    vertex.id = coordinatesToVertices.Count;
                    vertex.name = "Vertex" + vertex.id;
                    vertex.neighborHexes.Add(hex);
                    coordinatesToVertices.Add(new Vector2(vertexPosition.x, vertexPosition.y), vertex);
                }
                else
                {
                    hex.vertexDirectionToContainedVertiñes[vertexOffset.Key] = ver;
                    ver.neighborHexes.Add(hex);
                }
            }
        }
    }

    private void CreateEdges(Dictionary<Vector2, Hex> coordinatesToHexes, Dictionary<Vector2, Edge> coordinatesToEdges)
    {
        foreach (Hex hex in coordinatesToHexes.Values)
        {
            foreach (var edgeOffset in edgeOffsetsFromHexCenter)
            {
                Vector3 edgePosition = hex.transform.position + new Vector3(edgeOffset.Value.x, edgeOffset.Value.y);

                if (!coordinatesToEdges.TryGetValue(new Vector2(edgePosition.x, edgePosition.y), out Edge e))
                {
                    Edge edge = Instantiate(edgePrefab, transform).GetComponent<Edge>();
                    edge.transform.position = edgePosition;
                    hex.edgeDirectionToContainedEdges[edgeOffset.Key] = edge;
                    edge.id = coordinatesToEdges.Count;
                    edge.name = "Edge" + edge.id;
                    edge.neighborHexes.Add(hex);
                    coordinatesToEdges.Add(new Vector2(edgePosition.x, edgePosition.y), edge);
                }
                else
                {
                    hex.edgeDirectionToContainedEdges[edgeOffset.Key] = e;
                    e.neighborHexes.Add(hex);
                }
            }
        }
    }

    private void LinkEdgesAndVertices(Dictionary<Vector2, Hex> coordinatesToHexes)
    {
        foreach (Hex hex in coordinatesToHexes.Values)
        {
            foreach (var directionToEdge in hex.edgeDirectionToContainedEdges)
            {
                foreach (VertexDirection vertexDirection in edgeDirectionToNeighborVertexDirections[directionToEdge.Key])
                {
                    Vertex vertex = hex.vertexDirectionToContainedVertiñes[vertexDirection];
                    if (!directionToEdge.Value.neighborVertices.Contains(vertex))
                    {
                        directionToEdge.Value.neighborVertices.Add(vertex);
                        vertex.neighborEdges.Add(directionToEdge.Value);
                    }
                }
            }
        }
    }

    private void SetHexData(Dictionary<Vector2, Hex> coordinatesToHexes, List<HexDTO> hexesDTO)
    {
        List<Hex> hexes = coordinatesToHexes.Values.ToList();
        for (int i = 0; i < hexesDTO.Count; i++)
        {
            hexes[i].type = (HexType)Enum.Parse(typeof(HexType), hexesDTO[i].hexType);
            hexes[i].gameObject.GetComponent<SpriteRenderer>().sprite = hexes[i].sprites[hexes[i].type];
            hexes[i].numberToken = hexesDTO[i].numberToken;
            if (i != hexesDTO.Count / 2)
            {
                NumberToken numberToken = Instantiate(numberTokenPrefab, hexes[i].transform).GetComponent<NumberToken>();
                numberToken.numberText.text = hexesDTO[i].numberToken.ToString();
            }
        }
    }
}
