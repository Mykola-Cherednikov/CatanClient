using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public enum Card
{
    VICTORY_POINT,
    KNIGHT,
    ROAD_BUILDING,
    YEAR_OF_PLENTY,
    MONOPOLY,
    UNKNOWN
}

public class CardManager : MonoBehaviour
{
    private int numOfCardsInStorage;

    public UnityAction CARD_CHANGED_EVENT;

    private void Awake()
    {
        numOfCardsInStorage = 25;
    }

    public bool IsStorageHaveCard()
    {
        return numOfCardsInStorage > 0;
    } 

    public async void PlanToBuyCard()
    {
        if (!GameManager.Instance.userManager.IsCurrentUserCanBuyCardNow())
        {
            return;
        }

        if(!IsStorageHaveCard())
        {
            return;
        }

        await Multiplayer.Instance.SocketSendBuyCardRequest();
    }

    public void BuyCard(User user, Card card)
    {
        GameManager.Instance.resourceManager.BuyGoods(user, Goods.Card);
        AddCardToUser(user, card);
    }

    private void AddCardToUser(User user, Card card)
    {
        if (user == GameManager.Instance.userManager.currentUser)
        {
            user.userCards[card]++;
        }
        else
        {
            user.userCards[Card.UNKNOWN]++;
        }
        numOfCardsInStorage--;
        CARD_CHANGED_EVENT?.Invoke();
    }

    private void RemoveCardFromUser(User user, Card card)
    {
        if (user == GameManager.Instance.userManager.currentUser)
        {
            user.userCards[card]--;
        }
        else
        {
            user.userCards[Card.UNKNOWN]--;
        }
        CARD_CHANGED_EVENT?.Invoke();
    }
    
    public async void PlanUseKnightCard(int hexId)
    {
        if (!GameManager.Instance.userManager.IsCurrentUserCanUseCardNow(Card.KNIGHT))
        {
            return;
        }

        await Multiplayer.Instance.SocketSendUseKnightCardRequest(hexId);
    }

    public async void PlanUseRoadBuildingCard()
    {
        if (!GameManager.Instance.userManager.IsCurrentUserCanUseCardNow(Card.ROAD_BUILDING))
        {
            return;
        }

        await Multiplayer.Instance.SocketSendUseRoadBuildingCardRequest();
    }

    public async void PlanUseYearOfPlentyCard(List<Resource> resources)
    {
        if (!GameManager.Instance.userManager.IsCurrentUserCanUseCardNow(Card.YEAR_OF_PLENTY))
        {
            return;
        }

        await Multiplayer.Instance.SocketSendUseYearOfPlentyCardRequest(resources);
    }

    public async void PlanUseMonopolyCard(Resource resource)
    {
        if (!GameManager.Instance.userManager.IsCurrentUserCanUseCardNow(Card.MONOPOLY))
        {
            return;
        }

        await Multiplayer.Instance.SocketSendUseMonopolyCardRequest(resource);
    }

    public void UseKnightCard(User user, int hexId)
    {
        RemoveCardFromUser(user, Card.KNIGHT);
        GameManager.Instance.mapManager.PlaceRobber(hexId);
    }

    public void UseRoadBuildingCard(User user)
    {
        RemoveCardFromUser(user, Card.ROAD_BUILDING);
        user.buffDuration[Buff.ROAD_BUILDING] = 2; 
    }

    public void UseYearOfPlentyCard(User user, List<string> resources)
    {
        RemoveCardFromUser(user, Card.YEAR_OF_PLENTY);
        GameManager.Instance.resourceManager
            .YearOfPlentyCardMoveResources(user, SimixmanUtils.ResourceListToResourceDictionary(resources));
    }

    public void UseMonopolyCard(User user, Resource resource)
    {
        RemoveCardFromUser(user, Card.MONOPOLY);
        GameManager.Instance.resourceManager.MonopolyCardMoveResources(user, resource);
    }
}
