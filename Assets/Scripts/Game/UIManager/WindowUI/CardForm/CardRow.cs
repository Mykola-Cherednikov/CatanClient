using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardRow : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Button useButton;
    private CardForm cardForm;
    private Card card;
    private Dictionary<Card, string> cardToName;

    private void Awake()
    {
        GameManager.Instance.uiManager.UPDATE_UI_EVENT.AddListener(UpdateInfo);
        cardToName = new()
        {
            { Card.MONOPOLY, "Monopoly" },
            { Card.KNIGHT, "Knight" },
            { Card.VICTORY_POINT, "Victory point" },
            { Card.ROAD_BUILDING, "Road building" },
            { Card.YEAR_OF_PLENTY, "Year of plenty" }
        };
    }

    public void SetInfo(Card card, CardForm cardForm)
    {
        this.card = card;
        this.cardForm = cardForm;
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        nameText.text = cardToName[card];
        amountText.text = GameManager.Instance.userManager.thisUser.userCards[card].ToString();
        useButton.interactable = GameManager.Instance.userManager.IsThisUserCanUseCardNow(card);
    }

    public void UseCard()
    {
        if (!GameManager.Instance.userManager.IsThisUserCanUseCardNow(card))
        {
            return;
        }

        cardForm.PlanToUseCard(card);
    }

    private void OnDestroy()
    {
        GameManager.Instance.uiManager.UPDATE_UI_EVENT.RemoveListener(UpdateInfo);
    }
}
