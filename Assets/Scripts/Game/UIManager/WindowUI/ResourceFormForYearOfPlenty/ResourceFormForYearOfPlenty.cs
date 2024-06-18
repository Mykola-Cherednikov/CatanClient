using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceFormForYearOfPlenty : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject resourceRowForYearOfPlentyPrefab;
    public List<Resource> resources;

    private void Awake()
    {
        resources = new List<Resource>();
        foreach (var resource in Enum.GetValues(typeof(Resource)))
        {
            CreateResourceRowForMonopoly((Resource)resource);
        }
    }

    private void CreateResourceRowForMonopoly(Resource resource)
    {
        ResourceRowForYearOfPlenty resourceRow = Instantiate(resourceRowForYearOfPlentyPrefab, content.transform).GetComponent<ResourceRowForYearOfPlenty>();
        resourceRow.SetInfo(resource, this);
    }

    public void ChooseResourcesForYearOfPlentyCard(Resource resource)
    {
        resources.Add(resource);

        if (resources.Count == 2)
        {
            GameManager.Instance.cardManager.PlanUseYearOfPlentyCard(resources);
            Destroy(gameObject);
        }
    }
}
