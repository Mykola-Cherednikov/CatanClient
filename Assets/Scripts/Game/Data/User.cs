using System;
using System.Collections.Generic;
using UnityEngine;

public enum Buff
{
    ROAD_BUILDING
}

[Serializable]
public class User
{
    public int id;

    public string name;

    public Dictionary<Resource, int> userResources;

    public Dictionary<Card, int> userCards;

    public Dictionary<Buff, int> buffDuration;

    public Color color;

    public bool isLongestRoad;

    public bool isLargestArmy;

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

        userCards = new Dictionary<Card, int>() {
            { Card.VICTORY_POINT, 0 },
            { Card.KNIGHT, 0 },
            { Card.ROAD_BUILDING, 0 },
            { Card.YEAR_OF_PLENTY, 0 },
            { Card.MONOPOLY, 0 },
            { Card.UNKNOWN, 0 }
        };

        buffDuration = new Dictionary<Buff, int>()
        { 
            { Buff.ROAD_BUILDING, 0 } 
        };

        isLongestRoad = false;
    }
}

