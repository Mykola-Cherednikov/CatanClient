using UnityEngine;

public class GameNotificationsUI : MonoBehaviour
{
    [SerializeField] private GameObject notificationTextPrefab;

    public void CreateNotification(string username)
    {
        GameNotificationText notificationText = Instantiate(notificationTextPrefab, transform).GetComponent<GameNotificationText>();
        notificationText.SetText($"{username} TURN");
    }
}
