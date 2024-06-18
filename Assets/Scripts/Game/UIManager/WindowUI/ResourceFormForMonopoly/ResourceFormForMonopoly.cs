using System;
using UnityEngine;
using UnityEngine.UI;

public class MonopolyResourceForm : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject resourceRowForMonopolyPrefab;

    private void Awake()
    {
        foreach (var resource in Enum.GetValues(typeof(Resource)))
        {
            CreateResourceRowForMonopoly((Resource)resource);
        }
    }

    private void CreateResourceRowForMonopoly(Resource resource)
    {
        ResourceRowForMonopoly resourceRow = Instantiate(resourceRowForMonopolyPrefab, content.transform)
            .GetComponent<ResourceRowForMonopoly>();
        resourceRow.SetInfo(resource, this);
    }

    public void ChooseResourceForMonopolyCard(Resource resource)
    {
        GameManager.Instance.cardManager.PlanUseMonopolyCard(resource);
        Destroy(gameObject);
    }
}
