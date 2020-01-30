using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActions : MonoBehaviour
{

    public Animator animator;
    public new Collider collider;
    public Transform core;
    public GameObject FumePrefab;
    Vector3 startPosition;
    Quaternion startRotarion;
    void Start()
    {
        startPosition = transform.position;
        startRotarion = transform.rotation;
    }

    void Update()
    {
        
    }    
    public void EnableEnemy()
    {
       // transform.position = startPosition;
       // transform.rotation = startRotarion;
       // animator.SetBool("Death", false);
        gameObject.SetActive(true);
        collider.enabled = true; 
    }
    void ApplyDeath() 
    {
        animator.SetBool("Death", true);
        collider.enabled = false;
    }
    public void DisableEnemy() // "Death" Animation call
    {
        gameObject.SetActive(false);
        ApplyDeathFX();
    }



    void ApplyDeathFX()
    {
        FxController fxController = FxManager.GetFume();
        fxController.transform.position = core.transform.position;
        fxController.ActivateFx();
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Enemy: " + other.gameObject);
        if (other.gameObject.layer == Globals.weaponLayer)
        { ApplyDeath(); }
    }
}
