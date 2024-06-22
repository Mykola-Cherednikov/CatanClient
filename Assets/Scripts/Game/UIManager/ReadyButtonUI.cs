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
        GameManager.Instance.uiManager.UPDATE_UI_EVENT.AddListener(ChangeReadyButton);
    }

    private void ChangeReadyButton()
    {
        button.interactable = GameManager.Instance.userManager.IsThisUserTurn() && 
            GameManager.Instance.uiManager.uiState == UIState.USER_TURN;
    }

    public void Ready()
    {
        GameManager.Instance.userManager.UserTurnReady();
        button.interactable = false;
    }

    private void OnDestroy()
    {
        GameManager.Instance.uiManager.UPDATE_UI_EVENT.RemoveListener(ChangeReadyButton);
    }
}
