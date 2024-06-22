using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    [SerializeField] private GameObject hexPrefab;
    [SerializeField] private GameObject vertexPrefab;
    [SerializeField] private GameObject edgePrefab;
    [SerializeField] private GameObject seaHexPrefab;
    [SerializeField] private GameObject numberTokenPrefab;

    private Dictionary<EdgeDirection, Vector2> edgeDirectionsToNeighbourHexOffset;
    private Dictionary<VertexDirection, Vector2> vertexDirectionsToVertexOffset;
    private Dictionary<EdgeDirection, Vector2> edgeDirectionsToEdgeOffset;
    private Dictionary<EdgeDirection, List<VertexDirection>> edgeDirectionsToNeighbourVertexDirections;
    private Dictionary<EdgeDirection, float> edgeDirectionsToEdgeRotation;
    private Dictionary<Vector2, Hex> coordinatesToHexes;
    private Dictionary<Vector2, Vertex> coordinatesToVertices;
    private Dictionary<Vector2, Edge> coordinatesToEdges;
    private Dictionary<EdgeDirection, float> edgeDirectionsToSeaHexRotation;
    private List<int> hexesInRowCounts;

    private JavaRandom mapRandom;
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
        seaHexPrefab = Resources.Load<GameObject>("Prefabs/Game/SeaHex");

        edgeDirectionsToNeighbourHexOffset = new Dictionary<EdgeDirection, Vector2>()
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

        edgeDirectionsToNeighbourVertexDirections = new Dictionary<EdgeDirection, List<VertexDirection>>()
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

        edgeDirectionsToSeaHexRotation = new Dictionary<EdgeDirection, float>()
        {
            { EdgeDirection.TR, 240f},
            { EdgeDirection.R, 180f},
            { EdgeDirection.DR, 120f},
            { EdgeDirection.DL, 60f},
            { EdgeDirection.L, 0f},
            { EdgeDirection.TL, -60f}
        };

        coordinatesToHexes = new();
        coordinatesToVertices = new();
        coordinatesToEdges = new();
    }

    public MapInfo CreateMap(List<int> hexesInRowCounts, int seed)
    {
        mapRandom = new JavaRandom(seed);
        this.hexesInRowCounts = hexesInRowCounts;

        GenerateMap();

        return GenerateMapInfoWithExistingInfo();
    }

    private void GenerateMap()
    {
        CreateHexes();
        SetHexneighbours();
        CreateVertices();
        CreateEdges();
        LinkEdgesAndVerticesneighbours();
        GenerateAndSetHexTypes();
        GenerateHexNumberTokens();
        GenerateHarbors();
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

    private void SetHexneighbours()
    {
        foreach (Hex hex in coordinatesToHexes.Values)
        {
            SetneighboursForHex(hex);
        }
    }

    private void SetneighboursForHex(Hex hex)
    {
        foreach (var directionOffsetPair in edgeDirectionsToNeighbourHexOffset)
        {
            Vector2 neighbourPosition = GetneighbourHexPosition(hex, directionOffsetPair.Value);
            if (coordinatesToHexes.TryGetValue(neighbourPosition, out Hex neighbourHex))
            {
                hex.edgeDirectionToneighbourHexes[directionOffsetPair.Key] = neighbourHex;
            }
        }
    }

    private Vector2 GetneighbourHexPosition(Hex hex, Vector2 offset)
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
        hex.vertexDirectionToContainedVertices[direction] = vertex;
        vertex.neighbourHexes.Add(hex);
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
        edge.neighbourHexes.Add(hex);
    }

    private void LinkEdgesAndVerticesneighbours()
    {
        foreach (Hex hex in coordinatesToHexes.Values)
        {
            LinkneighboursEdgesAndVerticesForHex(hex);
        }
    }

    private void LinkneighboursEdgesAndVerticesForHex(Hex hex)
    {
        foreach (var edgeDirectionPair in hex.edgeDirectionToContainedEdges)
        {
            LinkEdgeWithVertices(hex, edgeDirectionPair.Key, edgeDirectionPair.Value);
        }
    }

    private void LinkEdgeWithVertices(Hex hex, EdgeDirection edgeDirection, Edge edge)
    {
        foreach (VertexDirection vertexDirection in edgeDirectionsToNeighbourVertexDirections[edgeDirection])
        {
            Vertex vertex = hex.vertexDirectionToContainedVertices[vertexDirection];
            AddVertexToEdgeIfNotExists(edge, vertex);
        }
    }

    private void AddVertexToEdgeIfNotExists(Edge edge, Vertex vertex)
    {
        if (!edge.neighbourVertices.Contains(vertex))
        {
            edge.neighbourVertices.Add(vertex);
            vertex.neighbourEdges.Add(edge);
        }
    }

    private void GenerateAndSetHexTypes()
    {
        int numOfHexes = hexesInRowCounts.Sum();
        numOfHexes--;
        List<HexType> hexTypes = new();
        CreateHexTypes(hexTypes, numOfHexes);
        Shuffle(hexTypes);
        AddDesertType(hexTypes, numOfHexes);
        SetHexTypes(hexTypes);
    }

    private List<HexType> CreateHexTypes(List<HexType> hexTypes, int numOfHexes)
    {
        int koefHexNum = numOfHexes / 3;
        int numOfPrimaryResources = koefHexNum * 2;
        int numOfSecondaryResources = koefHexNum;
        for (int i = 0; i < numOfPrimaryResources; i++)
        {
            hexTypes.Add((HexType)(i % 3));
        }

        for (int i = 0; i < numOfSecondaryResources; i++)
        {
            hexTypes.Add((HexType)(3 + (i % 2)));
        }

        return hexTypes;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int index = mapRandom.NextInt(list.Count);
            (list[i], list[index]) = (list[index], list[i]);
        }
    }

    private void AddDesertType(List<HexType> hexTypes, int numOfHexes)
    {
        hexTypes.Add(HexType.DESERT);
        numOfHexes++;
        (hexTypes[numOfHexes / 2], hexTypes[numOfHexes - 1]) = (hexTypes[numOfHexes - 1], hexTypes[numOfHexes / 2]);
    }

    private void SetHexTypes(List<HexType> hexTypes)
    {
        List<Hex> hexes = coordinatesToHexes.Values.ToList();
        for (int i = 0; i < hexes.Count; i++)
        {
            hexes[i].type = hexTypes[i];
            hexes[i].gameObject.GetComponent<SpriteRenderer>().sprite = hexes[i].sprites[hexes[i].type];
        }
    }

    private void GenerateHexNumberTokens()
    {
        List<Hex> hexes = coordinatesToHexes.Values.ToList();
        List<int> numberTokens = new List<int>() { 2, 3, 3, 4, 4, 5, 5, 6, 6, 8, 8, 9, 9, 10, 10, 11, 11, 12 };
        Queue<int> numberTokensQueue = new Queue<int>();

        CreateNumsToNumberTokensWithShuffle(numberTokens, numberTokensQueue, hexes);
        CreateNumberTokensToHexes(numberTokensQueue, hexes);
    }

    private void CreateNumsToNumberTokensWithShuffle(List<int> numberTokens, Queue<int> numberTokensQueue, List<Hex> hexes)
    {
        while (numberTokensQueue.Count < hexes.Count)
        {
            Shuffle(numberTokens);
            foreach (int num in numberTokens)
            {
                numberTokensQueue.Enqueue(num);
            }
        }
    }

    private void CreateNumberTokensToHexes(Queue<int> numberTokensQueue, List<Hex> hexes)
    {
        for (int i = 0; i < hexes.Count; i++)
        {
            hexes[i].numberToken = Instantiate(numberTokenPrefab, hexes[i].transform).GetComponent<NumberToken>();
            hexes[i].numberToken.id = hexes[i].id;
            if (hexes[i] == hexes[hexes.Count / 2])
            {
                hexes[i].numberToken.spriteRenderer.enabled = false;
                hexes[i].numberToken.numberText.text = "";
                continue;
            }
            int num = numberTokensQueue.Dequeue();
            hexes[i].numberToken.numberText.text = num.ToString();
        }
    }

    private void GenerateHarbors()
    {
        var boundaryEdges = GetBoundaryEdges();
        var boundaryEdgesInOrder = GetBoundaryEdgesInRightOrder(boundaryEdges);
        var harborTypes = GetShuffledHarborTypes();
        var spacesBetweenHarbors = new List<int> { 3, 3, 4 };

        int startIndexEdge = mapRandom.NextInt(boundaryEdgesInOrder.Count);
        int finalIndexEdge = startIndexEdge + boundaryEdges.Count;
        int index = startIndexEdge;
        int harborIndex = 0;

        while (index < finalIndexEdge)
        {
            var edge = boundaryEdgesInOrder[index % boundaryEdgesInOrder.Count];
            var harborType = harborTypes[harborIndex % harborTypes.Count];

            SimixmanLogger.Log($"Harbor at: {edge.id} with type: {Enum.GetName(typeof(HarborType), harborType)}");
            SetVerticesHarborType(edge, harborType);
            CreateSeaHex(edge, harborType);

            index += spacesBetweenHarbors[harborIndex % spacesBetweenHarbors.Count];
            harborIndex++;
        }
    }

    private List<Edge> GetBoundaryEdges()
    {
        return coordinatesToEdges.Values
            .Where(e => e.neighbourHexes.Count == 1)
            .ToList();
    }

    private List<HarborType> GetShuffledHarborTypes()
    {
        List<HarborType> harborTypes = new List<HarborType>()
        {
            HarborType.ORE, HarborType.BRICK, HarborType.LUMBER,
            HarborType.GRAIN, HarborType.WOOL, HarborType.GENERIC,
            HarborType.GENERIC, HarborType.GENERIC, HarborType.GENERIC
        };

        Shuffle(harborTypes);
        return harborTypes;
    }

    private void SetVerticesHarborType(Edge edge, HarborType harborType)
    {
        foreach (var vertex in edge.neighbourVertices)
        {
            vertex.harborType = harborType;
        }
    }

    private void CreateSeaHex(Edge edge, HarborType harborType)
    {
        var edgeHex = edge.neighbourHexes.First();
        var edgeDirection = edgeHex.edgeDirectionToContainedEdges
            .FirstOrDefault(e => e.Value == edge).Key;

        var newSeaHexPosition = SimixmanUtils.DecimalSumOfTwoVectors2D(
            edgeHex.transform.position,
            edgeDirectionsToNeighbourHexOffset[edgeDirection]
        );

        var seaHexGO = Instantiate(seaHexPrefab, transform);
        seaHexGO.transform.position = newSeaHexPosition;
        seaHexGO.transform.eulerAngles = new Vector3(0, 0, edgeDirectionsToSeaHexRotation[edgeDirection]);
        seaHexGO.GetComponent<SeaHex>().SetInfo(harborType);
    }

    private List<Edge> GetBoundaryEdgesInRightOrder(List<Edge> boundaryEdges)
    {
        var boundaryEdgesInOrder = new HashSet<Edge>();
        var currentEdge = boundaryEdges.First();
        boundaryEdgesInOrder.Add(currentEdge);

        while (true)
        {
            currentEdge = MapUtils.GetneighbourEdgesToEdge(currentEdge)
                .FirstOrDefault(e => boundaryEdges.Contains(e) && !boundaryEdgesInOrder.Contains(e));

            if (currentEdge == null)
                break;

            boundaryEdgesInOrder.Add(currentEdge);
        }

        return boundaryEdgesInOrder.ToList();
    }
}