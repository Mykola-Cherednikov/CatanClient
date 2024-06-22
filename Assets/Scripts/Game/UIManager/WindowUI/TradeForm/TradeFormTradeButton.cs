using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TradeFormTradeButton : MonoBehaviour
{
    [SerializeField] private TMP_InputField sellInputField;
    [SerializeField] private TMP_InputField buyInputField;
    [SerializeField] private TMP_Dropdown sellDropDown;
    [SerializeField] private TMP_Dropdown buyDropDown;
    private Button tradeButton;

    private void Awake()
    {
        tradeButton = GetComponent<Button>();
        tradeButton.interactable = false;
        sellInputField.onValueChanged.AddListener(OnChangeInputField);
        buyInputField.onValueChanged.AddListener(OnChangeInputField);
        sellDropDown.onValueChanged.AddListener(OnChangeDropDown);
        buyDropDown.onValueChanged.AddListener(OnChangeDropDown);
        GameManager.Instance.uiManager.UPDATE_UI_EVENT.AddListener(StartChangeTradeButtonInteractable);
    }

    private void OnChangeInputField(string whyINeedThisVariable)
    {
        StartChangeTradeButtonInteractable();
    }

    private void OnChangeDropDown(int whyINeedThisVariable)
    {
        StartChangeTradeButtonInteractable();
    }

    private void StartChangeTradeButtonInteractable()
    {
        Resource buyResource = (Resource) buyDropDown.value;
        Resource sellResource = (Resource) sellDropDown.value;

        int buyAmount = 0;

        if(buyInputField.text.Length != 0)
        {
            buyAmount = int.Parse(buyInputField.text);
        }

        int sellAmount = GameManager.Instance.resourceManager
            .GetAmountOfTradingSellResourceDependOnUserHarbour(GameManager.Instance.userManager.thisUser,
            sellResource, buyAmount);

        ChangeTradeButtonInteractable(buyResource, sellResource, buyAmount, sellAmount);
    }

    private void ChangeTradeButtonInteractable(Resource buyResource, Resource sellResource, int buyAmount, int sellAmount)
    {
        tradeButton.interactable = false;

        if(buyResource == sellResource)
        {
            return;
        }

        if(buyAmount <= 0)
        {
            return;
        }

        KeyValuePair<Resource, int> buyResourceAmount = new KeyValuePair<Resource, int>(buyResource, buyAmount);

        if (!GameManager.Instance.userManager.IsThisUserCanTradeNow(sellResource, buyResourceAmount))
        {
            return;
        }

        tradeButton.interactable = true;
    }

    private void OnDestroy()
    {
        sellInputField.onValueChanged.RemoveListener(OnChangeInputField);
        buyInputField.onValueChanged.RemoveListener(OnChangeInputField);
        sellDropDown.onValueChanged.RemoveListener(OnChangeDropDown);
        buyDropDown.onValueChanged.RemoveListener(OnChangeDropDown);
        GameManager.Instance.uiManager.UPDATE_UI_EVENT.RemoveListener(StartChangeTradeButtonInteractable);
    }
}
