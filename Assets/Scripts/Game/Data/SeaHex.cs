using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SeaHex : MonoBehaviour
{
    [SerializeField] private TMP_Text harborTypeText;

    [SerializeField] private TMP_Text koefText;

    public void SetInfo(HarborType harborType)
    {
        harborTypeText.text = Enum.GetName(typeof(HarborType), harborType);
        if(harborType == HarborType.GENERIC)
        {
            koefText.text = "3:1";
        }
        else
        {
            koefText.text = "2:1";
        }
    }
}
