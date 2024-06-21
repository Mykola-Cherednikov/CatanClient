using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeForm : MonoBehaviour
{
    [SerializeField] private Button tradeButton;
    [SerializeField] private TMP_InputField sellField;
    [SerializeField] private TMP_InputField buyField;
    [SerializeField] private TMP_Dropdown sellDropdown;
    [SerializeField] private TMP_Dropdown buyDropdown;

    private void Update()
    {
        ValidateForm();
    }

    public void ValidateForm()
    {
        tradeButton.interactable = true;

        Resource sellResource = (Resource)sellDropdown.value;
        Resource buyResource = (Resource)buyDropdown.value;

        if (sellResource == buyResource)
        {
            tradeButton.interactable = false;
        }

        if (buyField.text.Length == 0)
        {
            sellField.text = "";
            tradeButton.interactable = false;
            return;
        }

        int buyAmount = int.Parse(buyField.text);
        int sellAmount = GameManager.Instance.resourceManager
            .GetAmountOfTradingSellResourceDependOnUserHarbour(GameManager.Instance.userManager.currentUser, sellResource, buyAmount);

        if (buyAmount <= 0)
        {
            tradeButton.interactable = false;
            sellField.text = "0";
        }

        sellField.text = sellAmount.ToString();

        KeyValuePair<Resource, int> sellResourceAmount = new KeyValuePair<Resource, int>(sellResource, sellAmount);
        KeyValuePair<Resource, int> buyResourceAmount = new KeyValuePair<Resource, int>(buyResource, buyAmount);

        if (!GameManager.Instance.userManager.IsCurrentUserCanTradeNow(sellResourceAmount, buyResourceAmount))
        {
            tradeButton.interactable = false;
        }
    }

    public void Trade()
    {
        Resource sellResource = (Resource)sellDropdown.value;
        Resource buyResource = (Resource)buyDropdown.value;
        int buyNum = int.Parse(buyField.text);

        GameManager.Instance.resourceManager.UserTradeRequest(sellResource, buyResource, buyNum);
    }
}
