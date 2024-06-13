using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class User
{
    public int id;

    public string name;

    public Dictionary<Resource, int> userResources;

    public Color color;

    public User()
    {
        userResources = new Dictionary<Resource, int>()
        {
            { Resource.BRICK, 0 },
            { Resource.WOOL, 0 },
            { Resource.ORE, 0 },
            { Resource.GRAIN, 0 },
            { Resource.LUMBER, 0 }
        };
    }
}

