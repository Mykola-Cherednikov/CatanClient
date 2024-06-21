using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeForm : MonoBehaviour
{
    [SerializeField] private TMP_InputField buyField;
    [SerializeField] private TMP_Dropdown sellDropdown;
    [SerializeField] private TMP_Dropdown buyDropdown;

    public void Trade()
    {
        Resource sellResource = (Resource)sellDropdown.value;
        Resource buyResource = (Resource)buyDropdown.value;
        int buyNum = int.Parse(buyField.text);

        GameManager.Instance.resourceManager.UserTradeRequest(sellResource, buyResource, buyNum);
    }
}
