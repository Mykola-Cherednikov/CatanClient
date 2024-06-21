using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExchangeForm : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown initiatorDropDown;
    [SerializeField] private TMP_Dropdown targetDropDown;
    [SerializeField] private TMP_Dropdown initiatorResourceDropDown;
    [SerializeField] private TMP_Dropdown targetResourceDropDown;
    [SerializeField] private TMP_InputField initiatorResourceAmountField;
    [SerializeField] private TMP_InputField targetResourceAmountField;
    [SerializeField] private Button exchangeButton;

    private List<User> targetUsers;

    private void Awake()
    {
        targetUsers = new List<User>();
        targetUsers.AddRange(GameManager.Instance.userManager.users
            .Where(u => u != GameManager.Instance.userManager.currentUser));

        foreach (var user in targetUsers)
        {
            targetDropDown.options.Add(new(user.name));
        }

        initiatorDropDown.options.Add(new(GameManager.Instance.userManager.currentUser.name));
        
        initiatorDropDown.value = 0;
        targetDropDown.value = 0;
    }

    private void Update()
    {
        ValidateForm();
    }

    public void ValidateForm()
    {
        exchangeButton.interactable = true;

        int initiatorResourceAmount = int.Parse(initiatorResourceAmountField.text);
        int targetResourceAmount = int.Parse(targetResourceAmountField.text);

        if (initiatorResourceAmount < 0 || targetResourceAmount < 0)
        {
            exchangeButton.interactable = false;
            return;
        }

        Resource initiatorResource = (Resource)initiatorResourceDropDown.value;
        Resource targetResource = (Resource)targetResourceDropDown.value;
        User targetUser = targetUsers[targetDropDown.value];

        KeyValuePair<Resource, int> initiatorResourcesToAmount = new KeyValuePair<Resource, int>(initiatorResource, initiatorResourceAmount);
        KeyValuePair<Resource, int> targetResourcesToAmount = new KeyValuePair<Resource, int>(targetResource, targetResourceAmount);

        if (!GameManager.Instance.userManager.IsCurrentUserCanExchangeWithUserNow(targetUser, initiatorResourcesToAmount, targetResourcesToAmount))
        {
            exchangeButton.interactable = false;
        }
    }

    public void ExchangeOffer()
    {
        Resource initiatorResource = (Resource)initiatorResourceDropDown.value;
        Resource targetResource = (Resource)targetResourceDropDown.value;
        int initiatorResourceAmount = int.Parse(initiatorResourceAmountField.text);
        int targetResourceAmount = int.Parse(targetResourceAmountField.text);
        User targetUser = targetUsers[targetDropDown.value];

        GameManager.Instance.resourceManager.UserExchangeRequest(initiatorResource, 
            initiatorResourceAmount, targetUser, targetResource, targetResourceAmount);
    }
}
