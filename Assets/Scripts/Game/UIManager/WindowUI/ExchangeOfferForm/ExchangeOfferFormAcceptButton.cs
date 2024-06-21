using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ExchangeOfferFormAcceptButton : MonoBehaviour
{
    private Button acceptButton;
    private TMP_Text acceptButtonText;

    [SerializeField] private float awaitToAcceptTimer = 3f;

    private void Awake()
    {
        acceptButton = GetComponent<Button>();
        acceptButtonText = GetComponentInChildren<TMP_Text>();
        acceptButton.interactable = false;
    }

    private void Update()
    {
        if (awaitToAcceptTimer > 0f)
        {
            acceptButtonText.text = $"Accept({(int)awaitToAcceptTimer}s)";
            awaitToAcceptTimer -= Time.deltaTime;
        }
        else
        {
            acceptButtonText.text = "Accept";
            acceptButton.interactable = true;
            Destroy(this);
        }
    }
}
