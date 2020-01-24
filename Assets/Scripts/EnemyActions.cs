using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActions : MonoBehaviour
{

    public Animator animator;
    public new Collider collider;
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
        transform.position = startPosition;
        transform.rotation = startRotarion;
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
        animator.SetBool("Death", false);
        ApplyDeathFX();
    }



    void ApplyDeathFX()
    {
        FxController fxController = null;
        foreach (FxController fx in FxManager.fxList) 
        {
            if (fx.isActive) 
            {
                fxController = fx;
                break;
            }
        }
        if (!fxController) 
        {
            FxController fume = Instantiate(FumePrefab).GetComponent<FxController>();
            FxManager.fxList.Add(fume);
            fume.transform.position = transform.position;
            fume.ActivateFx();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enemy: " + other.gameObject);
        if (other.gameObject.layer == Globals.weaponLayer)
        { ApplyDeath(); }
    }
}
