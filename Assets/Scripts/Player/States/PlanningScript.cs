﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanningScript : BaseState
{
    private float gravity = 0;
    private float speed = 0;
    public float dividendoGravity = 60;
    public float dividendoSpeed = 2f;
    private CharacterController characterController;
    private Vector3 toMove;
    private SpriteRenderer spriteRenderer;

    public float staminaPerPunchAereo = 25f;


    private GameObject myCamera;

    // Start is called before the first frame update
    void Start()
    {
        myCamera = Camera.main.gameObject;
        spriteRenderer = player.spriteRenderer;
        characterController = player.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Enter()
    {
        gravity = player.gravity / dividendoGravity;
        speed = player.GetSpeed() / dividendoSpeed;
        player.currentTimeState = 0;
    }

    public override void Execute()
    {
        toMove = Vector3.zero;
        if (Input.GetKey(player.upKey))
            toMove += myCamera.transform.up;
        if (Input.GetKey(player.downKey))
            toMove -= myCamera.transform.up;
        if (Input.GetKey(player.rightKey))
        {
            toMove += myCamera.transform.right;
            spriteRenderer.flipX = false;
        }
        if (Input.GetKey(player.leftKey))
        {
            toMove -= myCamera.transform.right;
            spriteRenderer.flipX = true;

        }

        toMove = new Vector3(toMove.x, 0, toMove.z);
        toMove.Normalize();


        CollisionFlags collisionFlags = characterController.Move(toMove * Time.deltaTime * speed + Vector3.down * gravity * Time.deltaTime);

        if((collisionFlags & CollisionFlags.Below) != 0)
                player.ChangeState(PlayerScript.State.MOVING);

        if(Input.GetMouseButtonDown(0) && player.stamina.Stamina >= staminaPerPunchAereo)
        {
            if (!player.adrenalinaOn)
            {
                player.stamina.ModifiyStamina(-staminaPerPunchAereo);
                player.ChangeState(PlayerScript.State.AEREOPUNCH);
            }
            else
            {
                player.ChangeState(PlayerScript.State.ADRENALINAAEREOPUNCH);
            }
        }

    }

    public override void Exit()
    {
    }
}
