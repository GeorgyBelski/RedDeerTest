using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TankSate {Stay, Go, Return };
public class TankActions : MonoBehaviour
{
    public TankSate state = TankSate.Stay;
    public float speed = 10f;
    public float goDelay = 0.9f;
    float timerGoDelay;
    public float distanceToGo = 2f;
    float distanceTraveled =0;
    public float returnTime = 1;
    float timerReturnTime;
    Vector3 destination;
    Vector3 startPosition;
    Vector3 localForwardVector;
    void Start()
    {
        startPosition = transform.position;
        localForwardVector = transform.rotation * Vector3.forward;
        destination = startPosition + localForwardVector * distanceToGo;
    }

    void Update()
    {
        Going();
        Returning();
    }

    public void PlayerDetected()
    {
        state = TankSate.Go;
        timerGoDelay = goDelay;
        distanceTraveled = 0;
    }
    void Going() 
    {
    
        if(state == TankSate.Go) 
        {
            if (timerGoDelay > 0)
            {
                timerGoDelay -= Time.deltaTime;
            }
            else 
            {
                distanceTraveled += speed * Time.deltaTime;
                float ratio = distanceTraveled / distanceToGo;
                //transform.position = localForwardVector * distanceTraveled;
                transform.position = Vector3.Lerp(startPosition, destination, ratio);
                if (distanceTraveled >= distanceToGo)
                {
                    transform.position = destination;
                    state = TankSate.Return;
                    timerReturnTime = 0;
                }

            }
        }
    }
    void Returning() 
    {
        if(state == TankSate.Return) { 
            if (timerReturnTime < returnTime) 
            {
                float ratio = timerReturnTime / returnTime;
                transform.position = Vector3.Lerp(destination, startPosition, ratio);
                timerReturnTime += Time.deltaTime;
            }
            else
            {
                transform.position = startPosition;
                state = TankSate.Stay;
            }
        }
    }
}
