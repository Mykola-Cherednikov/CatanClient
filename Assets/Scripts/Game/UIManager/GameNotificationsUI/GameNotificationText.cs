using TMPro;
using UnityEngine;


public class GameNotificationText : MonoBehaviour
{
    [SerializeField] private TMP_Text gameInfoText;

    private float disappearSpeed = 51f / 255f;


    private void Update()
    {
        gameInfoText.alpha -= Time.deltaTime * disappearSpeed;

        if (gameInfoText.alpha <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void SetText(string text, int fontSize)
    {
        gameInfoText.text = text;
        gameInfoText.fontSize = fontSize;
    }
}

