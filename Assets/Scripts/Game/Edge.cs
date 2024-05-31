using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    public int id;

    public List<Vertex> neighborVertices;

    public List<Hex> neighborHexes;

    private void Awake()
    {
        neighborVertices = new List<Vertex>();
        neighborHexes = new List<Hex>();
    }
}
