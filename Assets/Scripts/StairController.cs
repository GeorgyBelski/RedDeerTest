using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairController : MonoBehaviour
{

    public new Collider collider;
    float disableColliderTime = 2;
    float timerDisableCollider;
    void Start()
    {
        
    }

    void Update()
    {
       // ReChargingColaider();
    }

    void ReChargingColaider() 
    {
        if (timerDisableCollider > 0)
        {
            timerDisableCollider -= Time.deltaTime;
            
            if(timerDisableCollider <=0) 
            { collider.enabled = true; }
        }
        
    }
    public void DisableCollider() 
    {
        timerDisableCollider = disableColliderTime;
        collider.enabled = false;
    }
}
