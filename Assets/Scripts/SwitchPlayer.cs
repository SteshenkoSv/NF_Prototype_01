﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwitchPlayer : MonoBehaviour
{
    [SerializeField] private Controls _playerControls;
    [SerializeField] private PlayerMovement playerMovement;

    private void Awake()
    {
        _playerControls = new Controls();

        if (playerMovement.isActive)
        {
            playerMovement.enabled = true;
        }
        else if (!playerMovement.isActive)
        {
            playerMovement.enabled = false; 
        }
    }

    private void OnEnable()
    {
        _playerControls.Enable();
        _playerControls.Player.Switch.performed += Switch;
    }

    private void OnDisable()
    {
        _playerControls.Disable();
        _playerControls.Player.Switch.performed -= Switch;
    }

    public void Switch(InputAction.CallbackContext context)
    {
        playerMovement.enabled = !playerMovement.enabled;
        playerMovement.isActive = !playerMovement.isActive;
    }

}