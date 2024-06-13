using UnityEngine;

public class GameNotificationsUI : MonoBehaviour
{
    [SerializeField] private GameObject notificationTextPrefab;
    [SerializeField] private GameObject gameNotificationTurnText;
    [SerializeField] private GameObject gameNotificationDiceText;

    public void CreateNotificationUserTurn(string username)
    {
        if(gameNotificationTurnText != null)
        {
            Destroy(gameNotificationTurnText.gameObject);
        }

        gameNotificationTurnText = Instantiate(notificationTextPrefab, transform);
        GameNotificationText notificationText = gameNotificationTurnText.GetComponent<GameNotificationText>();
        notificationText.SetText($"{username} TURN", 100);
    }

    public void CreateNotificationDiceThrow(string username, int diceNum)
    {
        if(gameNotificationDiceText != null)
        {
            Destroy(gameNotificationDiceText.gameObject);
        }

        gameNotificationDiceText = Instantiate(notificationTextPrefab, transform);
        GameNotificationText notificationText = gameNotificationDiceText.GetComponent<GameNotificationText>();
        notificationText.transform.position = new Vector3(notificationText.transform.position.x, notificationText.transform.parent.position.y, notificationText.transform.position.z);
        notificationText.SetText($"{username} DICE THROW: {diceNum}", 60);
    }
}
