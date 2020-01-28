using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public Animator animator;
    public Inventory inventory;
    public PlayerController playerController;
    void Start()
    {
        
    }
    void Update()
    {
        Attack();
    }

    void Attack() 
    {
        if ((Input.GetMouseButtonDown(0) ||Input.GetKey(KeyCode.E)) && inventory.currentWeapon) 
        {
            //Debug.Log("Attack");
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
    public void LendingOnSpringboard()
    {
        animator.SetBool("PlatformJump", false);
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
}
