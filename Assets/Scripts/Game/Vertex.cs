using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex : MonoBehaviour
{
    public int id;

    public List<Hex> neighborHexes;

    public List<Edge> neighborEdges;

    private void Awake()
    {
        neighborHexes = new();
        neighborEdges = new();
    }
}
