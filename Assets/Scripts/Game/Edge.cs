using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    public int id;

    public List<Vertex> vertices;

    public List<Hex> hexes;

    private void Awake()
    {
        vertices = new List<Vertex>();
        hexes = new List<Hex>();
    }
}
