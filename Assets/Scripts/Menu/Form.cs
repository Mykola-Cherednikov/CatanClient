using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Form : MonoBehaviour
{
    protected GameObject _infoFormGO;
    protected GameObject _errorFormGO;
    [SerializeField] protected List<Selectable> _interactiveItems;
    
    protected virtual void Awake()
    {
        SetDefaultInfoAndErrorForm();
    }

    protected void SetDefaultInfoAndErrorForm()
    {
        _infoFormGO = Resources.Load<GameObject>("Prefabs/InfoForm");
        _errorFormGO = Resources.Load<GameObject>("Prefabs/ErrorForm");
    }

    protected void CreateErrorForm(string error)
    {
        var errorForm = Instantiate(_errorFormGO, transform.parent).GetComponent<ErrorForm>();
        errorForm.SetErrorText(error);
    }

    protected void CreateInfoForm(string info)
    {
        var infoForm = Instantiate(_infoFormGO, transform.parent).GetComponent<InfoForm>();
        infoForm.SetInfoText(info);
    }

    protected void TurnOnInteractables()
    {
        foreach(var interactable in _interactiveItems)
        {
            interactable.interactable = true;
        }
    }

    protected void TurnOffInteractables()
    {
        foreach (var interactable in _interactiveItems)
        {
            interactable.interactable = false;
        }
    }
}
