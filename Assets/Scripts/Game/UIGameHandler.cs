using Assets.Scripts.Menu;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIGameHandler : MonoBehaviour
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

    private void OnDestroy()
    {
        foreach (GameObject go in transform)
        {
            Destroy(go);
        }
    }
}
