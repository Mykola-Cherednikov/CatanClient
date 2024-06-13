using UnityEngine;


public class EscapeForm : MonoBehaviour
{
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