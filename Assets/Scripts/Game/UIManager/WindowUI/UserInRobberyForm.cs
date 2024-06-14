using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UserInRobberyForm : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text brickText;
    [SerializeField] private TMP_Text lumberText;
    [SerializeField] private TMP_Text oreText;
    [SerializeField] private TMP_Text grainText;
    [SerializeField] private TMP_Text woolText;
    [SerializeField] private Image colorImage;
    [SerializeField] private Button button;


    internal void SetUserInRobberyFormInfo(User user, UnityAction<int> callPlanRobbery)
    {
        usernameText.text = user.name;
        brickText.text = "Brick: " + user.userResources[Resource.BRICK];
        lumberText.text = "Lumber: " + user.userResources[Resource.LUMBER];
        oreText.text = "Ore: " + user.userResources[Resource.ORE];
        grainText.text = "Grain: " + user.userResources[Resource.GRAIN];
        woolText.text = "Wool: " + user.userResources[Resource.WOOL];
        colorImage.color = user.color;
        button.onClick.AddListener(() => { callPlanRobbery(user.id); });
    }
}
