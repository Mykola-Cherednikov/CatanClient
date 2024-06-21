using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExchangeOfferForm : MonoBehaviour
{
    [SerializeField] private TMP_Text fromUserText;
    [SerializeField] private TMP_Text getResourceText;
    [SerializeField] private TMP_Text giveResourceText;

    private int exchangeId;
    private bool isSended;


    public void SetInfo(User u, string targetResource, int targetAmountOfResource, string initiatorResource, int initiatorAmountOfResource, int exchangeId)
    {
        fromUserText.text = $"From {u.name}";
        getResourceText.text = $"{initiatorResource}x{initiatorAmountOfResource}";
        giveResourceText.text = $"{targetResource}x{targetAmountOfResource}";
        this.exchangeId = exchangeId;
    }

    public void Accept()
    {
        if (!isSended)
        {
            GameManager.Instance.resourceManager.UserExchangeRequest(exchangeId, true);
            isSended = true;
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Deny()
    {
        if (!isSended)
        {
            GameManager.Instance.resourceManager.UserExchangeRequest(exchangeId, false);
            isSended = true;
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        if(!isSended)
        {
            Deny();
        }
    }
}
