using UnityEngine;

public class EscapeUI : MonoBehaviour
{
    private GameObject escapeFormPrefab;
    private GameObject escapeFormGO;

    private void Awake()
    {
        escapeFormPrefab = Resources.Load<GameObject>("Prefabs/Form/EscapeForm");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && escapeFormGO == null)
        {
            escapeFormGO = Instantiate(escapeFormPrefab, transform);
        }
    }
}
