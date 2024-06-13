using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyButtonUI : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Update()
    {
        button.interactable = GameManager.Instance.userManager.IsCurrentUserTurn() && 
            GameManager.Instance.gameState == GameState.GAME;
    }

    public void Ready()
    {
        GameManager.Instance.userManager.UserTurnReady();
    }
}
