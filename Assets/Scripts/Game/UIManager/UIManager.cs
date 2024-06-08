using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject uiCanvas;

    [SerializeField] private GameObject escapeUIPrefab;
    [SerializeField] private GameObject buildingsUIPrefab;
    [SerializeField] private GameObject gameNotificationsUIPrefab;

    private BuildingsUI buildingsUI;
    private GameNotificationsUI gameNotificationsUI;
    private EscapeUI escapeUI;

    private void Awake()
    {
        escapeUIPrefab = Resources.Load<GameObject>("Prefabs/Game/EscapeUI");
        buildingsUIPrefab = Resources.Load<GameObject>("Prefabs/Game/BuildingsUI");
        gameNotificationsUIPrefab = Resources.Load<GameObject>("Prefabs/Game/GameNotificationsUI");
    }

    public void InitializeUI(GameObject uiCanvas)
    {
        this.uiCanvas = uiCanvas;
        buildingsUI = Instantiate(buildingsUIPrefab, uiCanvas.transform).GetComponent<BuildingsUI>();
        gameNotificationsUI = Instantiate(gameNotificationsUIPrefab, uiCanvas.transform).GetComponent<GameNotificationsUI>();
        escapeUI = Instantiate(escapeUIPrefab, uiCanvas.transform).GetComponent<EscapeUI>();
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
        gameNotificationsUI.CreateNotification(user.name);
    }
}
