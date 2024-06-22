using TMPro;
using UnityEngine;


public class GameNotificationText : MonoBehaviour
{
    [SerializeField] private TMP_Text gameInfoText;

    private float startTime;
    private float timer = 0f;


    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Destroy(gameObject);
        }

        gameInfoText.alpha = timer / startTime;
    }

    public void SetText(string text, int fontSize, float time)
    {
        gameInfoText.text = text;
        gameInfoText.fontSize = fontSize;
        startTime = time;
        timer = startTime;
    }
}

