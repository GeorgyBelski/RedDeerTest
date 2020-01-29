using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public static Dictionary<GameObject, MovingPlatform> objectMovingPlatformMap = new Dictionary<GameObject, MovingPlatform>();

    public Transform platform;
    public Vector3 lookDirection = Vector3.up * 180;
    public float heightOfPlatform = 0.7f;

    [Header("References")]
    public Animator animator;
    public PlayerController playerController;

    void Start()
    {
        objectMovingPlatformMap.Add(platform.gameObject, this);
    }

    void Update()
    {
        
    }

    public void SpringCharging() 
    {
        animator.SetBool("springJump", true);
    }

    public void Launch() 
    {
        playerController.SpringLaunch(lookDirection);
    }
    public void EndSpringJump()
    {
        animator.SetBool("springJump", false);
    }
}
