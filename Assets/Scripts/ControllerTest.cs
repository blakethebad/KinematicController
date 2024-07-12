using System;
using System.Collections;
using System.Collections.Generic;
using KCC;
using UnityEngine;

public class ControllerTest : MonoBehaviour
{
    public KinematicController kc;
    
    void Update()
    {
        Vector3 inputDirection = transform.rotation * new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
        kc.Move(inputDirection);
        Vector2 mouseDelta = new Vector2(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));
        kc.Rotate(mouseDelta.x, mouseDelta.y);
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            kc.Jump(Vector3.up * 6.0f);
        }

    }
}
