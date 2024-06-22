using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class ExchangeFormInitiatorDropDown : MonoBehaviour
{
    private TMP_Dropdown initiatorDropDown;

    private void Awake()
    {
        initiatorDropDown = GetComponent<TMP_Dropdown>();
        initiatorDropDown.options.Add(new(GameManager.Instance.userManager.thisUser.name));
        initiatorDropDown.value = 0;
    }
}
