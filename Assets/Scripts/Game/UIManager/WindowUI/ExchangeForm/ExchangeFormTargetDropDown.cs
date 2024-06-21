using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class ExchangeFormTargetDropDown : MonoBehaviour
{
    private TMP_Dropdown targetDropDown;

    private void Awake()
    {
        var targetUsers = new List<User>();
        targetUsers.AddRange(GameManager.Instance.userManager.users
            .Where(u => u != GameManager.Instance.userManager.currentUser));

        targetDropDown = GetComponent<TMP_Dropdown>();
        foreach (var user in targetUsers)
        {
            targetDropDown.options.Add(new(user.name));
        }
        targetDropDown.value = 0;
    }
}
