using System;
using TMPro;
using UnityEngine;

public class ResourceRowUI : MonoBehaviour
{
    [SerializeField] private TMP_Text resourceText;
    [SerializeField] private TMP_Text numOfResouceText;
    [SerializeField] private Resource resource;

    private void Awake()
    {
        GameManager.Instance.userManager.CURRENT_USER_RESOURCES_CHANGED_EVENT.AddListener(UpdateResourcesData);
    }

    private void UpdateResourcesData()
    {
        if (GameManager.Instance.userManager.currentUser != null)
        {
            resourceText.text = Enum.GetName(typeof(Resource), resource);
            numOfResouceText.text = GameManager.Instance.userManager.currentUser.userResources[resource].ToString();
        }
    }
}
