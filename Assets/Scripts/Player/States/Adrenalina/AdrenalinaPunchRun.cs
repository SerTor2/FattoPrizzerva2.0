﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdrenalinaPunchRun : BaseState
{
    CharacterController characterController;
    private float speed = 0;
    private float multiply = 2;

    [SerializeField] private AdrenalinaRun run;

    // Start is called before the first frame update
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        run = GetComponent<AdrenalinaRun>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Enter()
    {
        player.currentTimeState = 0;
        speed = run.speed * multiply;
        player.speed = speed;
    }

    public override void Execute()
    {
        player.currentTimeState += Time.deltaTime;

        if (player.currentTimeState >= 0.15f)
            player.ChangeState(PlayerScript.State.MOVING);

        CollisionFlags collisionFlags = characterController.Move(run.toMove * Time.deltaTime * speed);

    }

    public override void Exit()
    {
        player.currentTimePunch += Time.deltaTime;
        player.ResetSpeed();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Enemie" && player.stateMachine.currentState == this)
        {
            hit.gameObject.GetComponent<EnemieBasic>().MoveDirectionHit((run.toMove).normalized, player.damageBase * player.speed / 2);
            player.ChangeState(PlayerScript.State.MOVING);
        }
    }
}
