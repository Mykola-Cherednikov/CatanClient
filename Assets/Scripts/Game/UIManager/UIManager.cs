using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject uiCanvas;

    private GameObject windowUIPrefab;
    private GameObject buildingsUIPrefab;
    private GameObject gameNotificationsUIPrefab;
    private GameObject resourceBoxUIPrefab;
    private GameObject readyButtonUIPrefab;

    private BuildingsUI buildingsUI;
    private GameNotificationsUI gameNotificationsUI;
    private WindowUI windowUI;
    private ResourceBoxUI resourceBoxUI;
    private GameObject readyButtonUI;

    private void Awake()
    {
        windowUIPrefab = Resources.Load<GameObject>("Prefabs/Game/WindowUI");
        buildingsUIPrefab = Resources.Load<GameObject>("Prefabs/Game/BuildingsUI");
        gameNotificationsUIPrefab = Resources.Load<GameObject>("Prefabs/Game/GameNotificationsUI");
        resourceBoxUIPrefab = Resources.Load<GameObject>("Prefabs/Game/ResourceBoxUI");
        readyButtonUIPrefab = Resources.Load<GameObject>("Prefabs/Game/ReadyButtonUI");
    }

    public void InitializeUI(GameObject uiCanvas)
    {
        this.uiCanvas = uiCanvas;
        buildingsUI = Instantiate(buildingsUIPrefab, uiCanvas.transform).GetComponent<BuildingsUI>();
        gameNotificationsUI = Instantiate(gameNotificationsUIPrefab, uiCanvas.transform).GetComponent<GameNotificationsUI>();
        resourceBoxUI = Instantiate(resourceBoxUIPrefab, uiCanvas.transform).GetComponent<ResourceBoxUI>();
        readyButtonUI = Instantiate(readyButtonUIPrefab, uiCanvas.transform);
        windowUI = Instantiate(windowUIPrefab, uiCanvas.transform).GetComponent<WindowUI>();
    }

    public void ClearAllElementsFromCanvas()
    {
        foreach (Transform transform in uiCanvas.transform)
        {
            Destroy(transform.gameObject);
        }
    }

    public void DisplayUserTurnText(User user)
    {
        gameNotificationsUI.CreateNotificationUserTurn(user.name);
    }

    public void DisplayUserDiceThrow(User user, int diceNum)
    {
        gameNotificationsUI.CreateNotificationDiceThrow(user.name, diceNum);
    }

    public void ChangeUIToGameState()
    {
        buildingsUI.ChangeUIToGameState();
    }

    public void ShowRobberyFormWithUniqueUsers(int hexId, List<User> uniqueUsers)
    {
        windowUI.OpenRobberyFormWithUniqueUsers(hexId, uniqueUsers);
    }
}
