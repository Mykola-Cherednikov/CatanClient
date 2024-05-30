using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    CamActionMap _input;

    private void Awake()
    {
        _input = new CamActionMap();
        _input.Enable();
    }

    private void Update()
    {
        Vector2 vector = _input.Camera.Move.ReadValue<Vector2>();
        Move(vector);
    }

    private void Move(Vector3 vector)
    {
        transform.position += vector * Time.deltaTime;
    }
}
