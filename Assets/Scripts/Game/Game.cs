using Assets.Scripts.Game;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    [SerializeField]private GameObject _lobbyFormGO;

    private GameObject _camera;
    private GameObject _canvas;
    private UIGameHandler _uiHandler;


    private List<Hex> _hexes;
    private List<Vertex> _vertexes;
    private List<Edge> _edges;
    private List<Player> _player;

    private void Awake()
    {
        Multiplayer.Instance.CONNECTION_ERROR_EVENT.AddListener(OnConnectionError);
        _camera = Camera.main.gameObject;
        _camera.transform.position = new Vector3(0f, 0f, -10f);
    }

    public async void SetStartGameData(List<int> numInRows, List<HexDTO> hexes, GameObject canvas)
    {
        _canvas = canvas;
        MapBuilder mapCreator = gameObject.AddComponent<MapBuilder>();
        MapData md = mapCreator.CreateMap(numInRows, hexes);
        _hexes = md.hexes;
        _vertexes = md.vertices;
        _edges = md.edges;
        Destroy(mapCreator);
        _uiHandler = _canvas.AddComponent<UIGameHandler>();

        //await Multiplayer.Instance.SocketReadyAndLoadMessage();
    }

    private void OnConnectionError(object data)
    {
        Destroy(_uiHandler);
        Instantiate(_lobbyFormGO, _canvas.transform);
        Destroy(gameObject);
    }
}
