using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    
    public Transform rightHand;
    public List<Loot> loot = new List<Loot>();
    public Loot currentWeapon = null;

    void Start()
    {
        
    }

    void Update()
    {
        
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
}
