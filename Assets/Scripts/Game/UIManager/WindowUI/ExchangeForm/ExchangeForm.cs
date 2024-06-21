using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ExchangeForm : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown initiatorDropDown;
    [SerializeField] private TMP_Dropdown targetDropDown;
    [SerializeField] private TMP_Dropdown initiatorResourceDropDown;
    [SerializeField] private TMP_Dropdown targetResourceDropDown;
    [SerializeField] private TMP_InputField initiatorResourceAmountField;
    [SerializeField] private TMP_InputField targetResourceAmountField;

    private List<User> targetUsers;

    private void Awake()
    {
        targetUsers = new List<User>();
        targetUsers.AddRange(GameManager.Instance.userManager.users
            .Where(u => u != GameManager.Instance.userManager.currentUser));
    }

    public void ExchangeOffer()
    {
        Resource initiatorResource = (Resource)initiatorResourceDropDown.value;
        Resource targetResource = (Resource)targetResourceDropDown.value;
        int initiatorResourceAmount = int.Parse(initiatorResourceAmountField.text);
        int targetResourceAmount = int.Parse(targetResourceAmountField.text);
        User targetUser = targetUsers[targetDropDown.value];

        GameManager.Instance.resourceManager.UserExchangeOfferRequest(initiatorResource,
            initiatorResourceAmount, targetUser, targetResource, targetResourceAmount);
    }
}
