using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Test2 : MonoBehaviour
{
    [SerializeField] Test test;

    private void Awake()
    {
        test.Prekol.AddListener(Mem);
    }

    public void Mem()
    {

    }
}
