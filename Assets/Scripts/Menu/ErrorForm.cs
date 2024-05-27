using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorForm : MonoBehaviour
{
    [SerializeField] private TMP_Text _errorText;

    public void SetErrorText(string text)
    {
        _errorText.text = text;
    }

    public void DestroyForm()
    {
        Destroy(gameObject);
    }
}
