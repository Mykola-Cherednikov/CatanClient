using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ExchangeFormExchangeButton : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown initiatorDropDown;
    [SerializeField] private TMP_Dropdown targetDropDown;
    [SerializeField] private TMP_Dropdown initiatorResourceDropDown;
    [SerializeField] private TMP_Dropdown targetResourceDropDown;
    [SerializeField] private TMP_InputField initiatorResourceAmountField;
    [SerializeField] private TMP_InputField targetResourceAmountField;

    [SerializeField] private float cooldownExchangeTime = 5f;
    private float cooldownTimer = 0f;
    private bool isTimerEndCalled = false;

    private Button exchangeButton;
    private TMP_Text exchangeButtonText;


    private List<User> targetUsers;

    private void Awake()
    {
        exchangeButton = GetComponent<Button>();
        exchangeButtonText = GetComponentInChildren<TMP_Text>();
        exchangeButton.interactable = false;

        targetUsers = new List<User>();
        targetUsers.AddRange(GameManager.Instance.userManager.users
            .Where(u => u != GameManager.Instance.userManager.thisUser));

        exchangeButton.onClick.AddListener(TimerStart);
        initiatorDropDown.onValueChanged.AddListener(OnChangeDropDown);
        targetDropDown.onValueChanged.AddListener(OnChangeDropDown);
        initiatorResourceDropDown.onValueChanged.AddListener(OnChangeDropDown);
        targetResourceDropDown.onValueChanged.AddListener(OnChangeDropDown);
        initiatorResourceAmountField.onValueChanged.AddListener(OnChangeInputField);
        targetResourceAmountField.onValueChanged.AddListener(OnChangeInputField);
        GameManager.Instance.uiManager.UPDATE_UI_EVENT.AddListener(StartChangeExchangeButtonInteractable);
    }

    private void Update()
    {
        if (!IsTimerEnd())
        {
            TimerCountDown();
        }
        else
        {
            if (!isTimerEndCalled)
            {
                OnTimerEnd();
            }
        }
    }

    private void OnChangeInputField(string whyINeedThisVariable)
    {
        StartChangeExchangeButtonInteractable();
    }

    private void OnChangeDropDown(int whyINeedThisVariable)
    {
        StartChangeExchangeButtonInteractable();
    }

    private void StartChangeExchangeButtonInteractable()
    {
        if(!IsTimerEnd())
        {
            return;
        }

        if(targetUsers.Count == 0 || initiatorResourceAmountField.text.Length == 0 || targetResourceAmountField.text.Length == 0)
        {
            return;
        }

        User initiatorUser = GameManager.Instance.userManager.thisUser;
        User targetUser = targetUsers[targetDropDown.value];
        Resource initiatorResource = (Resource)initiatorResourceDropDown.value;
        Resource targetResource = (Resource)targetResourceDropDown.value;
        int initiatorResourceAmount = int.Parse(initiatorResourceAmountField.text);
        int targetResourceAmount = int.Parse(targetResourceAmountField.text);

        ChangeExchangeButtonInteractable(initiatorUser, targetUser, initiatorResource, targetResource, initiatorResourceAmount, targetResourceAmount);
    }

    private void ChangeExchangeButtonInteractable(User initiatorUser, User targetUser, Resource initiatorResource, Resource targetResource, int initiatorResourceAmount, int targetResourceAmount)
    {
        exchangeButton.interactable = false;

        if(initiatorUser == targetUser)
        {
            return;
        }

        if (initiatorResourceAmount < 0 || targetResourceAmount < 0)
        {
            return;
        }

        KeyValuePair<Resource, int> initiatorResourcesToAmount = new KeyValuePair<Resource, int>(initiatorResource, initiatorResourceAmount);
        KeyValuePair<Resource, int> targetResourcesToAmount = new KeyValuePair<Resource, int>(targetResource, targetResourceAmount);

        if (!GameManager.Instance.userManager.IsThisUserCanExchangeWithUserNow(targetUser, initiatorResourcesToAmount, targetResourcesToAmount))
        {
            return;
        }

        exchangeButton.interactable = true;
    }

    private void TimerStart()
    {
        exchangeButton.interactable = false;
        cooldownTimer = cooldownExchangeTime;
        isTimerEndCalled = false;
    }

    private void OnTimerEnd()
    {
        StartChangeExchangeButtonInteractable();
        exchangeButtonText.text = $"Exchange";
        isTimerEndCalled = true;
    }

    private void TimerCountDown()
    {
        cooldownTimer -= Time.deltaTime;
        exchangeButtonText.text = $"Exchange({(int) cooldownTimer})";
    }

    private bool IsTimerEnd()
    {
        return cooldownTimer <= 0f;
    }

    private void OnDestroy()
    {
        exchangeButton.onClick.RemoveListener(TimerStart);
        initiatorDropDown.onValueChanged.RemoveListener(OnChangeDropDown);
        targetDropDown.onValueChanged.RemoveListener(OnChangeDropDown);
        initiatorResourceDropDown.onValueChanged.RemoveListener(OnChangeDropDown);
        targetResourceDropDown.onValueChanged.RemoveListener(OnChangeDropDown);
        initiatorResourceAmountField.onValueChanged.RemoveListener(OnChangeInputField);
        targetResourceAmountField.onValueChanged.RemoveListener(OnChangeInputField);
        GameManager.Instance.uiManager.UPDATE_UI_EVENT.RemoveListener(StartChangeExchangeButtonInteractable);
    }
}
