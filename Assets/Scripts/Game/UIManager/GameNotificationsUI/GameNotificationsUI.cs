using UnityEngine;

public class GameNotificationsUI : MonoBehaviour
{
    [SerializeField] private GameObject notificationTextPrefab;
    [SerializeField] private GameObject gameNotificationTurnText;
    [SerializeField] private GameObject gameNotificationDiceText;

    private void Awake()
    {
        Multiplayer.Instance.BROADCAST_PREPARE_USER_TURN_EVENT.AddListener(CreateNotificationPreparationUserTurn);
        Multiplayer.Instance.BROADCAST_DICE_THROW_EVENT.AddListener(CreateNotificationDiceThrow);

    }

    public void CreateNotificationPreparationUserTurn(object dtoObject)
    {
        SocketBroadcastUserTurnDTO dto = (SocketBroadcastUserTurnDTO)dtoObject;

        if(gameNotificationTurnText != null)
        {
            Destroy(gameNotificationTurnText.gameObject);
        }

        gameNotificationTurnText = Instantiate(notificationTextPrefab, transform);
        GameNotificationText notificationText = gameNotificationTurnText.GetComponent<GameNotificationText>();
        notificationText.SetText($"{GameManager.Instance.userManager.GetUserById(dto.userId).name} TURN", 100, 5f);
    }

    public void CreateNotificationDiceThrow(object dtoObject)
    {
        SocketBroadcastDiceThrowDTO dto = (SocketBroadcastDiceThrowDTO) dtoObject;

        if(gameNotificationDiceText != null)
        {
            Destroy(gameNotificationDiceText.gameObject);
        }

        gameNotificationDiceText = Instantiate(notificationTextPrefab, transform);
        GameNotificationText notificationText = gameNotificationDiceText.GetComponent<GameNotificationText>();
        notificationText.transform.position = new Vector3(notificationText.transform.position.x, notificationText.transform.parent.position.y, notificationText.transform.position.z);
        notificationText.SetText($"{GameManager.Instance.userManager.GetUserById(dto.userId).name} DICE THROW: {dto.firstDiceNum+dto.secondDiceNum}", 60, 5f);
    }

    public void CreateNotificationUserWin(object dtoObject)
    {
        SocketBroadcastUserWinDTO dto = (SocketBroadcastUserWinDTO)dtoObject;
        gameNotificationDiceText = Instantiate(notificationTextPrefab, transform);
        GameNotificationText notificationText = gameNotificationDiceText.GetComponent<GameNotificationText>();
        notificationText.transform.position = new Vector3(notificationText.transform.position.x, notificationText.transform.parent.position.y, notificationText.transform.position.z);
        notificationText.SetText($"{GameManager.Instance.userManager.GetUserById(dto.userId).name} WIN", 60, 600f);
    }

    private void OnDestroy()
    {
        Multiplayer.Instance.BROADCAST_PREPARE_USER_TURN_EVENT.RemoveListener(CreateNotificationPreparationUserTurn);
        Multiplayer.Instance.BROADCAST_DICE_THROW_EVENT.RemoveListener(CreateNotificationDiceThrow);
    }
}
