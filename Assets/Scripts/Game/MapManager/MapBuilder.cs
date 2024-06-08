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

    private Dictionary<EdgeDirection, Vector2> edgeDirectionsToNeighborHexOffset;
    private Dictionary<VertexDirection, Vector2> vertexDirectionsToVertexOffset;
    private Dictionary<EdgeDirection, Vector2> edgeDirectionsToEdgeOffset;
    private Dictionary<EdgeDirection, List<VertexDirection>> edgeDirectionsToNeighborVertexDirections;
    private Dictionary<EdgeDirection, float> edgeDirectionsToEdgeRotation;
    private Dictionary<Vector2, Hex> coordinatesToHexes;
    private Dictionary<Vector2, Vertex> coordinatesToVertices;
    private Dictionary<Vector2, Edge> coordinatesToEdges;
    private List<int> hexesInRowCounts;
    private List<SocketHexDTO> hexesDataFromServerDTO;

    private float xOffsetBetweenHex = 4.7f;
    private float yOffsetBetweenHex = 4f;
    private float xVertexOffsetFromHexCentre = 2.35f;
    private float yTopVertexOffsetFromHexCentre = 2.7f;
    private float yMiddleVertexOffsetFromHexCenter = 2f;
    private float yDownVertexOffsetFromHexCentre = 1.3f;

    private void Awake()
    {
        hexPrefab = Resources.Load<GameObject>("Prefabs/Game/Hex");
        vertexPrefab = Resources.Load<GameObject>("Prefabs/Game/Vertex");
        edgePrefab = Resources.Load<GameObject>("Prefabs/Game/Edge");
        numberTokenPrefab = Resources.Load<GameObject>("Prefabs/Game/NumberToken");

        edgeDirectionsToNeighborHexOffset = new Dictionary<EdgeDirection, Vector2>()
        {
            { EdgeDirection.TR, new Vector2(xOffsetBetweenHex / 2f, yOffsetBetweenHex) },
            { EdgeDirection.R, new Vector2(xOffsetBetweenHex, 0f)},
            { EdgeDirection.DR, new Vector2(xOffsetBetweenHex / 2f, -yOffsetBetweenHex)},
            { EdgeDirection.DL, new Vector2(-xOffsetBetweenHex / 2f,-yOffsetBetweenHex)},
            { EdgeDirection.L, new Vector2(-xOffsetBetweenHex, 0f)},
            { EdgeDirection.TL, new Vector2(-xOffsetBetweenHex / 2f, yOffsetBetweenHex)}
        };

        vertexDirectionsToVertexOffset = new Dictionary<VertexDirection, Vector2>()
        {
            { VertexDirection.N, new Vector2(0f, yTopVertexOffsetFromHexCentre) },
            { VertexDirection.NE, new Vector2(xVertexOffsetFromHexCentre, yDownVertexOffsetFromHexCentre)},
            { VertexDirection.SE, new Vector2(xVertexOffsetFromHexCentre, -yDownVertexOffsetFromHexCentre)},
            { VertexDirection.S, new Vector2(0f, -yTopVertexOffsetFromHexCentre)},
            { VertexDirection.SW, new Vector2(-xVertexOffsetFromHexCentre, -yDownVertexOffsetFromHexCentre)},
            { VertexDirection.NW, new Vector2(-xVertexOffsetFromHexCentre, yDownVertexOffsetFromHexCentre)}
        };

        edgeDirectionsToEdgeOffset = new Dictionary<EdgeDirection, Vector2>()
        {
            { EdgeDirection.TR, new Vector2(xVertexOffsetFromHexCentre / 2f, yMiddleVertexOffsetFromHexCenter) },
            { EdgeDirection.R, new Vector2(xVertexOffsetFromHexCentre, 0f)},
            { EdgeDirection.DR, new Vector2(xVertexOffsetFromHexCentre / 2f, -yMiddleVertexOffsetFromHexCenter)},
            { EdgeDirection.DL, new Vector2(-xVertexOffsetFromHexCentre / 2f, -yMiddleVertexOffsetFromHexCenter)},
            { EdgeDirection.L, new Vector2(-xVertexOffsetFromHexCentre, 0f)},
            { EdgeDirection.TL, new Vector2(-xVertexOffsetFromHexCentre / 2f, yMiddleVertexOffsetFromHexCenter)}
        };

        edgeDirectionsToNeighborVertexDirections = new Dictionary<EdgeDirection, List<VertexDirection>>()
        {
            { EdgeDirection.TR, new(){ VertexDirection.N, VertexDirection.NE} },
            { EdgeDirection.R, new(){ VertexDirection.NE, VertexDirection.SE} },
            { EdgeDirection.DR, new(){ VertexDirection.SE, VertexDirection.S} },
            { EdgeDirection.DL, new(){ VertexDirection.S, VertexDirection.SW} },
            { EdgeDirection.L, new(){ VertexDirection.SW, VertexDirection.NW} },
            { EdgeDirection.TL, new(){ VertexDirection.NW, VertexDirection.N} }
        };

        edgeDirectionsToEdgeRotation = new Dictionary<EdgeDirection, float>() 
        {
            { EdgeDirection.TR, -30f},
            { EdgeDirection.R, 90f},
            { EdgeDirection.DR, 30f},
            { EdgeDirection.DL, -30f},
            { EdgeDirection.L, 90f},
            { EdgeDirection.TL, 30f}
        };

        coordinatesToHexes = new();
        coordinatesToVertices = new();
        coordinatesToEdges = new();
    }

    public MapInfo CreateMap(List<int> hexesInRowCounts, List<SocketHexDTO> hexesDataFromServerDTO)
    {
        this.hexesDataFromServerDTO = hexesDataFromServerDTO;
        this.hexesInRowCounts = hexesInRowCounts;

        GenerateMap();

        return GenerateMapInfoWithExistingInfo();
    }

    private void GenerateMap()
    {
        CreateHexes();
        SetHexNeighbors();
        CreateVertices();
        CreateEdges();
        LinkEdgesAndVerticesNeighbors();
        SetHexesResourse();
    }

    private MapInfo GenerateMapInfoWithExistingInfo()
    {
        MapInfo mapInfo = new MapInfo();
        mapInfo.hexes = coordinatesToHexes.Values.ToList();
        mapInfo.vertices = coordinatesToVertices.Values.ToList();
        mapInfo.edges = coordinatesToEdges.Values.ToList();

        return mapInfo;
    }

    private void CreateHexes()
    {
        float yTopRow = (hexesInRowCounts.Count - 1) / 2f;

        for (int rowIndex = 0; rowIndex < hexesInRowCounts.Count; rowIndex++)
        {
            float yCoordinate = yTopRow - rowIndex;
            CreateRowOfHexes(hexesInRowCounts[rowIndex], yCoordinate);
        }
    }

    private void CreateRowOfHexes(int numHexesInRow, float yCoordinate)
    {
        float xFinalRow = (numHexesInRow - 1) / 2f;

        for (int hexIndex = 0; hexIndex < numHexesInRow; hexIndex++)
        {
            float xCoordinate = hexIndex - xFinalRow;
            InstantiateAndAddHex(xCoordinate, yCoordinate);
        }
    }

    private void InstantiateAndAddHex(float xCoordinate, float yCoordinate)
    {
        GameObject hexGameObject = Instantiate(hexPrefab, transform);
        Vector3 hexPosition = new Vector3(xCoordinate * xOffsetBetweenHex, yCoordinate * yOffsetBetweenHex);
        hexGameObject.transform.position = SimixmanUtils.DecimalSumOfTwoVectors2D(transform.position, hexPosition);

        Hex hex = hexGameObject.GetComponent<Hex>();
        hex.id = coordinatesToHexes.Count;
        hex.name = "Hex" + hex.id;

        coordinatesToHexes.Add(new Vector2(hexPosition.x, hexPosition.y), hex);
    }

    private void SetHexNeighbors()
    {
        foreach (Hex hex in coordinatesToHexes.Values)
        {
            SetNeighborsForHex(hex);
        }
    }

    private void SetNeighborsForHex(Hex hex)
    {
        foreach (var directionOffsetPair in edgeDirectionsToNeighborHexOffset)
        {
            Vector2 neighborPosition = GetNeighborHexPosition(hex, directionOffsetPair.Value);
            if (coordinatesToHexes.TryGetValue(neighborPosition, out Hex neighborHex))
            {
                hex.edgeDirectionToNeighborHexes[directionOffsetPair.Key] = neighborHex;
            }
        }
    }

    private Vector2 GetNeighborHexPosition(Hex hex, Vector2 offset)
    {
        Vector2 hexPosition = new Vector2(hex.transform.position.x, hex.transform.position.y);
        return SimixmanUtils.DecimalSumOfTwoVectors2D(hexPosition, offset);
    }

    private void CreateVertices()
    {
        foreach (Hex hex in coordinatesToHexes.Values)
        {
            CreateVerticesForHex(hex);
        }
    }

    private void CreateVerticesForHex(Hex hex)
    {
        foreach (var directionOffsetPair in vertexDirectionsToVertexOffset)
        {
            Vector2 vertexPosition = GetVertexPosition(hex, directionOffsetPair.Value);
            if (!coordinatesToVertices.TryGetValue(vertexPosition, out Vertex vertex))
            {
                vertex = CreateVertex(vertexPosition);
            }
            AssignVertexToHex(hex, vertex, directionOffsetPair.Key);
        }
    }

    private Vector2 GetVertexPosition(Hex hex, Vector2 offset)
    {
        Vector2 hexPosition = new Vector2(hex.transform.position.x, hex.transform.position.y);
        return SimixmanUtils.DecimalSumOfTwoVectors2D(hexPosition, offset);
    }

    private Vertex CreateVertex(Vector2 position)
    {
        Vertex vertex = Instantiate(vertexPrefab, transform).GetComponent<Vertex>();
        vertex.transform.position = position;
        vertex.id = coordinatesToVertices.Count;
        vertex.name = "Vertex" + vertex.id;
        coordinatesToVertices[position] = vertex;

        return vertex;
    }

    private void AssignVertexToHex(Hex hex, Vertex vertex, VertexDirection direction)
    {
        hex.vertexDirectionToContainedVertiñes[direction] = vertex;
        vertex.neighborHexes.Add(hex);
    }

    private void CreateEdges()
    {
        foreach (Hex hex in coordinatesToHexes.Values)
        {
            CreateEdgesForHex(hex);
        }
    }

    private void CreateEdgesForHex(Hex hex)
    {
        foreach (var directionOffsetPair in edgeDirectionsToEdgeOffset)
        {
            Vector2 edgePosition = GetEdgePosition(hex, directionOffsetPair.Value);

            if (!coordinatesToEdges.TryGetValue(edgePosition, out Edge edge))
            {
                edge = CreateEdge(edgePosition);
                RotateEdgeToHex(edge, directionOffsetPair.Key);
            }

            AssignEdgeToHex(hex, edge, directionOffsetPair.Key);
            
        }
    }

    private Vector2 GetEdgePosition(Hex hex, Vector2 offset)
    {
        Vector2 hexPosition = new Vector2(hex.transform.position.x, hex.transform.position.y);
        return SimixmanUtils.DecimalSumOfTwoVectors2D(hexPosition, offset);
    }

    private void RotateEdgeToHex(Edge edge, EdgeDirection direction)
    {
        float rotationAngle = edgeDirectionsToEdgeRotation[direction];
        edge.transform.eulerAngles = new Vector3(0f, 0f, rotationAngle);
    }

    private Edge CreateEdge(Vector2 position)
    {
        Edge edge = Instantiate(edgePrefab, transform).GetComponent<Edge>();
        edge.transform.position = position;
        edge.id = coordinatesToEdges.Count;
        edge.name = "Edge" + edge.id;
        coordinatesToEdges.Add(position, edge);

        return edge;
    }

    private void AssignEdgeToHex(Hex hex, Edge edge, EdgeDirection direction)
    {
        hex.edgeDirectionToContainedEdges[direction] = edge;
        edge.neighborHexes.Add(hex);
    }

    private void LinkEdgesAndVerticesNeighbors()
    {
        foreach (Hex hex in coordinatesToHexes.Values)
        {
            LinkNeighborsEdgesAndVerticesForHex(hex);
        }
    }

    private void LinkNeighborsEdgesAndVerticesForHex(Hex hex)
    {
        foreach (var edgeDirectionPair in hex.edgeDirectionToContainedEdges)
        {
            LinkEdgeWithVertices(hex, edgeDirectionPair.Key, edgeDirectionPair.Value);
        }
    }

    private void LinkEdgeWithVertices(Hex hex, EdgeDirection edgeDirection, Edge edge)
    {
        foreach (VertexDirection vertexDirection in edgeDirectionsToNeighborVertexDirections[edgeDirection])
        {
            Vertex vertex = hex.vertexDirectionToContainedVertiñes[vertexDirection];
            AddVertexToEdgeIfNotExists(edge, vertex);
        }
    }

    private void AddVertexToEdgeIfNotExists(Edge edge, Vertex vertex)
    {
        if (!edge.neighborVertices.Contains(vertex))
        {
            edge.neighborVertices.Add(vertex);
            vertex.neighborEdges.Add(edge);
        }
    }

    private void SetHexesResourse()
    {
        List<Hex> hexes = coordinatesToHexes.Values.ToList();
        int indexMapCentre = hexesDataFromServerDTO.Count / 2;
        for (int i = 0; i < hexes.Count; i++)
        {
            SetHexResourse(hexes[i], hexesDataFromServerDTO[i]);
            if (i != indexMapCentre)
            {
                SetHexNumberToken(hexes[i], hexesDataFromServerDTO[i]);
            }
        }
    }

    private void SetHexResourse(Hex hex, SocketHexDTO hexDTO)
    {
        hex.type = (HexType)Enum.Parse(typeof(HexType), hexDTO.hexType);
        hex.gameObject.GetComponent<SpriteRenderer>().sprite = hex.sprites[hex.type];
        hex.numberToken = hexDTO.numberToken;

    }

    private void SetHexNumberToken(Hex hex, SocketHexDTO hexDTO)
    {
        NumberToken numberToken = Instantiate(numberTokenPrefab, hex.transform).GetComponent<NumberToken>();
        numberToken.numberText.text = hexDTO.numberToken.ToString();
    }
}
