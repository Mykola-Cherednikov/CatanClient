using TMPro;
using UnityEngine;

public class InfoForm : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText;

    public void SetInfoText(string text)
    {
        infoText.text = text;
    }

    public void DestroyForm()
    {
        Destroy(gameObject);
    }
}
