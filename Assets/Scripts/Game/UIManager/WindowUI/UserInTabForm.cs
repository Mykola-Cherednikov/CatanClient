using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInTabForm : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text brickText;
    [SerializeField] private TMP_Text lumberText;
    [SerializeField] private TMP_Text oreText;
    [SerializeField] private TMP_Text grainText;
    [SerializeField] private TMP_Text woolText;
    [SerializeField] private Image colorImage;

    internal void SetUserInTabFormInfo(User user)
    {
        usernameText.text = user.name;
        brickText.text = "Brick: " + user.userResources[Resource.BRICK];
        lumberText.text = "Lumber: " + user.userResources[Resource.LUMBER];
        oreText.text = "Ore: " + user.userResources[Resource.ORE];
        grainText.text = "Grain: " + user.userResources[Resource.GRAIN];
        woolText.text = "Wool: " + user.userResources[Resource.WOOL];
        colorImage.color = user.color;
    }
}
