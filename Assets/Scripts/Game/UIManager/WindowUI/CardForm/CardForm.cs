using System;
using UnityEngine;
using UnityEngine.UI;

public class CardForm : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject cardRowPrefab;
    [SerializeField] private Button buyButton;

    private void Awake()
    {
        foreach (var card in Enum.GetValues(typeof(Card)))
        {
            if ((Card)card == Card.UNKNOWN)
            {
                continue;
            }

            CreateCardRow((Card)card);
        }

        GameManager.Instance.uiManager.CHANGE_UI_STATE += UpdateForm;
        GameManager.Instance.resourceManager.RESOURCES_CHANGED_EVENT += UpdateForm;
        GameManager.Instance.cardManager.CARD_CHANGED_EVENT += UpdateForm;
        UpdateForm();
    }

    private void UpdateForm()
    {
        buyButton.interactable = GameManager.Instance.userManager.IsCurrentUserCanBuyCardNow();
    }

    private void CreateCardRow(Card card)
    {
        CardRow cardRow = Instantiate(cardRowPrefab, content.transform).GetComponent<CardRow>();
        cardRow.SetInfo(card, this);
    }

    public void PlanToBuyCard()
    {
        GameManager.Instance.cardManager.PlanToBuyCard();
    }

    public void PlanToUseCard(Card card)
    {
        if (!GameManager.Instance.userManager.IsCurrentUserCanUseCardNow(card))
        {
            return;
        }

        switch (card)
        {
            case Card.KNIGHT:
                PlanToUseKnight();
                break;
            case Card.YEAR_OF_PLENTY:
                PlanToUseYearOfPleanty();
                break;
            case Card.MONOPOLY:
                PlanToUseMonopoly();
                break;
            case Card.ROAD_BUILDING:
                PlanToUseRoadBuilding();
                break;
        }
    }

    public void PlanToUseKnight()
    {
        GameManager.Instance.uiManager.StartUserMoveRobberState();
        Destroy(gameObject);
    }

    public void PlanToUseYearOfPleanty()
    {
        GameManager.Instance.uiManager.windowUI.OpenResourceFormForYearOfPlenty();
        Destroy(gameObject);
    }

    public void PlanToUseMonopoly()
    {
        GameManager.Instance.uiManager.windowUI.OpenResourceFormForMonopoly();
        Destroy(gameObject);
    }

    public void PlanToUseRoadBuilding()
    {
        GameManager.Instance.cardManager.PlanUseRoadBuildingCard();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.Instance.uiManager.CHANGE_UI_STATE -= UpdateForm;
        GameManager.Instance.resourceManager.RESOURCES_CHANGED_EVENT -= UpdateForm;
        GameManager.Instance.cardManager.CARD_CHANGED_EVENT -= UpdateForm;
    }
}
