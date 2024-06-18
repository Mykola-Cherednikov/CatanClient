using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeForm : MonoBehaviour
{
    [SerializeField] private Button tradeButton;
    [SerializeField] private TMP_InputField incomeField;
    [SerializeField] private TMP_InputField outgoingField;
    [SerializeField] private TMP_Dropdown incomeDropdown;
    [SerializeField] private TMP_Dropdown outgoingDropdown;

    private void FixedUpdate()
    {
        ValidateForm();
    }

    public void ValidateForm()
    {
        tradeButton.interactable = true;

        Resource incomeResource = (Resource)incomeDropdown.value;
        Resource outgoingResource = (Resource)outgoingDropdown.value;

        if (incomeResource == outgoingResource)
        {
            tradeButton.interactable = false;
        }

        if (outgoingField.text.Length == 0)
        {
            incomeField.text = "";
            tradeButton.interactable = false;
            return;
        }

        int outgoingNum = int.Parse(outgoingField.text);
        int incomeNum = outgoingNum * 4;

        if (outgoingNum <= 0)
        {
            tradeButton.interactable = false;
            incomeField.text = "0";
        }

        incomeField.text = incomeNum.ToString();

        KeyValuePair<Resource, int> incomeResourceAmount = new KeyValuePair<Resource, int>(incomeResource, incomeNum);
        KeyValuePair<Resource, int> outgoingResourceAmount = new KeyValuePair<Resource, int>(outgoingResource, outgoingNum);

        if (!GameManager.Instance.userManager.IsCurrentUserCanTradeNow(incomeResourceAmount, outgoingResourceAmount))
        {
            tradeButton.interactable = false;
        }
    }

    public void Trade()
    {
        Resource incomeResource = (Resource)incomeDropdown.value;
        Resource outgoingResource = (Resource)outgoingDropdown.value;
        int outgoingNum = int.Parse(outgoingField.text);

        GameManager.Instance.resourceManager.UserTradeResource(incomeResource, outgoingResource, outgoingNum);
    }
}
