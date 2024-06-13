using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private GameObject gamePrefab;
    [SerializeField] private GameObject canvas;

    private void Start()
    {
        GameManager game = Instantiate(gamePrefab).GetComponent<GameManager>();

        List<SocketHexDTO> testData = new();
        for (int i = 0; i < 19; i++)
        {
            var test = new SocketHexDTO();
            test.id = i;
            test.numberToken = i;
            test.hexType = "DESERT";
            testData.Add(test);
        }

        User user = new User();
        user.id = 1;
        User user1 = new User();
        user1.id = 2;
        SocketBroadcastStartGameDTO dto = new SocketBroadcastStartGameDTO();
        dto.hexesInRowCounts = new() { 4, 5, 6, 7, 6, 5, 4 };
        dto.seed = 1;
        dto.users = new List<User>() { user, user1 };
        dto.currentUser = user;
        game.StartGame(dto, canvas);


        /*
        Vertex vertex = vertices.FirstOrDefault(v => v.id == 3);
        vertex.SetVertexBuilding(VertexBuildingType.SETTLEMENT, user);
        Edge edge = edges.FirstOrDefault(e => e.id == vertex.neighborEdges[0].id);
        edge.SetEdgeBuilding(EdgeBuildingType.ROAD, user);
        Edge edge1 = edges.FirstOrDefault(e => e.id == 1);
        edge1.SetEdgeBuilding(EdgeBuildingType.ROAD, user);

        
        Vertex vertex1 = vertices.FirstOrDefault(v => v.id == 31);
        vertex1.SetVertexBuilding(VertexBuildingType.SETTLEMENT, user1);
        Edge edge2 = edges.FirstOrDefault(e => e.id == 39);
        edge2.SetEdgeBuilding(EdgeBuildingType.ROAD, user1);
        Edge edge3 = edges.FirstOrDefault(e => e.id == 40);
        edge3.SetEdgeBuilding(EdgeBuildingType.ROAD, user1);*/
    }
}
