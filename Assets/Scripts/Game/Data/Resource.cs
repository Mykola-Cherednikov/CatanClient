using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Resource
{
    BRICK,
    LUMBER,
    ORE,
    GRAIN,
    WOOL
}

public enum DevCard
{
    VICTORY_POINT_CARD,
    KNIGHT_CARD,
    ROAD_BUILDING_CARD,
    YEAR_OF_PLENTY_CARD,
    MONOPOLY_CARD
}

public class ResourceCost
{
    public static Dictionary<Resource, int> GetRoadCost()
    {
        Dictionary<Resource, int> roadCost = new Dictionary<Resource, int>()
        {
            { Resource.BRICK, 1 },
            { Resource.LUMBER, 1 }
        };

        return roadCost;
    }

    public static Dictionary<Resource, int> GetSettlementCost()
    {
        Dictionary<Resource, int> settlementCost = new Dictionary<Resource, int>()
        {
            { Resource.BRICK, 1 },
            { Resource.LUMBER, 1 },
            { Resource.GRAIN, 1 },
            { Resource.WOOL, 1 }
        };

        return settlementCost;
    }

    public static Dictionary<Resource, int> GetCityCost()
    {
        Dictionary<Resource, int> cityCost = new Dictionary<Resource, int>()
        {
            { Resource.GRAIN, 2 },
            { Resource.ORE, 3 }
        };

        return cityCost;
    }

    public static Dictionary<Resource, int> GetDevelopmentCardCost()
    {
        Dictionary<Resource, int> developmentCardCost = new Dictionary<Resource, int>()
        {
            { Resource.WOOL, 1 },
            { Resource.GRAIN, 1 },
            { Resource.ORE, 1 }
        };

        return developmentCardCost;
    }
}
