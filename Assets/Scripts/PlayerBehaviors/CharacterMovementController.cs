﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CharacterMovementController : MonoBehaviour
{
    public bool golemIsActive;
    public bool mushroomIsActive;

    [SerializeField] private MovementHandler _mushroomMovementHandler;
    [SerializeField] private InputHandler _golemInputHandler;
    [SerializeField] private InputHandler _mushroomInputHandler;
    [SerializeField] private MovementInputProcessor _golemMovementInputProcessor;
    [SerializeField] private MovementInputProcessor _mushroomMovementInputProcessor;
    [SerializeField] private GameObject _golemGameObject;
    [SerializeField] private GameObject _mushroomGameObject;
    [SerializeField] private CollisionProcessor _mushroomCollisionProcessor;
    [SerializeField] private Rigidbody _mushroomRigidbody;

    private RigidbodyInterpolation _savedInterpolation;
    private CollisionDetectionMode _savedCollisionDetectionMode;
    private bool _isJumpOffProcessed;
    private Transform _supposedParent;

    private void Start()
    {
        _savedInterpolation = _mushroomRigidbody.interpolation;
        _savedCollisionDetectionMode = _mushroomRigidbody.collisionDetectionMode;

        SetActive("golem");
        SetInactive("mushroom");
        _isJumpOffProcessed = false;
    }

    private void Update()
    {
        if (_mushroomGameObject.transform.parent != _golemGameObject.transform)
            _supposedParent = _mushroomGameObject.transform.parent;

        HandleInput();

        ProcessJumpOnTop();

        ProcessRiding();

        HandleMushroomRotation();
    }

    private void HandleMushroomRotation()
    {
        if (golemIsActive && _mushroomCollisionProcessor.isOnTopOfGolem)
        {
            if (_golemInputHandler.isFacingRight)
            {
                _mushroomGameObject.transform.eulerAngles = new Vector3(0, 90, 0);
                _mushroomInputHandler.isFacingRight = true;
            }
            else
            {
                _mushroomGameObject.transform.eulerAngles = new Vector3(0, -90, 0);
                _mushroomInputHandler.isFacingRight = false;
            }
        }
    }

    public void SetActive(string characterName)
    {
        if (characterName == "golem")
        {
            _golemInputHandler.enabled = true;
            golemIsActive = true;
        }
        else if (characterName == "mushroom")
        {
            Debug.Log(_supposedParent);
            _mushroomGameObject.transform.parent = _supposedParent;
            _mushroomInputHandler.enabled = true;
            mushroomIsActive = true;
        }
    }

    public void SetInactive(string characterName)
    {
        if (characterName == "golem")
        {
            _golemInputHandler.enabled = false;
            golemIsActive = false;
        }
        else if (characterName == "mushroom")
        {
            _mushroomInputHandler.enabled = false;
            mushroomIsActive = false;
        }
    }

    public void SetCharactersCollisions(bool isActive)
    {
        if (isActive)
        {
            Physics.IgnoreLayerCollision(8, 9, false);
        }
        else
        {
            Physics.IgnoreLayerCollision(8, 9);
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SwitchCharacters();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(JumpOnOff());
        }
    }

    private void SwitchCharacters()
    {
        _golemMovementInputProcessor.SetMovementInput(new Vector2(0, 0));
        _mushroomMovementInputProcessor.SetMovementInput(new Vector2(0, 0));

        if (_golemInputHandler.enabled == true)
        {
            SetInactive("golem");
            SetActive("mushroom");
        }
        else if (_mushroomInputHandler.enabled == true)
        {
            SetInactive("mushroom");
            SetActive("golem");
        }
    }

    private void ProcessJumpOnTop()
    {
        if (!_isJumpOffProcessed)
        {
            if ((_mushroomGameObject.transform.position.y - _golemGameObject.transform.position.y) > 2)
            {
                SetCharactersCollisions(true);
            }
            else
            {
                SetCharactersCollisions(false);
            }
        }
    }

    private IEnumerator JumpOnOff()
    {
        if (mushroomIsActive && _isJumpOffProcessed == false)
        {
            _isJumpOffProcessed = true;
            SetCharactersCollisions(false);
            yield return new WaitUntil(() => _mushroomCollisionProcessor.isGrounded == true);
            _isJumpOffProcessed = false;
        }
    }

    private void ProcessRiding()
    {
        if (_mushroomCollisionProcessor.isOnTopOfGolem && !mushroomIsActive)
        {
            _mushroomGameObject.transform.parent = _golemGameObject.transform;
            Destroy(_mushroomRigidbody);

            Vector3 currentPosition = _mushroomGameObject.transform.position;
            Vector3 targetPosition = new Vector3(_golemGameObject.transform.position.x, _golemGameObject.transform.position.y + 2.5f, _golemGameObject.transform.position.z);
            Vector3 newPosition = Vector3.Lerp(currentPosition, targetPosition, 0.2f);

            _mushroomGameObject.transform.position = newPosition;
        }
        else
        {
            if (_mushroomRigidbody == null)
            {
                _mushroomRigidbody = _mushroomGameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
                _mushroomRigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                _mushroomRigidbody.interpolation = _savedInterpolation;
                _mushroomRigidbody.collisionDetectionMode = _savedCollisionDetectionMode;
                _mushroomMovementHandler.SetRigidbody(_mushroomRigidbody);
            }
        }
    }
}
