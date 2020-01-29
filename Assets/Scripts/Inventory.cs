using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    
    public Transform rightHand;
    public List<Loot> loot = new List<Loot>();
    public Loot currentWeapon = null;
    bool isHidingWeapon = false;
    public float hideWeaponTime = 0.5f;
    float timerHideWeapon = 0;

    void Start()
    {
        
    }

    void Update()
    {
        //HidingWeapon();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Globals.lootLayer) 
        {
            //Debug.Log(other.gameObject);
            Loot.objectLootMap.TryGetValue(other.gameObject, out Loot newLoot);
            if (newLoot) 
            { 
                loot.Add(newLoot);
                if (newLoot.type == Type.Weapon) 
                {
                    newLoot.transform.parent = rightHand;
                    newLoot.PickUp(rightHand);
                    currentWeapon = newLoot;
                    currentWeapon.DisableCollider();
                }   
            }
            other.gameObject.layer = Globals.weaponLayer;

             
        }
    }
    void HidingWeapon()
    {
        if (isHidingWeapon && timerHideWeapon >= 0 && currentWeapon) 
        {
            float hideFactor = timerHideWeapon / hideWeaponTime;
            currentWeapon.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, hideFactor);
            timerHideWeapon -= Time.deltaTime;
        }
    }
    internal void HideWeapon()
    {
        timerHideWeapon = hideWeaponTime;
        isHidingWeapon = true;
        currentWeapon.animator.SetBool("hide",true );
    }

    public void ShowWeapon()
    {
        timerHideWeapon = hideWeaponTime;
        isHidingWeapon = false;
        currentWeapon.animator.SetBool("hide", false);
    }

    void AppearingWeapon()
    {
        

    }
}
