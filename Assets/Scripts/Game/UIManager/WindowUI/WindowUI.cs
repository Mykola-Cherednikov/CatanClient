using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowUI : MonoBehaviour
{
    private GameObject escapeFormPrefab;
    private GameObject tradeFormPrefab;
    private GameObject tradeButtonPrefab;
    private GameObject tabFormPrefab;
    private GameObject formGO;

    private void Awake()
    {
        escapeFormPrefab = Resources.Load<GameObject>("Prefabs/Form/EscapeForm");
        tradeFormPrefab = Resources.Load<GameObject>("Prefabs/Form/TradeForm");
        tabFormPrefab = Resources.Load<GameObject>("Prefabs/Form/TabForm");
        tabFormPrefab = Resources.Load<GameObject>("Prefabs/Form/RobberyForm");
        tradeButtonPrefab = Resources.Load<GameObject>("Prefabs/Game/TradeButton");
        Instantiate(tradeButtonPrefab, transform).GetComponent<Button>().onClick.AddListener(OpenTradeForm);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (formGO == null)
            {
                formGO = Instantiate(escapeFormPrefab, transform);
            }
            else
            {
                Destroy(formGO);
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (formGO == null)
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
        if (formGO == null)
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
        if (formGO != null)
        {
            Destroy(formGO);
        }

        formGO = Instantiate(tradeFormPrefab, transform);
        formGO.GetComponent<RobberyForm>().SetInfo(hexId, uniqueUsers);
    }
}
