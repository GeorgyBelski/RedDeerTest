﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public Animator animator;
    public Inventory inventory;
    public PlayerController playerController;
    public float scaleDelay = 0.2f;

    bool isPipe = false;
    float timerScaleDelay = 0;
    
    void Start()
    {
        
    }
    void Update()
    {
        Attack();
        Squeeze();
    }

    void Attack() 
    {
        if ((Input.GetMouseButtonDown(0) ||Input.GetKey(KeyCode.E)) && inventory.currentWeapon) 
        {
            animator.SetBool("Attack", true);
        }
    
    }
    public void ActivateWeapon() // "Attack" Animation call
    {
        inventory.currentWeapon.EnableCollider();
    }

    public void EndAttack() // "Attack" Animation call
    {
        animator.SetBool("Attack", false);
        inventory.currentWeapon.DisableCollider();
    }

    public void Restart()
    {
        playerController.Restart();
    }
    public void JumpOnSpringboard()
    {
        animator.SetBool("PlatformJump", true);
        playerController.state = PlayerState.Freeze;
    }
    
    public void LandedOnSpringboard()
    {
        animator.SetBool("PlatformJump", false);
        //playerController.isTransferEnd = true;
        playerController.LendedOnPlatform();
    }

    public void SqueezeThroughPipe() 
    {
        timerScaleDelay = scaleDelay;
        isPipe = true;
    }
    void Squeeze() 
    {
        if (isPipe) 
        {
            if (timerScaleDelay <= 0)
            {
                animator.SetBool("Squeeze", true);
                inventory.HideWeapon();
                playerController.StartGoDown();
                isPipe = false;
            }
            else 
            {
                timerScaleDelay -= Time.deltaTime;
            }
        }
    }
    
    public void EndSqueeze() 
    {
        animator.SetBool("Squeeze", false);
        inventory.ShowWeapon();
    }
    void Climb() 
    {
        animator.SetBool("Climb", true);
    }
    public void EndClimb() 
    {
        animator.SetBool("Climb", false);
        playerController.isTransferEnd = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == Globals.enemyLayer)
        {
            Restart();
        }
        else if (collision.gameObject.layer == Globals.springboardLayer) 
        {
            playerController.SetMovingPlatform(collision.gameObject);

            if (collision.transform.position.y > this.transform.position.y)
            { JumpOnSpringboard(); }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Globals.pipeLayer)
        {
            SqueeezeAttributes squeeezeAttributes = other.GetComponent<SqueeezeAttributes>();
            squeeezeAttributes.EnableNextTriggers();
            playerController.pipeDestination = squeeezeAttributes.destination;
            SqueezeThroughPipe();
        }
        else if (other.gameObject.layer == Globals.stairLayer) 
        {
            Debug.Log("other : " + other.gameObject.layer);
            other.gameObject.GetComponent<StairController>().DisableCollider();
            Climb();
        }
    }
}
