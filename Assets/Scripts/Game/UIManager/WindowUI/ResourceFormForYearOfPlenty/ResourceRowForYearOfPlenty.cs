using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceRowForYearOfPlenty : MonoBehaviour
{
    [SerializeField] private TMP_Text resourceText;
    [SerializeField] private TMP_Text numOfResouceText;
    [SerializeField] private Resource resource;
    [SerializeField] private Button chooseButton;
    private ResourceFormForYearOfPlenty yearOfPlentyResourceForm;

    private void Awake()
    {
        GameManager.Instance.resourceManager.RESOURCES_CHANGED_EVENT += UpdateInfo;
        GameManager.Instance.uiManager.CHANGE_UI_STATE += UpdateInfo;
    }

    public void SetInfo(Resource resource, ResourceFormForYearOfPlenty form)
    {
        this.resource = resource;
        yearOfPlentyResourceForm = form;
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        resourceText.text = Enum.GetName(typeof(Resource), resource);
        int numOfResource = GameManager.Instance.resourceManager.storage[resource];
        foreach (var r in yearOfPlentyResourceForm.resources)
        {
            if(resource == r)
            {
                numOfResource--;
            }
        }
        numOfResouceText.text = numOfResource.ToString();

        if (numOfResource > 0 && GameManager.Instance.userManager.IsCurrentUserCanUseCardNow(Card.YEAR_OF_PLENTY))
        {
            chooseButton.interactable = true;
        }
        else
        {
            chooseButton.interactable = false;
        }
    }

    public void ChoseResource()
    {
        yearOfPlentyResourceForm.ChooseResourcesForYearOfPlentyCard(resource);
        UpdateInfo();
    }

    private void OnDestroy()
    {
        GameManager.Instance.resourceManager.RESOURCES_CHANGED_EVENT -= UpdateInfo;
        GameManager.Instance.uiManager.CHANGE_UI_STATE -= UpdateInfo;
    }
}
