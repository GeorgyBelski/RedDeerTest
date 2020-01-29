using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SqueeezeAttributes : MonoBehaviour
{
    public Collider stairCollider;
    public Vector3 destination = new Vector3(-2.5f,0,0.5f);

    public void EnableNextTriggers() 
    {
        stairCollider.enabled = true;
    }
}
