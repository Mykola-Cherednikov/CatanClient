using Assets.Scripts.Game;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    [SerializeField]private GameObject lobbiesFormPrefab;

    private GameObject cameraGO;
    private GameObject uiCanvas;
    private UIGameHandler uiHandler;


    private List<Hex> hexes;
    private List<Vertex> vertices;
    private List<Edge> edges;
    private List<Player> players;

    private void Awake()
    {
        Multiplayer.Instance.CONNECTION_ERROR_EVENT.AddListener(OnConnectionError);
        cameraGO = Camera.main.gameObject;
        cameraGO.transform.position = new Vector3(0f, 0f, -10f);
    }

    public async void SetStartGameData(List<int> numInRows, List<HexDTO> hexes, GameObject canvas)
    {
        this.uiCanvas = canvas;
        MapBuilder mapCreator = gameObject.AddComponent<MapBuilder>();
        MapInfo md = mapCreator.CreateMap(numInRows, hexes);
        this.hexes = md.hexes;
        vertices = md.vertices;
        edges = md.edges;
        Destroy(mapCreator);
        uiHandler = this.uiCanvas.AddComponent<UIGameHandler>();

        await Multiplayer.Instance.SocketSendReadyAndLoadMessage();
    }

    private void OnConnectionError(object dtoObject)
    {
        Destroy(uiHandler);
        Instantiate(lobbiesFormPrefab, uiCanvas.transform);
        Destroy(gameObject);
    }
}
