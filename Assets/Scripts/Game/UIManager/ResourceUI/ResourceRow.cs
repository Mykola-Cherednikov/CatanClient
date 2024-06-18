using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceRow : MonoBehaviour
{
    [SerializeField] private TMP_Text resourceText;
    [SerializeField] private TMP_Text numOfResouceText;
    [SerializeField] private Resource resource;

    private void Awake()
    {
        GameManager.Instance.resourceManager.RESOURCES_CHANGED_EVENT += UpdateInfo;
    }

    public void SetInfo(Resource resource)
    {
        this.resource = resource;
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        resourceText.text = Enum.GetName(typeof(Resource), resource);
        numOfResouceText.text = GameManager.Instance.userManager.currentUser.userResources[resource].ToString();
    }
}
