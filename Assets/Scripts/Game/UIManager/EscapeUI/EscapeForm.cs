using UnityEngine;


public class EscapeForm : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(gameObject);
        }
    }

    public void Disconnect()
    {
        Multiplayer.Instance.Disconnect();
        Destroy(gameObject);
    }

    public void DestroyForm()
    {
        Destroy(gameObject);
    }
}