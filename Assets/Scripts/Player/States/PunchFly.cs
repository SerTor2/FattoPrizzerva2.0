﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchFly : BaseState
{
    public CharacterController characterController;
    private Vector3 direction;
    [SerializeField] private float damageBase = 22;
    private float speed = 0;

    [SerializeField] private SphereCollider colliderPunch;

    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StartFlyKick(Vector3 _direction, float _speed)
    {
        direction = _direction;
        speed = _speed;
        player.ChangeState(PlayerScript.State.FLYINGKICK);
    }

    public override void Enter()
    {
        player.currentTimeState = 0;
        colliderPunch.enabled = true;
    }

    public override void Execute()
    {
        player.currentTimeState += Time.deltaTime;
        if (player.currentTimeState >= 0.2f)
            player.ChangeState(PlayerScript.State.MOVING);
        CollisionFlags collisionFlags = characterController.Move(direction * Time.deltaTime * speed);

    }

    public override void Exit()
    {
        player.speed = player.normalSpeed;
        colliderPunch.enabled = false;
        player.currentTimePunch += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemie")
        {
            if (player.stateMachine.currentState == this) //(player.stateMachine.currentState == this)
            {
                other.gameObject.GetComponent<EnemieBasic>().MoveDirectionHit((direction).normalized, damageBase, true);
                player.ChangeState(PlayerScript.State.MOVING);
            }
        }
    }

}
