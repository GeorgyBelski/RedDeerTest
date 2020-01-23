using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    

    public Transform rightHand;
    public List<Loot> loot = new List<Loot>();



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
            Debug.Log(other.gameObject);
            Loot newLoot;
            Loot.objectLootMap.TryGetValue(other.gameObject, out newLoot);
            if (newLoot) 
            { 
                loot.Add(newLoot);
                other.transform.parent = rightHand;
                newLoot.PickUp(rightHand);
                
            }
            other.gameObject.layer = Globals.weaponLayer;

            
          
        }
    }
}
