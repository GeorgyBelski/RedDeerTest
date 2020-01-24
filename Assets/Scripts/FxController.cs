using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxController : MonoBehaviour
{
    public new ParticleSystem particleSystem;
    float timerLifetime;
    public bool isActive = true;

    void Start()
    {
        var maxLifetime = particleSystem.main.startLifetime.constantMax;
        timerLifetime = particleSystem.main.duration + maxLifetime;
    }

    void Update()
    {
        ReduceTimer();
    }
    public void ActivateFx() 
    {
        isActive = true;
        this.gameObject.SetActive(true);
    }
    void ReduceTimer()
    {
        if (isActive) 
        {
            timerLifetime -= Time.deltaTime;
            if (timerLifetime <= 0)
            {
                isActive = false;
                this.gameObject.SetActive(false);
                
            }
        }
    }
}
