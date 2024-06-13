using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceBoxUI : MonoBehaviour
{
    [SerializeField] private TMP_Text hideAndShowButtonText;
    private RectTransform rect;
    private bool isHiden;

    private void Awake()
    {
        isHiden = false;
        rect = GetComponent<RectTransform>();
    }

    public void HideOrShow()
    {
        float rectWidth = rect.sizeDelta.x - 15f;
        transform.position = new Vector2(transform.position.x + (rectWidth * (isHiden ? 1 : -1)), transform.position.y);
        hideAndShowButtonText.text = isHiden ? "H\ni\nd\ne\n" : "S\nh\no\nw\n";
        isHiden = !isHiden;
    }
}
