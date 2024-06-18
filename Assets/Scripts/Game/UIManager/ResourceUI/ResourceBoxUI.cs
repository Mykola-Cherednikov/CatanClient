using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceBoxUI : MonoBehaviour
{
    [SerializeField] private TMP_Text hideAndShowButtonText;
    [SerializeField] private GameObject resourceRowPrefab;
    [SerializeField] private GameObject content;
    private RectTransform rect;
    private bool isHiden;

    private void Awake()
    {
        isHiden = false;
        rect = GetComponent<RectTransform>();

        foreach (var resource in Enum.GetValues(typeof(Resource)))
        {
            CreateResourceRow((Resource)resource);
        }
    }

    private void CreateResourceRow(Resource resource)
    {
        ResourceRow resourceRow = Instantiate(resourceRowPrefab, content.transform).GetComponent<ResourceRow>();
        resourceRow.SetInfo(resource);
    }

    public void HideOrShow()
    {
        float rectWidth = rect.sizeDelta.x - 15f;
        transform.position = new Vector2(transform.position.x + (rectWidth * (isHiden ? 1 : -1)), transform.position.y);
        hideAndShowButtonText.text = isHiden ? "H\ni\nd\ne\n" : "S\nh\no\nw\n";
        isHiden = !isHiden;
    }
}
