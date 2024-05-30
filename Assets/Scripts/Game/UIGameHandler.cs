using Assets.Scripts.Menu;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIGameHandler : MonoBehaviour
{
    private GameObject _escapeFormGO;
    private GameObject _escapeForm;

    private void Awake()
    {
        _escapeFormGO = Resources.Load<GameObject>("Prefabs/Form/EscapeForm");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _escapeForm == null)
        {
            _escapeForm = Instantiate(_escapeFormGO, transform);
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
