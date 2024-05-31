using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Form : MonoBehaviour
{
    protected GameObject infoFormPrefab;
    protected GameObject errorFormPrefab;
    [SerializeField] protected List<Selectable> interactiveItems;
    
    protected virtual void Awake()
    {
        SetDefaultInfoAndErrorForm();
    }

    protected void SetDefaultInfoAndErrorForm()
    {
        infoFormPrefab = Resources.Load<GameObject>("Prefabs/Form/InfoForm");
        errorFormPrefab = Resources.Load<GameObject>("Prefabs/Form/ErrorForm");
    }

    protected void CreateErrorForm(string error)
    {
        var errorForm = Instantiate(errorFormPrefab, transform.parent).GetComponent<ErrorForm>();
        errorForm.SetErrorText(error);
    }

    protected void CreateInfoForm(string info)
    {
        var infoForm = Instantiate(infoFormPrefab, transform.parent).GetComponent<InfoForm>();
        infoForm.SetInfoText(info);
    }

    protected void TurnOnInteractables()
    {
        foreach(var interactable in interactiveItems)
        {
            interactable.interactable = true;
        }
    }

    protected void TurnOffInteractables()
    {
        foreach (var interactable in interactiveItems)
        {
            interactable.interactable = false;
        }
    }
}
