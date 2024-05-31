using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    CamActionMap inputMap;

    private void Awake()
    {
        inputMap = new CamActionMap();
        inputMap.Enable();
    }

    private void Update()
    {
        Vector2 vector = inputMap.Camera.Move.ReadValue<Vector2>();
        Move(vector);
    }

    private void Move(Vector3 vector)
    {
        transform.position += vector * Time.deltaTime;
    }
}
