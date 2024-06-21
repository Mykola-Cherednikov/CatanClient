using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class TradeFormSellField : MonoBehaviour
{
    [SerializeField] private TMP_InputField buyInputField;
    [SerializeField] private TMP_Dropdown sellDropDown;
    private TMP_InputField sellInputField;

    private void Awake()
    {
        sellInputField = GetComponent<TMP_InputField>();
        buyInputField.onValueChanged.AddListener(OnChangeBuyInputField);
        sellDropDown.onValueChanged.AddListener(OnChangeSellDropDown);
        GameManager.Instance.mapManager.MAP_CHANGED_EVENT += StartChangeSellInputField;
    }

    private void OnChangeBuyInputField(string buyInputFieldValue)
    {
        ChangeSellInputField(int.Parse(FixIncorrectBuyAmountText(buyInputFieldValue)), (Resource)sellDropDown.value);
    }

    private void OnChangeSellDropDown(int sellDropDownId)
    {
        ChangeSellInputField(int.Parse(FixIncorrectBuyAmountText(buyInputField.text)), (Resource)sellDropDownId);
    }

    private void StartChangeSellInputField()
    {
        ChangeSellInputField(int.Parse(FixIncorrectBuyAmountText(buyInputField.text)), (Resource)sellDropDown.value);
    }

    private void ChangeSellInputField(int buyAmount, Resource sellResource)
    {
        int sellAmount = GameManager.Instance.resourceManager
            .GetAmountOfTradingSellResourceDependOnUserHarbour(GameManager.Instance.userManager.currentUser,
            sellResource, buyAmount);
        sellInputField.text = sellAmount.ToString();
    }

    private string FixIncorrectBuyAmountText(string buyAmountText)
    {
        if (buyAmountText.Length == 0)
        {
            return "0";
        }

        return buyAmountText;
    }

    private void OnDestroy()
    {
        buyInputField.onValueChanged.RemoveListener(OnChangeBuyInputField);
        sellDropDown.onValueChanged.RemoveListener(OnChangeSellDropDown);
        GameManager.Instance.mapManager.MAP_CHANGED_EVENT -= StartChangeSellInputField;
    }
}
