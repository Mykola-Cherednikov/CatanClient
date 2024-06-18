using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceRowForMonopoly : MonoBehaviour
{
    [SerializeField] private TMP_Text resourceText;
    [SerializeField] private TMP_Text numOfResouceText;
    [SerializeField] private Resource resource;
    [SerializeField] private Button chooseButton;
    private MonopolyResourceForm monopolyResourceForm;

    private void Awake()
    {
        GameManager.Instance.resourceManager.RESOURCES_CHANGED_EVENT += UpdateInfo;
        GameManager.Instance.uiManager.CHANGE_UI_STATE += UpdateInfo;
    }

    public void SetInfo(Resource resource, MonopolyResourceForm form)
    {
        this.resource = resource;
        monopolyResourceForm = form;
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        resourceText.text = Enum.GetName(typeof(Resource), resource);
        int numOfResource = 0;
        foreach (User u in GameManager.Instance.userManager.users)
        {
            if (u == GameManager.Instance.userManager.currentUser)
            {
                continue;
            }
            numOfResource += u.userResources[resource];
        }
        numOfResouceText.text = numOfResource.ToString();

        if (numOfResource > 0 && GameManager.Instance.userManager.IsCurrentUserCanUseCardNow(Card.MONOPOLY))
        {
            chooseButton.interactable = true;
        }
        else
        {
            chooseButton.interactable = false;
        }
    }

    public void ChooseResource()
    {
        monopolyResourceForm.ChooseResourceForMonopolyCard(resource);
    }

    private void OnDestroy()
    {
        GameManager.Instance.resourceManager.RESOURCES_CHANGED_EVENT -= UpdateInfo;
        GameManager.Instance.uiManager.CHANGE_UI_STATE -= UpdateInfo;
    }
}
