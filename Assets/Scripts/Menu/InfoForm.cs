using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoForm : MonoBehaviour
{
    [SerializeField] private TMP_Text _infoText;

    public void SetInfoText(string text)
    {
        _infoText.text = text;
    }

    public void DestroyForm()
    {
        Destroy(gameObject);
    }
}
