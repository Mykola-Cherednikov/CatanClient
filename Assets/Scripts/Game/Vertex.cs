using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex : MonoBehaviour
{
    public int id;

    public List<Hex> hexes;

    public List<Edge> edges;

    private void Awake()
    {
        hexes = new();
        edges = new();
    }
}
