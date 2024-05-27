using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInLobbyRow : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image colorImage;
    [NonSerialized] public int id;

    internal void SetUserInLobbyInfo(int id, string name, Color color)
    {
        this.id = id;
        nameText.text = name;
        colorImage.color = color;
    }
}
