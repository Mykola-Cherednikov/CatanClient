using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowUI : MonoBehaviour
{
    private GameObject escapeFormPrefab;
    private GameObject tradeFormPrefab;
    private GameObject tradeButtonPrefab;
    private GameObject tabFormPrefab;
    private GameObject cardFormPrefab;
    private GameObject resourceFormForMonopolyPrefab;
    private GameObject resourceFormForYearOfPlentyPrefab;
    private GameObject robberyFormPrefab;
    private GameObject formGO;
    private GameObject cardsButtonPrefab;
    private GameObject exchangeButtonPrefab;
    private GameObject exchangeFormPrefab;
    private GameObject exchangeOfferFormPrefab;

    private void Awake()
    {
        escapeFormPrefab = Resources.Load<GameObject>("Prefabs/Form/EscapeForm");
        tradeFormPrefab = Resources.Load<GameObject>("Prefabs/Form/TradeForm");
        cardFormPrefab = Resources.Load<GameObject>("Prefabs/Form/CardsForm");
        resourceFormForMonopolyPrefab = Resources.Load<GameObject>("Prefabs/Form/ResourceFormForMonopoly");
        resourceFormForYearOfPlentyPrefab = Resources.Load<GameObject>("Prefabs/Form/ResourceFormForYearOfPlenty");
        tabFormPrefab = Resources.Load<GameObject>("Prefabs/Form/TabForm");
        robberyFormPrefab = Resources.Load<GameObject>("Prefabs/Form/RobberyForm");
        tradeButtonPrefab = Resources.Load<GameObject>("Prefabs/Game/TradeButton");
        cardsButtonPrefab = Resources.Load<GameObject>("Prefabs/Game/CardsButton");
        exchangeButtonPrefab = Resources.Load<GameObject>("Prefabs/Game/ExchangeButton");
        exchangeFormPrefab = Resources.Load<GameObject>("Prefabs/Form/ExchangeForm");
        exchangeOfferFormPrefab = Resources.Load<GameObject>("Prefabs/Form/ExchangeOfferForm");
        Instantiate(tradeButtonPrefab, transform).GetComponent<Button>().onClick.AddListener(OpenTradeForm);
        Instantiate(cardsButtonPrefab, transform).GetComponent<Button>().onClick.AddListener(OpenCardsForm);
        Instantiate(exchangeButtonPrefab, transform).GetComponent<Button>().onClick.AddListener(OpenExchangeForm);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!IsFormOpen())
            {
                formGO = Instantiate(tabFormPrefab, transform);
            }
            else if(formGO.GetComponent<TabForm>() != null) 
            {
                Destroy(formGO);
            }
        }
    }

    private void OpenTradeForm()
    {
        if (!IsFormOpen())
        {
            formGO = Instantiate(tradeFormPrefab, transform);
        }
        else if (formGO.GetComponent<TradeForm>() != null)
        {
            Destroy(formGO);
        }
    }

    public void OpenRobberyFormWithUniqueUsers(int hexId, List<User> uniqueUsers)
    {
        if (IsFormOpen())
        {
            Destroy(formGO);
        }

        formGO = Instantiate(robberyFormPrefab, transform);
        formGO.GetComponent<RobberyForm>().SetInfo(hexId, uniqueUsers);
    }

    public void OpenResourceFormForYearOfPlenty()
    {
        if (IsFormOpen())
        {
            Destroy(formGO);
        }

        formGO = Instantiate(resourceFormForYearOfPlentyPrefab, transform);
    }

    public void OpenResourceFormForMonopoly()
    {
        if (IsFormOpen())
        {
            Destroy(formGO);
        }

        formGO = Instantiate(resourceFormForMonopolyPrefab, transform);
    }

    private void OpenCardsForm()
    {
        if (!IsFormOpen())
        {
            formGO = Instantiate(cardFormPrefab, transform);
        }
        else if (formGO.GetComponent<CardForm>() != null)
        {
            Destroy(formGO);
        }
    }

    public void OnEscape()
    {
        if (!IsFormOpen())
        {
            OpenEscapeForm();
        }
        else
        {
            Destroy(formGO);
        }
    }

    private void OpenEscapeForm()
    {
        formGO = Instantiate(escapeFormPrefab, transform);
    }

    public void OpenExchangeForm()
    {
        if (!IsFormOpen())
        {
            formGO = Instantiate(exchangeFormPrefab, transform);
        }
        else
        {
            Destroy(formGO);
        }
    }

    public void OpenExchangeOfferForm(User u, string targetResource, int targetAmountOfResource, string initiatorResource, int initiatorAmountOfResource, int exchangeId)
    {
        if (IsFormOpen())
        {
            Destroy(formGO);
        }

        formGO = Instantiate(exchangeOfferFormPrefab, transform);
        ExchangeOfferForm exchangeOfferForm = formGO.GetComponent<ExchangeOfferForm>();
        exchangeOfferForm.SetInfo(u, targetResource, targetAmountOfResource, initiatorResource, initiatorAmountOfResource, exchangeId);
    }

    public bool IsFormOpen()
    {
        return formGO != null;
    }
}
