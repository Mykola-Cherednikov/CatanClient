using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexType
{
    DESERT,
    FIELD,
    FOREST,
    HILL,
    MOUNTAIN,
    PASTURE
}

public class Hex
{
    public int id;

    public HexType type;
}
