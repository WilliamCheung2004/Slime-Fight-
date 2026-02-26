using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.MovementActions movement;

    private PlayerMotor motor;
    private PlayerLook look;

    void Awake()
    {
        playerInput = new PlayerInput();
        movement = playerInput.Movement;
        motor = GetComponent<PlayerMotor>();
        movement.Jump.performed += ctx => motor.Jump();
        look = GetComponent<PlayerLook>();
        movement.Crouch.performed += ctx => motor.Crouch();
        movement.Sprint.performed += ctx => motor.Sprint();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (movement.enabled)
                movement.Disable();
            else
                movement.Enable();
        }
    }

    private void FixedUpdate()
    {
        motor.ProcessMove(movement.MoveInput.ReadValue<Vector2>());
    }

    private void LateUpdate()
    {
        if (look != null && movement.enabled)
            look.ProcessLook(movement.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        movement.Enable();
    }

    private void OnDisable()
    {
        movement.Disable();
    }
}
