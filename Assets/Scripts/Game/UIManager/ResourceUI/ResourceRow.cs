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
        GameManager.Instance.uiManager.UPDATE_UI_EVENT.AddListener(UpdateInfo);
    }

    public void SetInfo(Resource resource)
    {
        this.resource = resource;
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        resourceText.text = Enum.GetName(typeof(Resource), resource);
        numOfResouceText.text = GameManager.Instance.userManager.thisUser.userResources[resource].ToString();
    }

    private void OnDestroy()
    {
        GameManager.Instance.uiManager.UPDATE_UI_EVENT.RemoveListener(UpdateInfo);
    }
}
